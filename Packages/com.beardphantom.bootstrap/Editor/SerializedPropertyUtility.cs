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
        /// <param name="propertyName">The auto-property name.</param>
        /// <returns>The backing field name, e.g. <c>&lt;PropertyName&gt;k__BackingField</c>.</returns>
        public static string GetSerializedBackingFieldName(this string propertyName)
        {
            int totalLength = propertyName.Length + BackingFieldFmt.Length;
            var serializedBackingFieldName = string.Create(totalLength, propertyName, BuildFormattedString);
            return serializedBackingFieldName;
        }

        private static void BuildFormattedString(Span<char> span, string input)
        {
            var writeIndex = 0;
            span[writeIndex] = BackingFieldFmt[writeIndex];
            writeIndex++;
            foreach (char c in input)
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