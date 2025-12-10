using System.Linq;
using UnityEditor;

public static class EditorUtility
{
    [MenuItem("Assets/Reserialize")]
    private static void ReserializeAssets()
    {
        string[] selection = Selection.assetGUIDs;
        bool noSelection = selection?.Length == 0;
        var confirmation = true;
        if (noSelection)
        {
            confirmation = UnityEditor.EditorUtility.DisplayDialog(
                "Reserialize All?",
                "Are you sure?",
                "Yes",
                "No");
        }

        if (!confirmation)
        {
            return;
        }

        if (noSelection)
        {
            AssetDatabase.ForceReserializeAssets();
        }
        else
        {
            string[] paths = selection.Select(AssetDatabase.GUIDToAssetPath).ToArray();
            AssetDatabase.ForceReserializeAssets(paths);
        }

        AssetDatabase.SaveAssets();
    }
}