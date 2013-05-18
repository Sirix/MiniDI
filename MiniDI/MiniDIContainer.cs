using System;
using System.Collections.Generic;

namespace MiniDI
{
    public class MiniDIContainer
    {
        private static readonly Dictionary<Type, BaseInjectedType> Types;

        static MiniDIContainer()
        {
            Types = new Dictionary<Type, BaseInjectedType>();
        }

        /// <summary>
        /// Устанавливает соответствие между типом интерфейса и реализацией
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TRealisation"></typeparam>
        public static void Set<TInterface, TRealisation>() where TRealisation : TInterface
        {
            Types.Add(typeof (TInterface), InjectedType<TRealisation>.BuildAsBase());
        }

        /// <summary>
        /// Устанавливает соответствие между типом интерфейса и реализацией с указанием времени жизни объекта
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TRealisation"></typeparam>
        /// <param name="lifeTime"></param>
        public static void Set<TInterface, TRealisation>(LifeTime lifeTime)
        {
            Types.Add(typeof(TInterface), InjectedType<TRealisation>.Build(lifeTime));
        }

        public static TInterface Get<TInterface>()
        {
            var type = typeof(TInterface);

            if (!Types.ContainsKey(type))
                throw new ResolveException("Unable to find implementation of type " + type.FullName);

            var obj = Types[type];
            return (TInterface) obj.Resolve();
        }

        public static bool TryGet<TInterface>(out TInterface value)
        {
            value = default(TInterface);
            var type = typeof(TInterface);

            if (!Types.ContainsKey(type))
                return false;

            var obj = Types[type];
            value = (TInterface)obj.Resolve();

            return true;
        }

        public static void RemoveAll()
        {
            Types.Clear();
        }
    }
}