using System;
using UnityEngine;

namespace ClusterAudi
{
	/// <summary>
	/// Implementazione del servizio dati veicolo.
	/// Gestisce tutti i dati automotive e notifica i cambiamenti.
	/// </summary>
	public class VehicleDataService : IVehicleDataService
	{
		#region Private Fields

		private float _currentSpeed = 0f;
		private float _currentRPM = 0f;
		private int _currentGear = 0;
		private DriveMode _currentDriveMode = DriveMode.Comfort;
		private bool _isEngineRunning = false;

		// Dati avanzati per modalità sportiva
		private float _throttlePosition = 0f;
		private float _brakeForce = 0f;
		private float _acceleration = 0f;
		private float _maxRPM = 7000f;

		// Dati per efficienza (modalità Eco)
		private float _currentConsumption = 12f; // L/100km
		private float _estimatedRange = 450f; // km

		#endregion

		#region Public Properties

		public float CurrentSpeed => _currentSpeed;
		public float CurrentRPM => _currentRPM;
		public int CurrentGear => _currentGear;
		public DriveMode CurrentDriveMode => _currentDriveMode;
		public bool IsEngineRunning => _isEngineRunning;

		#endregion

		#region Events

		public event Action<float> OnSpeedChanged;
		public event Action<float> OnRPMChanged;
		public event Action<int> OnGearChanged;
		public event Action<DriveMode> OnDriveModeChanged;

		#endregion

		#region Constructor

		public VehicleDataService()
		{
			Debug.Log("[VEHICLE DATA SERVICE] 🚗 Servizio dati veicolo inizializzato");

			// Valori di default per testing
			_currentSpeed = 0f;
			_currentRPM = 800f; // RPM idle
			_currentGear = 0; // Parcheggio
			_currentDriveMode = DriveMode.Comfort;
			_isEngineRunning = true;
		}

		#endregion

		#region Public Methods

		public void SetSpeed(float speed)
		{
			if (Math.Abs(_currentSpeed - speed) > 0.1f) // Evita update troppo frequenti
			{
				_currentSpeed = Mathf.Clamp(speed, 0f, 300f); // Max 300 km/h
				OnSpeedChanged?.Invoke(_currentSpeed);

				// Calcola accelerazione per modalità sportiva
				UpdateAcceleration(speed);

				// Aggiorna consumo basato su velocità
				UpdateConsumption();
			}
		}

		public void SetRPM(float rpm)
		{
			if (Math.Abs(_currentRPM - rpm) > 10f) // Evita micro-update
			{
				_currentRPM = Mathf.Clamp(rpm, 0f, _maxRPM);
				OnRPMChanged?.Invoke(_currentRPM);

				// Auto-shift basato su RPM (simulazione)
				UpdateGearBasedOnRPM();
			}
		}

		public void SetGear(int gear)
		{
			if (_currentGear != gear)
			{
				_currentGear = Mathf.Clamp(gear, -1, 8); // -1=Retro, 0=Parcheggio, 1-8=Marce
				OnGearChanged?.Invoke(_currentGear);
				Debug.Log($"[VEHICLE DATA SERVICE] ⚙️ Marcia cambiata: {GetGearDisplayName()}");
			}
		}

		public void SetDriveMode(DriveMode mode)
		{
			if (_currentDriveMode != mode)
			{
				var previousMode = _currentDriveMode;
				_currentDriveMode = mode;
				OnDriveModeChanged?.Invoke(_currentDriveMode);

				Debug.Log($"[VEHICLE DATA SERVICE] 🏁 Drive Mode: {previousMode} → {_currentDriveMode}");

				// Configura parametri specifici per modalità
				ConfigureForDriveMode(mode);
			}
		}

		public void SetEngineRunning(bool isRunning)
		{
			if (_isEngineRunning != isRunning)
			{
				_isEngineRunning = isRunning;
				Debug.Log($"[VEHICLE DATA SERVICE] 🔧 Motore: {(_isEngineRunning ? "ACCESO" : "SPENTO")}");

				if (!_isEngineRunning)
				{
					_currentRPM = 0f;
					_currentSpeed = 0f;
					OnRPMChanged?.Invoke(_currentRPM);
					OnSpeedChanged?.Invoke(_currentSpeed);
				}
				else
				{
					_currentRPM = 800f; // RPM idle
					OnRPMChanged?.Invoke(_currentRPM);
				}
			}
		}

		#endregion

		#region Extended Methods for Advanced Features

		/// <summary>
		/// Metodi aggiuntivi per funzionalità avanzate degli stati
		/// </summary>

		public float GetThrottlePosition() => _throttlePosition;
		public float GetBrakeForce() => _brakeForce;
		public float GetAcceleration() => _acceleration;
		public float GetMaxRPM() => _maxRPM;

		public float GetCurrentConsumption() => _currentConsumption;
		public float GetEstimatedRange() => _estimatedRange;

