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
    [Serializable]
    public class PlayModeBootstrapHandler : IPreBootstrapHandler, IPostBootstrapHandler
    {
        public static readonly PlayModeBootstrapHandler Instance = new();

        public static void LoadScenesInPlayMode(IReadOnlyList<string> scenePaths)
        {
            Assert.IsNotNull(scenePaths, "scenePaths != null");
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
                return AwaitableUtility.GetCompleted();
            }

            context.EditModeState = JsonConvert.DeserializeObject<EditModeState>(editModeStateJson);
            return AwaitableUtility.GetCompleted();
        }

        /// <inheritdoc />
        async Awaitable IPostBootstrapHandler.OnPostBootstrapAsync(BootstrapContext context)
        {
            if (App.Instance.IsRunningTests)
            {
                return;
            }

            List<string> editModeScenePaths = context.EditModeState.LoadedScenes;
            if (editModeScenePaths == null || editModeScenePaths.Count == 0)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                LoadScenesInPlayMode(editModeScenePaths);
            }

            SelectedObjectPath[] serializedSelectedObjs = context.EditModeState.SelectedObjects;
            if (serializedSelectedObjs == null || serializedSelectedObjs.Length == 0)
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