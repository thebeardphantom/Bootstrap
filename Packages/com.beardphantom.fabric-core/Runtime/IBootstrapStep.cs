using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BeardPhantom.Fabric.Core
{
    public interface IBootstrapStep
    {
        #region Methods

        UniTask ExecuteAsync();

        #endregion
    }

    public class CreateServiceLocatorBootstrapStep : IBootstrapStep
    {
        #region Fields

        private readonly GameObject _servicesPrefab;

        #endregion

        #region Constructors

        public CreateServiceLocatorBootstrapStep(GameObject servicesPrefab)
        {
            _servicesPrefab = servicesPrefab;
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public UniTask ExecuteAsync()
        {
            return App.Instance.ServiceLocator.CreateAsync(_servicesPrefab);
        }

        #endregion
    }
}