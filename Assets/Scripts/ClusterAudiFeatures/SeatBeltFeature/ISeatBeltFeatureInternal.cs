using ClusterAudi;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia interna SeatBelt - SEMPLIFICATA
	/// Solo metodi essenziali per il behaviour
	/// </summary>
	public interface ISeatBeltFeatureInternal : IFeatureInternal
	{
		/// <summary>
		/// Ottiene il Client
		/// </summary>
		Client GetClient();

		/// <summary>
		/// Ottiene stati attuali di tutte le cinture
		/// </summary>
		SeatBeltData.SeatBeltStatus[] GetAllSeatBeltStates();
	}
}