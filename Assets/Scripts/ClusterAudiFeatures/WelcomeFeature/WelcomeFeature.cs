using ClusterAudi;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Implementazione concreta della WelcomeFeature.
	/// Segue ESATTAMENTE il pattern di DashboardFeature.cs del progetto Mercedes.
	/// 
	/// Pattern utilizzati:
	/// - BaseFeature: Eredità da classe base per funzionalità comuni
	/// - Multiple Interfaces: Implementa sia pubblica che interna
	/// - Dependency Injection: Riceve Client nel costruttore
	/// </summary>
	public class WelcomeFeature : BaseFeature, IWelcomeFeature, IWelcomeFeatureInternal
	{
		#region Constructor

		/// <summary>
		/// Costruttore della WelcomeFeature.
		/// IDENTICO al pattern del progetto Mercedes: riceve Client e chiama base().
		/// </summary>
		/// <param name="client">Istanza del client che contiene servizi e altre features</param>
		public WelcomeFeature(Client client) : base(client)
		{
			Debug.Log("[WELCOME FEATURE] 🎉 WelcomeFeature inizializzata");
		}

		#endregion

		#region IWelcomeFeature Implementation (Public Interface)

		/// <summary>
		/// Istanzia la Welcome Feature e tutti i suoi componenti.
		/// IDENTICO al pattern InstantiateDashboardFeature() del progetto Mercedes.
		/// </summary>
		/// <returns>Task completato quando l'istanziazione è terminata</returns>
		public async Task InstantiateWelcomeFeature()
		{
			Debug.Log("[WELCOME FEATURE] 🚀 Inizio istanziazione Welcome Feature...");

			try
			{
				// Crea il GameObject Welcome Screen
				await CreateWelcomeScreen();

				Debug.Log("[WELCOME FEATURE] 🎉 Welcome Feature istanziata!");
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[WELCOME FEATURE] ❌ Errore durante istanziazione: {ex.Message}");
				Debug.LogException(ex);
			}
		}

		/// <summary>
		/// Crea Welcome Screen caricando il prefab - PATTERN MERCEDES
		/// </summary>
		private async Task CreateWelcomeScreen()
		{
			Debug.Log("[WELCOME FEATURE] 🔧 Creazione Welcome Screen da prefab...");

			try
			{
				// METODO 1: Carica prefab via AssetService (IDENTICO a Mercedes)
				var welcomeScreenInstance = await _assetService.InstantiateAsset<WelcomeScreenBehaviour>(WelcomeData.WELCOME_SCREEN_PREFAB_PATH);

				if (welcomeScreenInstance != null)
				{
					// Prefab caricato con successo
					Debug.Log("[WELCOME FEATURE] ✅ Prefab caricato da Resources");
					welcomeScreenInstance.Initialize(this);
				}
				else
				{
					// Fallback: Carica prefab direttamente da Resources
					await CreateWelcomeScreenFromResources();
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[WELCOME FEATURE] ❌ Errore caricamento prefab: {ex.Message}");

				// Fallback: Carica prefab direttamente
				await CreateWelcomeScreenFromResources();
			}

			Debug.Log("[WELCOME FEATURE] ✅ Welcome Screen creato");
		}

		/// <summary>
		/// Fallback: Carica prefab direttamente da Resources
		/// </summary>
		private async Task CreateWelcomeScreenFromResources()
		{
			Debug.Log("[WELCOME FEATURE] 🔄 Fallback: Caricamento diretto da Resources...");

			await Task.Delay(100);

			// Carica prefab direttamente
			GameObject prefab = Resources.Load<GameObject>("WelcomeScreen/WelcomeScreenPrefab");

			if (prefab != null)
			{
				// Istanzia prefab
				GameObject welcomeInstance = Object.Instantiate(prefab);

				// Ottieni WelcomeScreenBehaviour dal prefab
				var welcomeBehaviour = welcomeInstance.GetComponent<WelcomeScreenBehaviour>();

				if (welcomeBehaviour == null)
				{
					// Aggiungi component se non presente
					welcomeBehaviour = welcomeInstance.AddComponent<WelcomeScreenBehaviour>();
				}

				// Inizializza
				welcomeBehaviour.Initialize(this);

				Debug.Log("[WELCOME FEATURE] ✅ Prefab caricato via Resources.Load");
			}
			else
			{
				Debug.LogError("[WELCOME FEATURE] ❌ Impossibile caricare prefab da Resources/WelcomeScreen/WelcomeScreenPrefab");

				// Ultimo fallback: Crea dinamicamente (metodo precedente)
				await CreateWelcomeScreenDynamically();
			}
		}

		/// <summary>
		/// Ultimo fallback: Crea UI dinamicamente (metodo precedente)
		/// </summary>
		private async Task CreateWelcomeScreenDynamically()
		{
			Debug.Log("[WELCOME FEATURE] 🔧 Ultimo fallback: Creazione dinamica...");

			await Task.Delay(100);

			// Crea Canvas per Welcome Screen
			var canvasObject = new GameObject("WelcomeCanvas");
			var canvas = canvasObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 100; // Sopra tutto

			var canvasScaler = canvasObject.AddComponent<CanvasScaler>();
			canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			canvasScaler.referenceResolution = new Vector2(1920, 1080);

			canvasObject.AddComponent<GraphicRaycaster>();

			// Crea CanvasGroup per fade
			var canvasGroup = canvasObject.AddComponent<CanvasGroup>();

			// Crea Welcome Screen GameObject
			var welcomeScreenObject = new GameObject("WelcomeScreen");
			welcomeScreenObject.transform.SetParent(canvasObject.transform, false);

			// Aggiungi WelcomeScreenBehaviour
			var welcomeBehaviour = welcomeScreenObject.AddComponent<WelcomeScreenBehaviour>();

			// Setup riferimenti UI (placeholder per ora)
			var rectTransform = welcomeScreenObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.sizeDelta = Vector2.zero;
			rectTransform.anchoredPosition = Vector2.zero;

			// Inizializza il behaviour
			welcomeBehaviour.Initialize(this);

			Debug.Log("[WELCOME FEATURE] ✅ Welcome Screen creato dinamicamente");
		}

		#endregion

		#region IWelcomeFeatureInternal Implementation (Internal Interface)

		/// <summary>
		/// Ottiene il riferimento al Client.
		/// IDENTICO al pattern GetClient() del progetto Mercedes.
		/// Utilizzato dai MonoBehaviour per accedere a servizi.
		/// </summary>
		/// <returns>Istanza del Client</returns>
		public Client GetClient()
		{
			return _client;
		}

		#endregion

		#region Private Helper Methods

		/// <summary>
		/// Crea un placeholder per la Welcome Screen.
		/// Questo metodo sarà sostituito quando implementeremo WelcomeScreenBehaviour.
		/// </summary>
		/// <returns>Task di creazione placeholder</returns>
		private async Task CreateWelcomeScreenPlaceholder()
		{
			Debug.Log("[WELCOME FEATURE] 🔧 Creazione placeholder Welcome Screen...");

			// Simula tempo di creazione
			await Task.Delay(100);

			// Crea un GameObject placeholder
			var welcomeScreenObject = new GameObject("WelcomeScreen_Placeholder");

			// Aggiunge un componente per identificare il placeholder
			var placeholderComponent = welcomeScreenObject.AddComponent<WelcomeScreenPlaceholder>();
			placeholderComponent.Initialize(this);

			Debug.Log("[WELCOME FEATURE] ✅ Placeholder creato");
		}

		#endregion

		#region Development Notes

		/*
         * CORREZIONI APPORTATE:
         * 
         * 1. Rimosso _featureData:
         *    - WelcomeData è static, non può essere istanziato
         *    - Le costanti si accedono direttamente: WelcomeData.WELCOME_STATE
         * 
         * 2. Rimosso riferimento a WelcomeScreenBehaviour:
         *    - Classe non ancora implementata
         *    - Creato placeholder temporaneo
         * 
         * 3. CreateWelcomeScreenPlaceholder():
         *    - Crea GameObject temporaneo per testing
         *    - Sarà sostituito con il vero WelcomeScreenBehaviour
         * 
         * PROSSIMO STEP:
         * - Implementare WelcomeScreenPlaceholder (componente temporaneo)
         * - Testare che la feature si carichi correttamente
         * - Implementare WelcomeScreenBehaviour completo
         */

		#endregion
	}

	/// <summary>
	/// Componente placeholder temporaneo per testare la WelcomeFeature.
	/// Sarà sostituito da WelcomeScreenBehaviour.
	/// </summary>
	public class WelcomeScreenPlaceholder : BaseMonoBehaviour<IWelcomeFeatureInternal>
	{
		protected override void ManagedAwake()
		{
			Debug.Log("[WELCOME PLACEHOLDER] 🎯 Placeholder Welcome Screen creato");
		}

		protected override void ManagedStart()
		{
			Debug.Log("[WELCOME PLACEHOLDER] ▶️ Placeholder avviato");

			// Test accesso al client tramite feature
			var client = _feature.GetClient();
			var vehicleData = client.Services.Get<IVehicleDataService>();

			Debug.Log($"[WELCOME PLACEHOLDER] 🚗 Accesso dati veicolo: Mode={vehicleData.CurrentDriveMode}");
		}

		protected override void ManagedUpdate()
		{
			// Placeholder update - niente per ora
		}

		protected override void ManagedOnDestroy()
		{
			Debug.Log("[WELCOME PLACEHOLDER] 🗑️ Placeholder distrutto");
		}
	}
}