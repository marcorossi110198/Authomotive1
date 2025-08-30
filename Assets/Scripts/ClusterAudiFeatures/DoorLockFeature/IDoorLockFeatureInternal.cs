using ClusterAudi;

namespace ClusterAudiFeatures
{
	public interface IDoorLockFeatureInternal : IFeatureInternal
	{
		Client GetClient();
		bool IsLocked { get; }
		void SetLocked(bool locked);
		void ToggleLock();
	}
}