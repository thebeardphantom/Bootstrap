using System;
using System.Text;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap
{
    [Serializable]
    public class SelectedObjectPath
    {
        #region Fields

        public string ScenePath;

        public string ObjectPath;

        #endregion

        #region Methods

        public static SelectedObjectPath CreateInstance(Object obj)
        {
            return obj switch
            {
                GameObject gObj => CreateFromGameObject(gObj),
                Component cmp => CreateFromGameObject(cmp.gameObject),
                _ => throw new ArgumentException("Invalid type", nameof(obj))
            };
        }

        private static SelectedObjectPath CreateFromGameObject(GameObject gameObject)
        {
            var scene = gameObject.scene;
            return new SelectedObjectPath
            {
                ObjectPath = GetGameObjectPath(gameObject),
                ScenePath = scene.path
            };
        }

        private static string GetGameObjectPath(GameObject gameObject)
        {
            using (GenericPool<StringBuilder>.Get(out var stringBuilder))
            {
                stringBuilder.Clear();
                var tform = gameObject.transform;
                while (tform != null)
                {
                    stringBuilder.Insert(0, tform.name);
                    tform = tform.parent;
                    if (tform != null)
                    {
                        stringBuilder.Insert(0, '/');
                    }
                }

                var gameObjectPath = stringBuilder.ToString().Trim();
                stringBuilder.Clear();
                return gameObjectPath;
            }
        }

        #endregion
    }
}