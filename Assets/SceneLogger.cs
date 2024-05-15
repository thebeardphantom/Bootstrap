using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLogger : MonoBehaviour
{
    #region Methods

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
    }

    private static void OnPlaymodeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.ExitingPlayMode:
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
                break;
            }
        }
    }

    private static void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log($"Scene path: {arg0.path}");
        Debug.Log($"Scene name: {arg0.name}");
    }

    #endregion
}