using System;
using UnityEngine;
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
                ObjectPath = ObjectUtility.GetGameObjectPath(gameObject),
                ScenePath = scene.path
            };
        }

        #endregion
    }
}