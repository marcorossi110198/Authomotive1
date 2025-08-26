using System.Threading.Tasks;
using ClusterAudi;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia pubblica per Speedometer Feature
	/// IDENTICA al pattern IClusterDriveModeFeature seguendo modello Mercedes
	/// Definisce COSA può fare la feature (contratto pubblico)
	/// </summary>
	public interface ISpeedometerFeature : IFeature
	{
		/// <summary>
		/// Istanzia la UI Speedometer completa
		/// IDENTICO al pattern InstantiateDashboardFeature() del Mercedes
		/// </summary>
		/// <returns>Task completato quando UI è istanziata</returns>
		Task InstantiateSpeedometerFeature();

		/// <summary>
		/// Aggiorna unità di misura velocità (km/h ↔ mph)
		/// </summary>
		void SetSpeedUnit(SpeedometerData.SpeedUnit unit);

		/// <summary>
		/// Aggiorna configurazione per modalità guida
		/// </summary>
		void UpdateConfigurationForDriveMode(DriveMode mode);
	}
}