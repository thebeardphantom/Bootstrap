using UnityEditor;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor
{
    [FilePath("UserSettings/EditorBootstrapSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class EditorBootstrapSingleton : ScriptableSingleton<EditorBootstrapSingleton>
    {
        #region Properties

        [field: SerializeField]
        private string EditModeServicesIdStr { get; set; } = string.Empty;

        #endregion

        #region Methods

        public bool TryGetActiveEditModeServices(out EditModeServices editModeServices)
        {
            editModeServices = default;
            if (!GlobalObjectId.TryParse(EditModeServicesIdStr, out var editModeServicesId))
            {
                EditModeServicesIdStr = string.Empty;
                return false;
            }

            editModeServices = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(editModeServicesId) as EditModeServices;
            if (editModeServices == null)
            {
                EditModeServicesIdStr = string.Empty;
                return false;
            }

            return true;
        }

        public void SetActiveEditModeServices(EditModeServices editModeServices)
        {
            EditModeServicesIdStr = GlobalObjectId.GetGlobalObjectIdSlow(editModeServices).ToString();
            Save(true);
        }

        #endregion
    }
}