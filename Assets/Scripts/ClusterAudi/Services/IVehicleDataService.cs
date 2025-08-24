using System;

namespace ClusterAudi
{
	/// <summary>
	/// Interface per VehicleDataService.
	/// Segue pattern identico a IAssetService del progetto base.
	/// </summary>
	public interface IVehicleDataService : IService
	{
		// Properties di sola lettura per accesso ai dati
		float CurrentSpeed { get; }
		float CurrentRPM { get; }
		int CurrentGear { get; }
		DriveMode CurrentDriveMode { get; }
		bool IsEngineRunning { get; }

		// Eventi per notificare cambiamenti
		event Action<float> OnSpeedChanged;
		event Action<float> OnRPMChanged;
		event Action<int> OnGearChanged;
		event Action<DriveMode> OnDriveModeChanged;

		// Metodi per aggiornare i dati
		void SetSpeed(float speed);
		void SetRPM(float rpm);
		void SetGear(int gear);
		void SetDriveMode(DriveMode mode);
		void SetEngineRunning(bool isRunning);
	}
}