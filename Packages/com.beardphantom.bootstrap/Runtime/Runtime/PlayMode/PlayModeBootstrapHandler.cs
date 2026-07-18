#if UNITY_EDITOR
using BeardPhantom.Bootstrap.EditMode;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Default pre/post bootstrap handler used when entering play mode in the editor. Restores the scenes and
    /// selection recorded in <see cref="EditModeState"/> before play mode was entered.
    /// </summary>
    [Serializable]
    public class PlayModeBootstrapHandler : IPreBootstrapHandler, IPostBootstrapHandler
    {
        /// <summary>
        /// The shared instance used as the default bootstrap handler when entering play mode.
        /// </summary>
        public static readonly PlayModeBootstrapHandler Instance = new();

        /// <summary>
        /// Loads the given scene paths in play mode. The first path replaces the currently loaded scene if only
        /// the initial bootstrap scene is loaded; otherwise all paths are loaded additively.
        /// </summary>
        /// <param name="scenePaths">The asset paths of the scenes to load.</param>
        public static void LoadScenesInPlayMode(IReadOnlyList<string> scenePaths)
        {
            Assert.IsNotNull(scenePaths, "scenePaths.IsNotNull()");
            Assert.IsTrue(scenePaths.Count > 0, "scenePaths.Length > 0");

            var loadFirstAsSingle = true;
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.buildIndex != 0)
                {
                    loadFirstAsSingle = false;
                    break;
                }
            }

            for (var i = 0; i < scenePaths.Count; i++)
            {
                string path = scenePaths[i];
                var loadSceneParameters = new LoadSceneParameters(
                    loadFirstAsSingle && i == 0
                        ? LoadSceneMode.Single
                        : LoadSceneMode.Additive);
                EditorSceneManager.LoadSceneInPlayMode(path, loadSceneParameters);
            }
        }

        /// <inheritdoc />
        Awaitable IPreBootstrapHandler.OnPreBootstrapAsync(in BootstrapContext context)
        {
            string editModeStateJson = SessionState.GetString(EditModeAppInstance.EditModeStateSessionStateKey, "");
            if (string.IsNullOrWhiteSpace(editModeStateJson))
            {
                return AwaitableUtility.Completed;
            }

            context.EditModeState = JsonConvert.DeserializeObject<EditModeState>(editModeStateJson);
            return AwaitableUtility.Completed;
        }

        /// <inheritdoc />
        async Awaitable IPostBootstrapHandler.OnPostBootstrapAsync(BootstrapContext context)
        {
            if (App.Instance.IsRunningTests)
            {
                return;
            }

            List<string> editModeScenePaths = context.EditModeState.LoadedScenes;
            if (editModeScenePaths.IsNull() || editModeScenePaths.Count == 0)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                LoadScenesInPlayMode(editModeScenePaths);
            }

            SelectedObjectPath[] serializedSelectedObjs = context.EditModeState.SelectedObjects;
            if (serializedSelectedObjs.IsNull() || serializedSelectedObjs.Length == 0)
            {
                return;
            }

            await Awaitable.NextFrameAsync();

            // Restore editor selection
            using (HashSetPool<Object>.Get(out HashSet<Object> selectedObjs))
            {
                foreach (Object o in Selection.objects)
                {
                    selectedObjs.Add(o);
                }

                for (var i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    foreach (SelectedObjectPath serializedSelectedObj in serializedSelectedObjs)
                    {
                        if (scene.path != serializedSelectedObj.ScenePath)
                        {
                            continue;
                        }

                        GameObject foundObj = GameObject.Find(serializedSelectedObj.ObjectPath);
                        if (!foundObj)
                        {
                            continue;
                        }

                        selectedObjs.Add(foundObj);
                    }
                }

                await Awaitable.NextFrameAsync();
                Selection.objects = selectedObjs.ToArray();
            }
        }
    }
}

#endif