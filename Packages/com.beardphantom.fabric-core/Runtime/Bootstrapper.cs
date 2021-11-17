using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace BeardPhantom.Fabric.Core
{
    public abstract class Bootstrapper : MonoBehaviour
    {
        #region Fields

        protected IPreBootstrapHandler PreHandler;

        protected IPostBootstrapHandler PostHandler;

        #endregion

        #region Properties

        [field: SerializeField]
        private GameObject ServicesPrefab { get; set; }

        #endregion

        #region Methods

        protected virtual void GetBootstrapSteps(List<IBootstrapStep> stepList)
        {
            stepList.Add(new CreateServiceLocatorBootstrapStep(ServicesPrefab));
        }

        protected virtual void AssignBootstrapHandlers(
            out IPreBootstrapHandler preHandler,
            out IPostBootstrapHandler postHandler)
        {
#if UNITY_EDITOR
            var editorBootstrapHandler = new EditorBootstrapHandler();
            preHandler = editorBootstrapHandler;
            postHandler = editorBootstrapHandler;
#else
            var bootstrapHandler = new BuildBootstrapHandler();
            preHandler = bootstrapHandler;
            postHandler = bootstrapHandler;
#endif
        }

        protected void Reset()
        {
            ResetGameObjectInHierarchy();
        }

        protected void OnValidate()
        {
            ResetGameObjectInHierarchy();
        }

        protected void ResetGameObjectInHierarchy()
        {
            transform.parent = null;
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.SetAsFirstSibling();
            name = "--BOOTSTRAP--";
        }

        [SuppressMessage("ReSharper", "Unity.IncorrectMethodSignature")]
        private async UniTaskVoid Start()
        {
            if (gameObject.scene.buildIndex != 0)
            {
                Destroy(gameObject);
            }
            else
            {
                AssignBootstrapHandlers(out PreHandler, out PostHandler);
                PreHandler.OnPreBootstrap(this);
                await BootstrapAppAsync();
                PostHandler.OnPostBootstrap(this);
            }
        }

        private async UniTask BootstrapAppAsync()
        {
            using (ListPool<IBootstrapStep>.Get(out var stepList))
            {
                GetBootstrapSteps(stepList);
                foreach (var step in stepList)
                {
                    FabricLog.Logger.Log($"Executing bootstrap step {step.GetType()}");
                    await step.ExecuteAsync();
                }
            }
        }

        #endregion
    }
}