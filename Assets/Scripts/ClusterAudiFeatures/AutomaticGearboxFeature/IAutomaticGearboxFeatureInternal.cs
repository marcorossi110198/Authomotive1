using ClusterAudi;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia interna per Tachometer Feature  
	/// IDENTICA al pattern ISpeedometerFeatureInternal del progetto Mercedes
	/// Definisce COME i MonoBehaviour accedono alla feature (contratto interno)
	/// </summary>
	public interface IAutomaticGearboxFeatureInternal : IFeatureInternal
	{
		/// <summary>
		/// Ottiene il Client per accesso ai servizi
		/// IDENTICO al pattern GetClient() del Mercedes
		/// </summary>
		Client GetClient();

		/// <summary>
		/// Ottiene configurazione corrente tachometer
		/// </summary>
		AutomaticGearboxConfig GetCurrentConfiguration();

		/// <summary>
		/// Verifica se red zone flash è abilitato
		/// </summary>
		bool IsRedZoneFlashEnabled();
	}
}