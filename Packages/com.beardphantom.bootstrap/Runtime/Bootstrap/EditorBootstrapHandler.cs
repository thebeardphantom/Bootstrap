#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Bootstrap
{
    public class EditorBootstrapHandler : IPreBootstrapHandler, IPostBootstrapHandler
    {
        public const string EditModeState = "EDIT_MODE_STATE";

        public const string TempBootstrapperPath = "Temp/Bootstrap_Override.prefab";

        public static readonly EditorBootstrapHandler Instance = new();

        public static void LoadScenesInPlayMode(IReadOnlyList<string> scenePaths)
        {
            Assert.IsNotNull(scenePaths, "scenePaths != null");
            Assert.IsTrue(scenePaths.Count > 0, "scenePaths.Length > 0");

            var loadFirstAsSingle = true;
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.buildIndex != 0)
                {
                    loadFirstAsSingle = false;
                    break;
                }
            }

            for (var i = 0; i < scenePaths.Count; i++)
            {
                var path = scenePaths[i];
                var loadSceneParameters = new LoadSceneParameters(
                    loadFirstAsSingle && i == 0
                        ? LoadSceneMode.Single
                        : LoadSceneMode.Additive);
                EditorSceneManager.LoadSceneInPlayMode(path, loadSceneParameters);
            }
        }

        /// <inheritdoc />
        UniTask IPreBootstrapHandler.OnPreBootstrapAsync(in BootstrapContext context)
        {
            var editModeStateJson = SessionState.GetString(EditModeState, "");
            context.EditModeState = new EditModeState();
            if (string.IsNullOrWhiteSpace(editModeStateJson))
            {
                return default;
            }

            EditorJsonUtility.FromJsonOverwrite(editModeStateJson, context.EditModeState);
            return default;
        }

        /// <inheritdoc />
        async UniTask IPostBootstrapHandler.OnPostBootstrapAsync(BootstrapContext context, Bootstrapper bootstrapper)
        {
            if (App.IsRunningTests)
            {
                return;
            }

            var editModeScenePaths = context.EditModeState.LoadedScenes;
            if (editModeScenePaths == null || editModeScenePaths.Count == 0)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                LoadScenesInPlayMode(editModeScenePaths);
            }

            var serializedSelectedObjs = context.EditModeState.SelectedObjects;
            if (serializedSelectedObjs == null)
            {
                return;
            }

            await UniTask.NextFrame();

            // Restore editor selection
            using (HashSetPool<Object>.Get(out var selectedObjs))
            {
                foreach (var o in Selection.objects)
                {
                    selectedObjs.Add(o);
                }

                for (var i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    foreach (var serializedSelectedObj in serializedSelectedObjs)
                    {
                        if (scene.path != serializedSelectedObj.ScenePath)
                        {
                            continue;
                        }

                        var foundObj = GameObject.Find(serializedSelectedObj.ObjectPath);
                        if (foundObj == null)
                        {
                            continue;
                        }

                        selectedObjs.Add(foundObj);
                    }
                }

                await UniTask.NextFrame();
                Selection.objects = selectedObjs.ToArray();
            }
        }
    }
}

#endif