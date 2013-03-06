using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MiniDI
{
    class TypeFactory<TRequested>
    {
        private bool _recursiveMetDuringBuild;
        private readonly MethodInfo _method;
        public TypeFactory()
        {
            _method = GetType().GetMethod("CreateInstanceRecursive", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static TRequested Build()
        {
            var factory = new TypeFactory<TRequested>();

            return factory.CreateInstanceRecursive<TRequested>();
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
            else
            {
                if (_recursiveMetDuringBuild)
                    throw
                        new ResolveException("A recursive dependency find during resolving of {0}", builtType.FullName);
                else
                    _recursiveMetDuringBuild = true;
            }
            if (builtType.IsAbstract || builtType.IsInterface)
            {
                throw new ResolveException("Unable to find a realisation of type {0}", builtType.FullName);
            }
            ConstructorInfo ci = null;
            var constructors = builtType.GetConstructors();
            if (constructors.Length == 1)
                ci = constructors[0];
            else
                foreach (var constructorInfo in constructors)
                {
                    if (constructorInfo.GetCustomAttributes(typeof (InjectedAttribute), false).Length == 1)
                        ci = constructorInfo;
                }

            if (ci == null)
                throw new ResolveException(
                    "Unable to determine a required constructor in type {0}. Consider using [Injected] attribute on constructor being used by MiniDI",
                    builtType.FullName);

            var constructedObject = TryInvoke<TCurrent>(ci);
            if (!constructedObject.Equals(defaultValue))
                return constructedObject;

            throw new ResolveException("Unable to resolve type " + builtType.FullName);
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
                                          typeof(TCurrent).FullName, p.ParameterType),
                            e.InnerException);
                    }
                }
            }
            return (TCurrent)constructorInfo.Invoke(values);
        }
    }
}
