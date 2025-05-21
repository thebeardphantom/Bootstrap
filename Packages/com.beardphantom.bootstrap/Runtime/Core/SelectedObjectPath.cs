using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap
{
    [Serializable]
    public class SelectedObjectPath
    {
        public string ScenePath { get; set; }

        public string ObjectPath { get; set; }

        public static SelectedObjectPath CreateInstance(Object obj)
        {
            return obj switch
            {
                GameObject gObj => CreateFromGameObject(gObj),
                Component cmp => CreateFromGameObject(cmp.gameObject),
                _ => throw new ArgumentException("Invalid type", nameof(obj)),
            };
        }

        private static SelectedObjectPath CreateFromGameObject(GameObject gameObject)
        {
            Scene scene = gameObject.scene;
            return new SelectedObjectPath
            {
                ObjectPath = ObjectUtility.GetGameObjectPath(gameObject),
                ScenePath = scene.path,
            };
        }
    }
}