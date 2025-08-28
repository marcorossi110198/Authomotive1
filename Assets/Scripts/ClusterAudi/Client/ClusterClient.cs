using ClusterAudiFeatures;
using UnityEngine;

namespace ClusterAudi
{
	/// <summary>
	/// Client concreto per il Cluster Audi.
	/// Segue ESATTAMENTE il pattern di DashboardClient.cs del progetto Mercedes.
	/// </summary>
	public class ClusterClient : Client
	{
		[Header("Cluster Configuration")]
		[SerializeField] private bool _autoStartOnAwake = true;
		[SerializeField] private bool _enableDebugMode = true;

		private ClusterFSM _clusterStateMachine;

		public override void InitFeatures()
		{
			Debug.Log("[CLUSTER CLIENT] 🚗 Inizializzazione Features...");

			// Nelle prossime fasi aggiungeremo WelcomeFeature, DriveModeFeature, etc.

			// TODO: Aggiungere features quando le implementiamo:
			// Features.Add<IDriveModeFeature>(new DriveModeFeature(this));

			// AudioFeature
			Features.Add<IAudioFeature>(new AudioFeature(this));

			// WelcomeFeature
			Features.Add<IWelcomeFeature>(new WelcomeFeature(this));

			// ClusterDriveModeFeature
			Features.Add<IClusterDriveModeFeature>(new ClusterDriveModeFeature(this));

			// Registra SpeedometerFeature
			Features.Add<ISpeedometerFeature>(new SpeedometerFeature(this));

			// AutomaticGearboxFeature
			Features.Add<IAutomaticGearboxFeature>(new AutomaticGearboxFeature(this));

			// SeatBeltFeature
			Features.Add<ISeatBeltFeature>(new SeatBeltFeature(this));

			// ClockFeature
			Features.Add<IClockFeature>(new ClockFeature(this));

			// DoorLockFeature
			Features.Add<IDoorLockFeature>(new DoorLockFeature(this));

			Debug.Log("[CLUSTER CLIENT] ✅ Features inizializzate (placeholder per ora)");
		}

		public override void StartClient()
		{
			Debug.Log("[CLUSTER CLIENT] 🚀 Avvio Cluster Client...");

			// Verifica che i servizi siano inizializzati
			ValidateServices();

			// Avvia il flusso di startup
			ClusterStartUpFlow startupFlow = new ClusterStartUpFlow();
			startupFlow.BeginStartUp(this);
		}

		#region Validation & Debug

