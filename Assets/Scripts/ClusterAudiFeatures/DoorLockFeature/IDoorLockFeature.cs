using ClusterAudi;
using System.Threading.Tasks;

namespace ClusterAudiFeatures
{
	public interface IDoorLockFeature : IFeature
	{
		Task InstantiateDoorLockFeature();
		bool IsLocked { get; }
		void ToggleLock();
	}
}