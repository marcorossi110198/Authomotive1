using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudi
{
    public interface IAssetService : IService
    {
        public Task<T> Load<T>(string path) where T : Object;
        public Task<T> InstantiateAsset<T>(string path, Transform parent = null) where T : Object;
    }
}
