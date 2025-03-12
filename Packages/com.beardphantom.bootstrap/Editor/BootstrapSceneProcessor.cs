using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Bootstrap.Editor
{
    internal class BootstrapSceneProcessor : IProcessSceneWithReport
    {
        /// <inheritdoc />
        public int callbackOrder { get; }

        /// <inheritdoc />
        void IProcessSceneWithReport.OnProcessScene(Scene scene, BuildReport report)
        {
            if (scene.buildIndex == 0)
            {
                return;
            }

            using var _ = ListPool<GameObject>.Get(out var rootGameObjects);
            scene.GetRootGameObjects(rootGameObjects);
            foreach (var rootGameObject in rootGameObjects)
            {
                var bootstrapper = rootGameObject.GetComponentInChildren<Bootstrapper>(true);
                if (bootstrapper == null)
                {
                    continue;
                }

                Logging.Info($"Destroying Bootstrapper {bootstrapper.name} in non-bootstrap scene '{scene.path}'");
                Object.DestroyImmediate(bootstrapper.gameObject);
            }
        }
    }
}