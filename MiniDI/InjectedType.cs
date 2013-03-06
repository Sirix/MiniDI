using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MiniDI
{
    internal class InjectedType<TRealisation> : BaseInjectedType
    {
        private bool _isBeingBuilt;
        public MiniDIContainer Container { get; set; }

        private TRealisation DefaultValue { get; set; }
        private TRealisation InstantiatedObject { get; set; }

        public InjectedType()
        {
            Type = typeof (TRealisation);
            DefaultValue = default(TRealisation);
        }

        private static InjectedType<TRealisation> BuildInternal()
        {
            var d1 = typeof(InjectedType<>);
            Type[] typeArgs = { typeof(TRealisation) };
            var makeme = d1.MakeGenericType(typeArgs);
            object o = Activator.CreateInstance(makeme);

            var obj = (InjectedType<TRealisation>) o;

            return obj;
        }

        internal static InjectedType<TRealisation> Build()
        {
            return BuildInternal();
        }

        internal static BaseInjectedType BuildAsBase()
        {
            return BuildInternal();
        }

        internal static InjectedType<TRealisation> Build(LifeTime lifeTime)
        {
            var obj = BuildInternal();
            obj.LifeTime = lifeTime;

            switch (lifeTime)
            {
                case LifeTime.Default:
                    //do  nothing
                    break;

                case LifeTime.Singleton:
                   // obj.InstantiatedObject = Activator.CreateInstance<TRealisation>();
                    break;

                case LifeTime.PerThread:
                    throw new ArgumentException("LifeTime.PerThread is not supported");
                    break;
            }
            return obj;
        }

        private TRealisation GetObject()
        {
            if (!(InstantiatedObject.Equals(DefaultValue)))
                InstantiatedObject = CreateInstance();

            return InstantiatedObject;
        }

        public override object Resolve()
        {
            TRealisation returnObject = DefaultValue;
            switch (LifeTime)
            {
                case LifeTime.Default:
                    returnObject = CreateInstance();
                    break;

                case LifeTime.Singleton:
                    returnObject = GetObject();
                    break;

                case LifeTime.PerThread:
                    throw new ArgumentException("LifeTime.PerThread is not supported");
                    break;
            }
            return returnObject;
        }

        private TRealisation CreateInstance()
        {
            if (_isBeingBuilt)
                throw new ResolveException("A recursive dependency find during resolving of {0}",
                                           typeof (TRealisation).FullName);

            _isBeingBuilt = true;
            TRealisation value = TypeFactory<TRealisation>.Build();
            _isBeingBuilt = false;
            return value;
        }
    }

    internal interface IInjectType
    {
        Type Type { get; set; }
        LifeTime LifeTime { get; set; }
    }

    internal class BaseInjectedType : IInjectType
    {
        public Type Type { get; set; }
        public LifeTime LifeTime { get; set; }

        public virtual object Resolve()
        {
            throw new NotImplementedException("Resolve is not implemented on base class");
        }
    }
}