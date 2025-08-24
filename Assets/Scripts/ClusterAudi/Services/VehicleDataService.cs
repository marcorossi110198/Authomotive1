using System;

namespace ClusterAudi
{
	/// <summary>
	/// Servizio centrale per gestione dati del veicolo.
	/// Pattern identico ad AssetService del progetto base.
	/// </summary>
	public class VehicleDataService : IVehicleDataService
	{
		// Dati correnti del veicolo
		private float _currentSpeed;
		private float _currentRPM;
		private int _currentGear;
		private DriveMode _currentDriveMode;
		private bool _isEngineRunning;

		// Eventi per notificare cambiamenti
		public event Action<float> OnSpeedChanged;
		public event Action<float> OnRPMChanged;
		public event Action<int> OnGearChanged;
		public event Action<DriveMode> OnDriveModeChanged;

		public VehicleDataService()
		{
			// Inizializzazione con valori di default
			_currentSpeed = 0f;
			_currentRPM = 800f; // RPM idle
			_currentGear = 1;
			_currentDriveMode = DriveMode.Comfort;
			_isEngineRunning = false;
		}

		// Properties per accesso ai dati
		public float CurrentSpeed => _currentSpeed;
		public float CurrentRPM => _currentRPM;
		public int CurrentGear => _currentGear;
		public DriveMode CurrentDriveMode => _currentDriveMode;
		public bool IsEngineRunning => _isEngineRunning;

		// Metodi per aggiornare i dati
		public void SetSpeed(float speed)
		{
			if (_currentSpeed != speed)
			{
				_currentSpeed = UnityEngine.Mathf.Max(0f, speed); // Non può essere negativa
				OnSpeedChanged?.Invoke(_currentSpeed);
			}
		}

		public void SetRPM(float rpm)
		{
			if (_currentRPM != rpm)
			{
				_currentRPM = UnityEngine.Mathf.Clamp(rpm, 600f, 8000f); // Range realistico
				OnRPMChanged?.Invoke(_currentRPM);
			}
		}

		public void SetGear(int gear)
		{
			if (_currentGear != gear)
			{
				_currentGear = UnityEngine.Mathf.Clamp(gear, 1, 6); // Marce 1-6
				OnGearChanged?.Invoke(_currentGear);
			}
		}

		public void SetDriveMode(DriveMode mode)
		{
			if (_currentDriveMode != mode)
			{
				_currentDriveMode = mode;
				OnDriveModeChanged?.Invoke(_currentDriveMode);
			}
		}

		public void SetEngineRunning(bool isRunning)
		{
			_isEngineRunning = isRunning;
			if (!isRunning)
			{
				SetRPM(0f);
				SetSpeed(0f);
			}
		}
	}

	/// <summary>
	/// Modalità di guida disponibili
	/// </summary>
	public enum DriveMode
	{
		Eco,
		Comfort,
		Sport
	}
}