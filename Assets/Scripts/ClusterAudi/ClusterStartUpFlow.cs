using ClusterAudiFeatures;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudi
{
	/// <summary>
	/// Flusso di avvio del cluster automotive.
	/// Segue il pattern di DashboardStartUpFlow.cs del progetto Mercedes.
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

				// 5. ISTANZIA LE FEATURES - AGGIUNTO!
				await LoadClusterFeatures(client);

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

			// Simula caricamento dati (in un'app reale potrebbe leggere da CAN bus, ecc.)
			await Task.Delay(500); // Simula tempo di caricamento

			// Configura stato iniziale del veicolo
			vehicleDataService.SetEngineRunning(true);
			vehicleDataService.SetSpeed(0f);
			vehicleDataService.SetRPM(800f); // RPM idle
			vehicleDataService.SetGear(0); // Parcheggio
			vehicleDataService.SetDriveMode(DriveMode.Comfort); // Modalità default

			Debug.Log("[CLUSTER STARTUP] ✅ Dati veicolo inizializzati");
		}

		private void SubscribeToVehicleEvents(IVehicleDataService vehicleDataService)
		{
			Debug.Log("[CLUSTER STARTUP] 📡 Sottoscrizione eventi veicolo...");

			// Sottoscrivi agli eventi per logging/debug
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

			// Crea State Machine e Context
			var stateMachine = new ClusterFSM();
			var context = new ClusterStateContext
			{
				Client = client,
				ClusterStateMachine = stateMachine,
				VehicleData = client.Services.Get<IVehicleDataService>(),
				Broadcaster = broadcaster
			};

			// REGISTRA TUTTI GLI STATI - Seguendo pattern Mercedes
			Debug.Log("[CLUSTER STARTUP] 📝 Registrazione stati...");
			stateMachine.AddState("WelcomeState", new WelcomeState(context));
			stateMachine.AddState("EcoModeState", new EcoModeState(context));
			stateMachine.AddState("ComfortModeState", new ComfortModeState(context));
			stateMachine.AddState("SportModeState", new SportModeState(context));

			Debug.Log("[CLUSTER STARTUP] ✅ Stati registrati nella State Machine");

			// AVVIA CON WELCOME STATE - Come nel progetto Mercedes
			stateMachine.GoTo("WelcomeState");
			Debug.Log("[CLUSTER STARTUP] 🚀 State Machine avviata con WelcomeState");

			// ⭐ NUOVO: Passa la State Machine al Client per Update
			if (client is ClusterClient clusterClient)
			{
				clusterClient.SetStateMachine(stateMachine);
			}

			Debug.Log("[CLUSTER STARTUP] ✅ State Machine completamente attiva");
		}

		private async Task LoadClusterFeatures(Client client)
		{
			Debug.Log("[CLUSTER STARTUP] 🧩 Caricamento features cluster...");

			try
			{
				// Istanzia WelcomeFeature
				var welcomeFeature = client.Features.Get<IWelcomeFeature>();
				await welcomeFeature.InstantiateWelcomeFeature();

				Debug.Log("[CLUSTER STARTUP] ✅ WelcomeFeature istanziata");
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[CLUSTER STARTUP] ❌ Errore caricamento features: {ex.Message}");
				Debug.LogException(ex);
			}

			Debug.Log("[CLUSTER STARTUP] ✅ Features cluster caricate");
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

	#region Events

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

	#endregion
}