using System;

namespace ClusterAudi
{
	/// <summary>
	/// Interface per VehicleDataService - VERSIONE COMPLETA.
	/// Include tutti i metodi avanzati utilizzati dagli stati delle modalità.
	/// </summary>
	public interface IVehicleDataService : IService
	{
		#region Properties Base (già esistenti)

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

		#endregion

		#region Metodi Avanzati per Stati Modalità (AGGIUNTI)

		// Dati di base avanzati
		float GetThrottlePosition();
		float GetBrakeForce();
		float GetAcceleration();
		float GetMaxRPM();

		// Dati efficienza (per EcoMode)
		float GetCurrentConsumption();
		float GetEstimatedRange();

		// Metodi per calcoli avanzati (per SportMode)
		float GetAccelerationSmoothness();
		float GetSpeedStability();
		float GetGearUsageOptimality();
		float GetDrivingEfficiency();
		float GetRPMEfficiency();
		float GetAccelerationControl();
		float GetCorneringPerformance();
		float GetRawSpeed(); // Per ComfortMode smoothing

		// Metodi di configurazione avanzata
		void SetThrottlePosition(float throttle);
		void SetBrakeForce(float brake);

		// metodi per controlli tachimetro

		bool IsInRedZone();
		float GetRedZoneThreshold();
		float GetOptimalShiftPointRPM();
		bool ShouldShiftUp();
		bool ShouldShiftDown();

		#endregion
	}
}