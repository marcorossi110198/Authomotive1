using ClusterAudiFeatures;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudi
{
	/// <summary>
	/// Flusso di avvio del cluster automotive.
	/// CORRETTO: WelcomeFeature NON si istanzia qui, ma solo nello stato.
	/// </summary>
	public class ClusterStartUpFlow
	{
		public void BeginStartUp(Client client)
		{
			Debug.Log("[CLUSTER STARTUP] 🚀 Inizio procedura di avvio cluster...");
			StartUpFlowTask(client);
		}

		private async Task StartUpFlowTask(Client client)
		{
			try
			{
				Debug.Log("[CLUSTER STARTUP] 🔄 Esecuzione flusso di avvio asincrono...");

				// 1. Ottieni servizi necessari
				IBroadcaster clientBroadcaster = client.Services.Get<IBroadcaster>();
				IVehicleDataService vehicleDataService = client.Services.Get<IVehicleDataService>();

				Debug.Log("[CLUSTER STARTUP] ✅ Servizi ottenuti con successo");

				// 2. Configura servizio dati veicolo con valori iniziali
				await InitializeVehicleData(vehicleDataService);

				// 3. Sottoscrivi agli eventi del veicolo per debug
				SubscribeToVehicleEvents(vehicleDataService);

				// 4. Avvia State Machine del cluster
				await InitializeStateMachine(client, clientBroadcaster);

				// 5. ISTANZIA SOLO LE FEATURES SEMPRE PRESENTI (non WelcomeFeature!)
				await LoadPersistentFeatures(client);

				// 6. Segnala completamento avvio
				clientBroadcaster.Broadcast(new ClusterStartupCompletedEvent());

				Debug.Log("[CLUSTER STARTUP] 🎉 Avvio cluster completato con successo!");
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[CLUSTER STARTUP] ❌ Errore durante avvio: {ex.Message}");
				Debug.LogException(ex);
			}
		}

		#region Initialization Steps

		private async Task InitializeVehicleData(IVehicleDataService vehicleDataService)
		{
			Debug.Log("[CLUSTER STARTUP] 🚗 Inizializzazione dati veicolo...");

			await Task.Delay(500);

			vehicleDataService.SetEngineRunning(true);
			vehicleDataService.SetSpeed(0f);
			vehicleDataService.SetRPM(800f);
			vehicleDataService.SetGear(0);
			vehicleDataService.SetDriveMode(DriveMode.Comfort);

			Debug.Log("[CLUSTER STARTUP] ✅ Dati veicolo inizializzati");
		}

		private void SubscribeToVehicleEvents(IVehicleDataService vehicleDataService)
		{
			Debug.Log("[CLUSTER STARTUP] 📡 Sottoscrizione eventi veicolo...");

			vehicleDataService.OnSpeedChanged += OnSpeedChanged;
			vehicleDataService.OnRPMChanged += OnRPMChanged;
			vehicleDataService.OnGearChanged += OnGearChanged;
			vehicleDataService.OnDriveModeChanged += OnDriveModeChanged;

			Debug.Log("[CLUSTER STARTUP] ✅ Eventi veicolo sottoscritti");
		}

		private async Task InitializeStateMachine(Client client, IBroadcaster broadcaster)
		{
			Debug.Log("[CLUSTER STARTUP] 🎰 Inizializzazione State Machine...");

			await Task.Delay(300);

			var stateMachine = new ClusterFSM();
			var context = new ClusterStateContext
			{
				Client = client,
				ClusterStateMachine = stateMachine,
				VehicleData = client.Services.Get<IVehicleDataService>(),
				Broadcaster = broadcaster
			};

			Debug.Log("[CLUSTER STARTUP] 📝 Registrazione stati...");
			stateMachine.AddState("WelcomeState", new WelcomeState(context));
			stateMachine.AddState("EcoModeState", new EcoModeState(context));
			stateMachine.AddState("ComfortModeState", new ComfortModeState(context));
			stateMachine.AddState("SportModeState", new SportModeState(context));

			Debug.Log("[CLUSTER STARTUP] ✅ Stati registrati nella State Machine");

			// AVVIA CON WELCOME STATE
			stateMachine.GoTo("WelcomeState");
			Debug.Log("[CLUSTER STARTUP] 🚀 State Machine avviata con WelcomeState");

			if (client is ClusterClient clusterClient)
			{
				clusterClient.SetStateMachine(stateMachine);
			}

			Debug.Log("[CLUSTER STARTUP] ✅ State Machine completamente attiva");
		}

		/// <summary>
		/// CORRETTO: Carica SOLO le features sempre presenti (non WelcomeFeature!)
		/// </summary>
		private async Task LoadPersistentFeatures(Client client)
		{
			Debug.Log("[CLUSTER STARTUP] 🧩 Caricamento features persistenti...");

			try
			{
				// ✅ FEATURES SEMPRE PRESENTI - SI ISTANZIANO NEL STARTUP

				// Istanzia AudioFeature (sempre presente)
				var audioFeature = client.Features.Get<IAudioFeature>();
				await audioFeature.InstantiateAudioFeature();

				// ClusterDriveModeFeature (sempre presente)
				var clusterDriveModeFeature = client.Features.Get<IClusterDriveModeFeature>();
				await clusterDriveModeFeature.InstantiateClusterDriveModeFeature();

				// Carica SpeedometerFeature (sempre presente)
				var speedometerFeature = client.Features.Get<ISpeedometerFeature>();
				await speedometerFeature.InstantiateSpeedometerFeature();

				// AutomaticGearboxFeature (sempre presente)
				var automaticgearboxFeature = client.Features.Get<IAutomaticGearboxFeature>();
				await automaticgearboxFeature.InstantiateAutomaticGearboxFeature();

				// SeatBeltFeature (sempre presente)
				var seatBeltFeature = client.Features.Get<ISeatBeltFeature>();
				await seatBeltFeature.InstantiateSeatBeltFeature();

				// ClockFeature (sempre presente)
				var clockFeature = client.Features.Get<IClockFeature>();
				await clockFeature.InstantiateClockFeature();

				// DoorLockFeature (sempre presente)
				var doorLockFeature = client.Features.Get<IDoorLockFeature>();
				await doorLockFeature.InstantiateDoorLockFeature();

				// LaneAssistFeature (sempre presente)
				var laneAssistFeature = client.Features.Get<ILaneAssistFeature>();
				await laneAssistFeature.InstantiateLaneAssistFeature();

				// ❌ RIMOSSA: WelcomeFeature - SI ISTANZIA SOLO NELLO STATO
				// var welcomeFeature = client.Features.Get<IWelcomeFeature>();
				// await welcomeFeature.InstantiateWelcomeFeature();

				Debug.Log("[CLUSTER STARTUP] ✅ Features persistenti caricate");
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[CLUSTER STARTUP] ❌ Errore caricamento features persistenti: {ex.Message}");
				Debug.LogException(ex);
			}
		}

		#endregion

		#region Event Handlers

		private void OnSpeedChanged(float newSpeed)
		{
			Debug.Log($"[CLUSTER STARTUP] 🏎️ Velocità cambiata: {newSpeed:F1} km/h");
		}

		private void OnRPMChanged(float newRPM)
		{
			Debug.Log($"[CLUSTER STARTUP] ⚙️ RPM cambiati: {newRPM:F0}");
		}

		private void OnGearChanged(int newGear)
		{
			string gearDisplay = newGear switch
			{
				-1 => "R",
				0 => "P",
				_ => newGear.ToString()
			};
			Debug.Log($"[CLUSTER STARTUP] 🔧 Marcia cambiata: {gearDisplay}");
		}

		private void OnDriveModeChanged(DriveMode newMode)
		{
			Debug.Log($"[CLUSTER STARTUP] 🏁 Modalità guida cambiata: {newMode}");
		}

		#endregion
	}

	/// <summary>
	/// Evento segnalato al completamento dell'avvio del cluster
	/// </summary>
	public class ClusterStartupCompletedEvent
	{
		public System.DateTime StartupTime { get; }

		public ClusterStartupCompletedEvent()
		{
			StartupTime = System.DateTime.Now;
		}
	}
}