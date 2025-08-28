using ClusterAudi;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia interna per Audio Feature
	/// IDENTICA al pattern ISeatBeltFeatureInternal
	/// </summary>
	public interface IAudioFeatureInternal : IFeatureInternal
	{
		/// <summary>
		/// Ottiene il Client - IDENTICO pattern Mercedes
		/// </summary>
		Client GetClient();
	}
}