namespace SharedUtilities.Extensions
{
    public static class FieldUtils
    {
        public static string GetBackingFieldName(string propertyName) => $"<{propertyName}>k__BackingField";
    }
}