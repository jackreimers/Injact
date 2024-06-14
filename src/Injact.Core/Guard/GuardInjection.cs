namespace Injact;

public static partial class Guard
{
    public static partial class Against
    {
        public static void IllegalInjection(Bindings bindings, Type requestedType, Type? requestingType)
        {
            if (requestingType == null)
            {
                return;
            }

            bindings.TryGetValue(requestedType, out var binding);

            if (binding == null || !binding.AllowedInjectionTypes.Any())
            {
                return;
            }

            var isAllowed = binding.AllowedInjectionTypes.Any(allowed => allowed.IsAssignableFrom(requestingType));
            if (!isAllowed)
            {
                throw new DependencyException($"{requestingType} requested type of {requestedType} when it is not allowed to!");
            }
        }
    }
}
