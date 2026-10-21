using System.Text;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Bootstrap
{
    internal static class ObjectUtility
    {
        public static string GetGameObjectPath(GameObject gameObject)
        {
            using (GenericPool<StringBuilder>.Get(out var stringBuilder))
            {
                stringBuilder.Clear();
                var tform = gameObject.transform;
                while (tform.IsNotNull())
                {
                    stringBuilder.Insert(0, tform.name);
                    stringBuilder.Insert(0, '/');
                    tform = tform.parent;
                }

                var gameObjectPath = stringBuilder.ToString();
                stringBuilder.Clear();
                return gameObjectPath;
            }
        }

        public static GameObject FindInScene(Scene scene, string path)
        {
            // TODO: Manually search through scene specifically
            return GameObject.Find(path);
        }
    }
}