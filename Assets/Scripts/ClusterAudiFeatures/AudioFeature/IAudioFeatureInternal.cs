using ClusterAudi;

namespace ClusterAudiFeatures
{
	public interface IAudioFeatureInternal : IFeatureInternal
	{
		Client GetClient();
	}
}