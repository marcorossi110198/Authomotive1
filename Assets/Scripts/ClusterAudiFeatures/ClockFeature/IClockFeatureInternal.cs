using ClusterAudi;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia interna per Clock Feature - SEMPLICE
	/// IDENTICA al pattern ISeatBeltFeatureInternal che hai già implementato
	/// </summary>
	public interface IClockFeatureInternal : IFeatureInternal
	{
		/// <summary>
		/// Ottiene il Client per accesso ai servizi
		/// IDENTICO al pattern GetClient() del Mercedes
		/// </summary>
		Client GetClient();
	}
}