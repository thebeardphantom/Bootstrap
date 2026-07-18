using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Serializable reference to an object's location, stored as a scene path plus a hierarchy path, so the
    /// object can be re-resolved later without holding a direct <see cref="Object"/> reference.
    /// </summary>
    [Serializable]
    public class SelectedObjectPath
    {
        /// <summary>
        /// The asset path of the scene containing the referenced object.
        /// </summary>
        public string ScenePath { get; set; }

        /// <summary>
        /// The slash-separated hierarchy path of the referenced object within its scene.
        /// </summary>
        public string ObjectPath { get; set; }

        /// <summary>
        /// Creates a <see cref="SelectedObjectPath"/> for a <see cref="GameObject"/> or <see cref="Component"/>.
        /// </summary>
        /// <param name="obj">The <see cref="GameObject"/> or <see cref="Component"/> to reference.</param>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is not a <see cref="GameObject"/> or <see cref="Component"/>.</exception>
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