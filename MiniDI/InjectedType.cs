using System;
using System.Reflection;

namespace MiniDI
{
    internal class InjectedType<TRealisation> : InjectedTypeBase
    {
        private bool _isBeingBuilt;

        private TRealisation DefaultValue { get; set; }
        private TRealisation InstantiatedObject { get; set; }

        internal ConstructorInfo SelectedConstructor { get; set; }

        private TypeFactory<TRealisation> typeFactory;

        public InjectedType()
        {
            Type = typeof (TRealisation);
            DefaultValue = default(TRealisation);

            typeFactory = new TypeFactory<TRealisation>(this);
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

        internal static InjectedTypeBase BuildAsBase()
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
            if (!Object.ReferenceEquals(InstantiatedObject, DefaultValue))
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
            TRealisation value = typeFactory.Create();
            _isBeingBuilt = false;
            return value;
        }
    }

    internal interface IInjectType
    {
        Type Type { get; set; }
        LifeTime LifeTime { get; set; }
    }

    internal class InjectedTypeBase : IInjectType
    {
        public Type Type { get; set; }
        public LifeTime LifeTime { get; set; }

        public virtual object Resolve()
        {
            throw new NotImplementedException("Resolve is not implemented on base class");
        }
    }
}