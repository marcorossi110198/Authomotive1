using System.Threading.Tasks;
using ClusterAudi;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia pubblica per SeatBelt Feature
	/// IDENTICA al pattern ISpeedometerFeature seguendo modello Mercedes
	/// Definisce COSA può fare la feature (contratto pubblico)
	/// </summary>
	public interface ISeatBeltFeature : IFeature
	{
		/// <summary>
		/// Istanzia la UI SeatBelt completa
		/// IDENTICO al pattern InstantiateDashboardFeature() del Mercedes
		/// </summary>
		/// <returns>Task completato quando UI è istanziata</returns>
		Task InstantiateSeatBeltFeature();

		/// <summary>
		/// Imposta lo stato di una specifica cintura
		/// </summary>
		/// <param name="position">Posizione della cintura (Driver, Passenger, etc.)</param>
		/// <param name="status">Nuovo stato (Fastened, Unfastened, etc.)</param>
		void SetSeatBeltStatus(SeatBeltData.SeatBeltPosition position, SeatBeltData.SeatBeltStatus status);

		/// <summary>
		/// Aggiorna configurazione per modalità guida
		/// </summary>
		/// <param name="mode">Nuova modalità guida</param>
		void UpdateConfigurationForDriveMode(DriveMode mode);

		/// <summary>
		/// Forza check del sistema cinture (per testing/debug)
		/// </summary>
		void ForceWarningCheck();

		/// <summary>
		/// Abilita/disabilita completamente il sistema cinture
		/// </summary>
		/// <param name="enabled">True per abilitare, false per disabilitare</param>
		void SetSeatBeltSystemEnabled(bool enabled);
	}
}