using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MiniDI
{
    class TypeFactory<TRequested>
    {
        private readonly MethodInfo _createInstanceRecursive;
        private InjectedType<TRequested> _injectedType;

        public static TRequested Build(InjectedType<TRequested> injectedType)
        {
            var factory = new TypeFactory<TRequested>(injectedType);

            return factory.CreateInstanceRecursive<TRequested>();
        }

        private TypeFactory(InjectedType<TRequested> injectedType)
        {
            _injectedType = injectedType;
            _createInstanceRecursive = GetType().GetMethod("CreateInstanceRecursive", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private TCurrent CreateInstanceRecursive<TCurrent>()
        {
            var defaultValue = default(TCurrent);
            var builtType = typeof(TCurrent);

            if (builtType != typeof(TRequested))
            {
                TCurrent value;
                var result = MiniDIContainer.TryGet<TCurrent>(out value);
                if (result && !value.Equals(defaultValue))
                    return value;
            }

            if (builtType.IsAbstract || builtType.IsInterface)
            {
                throw new ResolveException("Unable to find a realisation of type {0}", builtType.FullName);
            }

            var ci = GetConstructor(builtType);

            var constructedObject = TryInvoke<TCurrent>(ci);
            if (!constructedObject.Equals(defaultValue))
                return constructedObject;

            throw new ResolveException("Unable to resolve type " + builtType.FullName);
        }

        private ConstructorInfo GetConstructor(Type builtType)
        {
            ConstructorInfo ci = _injectedType.SelectedConstructor;

            var constructors = builtType.GetConstructors();
            if (constructors.Length == 1)
                ci = constructors[0];
            else
                foreach (var constructorInfo in constructors)
                    if (constructorInfo.GetCustomAttributes(typeof (InjectedAttribute), false).Length == 1)
                    {
                        ci = constructorInfo;
                        break;
                    }
            _injectedType.SelectedConstructor = ci;

            if (ci == null)
                throw new ResolveException(
                    "Unable to determine a required constructor in type {0}. Consider using [Injected] attribute on constructor being used by MiniDI",
                    builtType.FullName);
            return ci;
        }

        private TCurrent TryInvoke<TCurrent>(ConstructorInfo constructorInfo)
        {
            var parameters = constructorInfo.GetParameters();
            if (parameters.Length == 0)
                return (TCurrent)constructorInfo.Invoke(null);

            var values = new object[parameters.Length];

            for (int index = 0; index < parameters.Length; index++)
            {
                var p = parameters[index];

                try
                {
                    MethodInfo genericMethod = _createInstanceRecursive.MakeGenericMethod(new[] { p.ParameterType });
                    object result = genericMethod.Invoke(this, null);
                    values[index] = result;
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException is ResolveException)
                    {
                        throw new ResolveException(
                            string.Format("Unable to resolve type {0} because of error instantiating {1}",
                                          typeof(TCurrent).FullName, p.ParameterType),
                            e.InnerException);
                    }
                }
            }
            return (TCurrent)constructorInfo.Invoke(values);
        }
    }
}
