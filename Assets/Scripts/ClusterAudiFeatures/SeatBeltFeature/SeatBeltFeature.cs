using ClusterAudi;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Implementazione SeatBelt Feature
	/// IDENTICA al pattern SpeedometerFeature seguendo modello Mercedes
	/// 
	/// RESPONSABILITÀ:
	/// - Gestisce l'istanziazione UI seatbelt
	/// - Monitora 4 cinture di sicurezza  
	/// - Speed monitoring con soglie per modalità
	/// - Audio warning escalation system
	/// - Integration con VehicleDataService
	/// </summary>
	public class SeatBeltFeature : BaseFeature, ISeatBeltFeature, ISeatBeltFeatureInternal
	{
		#region Private Fields

		/// <summary>
		/// Configurazione corrente SeatBelt
		/// </summary>
		private SeatBeltConfig _currentConfiguration;

		/// <summary>
		/// Stati attuali delle 4 cinture
		/// Indici: 0=Driver, 1=Passenger, 2=RearLeft, 3=RearRight
		/// </summary>
		private SeatBeltData.SeatBeltStatus[] _seatBeltStates;

		/// <summary>
		/// Riferimento al behaviour istanziato (per aggiornamenti runtime)
		/// </summary>
		private SeatBeltBehaviour _seatBeltBehaviour;

		/// <summary>
		/// Warning system state
		/// </summary>
		private bool _isWarningSystemActive = false;
		private float _warningStartTime = 0f;
		private AudioEscalationLevel _currentAudioLevel = AudioEscalationLevel.None;

		/// <summary>
		/// Sistema abilitato/disabilitato
		/// </summary>
		private bool _systemEnabled = true;

		/// <summary>
		/// Ultima velocità controllata (per ottimizzazioni)
		/// </summary>
		private float _lastCheckedSpeed = 0f;

		/// <summary>
		/// Servizi cached per performance
		/// </summary>
		private IVehicleDataService _vehicleDataService;

		#endregion

		#region Constructor - IDENTICO Mercedes

		/// <summary>
		/// Costruttore - IDENTICO al pattern Mercedes
		/// </summary>
		public SeatBeltFeature(Client client) : base(client)
		{
			Debug.Log("[SEATBELT FEATURE] ??? SeatBeltFeature inizializzata");

			// Inizializza array stati cinture (default: sconosciuto)
			_seatBeltStates = new SeatBeltData.SeatBeltStatus[SeatBeltData.TOTAL_SEATBELTS];
			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				_seatBeltStates[i] = SeatBeltData.SeatBeltStatus.Unknown;
			}

			// Cache servizi
			_vehicleDataService = client.Services.Get<IVehicleDataService>();

			// Configurazione iniziale basata su modalità corrente
			_currentConfiguration = SeatBeltData.GetConfigForDriveMode(_vehicleDataService.CurrentDriveMode);

			// Sottoscrivi agli eventi modalità guida + velocità
			var broadcaster = client.Services.Get<IBroadcaster>();
			broadcaster.Add<DriveModeChangedEvent>(OnDriveModeChanged);
			_vehicleDataService.OnSpeedChanged += OnSpeedChanged;

			// Inizializza stati realistici per testing (normalmente verrebbero da sensori CAN bus)
			InitializeRealisticSeatBeltStates();
		}

		#endregion

		#region ISeatBeltFeature Implementation - Public Interface

		/// <summary>
		/// Istanzia la UI SeatBelt - IDENTICO al pattern Mercedes
		/// </summary>
		public async Task InstantiateSeatBeltFeature()
		{
			Debug.Log("[SEATBELT FEATURE] ?? Istanziazione SeatBelt UI...");

			try
			{
				// IDENTICO Mercedes: usa AssetService per caricare prefab
				var seatBeltInstance = await _assetService.InstantiateAsset<SeatBeltBehaviour>(
					SeatBeltData.SEATBELT_PREFAB_PATH);

				if (seatBeltInstance != null)
				{
					// IDENTICO Mercedes: Initialize con this per dependency injection
					seatBeltInstance.Initialize(this);
					_seatBeltBehaviour = seatBeltInstance;

					// Applica configurazione iniziale
					_seatBeltBehaviour.ApplyConfiguration(_currentConfiguration);
					_seatBeltBehaviour.UpdateAllSeatBeltStates(_seatBeltStates);

					Debug.Log("[SEATBELT FEATURE] ? SeatBelt UI istanziata da prefab");
				}
				else
				{
					Debug.LogWarning("[SEATBELT FEATURE] ?? Prefab non trovato: " +
						SeatBeltData.SEATBELT_PREFAB_PATH);

					// Fallback: Crea dinamicamente (per development)
					await CreateSeatBeltDynamically();
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[SEATBELT FEATURE] ? Errore istanziazione: {ex.Message}");
				Debug.LogException(ex);
			}
		}

		/// <summary>
		/// Imposta lo stato di una specifica cintura
		/// </summary>
		public void SetSeatBeltStatus(SeatBeltData.SeatBeltPosition position, SeatBeltData.SeatBeltStatus status)
		{
			int index = (int)position;
			if (index < 0 || index >= SeatBeltData.TOTAL_SEATBELTS)
			{
				Debug.LogError($"[SEATBELT FEATURE] ? Posizione cintura non valida: {position}");
				return;
			}

			var oldStatus = _seatBeltStates[index];
			if (oldStatus == status) return; // Nessun cambiamento

			_seatBeltStates[index] = status;

			// Broadcast evento cambio stato
			_broadcaster.Broadcast(new SeatBeltStatusChangedEvent(position, oldStatus, status));

			// Aggiorna UI se disponibile
			if (_seatBeltBehaviour != null)
			{
				_seatBeltBehaviour.UpdateSeatBeltStatus(position, status);
			}

			// Rivaluta warning system
			EvaluateWarningSystem();

			Debug.Log($"[SEATBELT FEATURE] ?? Cintura {position}: {oldStatus} ? {status}");
		}

		/// <summary>
		/// Aggiorna configurazione per modalità guida
		/// </summary>
		public void UpdateConfigurationForDriveMode(DriveMode mode)
		{
			var oldConfig = _currentConfiguration;
			_currentConfiguration = SeatBeltData.GetConfigForDriveMode(mode);

			// Broadcast evento configurazione
			_broadcaster.Broadcast(new SeatBeltConfigurationUpdatedEvent(
				mode, _currentConfiguration, oldConfig.SpeedThreshold, _currentConfiguration.SpeedThreshold));

			// Aggiorna behaviour se istanziato
			if (_seatBeltBehaviour != null)
			{
				_seatBeltBehaviour.ApplyConfiguration(_currentConfiguration);
			}

			// Rivaluta warning system con nuova configurazione
			EvaluateWarningSystem();

			Debug.Log($"[SEATBELT FEATURE] ?? Configurazione aggiornata per modalità: {mode}");
		}

		/// <summary>
		/// Forza check del sistema cinture (per testing/debug)
		/// </summary>
		public void ForceWarningCheck()
		{
			Debug.Log("[SEATBELT FEATURE] ?? Check forzato sistema cinture");
			EvaluateWarningSystem();

			// Broadcast evento debug
			_broadcaster.Broadcast(new SeatBeltDebugEvent(
				"Manual warning check triggered",
				SeatBeltDebugType.WarningTriggered,
				new { Speed = _vehicleDataService.CurrentSpeed, States = _seatBeltStates }));
		}

		/// <summary>
		/// Abilita/disabilita completamente il sistema cinture
		/// </summary>
		public void SetSeatBeltSystemEnabled(bool enabled)
		{
			if (_systemEnabled == enabled) return;

			_systemEnabled = enabled;

			// Stop warning se sistema disabilitato
			if (!enabled && _isWarningSystemActive)
			{
				StopWarningSystem(SeatBeltWarningStopReason.SystemDisabled);
			}

			// Broadcast evento sistema
			_broadcaster.Broadcast(new SeatBeltSystemEnabledEvent(enabled,
				enabled ? "System enabled" : "System disabled"));

			// Aggiorna UI
			if (_seatBeltBehaviour != null)
			{
				_seatBeltBehaviour.SetSystemEnabled(enabled);
			}

			Debug.Log($"[SEATBELT FEATURE] {(enabled ? "? Sistema abilitato" : "? Sistema disabilitato")}");
		}

		#endregion

		#region ISeatBeltFeatureInternal Implementation - Internal Interface

		/// <summary>
		/// Ottiene il Client - IDENTICO al pattern Mercedes
		/// </summary>
		public Client GetClient()
		{
			return _client;
		}

		/// <summary>
		/// Ottiene la configurazione corrente SeatBelt
		/// </summary>
		public SeatBeltConfig GetCurrentConfiguration()
		{
			return _currentConfiguration;
		}

		/// <summary>
		/// Ottiene lo stato corrente di tutte le cinture
		/// </summary>
		public SeatBeltData.SeatBeltStatus[] GetAllSeatBeltStates()
		{
			// Restituisce copia dell'array per sicurezza
			var copy = new SeatBeltData.SeatBeltStatus[SeatBeltData.TOTAL_SEATBELTS];
			System.Array.Copy(_seatBeltStates, copy, SeatBeltData.TOTAL_SEATBELTS);
			return copy;
		}

		/// <summary>
		/// Verifica se il sistema warning è attualmente attivo
		/// </summary>
		public bool IsWarningSystemActive()
		{
			return _isWarningSystemActive;
		}

		/// <summary>
		/// Ottiene il tempo totale dall'inizio del warning corrente
		/// </summary>
		public float GetCurrentWarningTime()
		{
			return _isWarningSystemActive ? (Time.time - _warningStartTime) : 0f;
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Gestisce cambio modalità guida
		/// </summary>
		private void OnDriveModeChanged(DriveModeChangedEvent e)
		{
			Debug.Log($"[SEATBELT FEATURE] ?? Modalità cambiata: {e.NewMode}");
			UpdateConfigurationForDriveMode(e.NewMode);
		}

		/// <summary>
		/// Gestisce cambio velocità per monitoring continuo
		/// </summary>
		private void OnSpeedChanged(float newSpeed)
		{
			// Ottimizzazione: controlla solo se velocità cambia significativamente
			if (Mathf.Abs(newSpeed - _lastCheckedSpeed) > 1f) // Soglia 1 km/h
			{
				_lastCheckedSpeed = newSpeed;
				EvaluateWarningSystem();
			}
		}

		#endregion

		#region Warning System Core Logic

		/// <summary>
		/// Valuta se attivare/disattivare il sistema di warning
		/// CORE LOGIC del sistema SeatBelt
		/// </summary>
		private void EvaluateWarningSystem()
		{
			if (!_systemEnabled) return;

			float currentSpeed = _vehicleDataService.CurrentSpeed;
			float speedThreshold = _currentConfiguration.SpeedThreshold;

			// Verifica condizione warning: velocità > soglia AND almeno una cintura slacciata
			bool shouldShowWarning = currentSpeed > speedThreshold && HasUnfastenedBelts();

			if (shouldShowWarning && !_isWarningSystemActive)
			{
				StartWarningSystem();
			}
			else if (!shouldShowWarning && _isWarningSystemActive)
			{
				var stopReason = currentSpeed <= speedThreshold
					? SeatBeltWarningStopReason.SpeedReduced
					: SeatBeltWarningStopReason.AllBeltsFastened;
				StopWarningSystem(stopReason);
			}

			// Se warning attivo, aggiorna escalation audio
			if (_isWarningSystemActive)
			{
				UpdateAudioEscalation();
			}
		}

		/// <summary>
		/// Avvia il sistema di warning
		/// </summary>
		private void StartWarningSystem()
		{
			_isWarningSystemActive = true;
			_warningStartTime = Time.time;
			_currentAudioLevel = AudioEscalationLevel.Soft;

			var unfastenedBelts = GetUnfastenedBeltPositions();

			// Broadcast evento inizio warning
			_broadcaster.Broadcast(new SeatBeltWarningStartedEvent(
				_vehicleDataService.CurrentSpeed,
				_currentConfiguration.SpeedThreshold,
				unfastenedBelts,
				_vehicleDataService.CurrentDriveMode));

			// Attiva visual warning
			if (_currentConfiguration.EnableVisualWarning)
			{
				_broadcaster.Broadcast(new SeatBeltVisualWarningEvent(
					true, "FASTEN SEATBELTS", SeatBeltData.WARNING_TEXT_COLOR, unfastenedBelts));

				// Attiva flashing se abilitato
				if (_currentConfiguration.FlashingEnabled)
				{
					_broadcaster.Broadcast(new SeatBeltFlashIconsEvent(unfastenedBelts, true));
				}
			}

			// Attiva primo livello audio
			if (_currentConfiguration.EnableAudioWarning)
			{
				_broadcaster.Broadcast(new PlaySeatBeltAudioEvent(
					SeatBeltData.SOFT_BEEP_AUDIO_PATH, 0.7f, 2, AudioEscalationLevel.Soft));
			}

			Debug.Log("[SEATBELT FEATURE] ?? WARNING SYSTEM ATTIVATO");
		}

		/// <summary>
		/// Ferma il sistema di warning
		/// </summary>
		private void StopWarningSystem(SeatBeltWarningStopReason reason)
		{
			if (!_isWarningSystemActive) return;

			float totalDuration = Time.time - _warningStartTime;
			_isWarningSystemActive = false;
			_currentAudioLevel = AudioEscalationLevel.None;

			// Broadcast evento fine warning
			_broadcaster.Broadcast(new SeatBeltWarningStoppedEvent(totalDuration, reason));

			// Disattiva visual warning
			_broadcaster.Broadcast(new SeatBeltVisualWarningEvent(false));

			// Disattiva flashing
			_broadcaster.Broadcast(new SeatBeltFlashIconsEvent(new SeatBeltData.SeatBeltPosition[0], false));

			Debug.Log($"[SEATBELT FEATURE] ? WARNING SYSTEM FERMATO: {reason} (durata: {totalDuration:F1}s)");
		}

		/// <summary>
		/// Aggiorna escalation audio basata sul tempo
		/// </summary>
		private void UpdateAudioEscalation()
		{
			if (!_currentConfiguration.AudioEscalationEnabled) return;

			float warningDuration = Time.time - _warningStartTime;
			var newLevel = SeatBeltData.GetAudioEscalationLevel(warningDuration);

			if (newLevel != _currentAudioLevel)
			{
				var oldLevel = _currentAudioLevel;
				_currentAudioLevel = newLevel;

				string audioPath = newLevel switch
				{
					AudioEscalationLevel.Soft => SeatBeltData.SOFT_BEEP_AUDIO_PATH,
					AudioEscalationLevel.Urgent => SeatBeltData.URGENT_BEEP_AUDIO_PATH,
					AudioEscalationLevel.Continuous => SeatBeltData.CONTINUOUS_BEEP_AUDIO_PATH,
					_ => SeatBeltData.SOFT_BEEP_AUDIO_PATH
				};

				// Broadcast escalation event
				_broadcaster.Broadcast(new SeatBeltAudioEscalationEvent(
					oldLevel, newLevel, warningDuration, audioPath));

				// Play nuovo livello audio
				_broadcaster.Broadcast(new PlaySeatBeltAudioEvent(audioPath, 0.8f, 3, newLevel));

				Debug.Log($"[SEATBELT FEATURE] ?? Audio escalation: {oldLevel} ? {newLevel}");
			}
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Verifica se ci sono cinture slacciate
		/// </summary>
		private bool HasUnfastenedBelts()
		{
			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				if (_seatBeltStates[i] == SeatBeltData.SeatBeltStatus.Unfastened)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Ottiene array delle posizioni con cinture slacciate
		/// </summary>
		private SeatBeltData.SeatBeltPosition[] GetUnfastenedBeltPositions()
		{
			var unfastened = new System.Collections.Generic.List<SeatBeltData.SeatBeltPosition>();

			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				if (_seatBeltStates[i] == SeatBeltData.SeatBeltStatus.Unfastened)
				{
					unfastened.Add((SeatBeltData.SeatBeltPosition)i);
				}
			}

			return unfastened.ToArray();
		}

		/// <summary>
		/// Inizializza stati realistici per testing (simulazione sensori CAN)
		/// </summary>
		private void InitializeRealisticSeatBeltStates()
		{
			// NUOVO: Tutte le cinture iniziano slacciate per testing
			_seatBeltStates[0] = SeatBeltData.SeatBeltStatus.Unfastened;  // Driver
			_seatBeltStates[1] = SeatBeltData.SeatBeltStatus.Unfastened;  // Passenger 
			_seatBeltStates[2] = SeatBeltData.SeatBeltStatus.Unfastened;  // Rear Left
			_seatBeltStates[3] = SeatBeltData.SeatBeltStatus.Unfastened;  // Rear Right

			Debug.Log("[SEATBELT FEATURE] 🔧 Stati iniziali: TUTTE LE CINTURE SLACCIATE (D=❌ P=❌ RL=❌ RR=❌)");
		}

		#endregion

		#region Fallback Creation (Development)

		// SOSTITUISCI il metodo CreateSeatBeltDynamically nel tuo SeatBeltFeature.cs
		// con questa versione CORRETTA:

		/// <summary>
		/// Creazione dinamica per development (quando prefab non disponibile)
		/// VERSIONE CORRETTA - Crea UI GameObject con RectTransform
		/// </summary>
		private async Task CreateSeatBeltDynamically()
		{
			Debug.Log("[SEATBELT FEATURE] 🔧 Creazione dinamica seatbelt...");

			await Task.Delay(100);

			// 1. TROVA O CREA CANVAS ESISTENTE
			Canvas existingCanvas = Object.FindObjectOfType<Canvas>();

			if (existingCanvas == null)
			{
				// Crea nuovo canvas se non esiste
				var canvasObject = new GameObject("SeatBeltCanvas");
				var canvas = canvasObject.AddComponent<Canvas>();
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				canvas.sortingOrder = 15; // Sopra speedometer ma sotto warnings critici

				// Aggiungi CanvasScaler e GraphicRaycaster
				var canvasScaler = canvasObject.AddComponent<CanvasScaler>();
				canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
				canvasScaler.referenceResolution = new Vector2(1920, 1080);

				canvasObject.AddComponent<GraphicRaycaster>();
				existingCanvas = canvas;

				Debug.Log("[SEATBELT FEATURE] ✅ Nuovo Canvas creato");
			}
			else
			{
				Debug.Log("[SEATBELT FEATURE] ✅ Canvas esistente trovato");
			}

			// 2. CREA UI GAMEOBJECT CON RECTTRANSFORM
			// IMPORTANTE: Usa il metodo corretto per creare UI GameObject
			var seatBeltObject = new GameObject("SeatBelt", typeof(RectTransform));
			seatBeltObject.transform.SetParent(existingCanvas.transform, false);

			// 3. SETUP RECTTRANSFORM CORRETTAMENTE
			var rectTransform = seatBeltObject.GetComponent<RectTransform>();
			if (rectTransform != null)
			{
				rectTransform.anchorMin = new Vector2(0.7f, 0.1f);  // Bottom-right
				rectTransform.anchorMax = new Vector2(0.95f, 0.4f); // Dimensioni moderate
				rectTransform.anchoredPosition = Vector2.zero;
				rectTransform.sizeDelta = Vector2.zero;

				Debug.Log("[SEATBELT FEATURE] ✅ RectTransform configurato");
			}
			else
			{
				Debug.LogError("[SEATBELT FEATURE] ❌ RectTransform non trovato dopo creazione!");
				return;
			}

			// 4. AGGIUNGI BEHAVIOUR
			var seatBeltBehaviour = seatBeltObject.AddComponent<SeatBeltBehaviour>();
			if (seatBeltBehaviour == null)
			{
				Debug.LogError("[SEATBELT FEATURE] ❌ Impossibile aggiungere SeatBeltBehaviour!");
				return;
			}

			// 5. CREA COMPONENTI UI DINAMICAMENTE (TEMPORARY)
			await CreateBasicUIComponents(seatBeltObject);

			// 6. INIZIALIZZA BEHAVIOUR
			seatBeltBehaviour.Initialize(this);
			_seatBeltBehaviour = seatBeltBehaviour;

			// 7. APPLICA CONFIGURAZIONE
			_seatBeltBehaviour.ApplyConfiguration(_currentConfiguration);
			_seatBeltBehaviour.UpdateAllSeatBeltStates(_seatBeltStates);

			Debug.Log("[SEATBELT FEATURE] ✅ SeatBelt creato dinamicamente con RectTransform");
		}

		/// <summary>
		/// NUOVO: Crea componenti UI basic per testing dinamico
		/// </summary>
		private async Task CreateBasicUIComponents(GameObject parent)
		{
			await Task.Delay(50);

			Debug.Log("[SEATBELT FEATURE] 🔧 Creazione componenti UI basic...");

			// Crea container per icone
			var iconsContainer = new GameObject("IconsContainer", typeof(RectTransform));
			iconsContainer.transform.SetParent(parent.transform, false);

			var iconsRect = iconsContainer.GetComponent<RectTransform>();
			iconsRect.anchorMin = Vector2.zero;
			iconsRect.anchorMax = Vector2.one;
			iconsRect.sizeDelta = Vector2.zero;
			iconsRect.anchoredPosition = Vector2.zero;

			// Crea 4 icone placeholder (semplici Image)
			for (int i = 0; i < 4; i++)
			{
				var iconObject = new GameObject($"SeatBeltIcon_{i}", typeof(RectTransform));
				iconObject.transform.SetParent(iconsContainer.transform, false);

				// Aggiungi Image component
				var image = iconObject.AddComponent<Image>();
				image.color = SeatBeltData.GetColorForStatus(SeatBeltData.SeatBeltStatus.Unknown);

				// Setup posizione (layout 2x2)
				var iconRect = iconObject.GetComponent<RectTransform>();
				float x = (i % 2) * 0.6f + 0.2f; // 0.2 o 0.8
				float y = (i < 2) ? 0.7f : 0.3f;  // Top o bottom

				iconRect.anchorMin = new Vector2(x - 0.1f, y - 0.1f);
				iconRect.anchorMax = new Vector2(x + 0.1f, y + 0.1f);
				iconRect.sizeDelta = Vector2.zero;
				iconRect.anchoredPosition = Vector2.zero;

				Debug.Log($"[SEATBELT FEATURE] ✅ Icona {i} creata a ({x}, {y})");
			}

			// Crea warning panel placeholder
			var warningPanel = new GameObject("WarningPanel", typeof(RectTransform));
			warningPanel.transform.SetParent(parent.transform, false);
			warningPanel.SetActive(false); // Inizialmente nascosto

			var warningRect = warningPanel.GetComponent<RectTransform>();
			warningRect.anchorMin = new Vector2(0.1f, 0.4f);
			warningRect.anchorMax = new Vector2(0.9f, 0.6f);
			warningRect.sizeDelta = Vector2.zero;
			warningRect.anchoredPosition = Vector2.zero;

			// Aggiungi background al warning panel
			var warningBg = warningPanel.AddComponent<Image>();
			warningBg.color = new Color(1f, 0f, 0f, 0.3f); // Rosso trasparente

			Debug.Log("[SEATBELT FEATURE] ✅ Componenti UI basic creati");
		}

		#endregion

		#region Cleanup

		/// <summary>
		/// Cleanup quando feature viene distrutta
		/// </summary>
		~SeatBeltFeature()
		{
			// Rimuovi sottoscrizione eventi
			if (_broadcaster != null)
			{
				_broadcaster.Remove<DriveModeChangedEvent>(OnDriveModeChanged);
			}

			if (_vehicleDataService != null)
			{
				_vehicleDataService.OnSpeedChanged -= OnSpeedChanged;
			}

			// Stop warning system se attivo
			if (_isWarningSystemActive)
			{
				StopWarningSystem(SeatBeltWarningStopReason.SystemDisabled);
			}
		}

		#endregion
	}
}