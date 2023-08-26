using System.Linq;
using Injact.Internal;

namespace Injact
{
    public class Injector
    {
        private readonly DiContainer _container;

        public Injector(DiContainer container)
        {
            _container = container;
        }

        public void InjectInto(object target)
        {
            Assert.IsNotNull(target, "Cannot inject into null target!");

            InjectIntoFields(target);
            InjectIntoProperties(target);
            InjectIntoMethods(target);
        }

        private void InjectIntoFields(object requestingObject)
        {
            var fields = requestingObject
                .GetType()
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var requiredFields = fields.Where(s => s.GetCustomAttributes(typeof(InjectAttribute), true).Length > 0);
            var optionalFields = fields.Where(s => s.GetCustomAttributes(typeof(InjectOptionalAttribute), true).Length > 0);

            var requestingType = requestingObject.GetType();

            foreach (var field in requiredFields)
                field.SetValue(requestingObject, _container.Resolve(field.FieldType, requestingType));

            foreach (var field in optionalFields)
                field.SetValue(requestingObject, _container.Resolve(field.FieldType, requestingType, false));
        }

        private void InjectIntoProperties(object requestingObject)
        {
            var properties = requestingObject
                .GetType()
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var requiredProperties = properties.Where(s => s.GetCustomAttributes(typeof(InjectAttribute), true).Length > 0);
            var optionalProperties = properties.Where(s => s.GetCustomAttributes(typeof(InjectOptionalAttribute), true).Length > 0);

            var requestingType = requestingObject.GetType();

            foreach (var property in requiredProperties)
            {
                var type = property.PropertyType;
                property.SetValue(requestingObject, _container.Resolve(type, requestingType));
            }

            foreach (var property in optionalProperties)
            {
                var type = property.PropertyType;
                property.SetValue(requestingObject, _container.Resolve(type, requestingType, false));
            }
        }

        private void InjectIntoMethods(object requestingObject)
        {
            var methods = requestingObject
                .GetType()
                .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var requiredMethods = methods.Where(s => s.GetCustomAttributes(typeof(InjectAttribute), true).Length > 0);
            var optionalMethods = methods.Where(s => s.GetCustomAttributes(typeof(InjectOptionalAttribute), true).Length > 0);

            var requestingType = requestingObject.GetType();

            foreach (var method in requiredMethods)
            {
                var parameters = method
                    .GetParameters()
                    .Select(s => _container.Resolve(s.ParameterType, requestingType))
                    .ToArray();

                method.Invoke(requestingObject, parameters);
            }

            foreach (var method in optionalMethods)
            {
                var parameters = method
                    .GetParameters()
                    .Select(s => _container.Resolve(s.ParameterType, requestingType, false))
                    .ToArray();

                method.Invoke(requestingObject, parameters);
            }
        }
    }
}