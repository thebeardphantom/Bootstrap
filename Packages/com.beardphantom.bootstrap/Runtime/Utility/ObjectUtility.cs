using System.Text;
using UnityEngine;
using UnityEngine.Pool;

namespace BeardPhantom.Bootstrap
{
    internal static class ObjectUtility
    {
        internal static string GetGameObjectPath(GameObject gameObject)
        {
            using (GenericPool<StringBuilder>.Get(out StringBuilder stringBuilder))
            {
                stringBuilder.Clear();
                Transform tform = gameObject.transform;
                while (tform != null)
                {
                    stringBuilder.Insert(0, tform.name);
                    stringBuilder.Insert(0, '/');
                    tform = tform.parent;
                }

                string gameObjectPath = stringBuilder.ToString().Trim();
                stringBuilder.Clear();
                return gameObjectPath;
            }
        }
    }
}