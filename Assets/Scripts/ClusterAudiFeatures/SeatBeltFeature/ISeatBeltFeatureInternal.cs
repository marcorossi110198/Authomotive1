using ClusterAudi;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia interna per SeatBelt Feature
	/// IDENTICA al pattern ISpeedometerFeatureInternal seguendo modello Mercedes
	/// Definisce COME i MonoBehaviour accedono alla feature (contratto interno)
	/// </summary>
	public interface ISeatBeltFeatureInternal : IFeatureInternal
	{
		/// <summary>
		/// Ottiene il Client per accesso ai servizi
		/// IDENTICO al pattern GetClient() del Mercedes
		/// </summary>
		Client GetClient();

		/// <summary>
		/// Ottiene la configurazione corrente SeatBelt
		/// </summary>
		SeatBeltConfig GetCurrentConfiguration();

		/// <summary>
		/// Ottiene lo stato corrente di tutte le cinture
		/// </summary>
		SeatBeltData.SeatBeltStatus[] GetAllSeatBeltStates();

		/// <summary>
		/// Verifica se il sistema warning è attualmente attivo
		/// </summary>
		bool IsWarningSystemActive();

		/// <summary>
		/// Ottiene il tempo totale dall'inizio del warning corrente
		/// </summary>
		float GetCurrentWarningTime();
	}
}