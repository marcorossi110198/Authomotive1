using UnityEngine;

namespace ClusterAudi {
    public class BaseMonoBehaviour<TFeatureInternal> : MonoBehaviour where TFeatureInternal : IFeatureInternal
    {
        protected TFeatureInternal _feature;
        protected IBroadcaster _featureBroadcaster;

        private bool _isInitialized = false;

        public void Initialize(TFeatureInternal feature)
        {
            _feature = feature;
            _featureBroadcaster = feature.FeatureBroadcaster;

            ManagedAwake();
            ManagedStart();

            _isInitialized = true;
        }

        private void Update()
        {
            if (_isInitialized)
            {
                ManagedUpdate();
            }
        }

        private void OnDestroy()
        {
            ManagedOnDestroy();
        }

        protected virtual void ManagedAwake()
        {
        }

        protected virtual void ManagedStart()
        {
            
        }

        protected virtual void ManagedUpdate()
        {

        }

        protected virtual void ManagedOnDestroy()
        {

        }
    }
}