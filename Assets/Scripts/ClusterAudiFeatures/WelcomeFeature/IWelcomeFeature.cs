using System.Threading.Tasks;
using ClusterAudi;

namespace ClusterAudiFeatures
{
	public interface IWelcomeFeature : IFeature
	{
		Task InstantiateWelcomeFeature();
	}
}