#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap.EditMode
{
    public static class EditModeBootstrapping
    {
        public const string EditModeServicesInstanceIdSessionStateKey = "EditModeServicesIId";

        public static bool TryGetServicesInstance(out EditModeServicesInstance instance)
        {
            int instanceId = SessionState.GetInt(EditModeServicesInstanceIdSessionStateKey, 0);
            if (!Resources.InstanceIDIsValid(instanceId))
            {
                instance = null;
                return false;
            }

            instance = Resources.InstanceIDToObject(instanceId) as EditModeServicesInstance;
            return instance;
        }

        // public static void Cleanup()
        // {
        //     if (App.TryGetInstance<EditModeAppInstance>(out _))
        //     {
        //         App.Deinitialize();
        //     }
        //
        //     if (TryGetServicesInstance(out EditModeServicesInstance servicesInstance))
        //     {
        //         Object.DestroyImmediate(servicesInstance, false);
        //     }
        //
        //     SessionState.EraseInt(EditModeServicesInstanceIdSessionStateKey);
        // }
    }
}
#endif