		// Metodi per calcoli avanzati modalità Sport
		public float GetAccelerationSmoothness() => CalculateAccelerationSmoothness();
		public float GetSpeedStability() => CalculateSpeedStability();
		public float GetGearUsageOptimality() => CalculateGearUsageOptimality();
		public float GetDrivingEfficiency() => CalculateDrivingEfficiency();
		public float GetRPMEfficiency() => CalculateRPMEfficiency();
		public float GetAccelerationControl() => CalculateAccelerationControl();
		public float GetCorneringPerformance() => CalculateCorneringPerformance();
		public float GetRawSpeed() => _currentSpeed + UnityEngine.Random.Range(-2f, 2f); // Simula dati non filtrati

		public void SetThrottlePosition(float throttle)
		{
			_throttlePosition = Mathf.Clamp01(throttle);
		}

		public void SetBrakeForce(float brake)
		{
			_brakeForce = Mathf.Clamp01(brake);
		}

		#endregion

		#region Private Helper Methods

		private void UpdateAcceleration(float newSpeed)
		{
			_acceleration = (newSpeed - _currentSpeed) / Time.fixedDeltaTime;
		}

		private void UpdateConsumption()
		{
			// Calcolo consumo semplificato basato su velocità e RPM
			float baseConsumption = 8f; // L/100km base
			float speedFactor = (_currentSpeed / 100f) * 0.3f;
			float rpmFactor = (_currentRPM / 3000f) * 0.2f;

			_currentConsumption = baseConsumption + speedFactor + rpmFactor;

			// Aggiorna autonomia
			_estimatedRange = 450f - (_currentConsumption * 10f);
		}

		private void UpdateGearBasedOnRPM()
		{
			if (!_isEngineRunning) return;

			// Logica cambio automatico semplificata
			if (_currentRPM > 3000f && _currentGear < 6 && _currentSpeed > 20f)
			{
				SetGear(_currentGear + 1);
			}
			else if (_currentRPM < 1500f && _currentGear > 1 && _currentSpeed < 80f)
			{
				SetGear(_currentGear - 1);
			}
		}

		private void ConfigureForDriveMode(DriveMode mode)
		{
			switch (mode)
			{
				case DriveMode.Eco:
					_maxRPM = 5000f; // Limita RPM per efficienza
					Debug.Log("[VEHICLE DATA SERVICE] 🌱 Configurazione ECO: RPM limitati, focus efficienza");
					break;

				case DriveMode.Comfort:
					_maxRPM = 6500f; // RPM bilanciati
					Debug.Log("[VEHICLE DATA SERVICE] 🛣️ Configurazione COMFORT: Bilanciamento performance/efficienza");
					break;

				case DriveMode.Sport:
					_maxRPM = 7000f; // RPM massimi
					Debug.Log("[VEHICLE DATA SERVICE] 🏁 Configurazione SPORT: Performance massime, RPM estesi");
					break;
			}
		}

		private string GetGearDisplayName()
		{
			return _currentGear switch
			{
				-1 => "R",
				0 => "P",
				_ => _currentGear.ToString()
			};
		}

		#region Performance Calculation Methods

		private float CalculateAccelerationSmoothness()
		{
			// Simula calcolo dolcezza accelerazione (0-1)
			return Mathf.Clamp01(1f - (Mathf.Abs(_acceleration) / 5f));
		}

		private float CalculateSpeedStability()
		{
			// Simula stabilità velocità (0-1)
			return UnityEngine.Random.Range(0.6f, 0.95f);
		}

		private float CalculateGearUsageOptimality()
		{
			// Simula uso ottimale marce (0-1)
			if (_currentGear == 0) return 1f;

			float optimalRPMRange = Mathf.InverseLerp(1500f, 3000f, _currentRPM);
			return Mathf.Clamp01(1f - Mathf.Abs(optimalRPMRange - 0.5f) * 2f);
		}

		private float CalculateDrivingEfficiency()
		{
			// Efficienza generale di guida (0-1)
			float speedEfficiency = _currentSpeed < 90f ? 0.8f : 0.6f;
			float rpmEfficiency = _currentRPM < 2500f ? 0.9f : 0.7f;
			return (speedEfficiency + rpmEfficiency) / 2f;
		}

		private float CalculateRPMEfficiency()
		{
			// Efficienza uso RPM (0-1)
			return Mathf.InverseLerp(_maxRPM, 2000f, _currentRPM);
		}

		private float CalculateAccelerationControl()
		{
			// Controllo accelerazione (0-1)
			return Mathf.Clamp01(1f - Mathf.Abs(_acceleration) / 3f);
		}

		private float CalculateCorneringPerformance()
		{
			// Simula performance in curva (0-1)
			return UnityEngine.Random.Range(0.7f, 0.9f);
		}

		#endregion

		#endregion
	}

	/// <summary>
	/// Enum per le modalità di guida
	/// </summary>
	public enum DriveMode
	{
		Eco,     // Modalità ecologica - efficienza massima
		Comfort, // Modalità comfort - bilanciamento
		Sport    // Modalità sportiva - performance massima
	}
}