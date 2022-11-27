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
        #region Fields

        public const string EDIT_MODE_STATE = "EDIT_MODE_STATE";

        private EditModeState _editModeState;

        #endregion

        #region Methods

        public static void OverwriteFromSerializedObject(Bootstrapper bootstrapper, string json)
        {
            if (!string.IsNullOrWhiteSpace(json))
            {
                EditorJsonUtility.FromJsonOverwrite(json, bootstrapper);
            }
        }

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
        public UniTask OnPreBootstrapAsync(Bootstrapper bootstrapper)
        {
            var editModeStateJson = SessionState.GetString(EDIT_MODE_STATE, "");
            if (string.IsNullOrWhiteSpace(editModeStateJson))
            {
                return default;
            }

            _editModeState = new EditModeState();
            EditorJsonUtility.FromJsonOverwrite(editModeStateJson, _editModeState);
            OverwriteFromSerializedObject(bootstrapper, _editModeState.BootstrapperJson);
            return default;
        }

        /// <inheritdoc />
        public async UniTask OnPostBootstrap(Bootstrapper bootstrapper)
        {
            var editModeScenePaths = _editModeState?.LoadedScenes;
            if (editModeScenePaths == null || editModeScenePaths.Length == 0)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                LoadScenesInPlayMode(editModeScenePaths);
            }

            var serializedSelectedObjs = _editModeState.SelectedObjects;
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
                        if (foundObj != null)
                        {
                            selectedObjs.Add(foundObj);
                        }
                    }
                }


                Selection.objects = selectedObjs.ToArray();
            }
        }

        #endregion
    }
}

#endif