using ClusterAudi;
using System.Threading.Tasks;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia pubblica per Cluster Drive Mode Feature
	/// IDENTICA al pattern IDashboardFeature del progetto Mercedes
	/// </summary>
	public interface IClusterDriveModeFeature : IFeature
	{
		/// <summary>
		/// Istanzia l'UI per le modalità di guida del cluster
		/// IDENTICO al pattern InstantiateDashboardFeature() del Mercedes
		/// </summary>
		Task InstantiateClusterDriveModeFeature();
	}
}