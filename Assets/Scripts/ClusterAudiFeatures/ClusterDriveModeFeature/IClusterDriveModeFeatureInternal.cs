using ClusterAudi;


namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia interna per Cluster Drive Mode Feature  
	/// IDENTICA al pattern IDashboardFeatureInternal del progetto Mercedes
	/// </summary>
	public interface IClusterDriveModeFeatureInternal : IFeatureInternal
	{
		/// <summary>
		/// Ottiene il Client per accesso ai servizi
		/// IDENTICO al pattern GetClient() del Mercedes
		/// </summary>
		Client GetClient();
	}
}