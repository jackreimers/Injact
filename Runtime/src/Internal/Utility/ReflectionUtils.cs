using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Injact.Internal
{
    public static class ReflectionUtils
    {
        public static ConstructorInfo GetConstructor(Type type)
        {
            var constructors = type.GetConstructors();
            var defaultConstructor = constructors[0];

            foreach (var constructor in constructors)
            {
                if (!constructor.GetCustomAttributes(typeof(InjectAttribute), true).Any())
                    continue;

                defaultConstructor = constructor;
                break;
            }

            return defaultConstructor;
        }

        public static IEnumerable<ParameterInfo> GetParameters(Type type)
        {
            var constructor = GetConstructor(type);
            return constructor.GetParameters();
        }
    }
}