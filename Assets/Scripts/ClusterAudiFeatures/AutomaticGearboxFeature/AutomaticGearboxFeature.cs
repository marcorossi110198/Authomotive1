using System.Threading.Tasks;
using ClusterAudi;
using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Implementazione AutomaticGearbox Feature
	/// IDENTICA al pattern SpeedometerFeature seguendo modello Mercedes
	/// 
	/// RESPONSABILITÀ:
	/// - Gestisce l'istanziazione UI AutomaticGearbox
	/// - Fornisce accesso al Client per MonoBehaviour
	/// - Gestisce configurazione per modalità guida
	/// - Gestisce red zone warnings e shift indicators
	/// </summary>
	public class AutomaticGearboxFeature : BaseFeature, IAutomaticGearboxFeature, IAutomaticGearboxFeatureInternal
	{
		#region Private Fields

		/// <summary>
		/// Configurazione corrente AutomaticGearbox
		/// </summary>
		private AutomaticGearboxConfig _currentConfiguration;

		/// <summary>
		/// Riferimento al behaviour istanziato (per aggiornamenti runtime)
		/// </summary>
		private AutomaticGearboxBehaviour _automaticgearboxBehaviour;

		/// <summary>
		/// Flag per red zone flash
		/// </summary>
		private bool _redZoneFlashEnabled;

		#endregion

		#region Constructor - IDENTICO Mercedes

		/// <summary>
		/// Costruttore - IDENTICO al pattern Mercedes
		/// </summary>
		public AutomaticGearboxFeature(Client client) : base(client)
		{
			Debug.Log("[AutomaticGearbox FEATURE] 🏁 AutomaticGearboxFeature inizializzata");

			// Configurazione iniziale basata su modalità corrente
			var vehicleService = client.Services.Get<IVehicleDataService>();
			_currentConfiguration = AutomaticGearboxData.GetConfigForDriveMode(vehicleService.CurrentDriveMode);
			_redZoneFlashEnabled = _currentConfiguration.RedZoneFlashEnabled;

			// Sottoscrivi agli eventi modalità guida per aggiornare configurazione
			var broadcaster = client.Services.Get<IBroadcaster>();
			broadcaster.Add<DriveModeChangedEvent>(OnDriveModeChanged);

			// Sottoscrivi agli eventi RPM per red zone warnings
			vehicleService.OnRPMChanged += OnRPMChanged;
		}

		#endregion

		#region IAutomaticGearboxFeature Implementation - Public Interface

		/// <summary>
		/// Istanzia la UI AutomaticGearbox - IDENTICO al pattern Mercedes
		/// </summary>
		public async Task InstantiateAutomaticGearboxFeature()
		{
			Debug.Log("[AutomaticGearbox FEATURE] 🚀 Istanziazione AutomaticGearbox UI...");

			try
			{
				// IDENTICO Mercedes: usa AssetService per caricare prefab
				var automaticgearboxInstance = await _assetService.InstantiateAsset<AutomaticGearboxBehaviour>(
					AutomaticGearboxData.AutomaticGearbox_PREFAB_PATH);

				if (automaticgearboxInstance != null)
				{
					// IDENTICO Mercedes: Initialize con this per dependency injection
					automaticgearboxInstance.Initialize(this);
					_automaticgearboxBehaviour = automaticgearboxInstance;

					// Applica configurazione iniziale
					_automaticgearboxBehaviour.ApplyConfiguration(_currentConfiguration);

					Debug.Log("[AutomaticGearbox FEATURE] ✅ AutomaticGearbox UI istanziata da prefab");
				}
				else
				{
					Debug.LogWarning("[AutomaticGearbox FEATURE] ⚠️ Prefab non trovato: " +
						AutomaticGearboxData.AutomaticGearbox_PREFAB_PATH);

					// Fallback: Crea dinamicamente (per development)
					await CreateAutomaticGearboxDynamically();
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[AutomaticGearbox FEATURE] ❌ Errore istanziazione: {ex.Message}");
				Debug.LogException(ex);
			}
		}

		/// <summary>
		/// Aggiorna configurazione per modalità guida
		/// </summary>
		public void UpdateConfigurationForDriveMode(DriveMode mode)
		{
			_currentConfiguration = AutomaticGearboxData.GetConfigForDriveMode(mode);
			_redZoneFlashEnabled = _currentConfiguration.RedZoneFlashEnabled;

			// Aggiorna behaviour se istanziato
			if (_automaticgearboxBehaviour != null)
			{
				_automaticgearboxBehaviour.ApplyConfiguration(_currentConfiguration);
			}

			Debug.Log($"[AutomaticGearbox FEATURE] 🔧 Configurazione aggiornata per modalità: {mode}");
		}

		/// <summary>
		/// Forza red zone warning
		/// </summary>
		public void TriggerRedZoneWarning(bool isInRedZone)
		{
			if (_automaticgearboxBehaviour != null)
			{
				_automaticgearboxBehaviour.SetRedZoneWarning(isInRedZone);
			}
		}

		/// <summary>
		/// Abilita/disabilita shift indicator
		/// </summary>
		public void SetShiftIndicatorEnabled(bool enabled)
		{
			if (_automaticgearboxBehaviour != null)
			{
				_automaticgearboxBehaviour.SetShiftIndicatorEnabled(enabled);
			}
		}

		#endregion

		#region IAutomaticGearboxFeatureInternal Implementation - Internal Interface

		/// <summary>
		/// Ottiene il Client - IDENTICO al pattern Mercedes
		/// </summary>
		public Client GetClient()
		{
			return _client;
		}

		/// <summary>
		/// Ottiene configurazione corrente AutomaticGearbox
		/// </summary>
		public AutomaticGearboxConfig GetCurrentConfiguration()
		{
			return _currentConfiguration;
		}

		/// <summary>
		/// Verifica se red zone flash è abilitato
		/// </summary>
		public bool IsRedZoneFlashEnabled()
		{
			return _redZoneFlashEnabled;
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Gestisce cambio modalità guida
		/// </summary>
		private void OnDriveModeChanged(DriveModeChangedEvent e)
		{
			Debug.Log($"[AutomaticGearbox FEATURE] 🔄 Modalità cambiata: {e.NewMode}");
			UpdateConfigurationForDriveMode(e.NewMode);
		}

		/// <summary>
		/// Gestisce cambio RPM per red zone warnings
		/// </summary>
		private void OnRPMChanged(float newRPM)
		{
			// Controlla red zone solo se flash abilitato
			if (_redZoneFlashEnabled && AutomaticGearboxData.IsInRedZone(newRPM))
			{
				TriggerRedZoneWarning(true);
			}
			else if (AutomaticGearboxData.IsInWarningZone(newRPM))
			{
				// Warning zone senza flash
				TriggerRedZoneWarning(false);
			}
		}

		#endregion

		#region Fallback Creation (Development)

		/// <summary>
		/// Creazione dinamica per development (quando prefab non disponibile)
		/// </summary>
		private async Task CreateAutomaticGearboxDynamically()
		{
			Debug.Log("[AutomaticGearbox FEATURE] 🔧 Creazione dinamica AutomaticGearbox...");

			await Task.Delay(100);

			// Crea Canvas per AutomaticGearbox (se non esiste già)
			Canvas existingCanvas = Object.FindObjectOfType<Canvas>();

			if (existingCanvas == null)
			{
				var canvasObject = new GameObject("AutomaticGearboxCanvas");
				var canvas = canvasObject.AddComponent<Canvas>();
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				canvas.sortingOrder = 10; // Sotto il DriveMode ma sopra il background

				canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
				canvasObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
				existingCanvas = canvas;
			}

			// Crea AutomaticGearbox GameObject
			var automaticgearboxObject = new GameObject("AutomaticGearbox");
			automaticgearboxObject.transform.SetParent(existingCanvas.transform, false);

			// Aggiungi AutomaticGearboxBehaviour
			var automaticgearboxBehaviour = automaticgearboxObject.AddComponent<AutomaticGearboxBehaviour>();

			// Setup RectTransform (posizione in alto a destra - opposto al Speedometer)
			var rectTransform = automaticgearboxObject.GetComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.6f, 0.7f);  // Alto a destra
			rectTransform.anchorMax = new Vector2(0.9f, 0.9f);  // Dimensioni moderate
			rectTransform.anchoredPosition = Vector2.zero;
			rectTransform.sizeDelta = Vector2.zero;

			// Inizializza behaviour
			automaticgearboxBehaviour.Initialize(this);
			_automaticgearboxBehaviour = automaticgearboxBehaviour;

			// Applica configurazione
			_automaticgearboxBehaviour.ApplyConfiguration(_currentConfiguration);

			Debug.Log("AutomaticGearbox FEATURE] ✅ AutomaticGearbox creato dinamicamente");
		}

		#endregion

		#region Cleanup

		/// <summary>
		/// Cleanup quando feature viene distrutta
		/// </summary>
		~AutomaticGearboxFeature()
		{
			// Rimuovi sottoscrizione eventi
			if (_broadcaster != null)
			{
				_broadcaster.Remove<DriveModeChangedEvent>(OnDriveModeChanged);
			}

			// Rimuovi sottoscrizione RPM events
			var vehicleService = _client?.Services?.Get<IVehicleDataService>();
			if (vehicleService != null)
			{
				vehicleService.OnRPMChanged -= OnRPMChanged;
			}
		}

		#endregion

		#region Development Notes

		/*
         * AutomaticGearbox FEATURE - IMPLEMENTAZIONE COMPLETA
         * 
         * PATTERN SEGUITO:
         * - BaseFeature: Eredita funzionalità comuni
         * - Dual Interfaces: Public + Internal
         * - Dependency Injection: Client nel costruttore  
         * - Event Subscription: Risponde a DriveModeChangedEvent + OnRPMChanged
         * 
         * FUNZIONALITÀ:
         * 1. Istanziazione UI (prefab o dinamica)
         * 2. Configurazione per modalità guida
         * 3. Red zone warnings automatici
         * 4. Shift indicators
         * 5. Integration con VehicleDataService
         * 6. Automatic transmission support
         * 
         * DIFFERENZE da SpeedometerFeature:
         * - Red zone management
         * - Shift indicators
         * - Gear display integration
         * - RPM event handling
         * 
         * PROSSIMO STEP:
         * - Implementare AutomaticGearboxBehaviour.cs (MonoBehaviour UI)
         * - Creare prefab AutomaticGearboxPrefab
         * - Testare integration con sistema esistente
         */

		#endregion
	}
}