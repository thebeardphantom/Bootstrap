namespace BeardPhantom.Bootstrap.Editor
{
    public static class SerializedPropertyUtility
    {
        public static string GetSerializedBackingFieldName(this string propertyName)
        {
            return $"<{propertyName}>k__BackingField";
        }
    }
}