		private void ValidateServices()
		{
			Debug.Log("[CLUSTER CLIENT] 🔍 Validazione servizi...");

			try
			{
				var broadcaster = Services.Get<IBroadcaster>();
				var assetService = Services.Get<IAssetService>();
				var vehicleDataService = Services.Get<IVehicleDataService>();

				Debug.Log("[CLUSTER CLIENT] ✅ Tutti i servizi sono disponibili");

				if (_enableDebugMode)
				{
					LogServiceInfo(broadcaster, assetService, vehicleDataService);
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[CLUSTER CLIENT] ❌ Errore validazione servizi: {ex.Message}");
			}
		}

		private void LogServiceInfo(IBroadcaster broadcaster, IAssetService assetService, IVehicleDataService vehicleDataService)
		{
			Debug.Log($"[CLUSTER CLIENT] 📊 Servizi registrati:");
			Debug.Log($"  • IBroadcaster: {broadcaster.GetType().Name}");
			Debug.Log($"  • IAssetService: {assetService.GetType().Name}");
			Debug.Log($"  • IVehicleDataService: {vehicleDataService.GetType().Name}");
			Debug.Log($"  • Modalità attuale: {vehicleDataService.CurrentDriveMode}");
			Debug.Log($"  • Motore: {(vehicleDataService.IsEngineRunning ? "Acceso" : "Spento")}");
		}

		#endregion

		#region Unity Lifecycle Debug

		private void Update()
		{
			if (_enableDebugMode)
			{
				HandleDebugInput();
			}

			// AGGIUNTO: Update State Machine ogni frame
			UpdateStateMachine();
		}

		private void HandleDebugInput()
		{
			// Debug input per testare il sistema
			if (Input.GetKeyDown(KeyCode.F5))
			{
				LogSystemStatus();
			}

			if (Input.GetKeyDown(KeyCode.F6))
			{
				TestVehicleDataService();
			}

			if (Input.GetKeyDown(KeyCode.F7))
			{
				TestBroadcasterSystem();
			}
		}

		private void LogSystemStatus()
		{
			Debug.Log("=== CLUSTER CLIENT STATUS ===");
			Debug.Log($"Client Instance: {(Instance != null ? "Active" : "Null")}");
			Debug.Log($"Services Count: {Services?.GetAll()?.Count ?? 0}");
			Debug.Log($"Features Count: {Features?.GetAll()?.Count ?? 0}");

			var vehicleService = Services?.Get<IVehicleDataService>();
			if (vehicleService != null)
			{
				Debug.Log($"Speed: {vehicleService.CurrentSpeed:F1} km/h");
				Debug.Log($"RPM: {vehicleService.CurrentRPM:F0}");
				Debug.Log($"Gear: {vehicleService.CurrentGear}");
				Debug.Log($"Mode: {vehicleService.CurrentDriveMode}");
			}
		}

		private void TestVehicleDataService()
		{
			var vehicleService = Services.Get<IVehicleDataService>();

			Debug.Log("[CLUSTER CLIENT] 🧪 Test VehicleDataService...");

			// Test cambio velocità
			vehicleService.SetSpeed(Random.Range(0f, 120f));

			// Test cambio RPM
			vehicleService.SetRPM(Random.Range(800f, 5000f));

			// Test cambio marcia
			vehicleService.SetGear(Random.Range(1, 6));

			Debug.Log("[CLUSTER CLIENT] ✅ Test VehicleDataService completato");
		}

		private void TestBroadcasterSystem()
		{
			var broadcaster = Services.Get<IBroadcaster>();

			Debug.Log("[CLUSTER CLIENT] 🧪 Test Broadcaster System...");

			// Sottoscrivi a evento di test
			broadcaster.Add<TestEvent>(OnTestEventReceived);

			// Invia evento di test
			broadcaster.Broadcast(new TestEvent("Test message from ClusterClient"));

			// Rimuovi sottoscrizione
			broadcaster.Remove<TestEvent>(OnTestEventReceived);

			Debug.Log("[CLUSTER CLIENT] ✅ Test Broadcaster System completato");
		}

		private void OnTestEventReceived(TestEvent testEvent)
		{
			Debug.Log($"[CLUSTER CLIENT] 📨 Evento di test ricevuto: {testEvent.Message}");
		}

		/// <summary>
		/// Aggiorna la State Machine se disponibile - QUESTO È IL FIX!
		/// </summary>
		private void UpdateStateMachine()
		{
			if (_clusterStateMachine != null)
			{
				_clusterStateMachine.UpdateState();
			}
		}

		/// <summary>
		/// Imposta il riferimento alla State Machine (chiamato da ClusterStartUpFlow)
		/// </summary>
		public void SetStateMachine(ClusterFSM stateMachine)
		{
			_clusterStateMachine = stateMachine;
			Debug.Log("[CLUSTER CLIENT] 🎰 State Machine collegata per Update");
		}

		#endregion

		#region Unity Messages

		private void OnApplicationPause(bool pauseStatus)
		{
			if (_enableDebugMode)
			{
				Debug.Log($"[CLUSTER CLIENT] ⏸️ Application Pause: {pauseStatus}");
			}
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (_enableDebugMode)
			{
				Debug.Log($"[CLUSTER CLIENT] 👁️ Application Focus: {hasFocus}");
			}
		}

		#endregion
	}

	#region Test Event Class

	/// <summary>
	/// Evento di test per verificare il sistema broadcaster
	/// </summary>
	public class TestEvent
	{
		public string Message { get; }

		public TestEvent(string message)
		{
			Message = message;
		}
	}

	#endregion
}