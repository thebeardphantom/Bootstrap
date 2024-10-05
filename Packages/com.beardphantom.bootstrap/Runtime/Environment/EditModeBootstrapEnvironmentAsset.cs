using UnityEngine;

namespace BeardPhantom.Bootstrap.Environment
{
    [CreateAssetMenu(menuName = "Bootstrap/Edit Mode Environment")]
    public class EditModeBootstrapEnvironmentAsset : BootstrapEnvironmentAsset
    {
        [field: SerializeField]
        [field: DisallowSceneObjects]
        public GameObject ServicesPrefab { get; private set; }

        internal override GameObject StartEnvironment()
        {
            GameObject servicesInstance = BootstrapUtility.InstantiateAsInactive(ServicesPrefab);
            servicesInstance.name = $"{ServicesPrefab.name} Instance";
            return servicesInstance;
        }
    }
}