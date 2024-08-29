#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
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
        UniTask IPreBootstrapHandler.OnPreBootstrapAsync(in BootstrapContext context)
        {
            string editModeStateJson = SessionState.GetString(EditModeState, "");
            if (string.IsNullOrWhiteSpace(editModeStateJson))
            {
                return default;
            }

            context.EditModeState = JsonConvert.DeserializeObject<EditModeState>(editModeStateJson);
            return default;
        }

        /// <inheritdoc />
        async UniTask IPostBootstrapHandler.OnPostBootstrapAsync(BootstrapContext context, Bootstrapper bootstrapper)
        {
            if (App.IsRunningTests)
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

            await UniTask.NextFrame();

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