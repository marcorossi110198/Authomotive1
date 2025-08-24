using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudi
{
    public class AssetService : IAssetService
    {
        private Dictionary<string, Object> _myAssets;
        private Dictionary<string, ResourceRequest> _currentlyLoadingAssets;

        public AssetService()
        {
            _myAssets = new Dictionary<string, Object>();
            _currentlyLoadingAssets = new Dictionary<string, ResourceRequest>();
        }

        public async Task<T> Load<T>(string path) where T : Object
        {
            if (_myAssets.ContainsKey(path))
            {
                return (T)_myAssets[path];
            }

            if (!_currentlyLoadingAssets.ContainsKey(path))
            {
                _currentlyLoadingAssets[path] = Resources.LoadAsync<T>(path);
            }

            ResourceRequest myLoadingProcess = _currentlyLoadingAssets[path];

            while (!myLoadingProcess.isDone)
            {
                await Task.Delay(1);
            }

            _currentlyLoadingAssets.Remove(path);

            _myAssets[path] = myLoadingProcess.asset;

            return (T)_myAssets[path];
        }

        public async Task<T> InstantiateAsset<T>(string path, Transform parent = null) where T : Object
        {
            T asset = await Load<T>(path);
            return asset == null ? null : Object.Instantiate(asset, parent);
        }
    }
}

