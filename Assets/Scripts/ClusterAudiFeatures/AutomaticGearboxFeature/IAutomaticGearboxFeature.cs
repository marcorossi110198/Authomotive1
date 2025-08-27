using System.Threading.Tasks;
using ClusterAudi;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia pubblica per AutomaticGearbox Feature
	/// IDENTICA al pattern ISpeedometerFeature seguendo modello Mercedes
	/// Definisce COSA può fare la feature (contratto pubblico)
	/// </summary>
	public interface IAutomaticGearboxFeature : IFeature
	{
		/// <summary>
		/// Istanzia la UI AutomaticGearbox completa
		/// IDENTICO al pattern InstantiateDashboardFeature() del Mercedes
		/// </summary>
		/// <returns>Task completato quando UI è istanziata</returns>
		Task InstantiateAutomaticGearboxFeature();

		/// <summary>
		/// Aggiorna configurazione per modalità guida
		/// </summary>
		void UpdateConfigurationForDriveMode(DriveMode mode);

		/// <summary>
		/// Forza aggiornamento red zone warning
		/// </summary>
		void TriggerRedZoneWarning(bool isInRedZone);

		/// <summary>
		/// Abilita/disabilita shift indicator
		/// </summary>
		void SetShiftIndicatorEnabled(bool enabled);
	}
}