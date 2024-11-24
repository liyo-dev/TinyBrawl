using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Service
{
    public static class ServiceLocator
    {
        private static Dictionary<Type, object> services = new Dictionary<Type, object>();
        public static bool IsInitialized = false;
        
        public static void AddService<T>(T instance)
        {
            Type type = typeof(T);
            Assert.IsFalse(services.ContainsKey(type), $"Service of type {type} is already registered.");
            services.Add(type, instance);
        }

        public static T GetService<T>()
        {
            Type type = typeof(T);
            Assert.IsTrue(services.ContainsKey(type), $"Service of type {type} not found.");
            return (T)services[type];
        }
    }
}
