/// <summary>
/// SEATBELT BEHAVIOUR - UI COMPONENT
/// 
/// FILE: Behaviours/SeatBeltBehaviour.cs
/// 
/// MonoBehaviour per SeatBelt UI
/// IDENTICO al pattern SpeedometerBehaviour seguendo modello Mercedes
/// Gestisce display 4 cinture, warning panel, flash effects e audio integration
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using ClusterAudi;
using ClusterAudiFeatures;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// MonoBehaviour per SeatBelt UI
	/// Segue ESATTAMENTE il pattern BaseMonoBehaviour del modello Mercedes
	/// </summary>
	public class SeatBeltBehaviour : BaseMonoBehaviour<ISeatBeltFeatureInternal>
	{
		#region Serialized Fields - UI COMPONENTS

		[Header("SeatBelt Icons - ASSIGN IN PREFAB")]
		[Tooltip("Icona cintura guidatore (Driver)")]
		[SerializeField] private Image _driverIcon;

		[Tooltip("Icona cintura passeggero (Passenger)")]
		[SerializeField] private Image _passengerIcon;

		[Tooltip("Icona cintura posteriore sinistra (Rear Left)")]
		[SerializeField] private Image _rearLeftIcon;

		[Tooltip("Icona cintura posteriore destra (Rear Right)")]
		[SerializeField] private Image _rearRightIcon;

		[Header("Warning Panel - ASSIGN IN PREFAB")]
		[Tooltip("Panel principale per warning message")]
		[SerializeField] private GameObject _warningPanel;

		[Tooltip("Testo principale warning")]
		[SerializeField] private TextMeshProUGUI _warningText;

		[Tooltip("Background del panel warning")]
		[SerializeField] private Image _warningBackground;

		[Header("Optional Components - ASSIGN IF NEEDED")]
		[Tooltip("Testo etichetta sistema (opzionale)")]
		[SerializeField] private TextMeshProUGUI _systemLabel;

		[Tooltip("Panel container per tutte le icone")]
		[SerializeField] private GameObject _iconsContainer;

		#endregion

		#region Private Fields

		// Servizi
		private IBroadcaster _broadcaster;
		private IVehicleDataService _vehicleDataService;

		// Configurazione corrente
		private SeatBeltConfig _currentConfiguration;

		// Array icone per accesso indicizzato
		private Image[] _seatBeltIcons;

		// Stati visual e animazioni
		private bool _isWarningActive = false;
		private bool _isFlashingActive = false;
		private bool _systemEnabled = true;

		// Coroutines per animazioni
		private Coroutine _warningFlashCoroutine;
		private Coroutine _iconFlashCoroutine;
		private Coroutine _warningPanelCoroutine;

		// Performance tracking
		private float _lastUpdateTime;
		private int _frameCounter;

		// Stati locali per ottimizzazioni
		private SeatBeltData.SeatBeltStatus[] _displayedStates;
		private bool[] _iconFlashStates;

		#endregion

		#region BaseMonoBehaviour Override

		/// <summary>
		/// Inizializzazione - IDENTICA al pattern Mercedes
		/// </summary>
		protected override void ManagedAwake()
		{
			Debug.Log("[SEATBELT UI] 🛡️ SeatBeltBehaviour inizializzato");

			// Ottieni servizi dal Client via feature
			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();
			_vehicleDataService = client.Services.Get<IVehicleDataService>();

			// Ottieni configurazione iniziale dalla feature
			_currentConfiguration = _feature.GetCurrentConfiguration();

			// Inizializza array stati locali
			_displayedStates = new SeatBeltData.SeatBeltStatus[SeatBeltData.TOTAL_SEATBELTS];
			_iconFlashStates = new bool[SeatBeltData.TOTAL_SEATBELTS];

			// Setup UI e validazione
			ValidateUIComponents();
			SetupUIReferences();
			SetupInitialUI();
			SubscribeToEvents();
		}

		/// <summary>
		/// Avvio - Setup iniziale
		/// </summary>
		protected override void ManagedStart()
		{
			Debug.Log("[SEATBELT UI] ▶️ SeatBelt UI avviata");

			// Ottieni stati iniziali dalla feature
			var initialStates = _feature.GetAllSeatBeltStates();
			UpdateAllSeatBeltStates(initialStates);

			// Setup warning panel inizialmente nascosto
			SetWarningPanelVisible(false);
		}

		/// <summary>
		/// Update continuo - Animazioni e monitoring
		/// </summary>
		protected override void ManagedUpdate()
		{
			// Update animazioni attive
			UpdateActiveAnimations();

			// Performance tracking (optional)
			TrackPerformance();

			// Update debug keys (solo in development)
			HandleDebugInput();
		}

		/// <summary>
		/// Cleanup
		/// </summary>
		protected override void ManagedOnDestroy()
		{
			Debug.Log("[SEATBELT UI] 🗑️ SeatBeltBehaviour distrutto");

			// Cleanup eventi
			UnsubscribeFromEvents();

			// Stop tutte le coroutines
			StopAllCoroutines();
		}

		#endregion

		#region Public Methods (Called by Feature)

		/// <summary>
		/// Applica configurazione seatbelt
		/// Chiamato da SeatBeltFeature quando cambia modalità guida
		/// </summary>
		public void ApplyConfiguration(SeatBeltConfig config)
		{
			_currentConfiguration = config;

			// Aggiorna visibility componenti basata su configurazione
			UpdateComponentVisibility();

			Debug.Log($"[SEATBELT UI] 🔧 Configurazione applicata per modalità");
		}

		/// <summary>
		/// Aggiorna stato di una singola cintura
		/// Chiamato da SeatBeltFeature quando cambia stato
		/// </summary>
		public void UpdateSeatBeltStatus(SeatBeltData.SeatBeltPosition position, SeatBeltData.SeatBeltStatus status)
		{
			int index = (int)position;
			if (index < 0 || index >= SeatBeltData.TOTAL_SEATBELTS) return;

			// Update stato locale se cambiato
			if (_displayedStates[index] != status)
			{
				_displayedStates[index] = status;
				UpdateSingleIconVisual(position, status);
			}
		}

		/// <summary>
		/// Aggiorna stati di tutte le cinture
		/// </summary>
		public void UpdateAllSeatBeltStates(SeatBeltData.SeatBeltStatus[] states)
		{
			if (states == null || states.Length != SeatBeltData.TOTAL_SEATBELTS) return;

			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				if (_displayedStates[i] != states[i])
				{
					_displayedStates[i] = states[i];
					UpdateSingleIconVisual((SeatBeltData.SeatBeltPosition)i, states[i]);
				}
			}
		}

		/// <summary>
		/// Abilita/disabilita il sistema visivamente
		/// </summary>
		public void SetSystemEnabled(bool enabled)
		{
			_systemEnabled = enabled;

			// Aggiorna alpha di tutte le icone
			float alpha = enabled ? 1f : 0.3f;
			foreach (var icon in _seatBeltIcons)
			{
				if (icon != null)
				{
					var color = icon.color;
					color.a = alpha;
					icon.color = color;
				}
			}

			// Nasconde warning se sistema disabilitato
			if (!enabled)
			{
				SetWarningPanelVisible(false);
			}

			Debug.Log($"[SEATBELT UI] {(enabled ? "✅ Sistema UI abilitato" : "❌ Sistema UI disabilitato")}");
		}

		#endregion

		#region UI Setup Methods

		/// <summary>
		/// Validazione componenti UI - IDENTICA al pattern Mercedes
		/// </summary>
		private void ValidateUIComponents()
		{
			int missingComponents = 0;

			// Componenti essenziali - icone cinture
			if (_driverIcon == null)
			{
				Debug.LogError("[SEATBELT UI] ❌ _driverIcon non assegnato nel prefab!");
				missingComponents++;
			}

			if (_passengerIcon == null)
			{
				Debug.LogError("[SEATBELT UI] ❌ _passengerIcon non assegnato nel prefab!");
				missingComponents++;
			}

			if (_rearLeftIcon == null)
			{
				Debug.LogError("[SEATBELT UI] ❌ _rearLeftIcon non assegnato nel prefab!");
				missingComponents++;
			}

			if (_rearRightIcon == null)
			{
				Debug.LogError("[SEATBELT UI] ❌ _rearRightIcon non assegnato nel prefab!");
				missingComponents++;
			}

			// Componenti warning (importanti ma non critici)
			if (_warningPanel == null)
			{
				Debug.LogWarning("[SEATBELT UI] ⚠️ _warningPanel non assegnato (warning limitati)");
			}

			if (_warningText == null)
			{
				Debug.LogWarning("[SEATBELT UI] ⚠️ _warningText non assegnato (messaggi limitati)");
			}

			// Risultato validazione
			if (missingComponents == 0)
			{
				Debug.Log("[SEATBELT UI] ✅ Tutti i componenti essenziali assegnati");
			}
			else
			{
				Debug.LogError($"[SEATBELT UI] ❌ {missingComponents} componenti essenziali mancanti!");
			}
		}

		/// <summary>
		/// Setup riferimenti array per accesso indicizzato
		/// </summary>
		private void SetupUIReferences()
		{
			// Setup array icone per accesso tramite indice
			_seatBeltIcons = new Image[SeatBeltData.TOTAL_SEATBELTS];
			_seatBeltIcons[0] = _driverIcon;      // Driver
			_seatBeltIcons[1] = _passengerIcon;   // Passenger
			_seatBeltIcons[2] = _rearLeftIcon;    // Rear Left
			_seatBeltIcons[3] = _rearRightIcon;   // Rear Right
		}

		/// <summary>
		/// Setup iniziale dell'UI
		/// </summary>
		private void SetupInitialUI()
		{
			// Setup icone cinture - stato iniziale Unknown (grigio)
			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				if (_seatBeltIcons[i] != null)
				{
					_displayedStates[i] = SeatBeltData.SeatBeltStatus.Unknown;
					_seatBeltIcons[i].color = SeatBeltData.GetColorForStatus(SeatBeltData.SeatBeltStatus.Unknown);
					_iconFlashStates[i] = false;
				}
			}

			// Setup warning panel inizialmente nascosto
			SetWarningPanelVisible(false);

			// Setup label sistema opzionale
			if (_systemLabel != null && string.IsNullOrEmpty(_systemLabel.text))
			{
				_systemLabel.text = "SEATBELTS";
			}

			Debug.Log("[SEATBELT UI] ✅ UI iniziale configurata");
		}

		/// <summary>
		/// Aggiorna visibility componenti basata su configurazione
		/// </summary>
		private void UpdateComponentVisibility()
		{
			// Mostra/nasconde warning panel basato su configurazione
			if (_warningPanel != null && !_currentConfiguration.EnableVisualWarning)
			{
				SetWarningPanelVisible(false);
			}

			// Altri aggiornamenti configurazione se necessari
		}

		#endregion

		#region Event System

		/// <summary>
		/// Sottoscrizione agli eventi
		/// </summary>
		private void SubscribeToEvents()
		{
			// Eventi SeatBelt specifici
			_broadcaster.Add<SeatBeltWarningStartedEvent>(OnWarningStarted);
			_broadcaster.Add<SeatBeltWarningStoppedEvent>(OnWarningStopped);
			_broadcaster.Add<SeatBeltVisualWarningEvent>(OnVisualWarning);
			_broadcaster.Add<SeatBeltFlashIconsEvent>(OnFlashIcons);
			_broadcaster.Add<SeatBeltSystemEnabledEvent>(OnSystemEnabledChanged);

			Debug.Log("[SEATBELT UI] 📡 Eventi sottoscritti");
		}

		/// <summary>
		/// Rimozione sottoscrizioni eventi
		/// </summary>
		private void UnsubscribeFromEvents()
		{
			if (_broadcaster != null)
			{
				_broadcaster.Remove<SeatBeltWarningStartedEvent>(OnWarningStarted);
				_broadcaster.Remove<SeatBeltWarningStoppedEvent>(OnWarningStopped);
				_broadcaster.Remove<SeatBeltVisualWarningEvent>(OnVisualWarning);
				_broadcaster.Remove<SeatBeltFlashIconsEvent>(OnFlashIcons);
				_broadcaster.Remove<SeatBeltSystemEnabledEvent>(OnSystemEnabledChanged);
			}

			Debug.Log("[SEATBELT UI] 📡 Eventi rimossi");
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Gestisce inizio warning system
		/// </summary>
		private void OnWarningStarted(SeatBeltWarningStartedEvent e)
		{
			Debug.Log($"[SEATBELT UI] 🚨 Warning attivato: {e.UnfastenedBelts.Length} cinture slacciate");

			_isWarningActive = true;

			// Attiva warning panel se abilitato
			if (_currentConfiguration.EnableVisualWarning)
			{
				SetWarningPanelVisible(true);
			}
		}

		/// <summary>
		/// Gestisce fine warning system
		/// </summary>
		private void OnWarningStopped(SeatBeltWarningStoppedEvent e)
		{
			Debug.Log($"[SEATBELT UI] ✅ Warning fermato: {e.StopReason} (durata: {e.TotalWarningDuration:F1}s)");

			_isWarningActive = false;

			// Disattiva warning panel
			SetWarningPanelVisible(false);

			// Ferma flash se attivo
			StopIconFlashing();
		}

		/// <summary>
		/// Gestisce visual warning updates
		/// </summary>
		private void OnVisualWarning(SeatBeltVisualWarningEvent e)
		{
			if (e.ShowWarning)
			{
				SetWarningPanelVisible(true);
				UpdateWarningColor(e.WarningColor);
			}
			else
			{
				SetWarningPanelVisible(false);
			}
		}

		/// <summary>
		/// Gestisce flash icone
		/// </summary>
		private void OnFlashIcons(SeatBeltFlashIconsEvent e)
		{
			if (e.StartFlashing)
			{
				StartIconFlashing(e.PositionsToFlash, e.FlashInterval);
			}
			else
			{
				StopIconFlashing();
			}
		}

		/// <summary>
		/// Gestisce cambio abilitazione sistema
		/// </summary>
		private void OnSystemEnabledChanged(SeatBeltSystemEnabledEvent e)
		{
			SetSystemEnabled(e.IsEnabled);
		}

		#endregion

		#region Visual Updates

		/// <summary>
		/// Aggiorna visual di una singola icona
		/// </summary>
		private void UpdateSingleIconVisual(SeatBeltData.SeatBeltPosition position, SeatBeltData.SeatBeltStatus status)
		{
			int index = (int)position;
			if (index < 0 || index >= SeatBeltData.TOTAL_SEATBELTS || _seatBeltIcons[index] == null)
				return;

			// Aggiorna colore basato su stato (se non sta flashando)
			if (!_iconFlashStates[index])
			{
				_seatBeltIcons[index].color = SeatBeltData.GetColorForStatus(status);
			}

			Debug.Log($"[SEATBELT UI] 🔄 Icona {position} aggiornata: {status}");
		}

		/// <summary>
		/// Mostra/nasconde warning panel
		/// </summary>
		private void SetWarningPanelVisible(bool visible)
		{
			if (_warningPanel != null)
			{
				_warningPanel.SetActive(visible);
			}

			// Anima fade in/out se disponibile
			if (visible && _warningPanelCoroutine == null)
			{
				_warningPanelCoroutine = StartCoroutine(AnimateWarningPanelFadeIn());
			}
		}

		/// <summary>
		/// Aggiorna colore warning - SOLO se esplicitamente richiesto
		/// </summary>
		private void UpdateWarningColor(Color color)
		{
			// NON aggiorniamo automaticamente il colore del testo
			// Il colore rimane quello impostato nell'Inspector di Unity

			// Aggiorniamo SOLO il background se disponibile
			if (_warningBackground != null)
			{
				var bgColor = color;
				bgColor.a = 0.1f; // Background semi-trasparente
				_warningBackground.color = bgColor;
			}

			// DEBUG: Log per vedere che colore è stato richiesto
			Debug.Log($"[SEATBELT UI] 🎨 Colore warning richiesto: {color} (ma testo mantiene colore Unity)");
		}

		#endregion

		#region Animation Methods

		/// <summary>
		/// Avvia flash delle icone specificate
		/// </summary>
		private void StartIconFlashing(SeatBeltData.SeatBeltPosition[] positions, float interval)
		{
			// Stop flash precedente
			StopIconFlashing();

			if (positions == null || positions.Length == 0) return;

			_isFlashingActive = true;

			// Marca posizioni per flash
			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				_iconFlashStates[i] = false;
			}

			foreach (var pos in positions)
			{
				int index = (int)pos;
				if (index >= 0 && index < SeatBeltData.TOTAL_SEATBELTS)
				{
					_iconFlashStates[index] = true;
				}
			}

			// Avvia coroutine flash
			_iconFlashCoroutine = StartCoroutine(FlashIconsCoroutine(positions, interval));
		}

		/// <summary>
		/// Ferma flash delle icone
		/// </summary>
		private void StopIconFlashing()
		{
			if (_iconFlashCoroutine != null)
			{
				StopCoroutine(_iconFlashCoroutine);
				_iconFlashCoroutine = null;
			}

			_isFlashingActive = false;

			// Reset colori a stati normali
			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				if (_iconFlashStates[i] && _seatBeltIcons[i] != null)
				{
					_iconFlashStates[i] = false;
					_seatBeltIcons[i].color = SeatBeltData.GetColorForStatus(_displayedStates[i]);
				}
			}
		}

		/// <summary>
		/// Coroutine per flash icone
		/// </summary>
		private IEnumerator FlashIconsCoroutine(SeatBeltData.SeatBeltPosition[] positions, float interval)
		{
			while (_isFlashingActive)
			{
				// Flash ON - colore warning
				foreach (var pos in positions)
				{
					int index = (int)pos;
					if (index >= 0 && index < SeatBeltData.TOTAL_SEATBELTS && _seatBeltIcons[index] != null)
					{
						_seatBeltIcons[index].color = SeatBeltData.SEATBELT_WARNING_COLOR;
					}
				}

				yield return new WaitForSeconds(interval * 0.4f); // 40% del tempo ON

				// Flash OFF - colore originale
				foreach (var pos in positions)
				{
					int index = (int)pos;
					if (index >= 0 && index < SeatBeltData.TOTAL_SEATBELTS && _seatBeltIcons[index] != null)
					{
						_seatBeltIcons[index].color = SeatBeltData.GetColorForStatus(_displayedStates[index]);
					}
				}

				yield return new WaitForSeconds(interval * 0.6f); // 60% del tempo OFF
			}
		}

		/// <summary>
		/// Animazione fade in warning panel
		/// </summary>
		private IEnumerator AnimateWarningPanelFadeIn()
		{
			if (_warningPanel == null) yield break;

			var canvasGroup = _warningPanel.GetComponent<CanvasGroup>();
			if (canvasGroup == null)
				canvasGroup = _warningPanel.AddComponent<CanvasGroup>();

			float duration = SeatBeltData.WARNING_PANEL_FADE_DURATION;
			float elapsedTime = 0f;

			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				float alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
				canvasGroup.alpha = alpha;
				yield return null;
			}

			canvasGroup.alpha = 1f;
			_warningPanelCoroutine = null;
		}

		/// <summary>
		/// Update animazioni attive ogni frame
		/// </summary>
		private void UpdateActiveAnimations()
		{
			// Placeholder per animazioni che richiedono update continuo
			// (attualmente tutte le animazioni sono gestite da coroutines)
		}

		#endregion

		#region Debug & Performance

		/// <summary>
		/// Gestisce input debug per testing - UNA LETTERA PER CINTURA
		/// </summary>
		private void HandleDebugInput()
		{
			#if UNITY_EDITOR
			// Q: Toggle DRIVER seatbelt
			if (Input.GetKeyDown(KeyCode.Q))
			{
				ToggleSeatBelt(SeatBeltData.SeatBeltPosition.Driver, "DRIVER");
			}

			// W: Toggle PASSENGER seatbelt  
			if (Input.GetKeyDown(KeyCode.E))
			{
				ToggleSeatBelt(SeatBeltData.SeatBeltPosition.Passenger, "PASSENGER");
			}

			// R: Toggle REAR LEFT seatbelt
			if (Input.GetKeyDown(KeyCode.R))
			{
				ToggleSeatBelt(SeatBeltData.SeatBeltPosition.RearLeft, "REAR LEFT");
			}

			// T: Toggle REAR RIGHT seatbelt
			if (Input.GetKeyDown(KeyCode.T))
			{
				ToggleSeatBelt(SeatBeltData.SeatBeltPosition.RearRight, "REAR RIGHT");
			}

			// M: Force warning check per testing
			if (Input.GetKeyDown(KeyCode.M))
			{
				var client = _feature.GetClient();
				var seatBeltFeature = client.Features.Get<ISeatBeltFeature>();
				seatBeltFeature.ForceWarningCheck();

				Debug.Log("[SEATBELT UI] 🔧 DEBUG: Force warning check");
			}

			// I: Info stato di tutte le cinture
			if (Input.GetKeyDown(KeyCode.I))
			{
				LogAllSeatBeltStates();
			}
#endif
		}

		/// <summary>
		/// Helper per toggle singola cintura
		/// </summary>
		private void ToggleSeatBelt(SeatBeltData.SeatBeltPosition position, string name)
		{
			var currentState = _feature.GetAllSeatBeltStates()[(int)position];
			var newState = currentState == SeatBeltData.SeatBeltStatus.Fastened
				? SeatBeltData.SeatBeltStatus.Unfastened
				: SeatBeltData.SeatBeltStatus.Fastened;

			var client = _feature.GetClient();
			var seatBeltFeature = client.Features.Get<ISeatBeltFeature>();
			seatBeltFeature.SetSeatBeltStatus(position, newState);

			string statusIcon = newState == SeatBeltData.SeatBeltStatus.Fastened ? "✅" : "❌";
			Debug.Log($"[SEATBELT UI] 🔧 DEBUG: {name} seatbelt → {statusIcon} {newState}");
		}

		/// <summary>
		/// Log stato di tutte le cinture per debug
		/// </summary>
		private void LogAllSeatBeltStates()
		{
			var states = _feature.GetAllSeatBeltStates();
			var client = _feature.GetClient();
			var vehicleService = client.Services.Get<IVehicleDataService>();

			Debug.Log("=== SEATBELT STATUS ===");
			Debug.Log($"Driver:     {GetStatusIcon(states[0])} {states[0]}");
			Debug.Log($"Passenger:  {GetStatusIcon(states[1])} {states[1]}");
			Debug.Log($"Rear Left:  {GetStatusIcon(states[2])} {states[2]}");
			Debug.Log($"Rear Right: {GetStatusIcon(states[3])} {states[3]}");
			Debug.Log($"Speed: {vehicleService.CurrentSpeed:F1} km/h");
			Debug.Log($"Warning Active: {_feature.IsWarningSystemActive()}");
			Debug.Log("======================");
			Debug.Log("Controls: Q=Driver | E=Passenger | R=RearLeft | T=RearRight | M=ForceCheck | I=Info");
		}

		/// <summary>
		/// Helper per icona stato
		/// </summary>
		private string GetStatusIcon(SeatBeltData.SeatBeltStatus status)
		{
			return status switch
			{
				SeatBeltData.SeatBeltStatus.Fastened => "✅",
				SeatBeltData.SeatBeltStatus.Unfastened => "❌",
				SeatBeltData.SeatBeltStatus.Warning => "⚠️",
				_ => "❔"
			};
		}

		/// <summary>
		/// Tracking performance per debug (opzionale)
		/// </summary>
		private void TrackPerformance()
		{
			_frameCounter++;

			if (Time.time - _lastUpdateTime >= 1.0f) // Ogni secondo
			{
				float fps = _frameCounter / (Time.time - _lastUpdateTime);

				if (fps < 30f) // Warning se FPS bassi
				{
					Debug.LogWarning($"[SEATBELT UI] ⚠️ Performance warning: {fps:F1} FPS");
				}

				_lastUpdateTime = Time.time;
				_frameCounter = 0;
			}
		}

		#endregion

		#region Development Notes

		/*
         * SEATBELT BEHAVIOUR - IMPLEMENTAZIONE COMPLETA
         * 
         * PATTERN SEGUITO:
         * - BaseMonoBehaviour<T>: Pattern Mercedes per UI components
         * - SerializeField validation: Controllo componenti prefab
         * - Event subscription: Risponde a tutti eventi SeatBelt
         * - Animation system: Flash effects e fade animations
         * 
         * FUNZIONALITÀ IMPLEMENTATE:
         * 1. Display 4 icone cinture con colori di stato
         * 2. Warning panel con messaggi dinamici
         * 3. Flash effects per cinture slacciate
         * 4. Integration completa con SeatBeltFeature
         * 5. Audio integration tramite eventi
         * 6. Debug controls (B/N/M keys in editor)
         * 
         * UI COMPONENTS RICHIESTI NEL PREFAB:
         * - 4 Image per icone cinture (Driver, Passenger, RearLeft, RearRight)
         * - GameObject warning panel
         * - TextMeshProUGUI per testo warning
         * - Image per background warning (opzionale)
         * - Componenti opzionali per labeling
         * 
         * LAYOUT SUGGERITO:
         * Driver Icon    |    Passenger Icon
         *      [WARNING PANEL]
         * RearLeft Icon  |   RearRight Icon
         * 
         * DEBUG CONTROLS (Editor only):
         * - B: Toggle driver seatbelt
         * - N: Toggle passenger seatbelt  
         * - M: Force warning check
         * 
         * PROSSIMO STEP:
         * - Creare prefab SeatBeltPrefab
         * - Testare integration con sistema esistente
         * - Verificare escalation audio tramite eventi
         */

		#endregion
	}
}