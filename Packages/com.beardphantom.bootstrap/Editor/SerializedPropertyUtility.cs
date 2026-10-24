using System;

namespace BeardPhantom.Bootstrap.Editor
{
    /// <summary>
    /// Utility methods for working with Unity's serialized backing field naming.
    /// </summary>
    public static class SerializedPropertyUtility
    {
        private const string BackingFieldFmt = "<>k__BackingField";

        /// <summary>
        /// Converts an auto-property name to the name Unity uses internally for its serialized backing field.
        /// </summary>
        public static string GetSerializedBackingFieldName(this string propertyName)
        {
            var totalLength = propertyName.Length + BackingFieldFmt.Length;
            var serializedBackingFieldName = string.Create(totalLength, propertyName, BuildFormattedString);
            return serializedBackingFieldName;
        }

        private static void BuildFormattedString(Span<char> span, string input)
        {
            var writeIndex = 0;
            span[writeIndex] = BackingFieldFmt[writeIndex];
            writeIndex++;
            foreach (var c in input)
            {
                span[writeIndex++] = c;
            }

            for (var i = 1; i < BackingFieldFmt.Length; i++)
            {
                span[writeIndex++] = BackingFieldFmt[i];
            }
        }
    }
}