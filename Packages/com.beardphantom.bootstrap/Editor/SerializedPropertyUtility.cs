using System;

namespace BeardPhantom.Bootstrap.Editor
{
    public static class SerializedPropertyUtility
    {
        private const string BackingFieldFmt = "<>k__BackingField";

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