using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MiniDI
{
    internal class InjectedType<TRealisation> : BaseInjectedType
    {
        public MiniDIContainer Container { get; set; }

        private TRealisation DefaultValue { get; set; }
        private TRealisation InstantiatedObject { get; set; }
        private readonly MethodInfo _method;

        public InjectedType()
        {
            Type = typeof (TRealisation);
            DefaultValue = default(TRealisation);
            _method = GetType().GetMethod("CreateInstanceRecursive", BindingFlags.NonPublic | BindingFlags.Instance);
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
            return CreateInstanceRecursive<TRealisation>();
        }

        private TRequested TryInvoke<TRequested>(ConstructorInfo constructorInfo)
        {
            var parameters = constructorInfo.GetParameters();
            if (parameters.Length == 0)
                return (TRequested)constructorInfo.Invoke(null);

            var values = new object[parameters.Length];

            for (int index = 0; index < parameters.Length; index++)
            {
                var p = parameters[index];

                try
                {
                    MethodInfo genericMethod = _method.MakeGenericMethod(new[] { p.ParameterType });
                    object result = genericMethod.Invoke(this, null);
                    values[index] = result;
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException is ResolveException)
                    {
                        throw new ResolveException(
                            string.Format("Unable to resolve type {0} because of error instantiating {1}",
                                          typeof (TRequested).FullName, p.ParameterType),
                            e.InnerException);
                    }
                }
            }
            return (TRequested)constructorInfo.Invoke(values);
        }

        private TRequested CreateInstanceRecursive<TRequested>()
        {
            var requestedType = typeof (TRequested);
            if (requestedType != typeof(TRealisation))
            {
                TRequested value;
                var result = MiniDIContainer.TryGet<TRequested>(out value);
                if (result && !value.Equals(DefaultValue))
                    return value;
            }

            if(requestedType.IsAbstract || requestedType.IsInterface)
            {
                throw new ResolveException("Unable to find a realisation of type {0}", requestedType.FullName);
            }
            ConstructorInfo ci = null;
            var constructors = requestedType.GetConstructors();
            if (constructors.Length == 1)
                ci = constructors[0];

            foreach (var constructorInfo in constructors)
            {
                if (constructorInfo.GetCustomAttributes(typeof(InjectedAttribute), false).Length == 1)
                    ci = constructorInfo;
            }

            if (ci == null)
                throw new ResolveException(
                    "Unable to determine a required constructor in type {0}. Consider using [Injected] attribute on constructor being used by MiniDI",
                    requestedType.FullName);

            var constructedObject = TryInvoke<TRequested>(ci);
            if (!constructedObject.Equals(DefaultValue))
                return constructedObject;

            throw new ResolveException("Unable to resolve type " + requestedType.FullName);
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