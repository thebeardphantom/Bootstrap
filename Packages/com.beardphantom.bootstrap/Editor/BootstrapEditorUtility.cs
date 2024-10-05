using System;
using UnityEditor.Search;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor
{
    internal static class BootstrapEditorUtility
    {
        private const string PickPrefabTextFormat = "p: prefab:any t:{0}";

        public static void PickPrefabWithComponent<T>(Action<T> onSelected) where T : class
        {
            SearchService.ShowObjectPicker(
                (result, wasCanceled) =>
                {
                    if (wasCanceled)
                    {
                        return;
                    }

                    if (result is T typedResult)
                    {
                        onSelected(typedResult);
                    }
                },
                default,
                string.Format(PickPrefabTextFormat, typeof(T).Name),
                typeof(T).Name,
                typeof(GameObject));
        }

        public static void PickAsset<T>(Action<T> onSelected) where T : class
        {
            SearchService.ShowObjectPicker(
                (result, wasCanceled) =>
                {
                    if (wasCanceled)
                    {
                        return;
                    }

                    if (result is T typedResult)
                    {
                        onSelected(typedResult);
                    }

                    if (result == default)
                    {
                        onSelected(default);
                    }
                },
                default,
                default,
                default,
                typeof(T));
        }
    }
}