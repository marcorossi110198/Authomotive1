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
			if (Math.Abs(_currentSpeed - speed) > 0.1f)
			{
				float oldSpeed = _currentSpeed;
				_currentSpeed = Mathf.Clamp(speed, 0f, 300f);

				// 🆕 AUTOMATIC TRANSMISSION: Aggiorna marce automaticamente
				UpdateAutomaticGearShifting();

				// 🆕 AUTOMATIC TRANSMISSION: Aggiorna RPM basato su velocità e marcia
				UpdateRPMBasedOnSpeedAndGear();

				OnSpeedChanged?.Invoke(_currentSpeed);

				// 🆕 Notifica anche RPM changes (potrebbero essere cambiati con le marce)
				OnRPMChanged?.Invoke(_currentRPM);

				// Calcola accelerazione per modalità sportiva
				UpdateAcceleration(speed);

				// Aggiorna consumo basato su velocità
				UpdateConsumption();

				// Debug cambio automatico
				if (Mathf.Abs(oldSpeed - _currentSpeed) > 5f) // Solo per cambi significativi
				{
					Debug.Log($"[AUTO TRANSMISSION] Speed: {_currentSpeed:F0}km/h | Gear: {GetGearDisplayName()} | RPM: {_currentRPM:F0}");
				}
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

		/// <summary>
		/// SetGear - MODIFICARE per aggiornare RPM quando cambia marcia
		/// </summary>
		public void SetGear(int gear)
		{
			if (_currentGear != gear)
			{
				_currentGear = Mathf.Clamp(gear, -1, 8);

				// Update RPM per nuova marcia
				UpdateRPMBasedOnSpeedAndGear();

				OnGearChanged?.Invoke(_currentGear);
				OnRPMChanged?.Invoke(_currentRPM); // RPM cambiano con marcia

				Debug.Log($"[VEHICLE DATA SERVICE] ⚙️ Marcia: {GetGearDisplayName()}, RPM: {_currentRPM:F0}");
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

		#region Automatic Transmission Logic - NUOVO per AutomaticGearboxFeature

		/// <summary>
		/// Logica cambio automatico basata su velocità e RPM
		/// Simula un cambio automatico a 6 marce realistico
		/// </summary>

		// Configurazione cambio automatico
		private readonly float[] _gearRatios = new float[]
		{
			0f,    // P - Parcheggio
			3.5f,  // 1a marcia
			2.1f,  // 2a marcia  
			1.4f,  // 3a marcia
			1.0f,  // 4a marcia
			0.8f,  // 5a marcia
			0.65f  // 6a marcia
		};

		private readonly float[] _shiftUpSpeeds = new float[]
		{
			0f,    // P
			25f,   // 1a -> 2a a 25 km/h
			45f,   // 2a -> 3a a 45 km/h  
			70f,   // 3a -> 4a a 70 km/h
			95f,   // 4a -> 5a a 95 km/h
			125f   // 5a -> 6a a 125 km/h
		};

		private readonly float _redZoneRPM = 6500f;
		private readonly float _optimalShiftRPM = 3000f;

		/// <summary>
		/// Update automatico RPM basato su velocità e marcia
		/// </summary>
		private void UpdateRPMBasedOnSpeedAndGear()
		{
			if (!_isEngineRunning)
			{
				_currentRPM = 0f;
				return;
			}

			if (_currentSpeed <= 0f)
			{
				_currentRPM = 800f; // Idle RPM
				return;
			}

			if (_currentGear <= 0 || _currentGear >= _gearRatios.Length)
			{
				_currentRPM = 800f + (_throttlePosition * 1000f); // Neutral + throttle
				return;
			}

			// Calcola RPM basato su velocità e rapporto marcia
			float gearRatio = _gearRatios[_currentGear];
			float baseRPM = (_currentSpeed * gearRatio * 45f) + 800f;

			// Aggiungi influenza throttle (se implementato)
			float throttleInfluence = _throttlePosition * 800f;

			float targetRPM = baseRPM + throttleInfluence;

			// Applica damping basato su modalità guida
			float damping = _currentDriveMode switch
			{
				DriveMode.Eco => 0.85f,     // Smooth per efficienza
				DriveMode.Comfort => 0.75f, // Bilanciato
				DriveMode.Sport => 0.95f,   // Risposta immediata
				_ => 0.8f
			};

			_currentRPM = Mathf.Lerp(_currentRPM, targetRPM, damping);
			_currentRPM = Mathf.Clamp(_currentRPM, 800f, _maxRPM);
		}

		/// <summary>
		/// Update automatico marce basato su velocità
		/// </summary>
		private void UpdateAutomaticGearShifting()
		{
			if (!_isEngineRunning || _currentSpeed <= 0f)
			{
				// Motore spento o fermo = Parcheggio
				if (_currentGear != 0)
				{
					SetGear(0);
				}
				return;
			}

			// Logica cambio automatico
			if (_currentSpeed > 5f && _currentGear == 0)
			{
				// Inizio movimento = 1a marcia
				SetGear(1);
			}
			else if (ShouldShiftUp())
			{
				int newGear = Mathf.Min(_currentGear + 1, 6);
				SetGear(newGear);
			}
			else if (ShouldShiftDown())
			{
				int newGear = Mathf.Max(_currentGear - 1, 1);
				SetGear(newGear);
			}
		}

		/// <summary>
		/// Determina se dovrebbe fare shift up
		/// </summary>
		public bool ShouldShiftUp()
		{
			if (_currentGear >= 6 || _currentGear <= 0) return false;

			// Shift basato su velocità
			if (_currentSpeed > _shiftUpSpeeds[_currentGear])
				return true;

			return false;
		}

		/// <summary>
		/// Determina se dovrebbe fare shift down
		/// </summary>
		public bool ShouldShiftDown()
		{
			if (_currentGear <= 1) return false;

			// Shift down se velocità troppo bassa per marcia corrente
			float minSpeedForGear = _shiftUpSpeeds[_currentGear - 1] - 15f;

			return _currentSpeed < minSpeedForGear && _currentSpeed > 0f;
		}

		/// <summary>
		/// Controlla se è in red zone
		/// </summary>
		public bool IsInRedZone()
		{
			return _currentRPM > _redZoneRPM;
		}

		/// <summary>
		/// Ottiene soglia red zone
		/// </summary>
		public float GetRedZoneThreshold()
		{
			return _redZoneRPM;
		}

		/// <summary>
		/// Ottiene RPM ottimale per cambio marcia
		/// </summary>
		public float GetOptimalShiftPointRPM()
		{
			return _optimalShiftRPM;
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