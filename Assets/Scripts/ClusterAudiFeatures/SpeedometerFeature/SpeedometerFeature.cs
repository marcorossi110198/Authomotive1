using System.Threading.Tasks;
using ClusterAudi;
using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Implementazione Speedometer Feature
	/// IDENTICA al pattern ClusterDriveModeFeature seguendo modello Mercedes
	/// 
	/// RESPONSABILITÀ:
	/// - Gestisce l'istanziazione UI speedometer
	/// - Fornisce accesso al Client per MonoBehaviour
	/// - Gestisce configurazione per modalità guida
	/// - Gestisce cambio unità di misura
	/// </summary>
	public class SpeedometerFeature : BaseFeature, ISpeedometerFeature, ISpeedometerFeatureInternal
	{
		#region Private Fields

		/// <summary>
		/// Configurazione corrente speedometer
		/// </summary>
		private SpeedometerConfig _currentConfiguration;

		/// <summary>
		/// Unità di misura corrente
		/// </summary>
		private SpeedometerData.SpeedUnit _currentSpeedUnit;

		/// <summary>
		/// Riferimento al behaviour istanziato (per aggiornamenti runtime)
		/// </summary>
		private SpeedometerBehaviour _speedometerBehaviour;

		#endregion

		#region Constructor - IDENTICO Mercedes

		/// <summary>
		/// Costruttore - IDENTICO al pattern Mercedes
		/// </summary>
		public SpeedometerFeature(Client client) : base(client)
		{
			Debug.Log("[SPEEDOMETER FEATURE] 🏎️ SpeedometerFeature inizializzata");

			// Configurazione iniziale basata su modalità corrente
			var vehicleService = client.Services.Get<IVehicleDataService>();
			_currentConfiguration = SpeedometerData.GetConfigForDriveMode(vehicleService.CurrentDriveMode);
			_currentSpeedUnit = _currentConfiguration.PreferredUnit;

			// Sottoscrivi agli eventi modalità guida per aggiornare configurazione
			var broadcaster = client.Services.Get<IBroadcaster>();
			broadcaster.Add<DriveModeChangedEvent>(OnDriveModeChanged);
		}

		#endregion

		#region ISpeedometerFeature Implementation - Public Interface

		/// <summary>
		/// Istanzia la UI Speedometer - IDENTICO al pattern Mercedes
		/// </summary>
		public async Task InstantiateSpeedometerFeature()
		{
			Debug.Log("[SPEEDOMETER FEATURE] 🚀 Istanziazione Speedometer UI...");

			try
			{
				// IDENTICO Mercedes: usa AssetService per caricare prefab
				var speedometerInstance = await _assetService.InstantiateAsset<SpeedometerBehaviour>(
					SpeedometerData.SPEEDOMETER_PREFAB_PATH);

				if (speedometerInstance != null)
				{
					// IDENTICO Mercedes: Initialize con this per dependency injection
					speedometerInstance.Initialize(this);
					_speedometerBehaviour = speedometerInstance;

					// Applica configurazione iniziale
					_speedometerBehaviour.ApplyConfiguration(_currentConfiguration);
					_speedometerBehaviour.SetSpeedUnit(_currentSpeedUnit);

					Debug.Log("[SPEEDOMETER FEATURE] ✅ Speedometer UI istanziata da prefab");
				}
				else
				{
					Debug.LogWarning("[SPEEDOMETER FEATURE] ⚠️ Prefab non trovato: " +
						SpeedometerData.SPEEDOMETER_PREFAB_PATH);

					// Fallback: Crea dinamicamente (per development)
					await CreateSpeedometerDynamically();
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[SPEEDOMETER FEATURE] ❌ Errore istanziazione: {ex.Message}");
				Debug.LogException(ex);
			}
		}

		/// <summary>
		/// Aggiorna unità di misura velocità
		/// </summary>
		public void SetSpeedUnit(SpeedometerData.SpeedUnit unit)
		{
			_currentSpeedUnit = unit;

			// Aggiorna behaviour se istanziato
			if (_speedometerBehaviour != null)
			{
				_speedometerBehaviour.SetSpeedUnit(unit);
			}

			Debug.Log($"[SPEEDOMETER FEATURE] 📏 Unità velocità aggiornata: {unit}");
		}

		/// <summary>
		/// Aggiorna configurazione per modalità guida
		/// </summary>
		public void UpdateConfigurationForDriveMode(DriveMode mode)
		{
			_currentConfiguration = SpeedometerData.GetConfigForDriveMode(mode);

			// Aggiorna behaviour se istanziato
			if (_speedometerBehaviour != null)
			{
				_speedometerBehaviour.ApplyConfiguration(_currentConfiguration);
			}

			Debug.Log($"[SPEEDOMETER FEATURE] 🔧 Configurazione aggiornata per modalità: {mode}");
		}

		#endregion

		#region ISpeedometerFeatureInternal Implementation - Internal Interface

		/// <summary>
		/// Ottiene il Client - IDENTICO al pattern Mercedes
		/// </summary>
		public Client GetClient()
		{
			return _client;
		}

		/// <summary>
		/// Ottiene configurazione corrente speedometer
		/// </summary>
		public SpeedometerConfig GetCurrentConfiguration()
		{
			return _currentConfiguration;
		}

		/// <summary>
		/// Ottiene unità di misura corrente
		/// </summary>
		public SpeedometerData.SpeedUnit GetCurrentSpeedUnit()
		{
			return _currentSpeedUnit;
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Gestisce cambio modalità guida
		/// </summary>
		private void OnDriveModeChanged(DriveModeChangedEvent e)
		{
			Debug.Log($"[SPEEDOMETER FEATURE] 🔄 Modalità cambiata: {e.NewMode}");
			UpdateConfigurationForDriveMode(e.NewMode);
		}

		#endregion

		#region Fallback Creation (Development)

		/// <summary>
		/// Creazione dinamica per development (quando prefab non disponibile)
		/// </summary>
		private async Task CreateSpeedometerDynamically()
		{
			Debug.Log("[SPEEDOMETER FEATURE] 🔧 Creazione dinamica speedometer...");

			await Task.Delay(100);

			// Crea Canvas per Speedometer (se non esiste già)
			Canvas existingCanvas = Object.FindObjectOfType<Canvas>();

			if (existingCanvas == null)
			{
				var canvasObject = new GameObject("SpeedometerCanvas");
				var canvas = canvasObject.AddComponent<Canvas>();
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				canvas.sortingOrder = 10; // Sotto il DriveMode ma sopra il background

				canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
				canvasObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
				existingCanvas = canvas;
			}

			// Crea Speedometer GameObject
			var speedometerObject = new GameObject("Speedometer");
			speedometerObject.transform.SetParent(existingCanvas.transform, false);

			// Aggiungi SpeedometerBehaviour
			var speedometerBehaviour = speedometerObject.AddComponent<SpeedometerBehaviour>();

			// Setup RectTransform (posizione in alto a sinistra)
			var rectTransform = speedometerObject.GetComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.1f, 0.7f);  // Alto a sinistra
			rectTransform.anchorMax = new Vector2(0.4f, 0.9f);  // Dimensioni moderate
			rectTransform.anchoredPosition = Vector2.zero;
			rectTransform.sizeDelta = Vector2.zero;

			// Inizializza behaviour
			speedometerBehaviour.Initialize(this);
			_speedometerBehaviour = speedometerBehaviour;

			// Applica configurazione
			_speedometerBehaviour.ApplyConfiguration(_currentConfiguration);
			_speedometerBehaviour.SetSpeedUnit(_currentSpeedUnit);

			Debug.Log("[SPEEDOMETER FEATURE] ✅ Speedometer creato dinamicamente");
		}

		#endregion

		#region Cleanup

		/// <summary>
		/// Cleanup quando feature viene distrutta
		/// </summary>
		~SpeedometerFeature()
		{
			// Rimuovi sottoscrizione eventi
			if (_broadcaster != null)
			{
				_broadcaster.Remove<DriveModeChangedEvent>(OnDriveModeChanged);
			}
		}

		#endregion

		#region Development Notes

		/*
         * SPEEDOMETER FEATURE - IMPLEMENTAZIONE COMPLETA
         * 
         * PATTERN SEGUITO:
         * - BaseFeature: Eredita funzionalità comuni
         * - Dual Interfaces: Public + Internal
         * - Dependency Injection: Client nel costruttore
         * - Event Subscription: Risponde a DriveModeChangedEvent
         * 
         * FUNZIONALITÀ:
         * 1. Istanziazione UI (prefab o dinamica)
         * 2. Configurazione per modalità guida
         * 3. Cambio unità di misura runtime
         * 4. Integration con VehicleDataService
         * 
         * PROSSIMO STEP:
         * - Implementare SpeedometerBehaviour.cs (MonoBehaviour UI)
         * - Creare prefab SpeedometerPrefab
         * - Testare integration con sistema esistente
         */

		#endregion
	}
}