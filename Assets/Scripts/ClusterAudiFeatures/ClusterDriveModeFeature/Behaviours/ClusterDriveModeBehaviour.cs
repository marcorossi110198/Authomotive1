/// <summary>
/// CLUSTER DRIVE MODE BEHAVIOUR - VERSIONE SENZA BOTTONI UI
/// 
/// FILE: Behaviours/ClusterDriveModeBehaviour.cs
/// 
/// MonoBehaviour per Cluster Drive Mode UI - SOLO DISPLAY
/// IDENTICO al pattern Mercedes ma senza interazione bottoni
/// Controllo solo tramite F1-F3 dagli stati FSM
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
	/// MonoBehaviour per Cluster Drive Mode UI
	/// VERSIONE DISPLAY-ONLY - Senza interazione bottoni
	/// </summary>
	public class ClusterDriveModeBehaviour : BaseMonoBehaviour<IClusterDriveModeFeatureInternal>
	{
		#region Serialized Fields - UI COMPONENTS (Display Only)

		[Header("Mode Indicator Images - ASSIGN IN PREFAB")]
		[Tooltip("Indicatore visuale ECO - Solo display")]
		[SerializeField] private Image _ecoModeIndicator;

		[Tooltip("Indicatore visuale COMFORT - Solo display")]
		[SerializeField] private Image _comfortModeIndicator;

		[Tooltip("Indicatore visuale SPORT - Solo display")]
		[SerializeField] private Image _sportModeIndicator;

		[Header("Mode Text Labels - ASSIGN IN PREFAB")]
		[Tooltip("Testo ECO - Solo display")]
		[SerializeField] private TextMeshProUGUI _ecoModeText;

		[Tooltip("Testo COMFORT - Solo display")]
		[SerializeField] private TextMeshProUGUI _comfortModeText;

		[Tooltip("Testo SPORT - Solo display")]
		[SerializeField] private TextMeshProUGUI _sportModeText;

		[Header("Vehicle Info Display - ASSIGN IN PREFAB")]
		[Tooltip("Display velocità")]
		[SerializeField] private TextMeshProUGUI _speedText;

		[Tooltip("Display RPM")]
		[SerializeField] private TextMeshProUGUI _rpmText;

		[Tooltip("Display marcia")]
		[SerializeField] private TextMeshProUGUI _gearText;

		[Tooltip("Display modalità corrente")]
		[SerializeField] private TextMeshProUGUI _currentModeText;

		[Header("BACKGROUND IMAGE - ASSIGN IN PREFAB")]
		[Tooltip("Image component che mostra il background per la modalità corrente")]
		[SerializeField] private Image _backgroundImage;

		#endregion

		#region Private Fields

		// Servizi (ottenuti dal Client via feature)
		private IBroadcaster _broadcaster;
		private IVehicleDataService _vehicleDataService;

		// Stato corrente
		private DriveMode _currentDriveMode = DriveMode.Comfort;

		// Animazioni
		private Coroutine _modeTransitionCoroutine;
		private Coroutine _backgroundTransitionCoroutine;

		// 🆕 CACHE TEXTURES - Ottimizzazione caricamento
		private Sprite _ecoBackgroundSprite;
		private Sprite _comfortBackgroundSprite;
		private Sprite _sportBackgroundSprite;

		// 🆕 STATO CARICAMENTO
		private bool _backgroundResourcesLoaded = false;

		#endregion

		#region BaseMonoBehaviour Override

		/// <summary>
		/// Inizializzazione
		/// </summary>
		protected override void ManagedAwake()
		{
			Debug.Log("[CLUSTER DRIVE MODE] 🎛️ ClusterDriveModeBehaviour inizializzato (Display Only)");

			// Ottieni servizi dal Client via feature
			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();
			_vehicleDataService = client.Services.Get<IVehicleDataService>();

			// Setup iniziale (senza bottoni)
			ValidateUIComponents();
			SubscribeToEvents();
			SetupInitialUI();

			LoadBackgroundResources();
		}

		/// <summary>
		/// Avvio - CORRETTO per inizializzazione modalità
		/// </summary>
		protected override void ManagedStart()
		{
			Debug.Log("[CLUSTER DRIVE MODE] ▶️ Cluster Drive Mode UI avviata (Display Only)");

			// Forza aggiornamento iniziale con modalità corrente del servizio
			var initialMode = _vehicleDataService.CurrentDriveMode;
			Debug.Log($"[CLUSTER DRIVE MODE] 🔧 Modalità iniziale dal servizio: {initialMode}");

			// Reset _currentDriveMode per forzare l'aggiornamento
			_currentDriveMode = (DriveMode)(-1);

			// Aggiorna UI con modalità corrente
			UpdateModeDisplay(initialMode);

			// Applica immediatamente il tema per la modalità iniziale
			ApplyModeTheme(initialMode);

			ApplyBackgroundForMode(initialMode);

		}

		/// <summary>
		/// Update continuo
		/// </summary>
		protected override void ManagedUpdate()
		{
			// Update continuo delle informazioni veicolo
			UpdateVehicleInfo();
		}

		/// <summary>
		/// Cleanup
		/// </summary>
		protected override void ManagedOnDestroy()
		{
			Debug.Log("[CLUSTER DRIVE MODE] 🗑️ Cluster Drive Mode UI distrutta");

			// Cleanup eventi
			UnsubscribeFromEvents();

			// Stop animazioni
			if (_modeTransitionCoroutine != null)
			{
				StopCoroutine(_modeTransitionCoroutine);
			}

			// Stop animazioni background
			if (_backgroundTransitionCoroutine != null)
			{
				StopCoroutine(_backgroundTransitionCoroutine);
			}

			// 🆕 NUOVO: Stop animazioni background e cleanup overlay
			if (_backgroundTransitionCoroutine != null)
			{
				StopCoroutine(_backgroundTransitionCoroutine);

				// 🆕 Cleanup eventuali overlay rimasti
				CleanupAnyRemainingOverlays();
			}

		}

		/// <summary>
		/// 🆕 NUOVO: Cleanup di sicurezza per overlay rimasti
		/// </summary>
		private void CleanupAnyRemainingOverlays()
		{
			// Cerca overlay temporanei rimasti nella scena
			GameObject[] overlays = GameObject.FindGameObjectsWithTag("Untagged");
			foreach (var obj in overlays)
			{
				if (obj.name == "BackgroundOverlay_Temp")
				{
					Debug.Log("[CLUSTER DRIVE MODE] 🧹 Cleanup overlay rimasto");
					Destroy(obj);
				}
			}
		}

		#endregion

		#region UI Setup Methods

		/// <summary>
		/// Valida che tutti i SerializeField siano assegnati nel prefab
		/// VERSIONE SENZA BOTTONI
		/// </summary>
		private void ValidateUIComponents()
		{
			int missingComponents = 0;

			// Validazione indicatori (solo immagini, no bottoni)
			if (_ecoModeIndicator == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _ecoModeIndicator non assegnato nel prefab!");
				missingComponents++;
			}
			if (_comfortModeIndicator == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _comfortModeIndicator non assegnato nel prefab!");
				missingComponents++;
			}
			if (_sportModeIndicator == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _sportModeIndicator non assegnato nel prefab!");
				missingComponents++;
			}

			// Validazione testi modalità
			if (_ecoModeText == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _ecoModeText non assegnato nel prefab!");
				missingComponents++;
			}
			if (_comfortModeText == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _comfortModeText non assegnato nel prefab!");
				missingComponents++;
			}
			if (_sportModeText == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _sportModeText non assegnato nel prefab!");
				missingComponents++;
			}

			// Validazione background image
			if (_backgroundImage == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _backgroundImage non assegnato nel prefab!");
				missingComponents++;
			}

			if (missingComponents == 0)
			{
				Debug.Log("[CLUSTER DRIVE MODE] ✅ Tutti i SerializeField assegnati correttamente (incluso Background)");
			}
			else
			{
				Debug.LogError($"[CLUSTER DRIVE MODE] ❌ {missingComponents} componenti mancanti!");
			}

			// Validazione info veicolo
			if (_speedText == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _speedText non assegnato nel prefab!");
				missingComponents++;
			}
			if (_rpmText == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _rpmText non assegnato nel prefab!");
				missingComponents++;
			}
			if (_gearText == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _gearText non assegnato nel prefab!");
				missingComponents++;
			}
			if (_currentModeText == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ _currentModeText non assegnato nel prefab!");
				missingComponents++;
			}

			// Risultato validazione
			if (missingComponents == 0)
			{
				Debug.Log("[CLUSTER DRIVE MODE] ✅ Tutti i SerializeField assegnati correttamente (Display Only)");
			}
			else
			{
				Debug.LogError($"[CLUSTER DRIVE MODE] ❌ {missingComponents} componenti mancanti! " +
							   "Configura il prefab ClusterDriveModePrefab");
			}
		}

		/// <summary>
		/// Setup UI iniziale (senza bottoni)
		/// </summary>
		private void SetupInitialUI()
		{
			// Configura colori iniziali
			SetInitialColors();

			// Configura testi modalità
			SetupModeTexts();

			Debug.Log("[CLUSTER DRIVE MODE] ✅ UI iniziale configurata (Display Only)");
		}

		/// <summary>
		/// Configura colori iniziali degli indicatori
		/// </summary>
		private void SetInitialColors()
		{
			Debug.Log("[CLUSTER DRIVE MODE] 🎨 Configurazione colori iniziali...");

			// Tutti gli indicatori iniziano inattivi
			if (_ecoModeIndicator != null)
			{
				_ecoModeIndicator.color = ClusterDriveModeData.INACTIVE_COLOR;
			}

			if (_comfortModeIndicator != null)
			{
				_comfortModeIndicator.color = ClusterDriveModeData.INACTIVE_COLOR;
			}

			if (_sportModeIndicator != null)
			{
				_sportModeIndicator.color = ClusterDriveModeData.INACTIVE_COLOR;
			}

			Debug.Log("[CLUSTER DRIVE MODE] ✅ Colori iniziali configurati");
		}

		/// <summary>
		/// Configura testi delle modalità
		/// </summary>
		private void SetupModeTexts()
		{
			// Configura testi se non già impostati nel prefab
			if (_ecoModeText != null && string.IsNullOrEmpty(_ecoModeText.text))
				_ecoModeText.text = ClusterDriveModeData.ECO_MODE_TEXT;

			if (_comfortModeText != null && string.IsNullOrEmpty(_comfortModeText.text))
				_comfortModeText.text = ClusterDriveModeData.COMFORT_MODE_TEXT;

			if (_sportModeText != null && string.IsNullOrEmpty(_sportModeText.text))
				_sportModeText.text = ClusterDriveModeData.SPORT_MODE_TEXT;

			// Colore testi
			Color textColor = ClusterDriveModeData.TEXT_COLOR;
			if (_ecoModeText != null) _ecoModeText.color = textColor;
			if (_comfortModeText != null) _comfortModeText.color = textColor;
			if (_sportModeText != null) _sportModeText.color = textColor;
		}

		#endregion

		#region 🆕 NUOVO: Background Management

		/// <summary>
		/// 🆕 NUOVO: Carica tutte le risorse background da Resources
		/// Ottimizzazione: Carica una sola volta all'avvio invece che a ogni cambio
		/// </summary>
		private void LoadBackgroundResources()
		{
			Debug.Log("[CLUSTER DRIVE MODE] 🖼️ Caricamento risorse background...");

			try
			{
				// Carica sprites da Resources seguendo le tue convenzioni di naming
				_ecoBackgroundSprite = Resources.Load<Sprite>(ClusterDriveModeData.ECO_BACKGROUND_PATH);
				_comfortBackgroundSprite = Resources.Load<Sprite>(ClusterDriveModeData.COMFORT_BACKGROUND_PATH);
				_sportBackgroundSprite = Resources.Load<Sprite>(ClusterDriveModeData.SPORT_BACKGROUND_PATH);

				// Valida caricamento
				int loadedSprites = 0;
				if (_ecoBackgroundSprite != null) loadedSprites++;
				if (_comfortBackgroundSprite != null) loadedSprites++;
				if (_sportBackgroundSprite != null) loadedSprites++;

				if (loadedSprites == 3)
				{
					_backgroundResourcesLoaded = true;
					Debug.Log("[CLUSTER DRIVE MODE] ✅ Tutte le risorse background caricate con successo");
				}
				else
				{
					Debug.LogWarning($"[CLUSTER DRIVE MODE] ⚠️ Solo {loadedSprites}/3 background caricati. " +
						"Verifica che le immagini siano in Resources/ClusterDriveMode/");
					LogMissingBackgrounds();
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[CLUSTER DRIVE MODE] ❌ Errore caricamento background: {ex.Message}");
				_backgroundResourcesLoaded = false;
			}
		}

		/// <summary>
		/// 🆕 NUOVO: Log dettagliato per background mancanti
		/// </summary>
		private void LogMissingBackgrounds()
		{
			if (_ecoBackgroundSprite == null)
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ ClusterECO.png non trovato in Resources/ClusterDriveMode/");

			if (_comfortBackgroundSprite == null)
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ ClusterCOMFORT.png non trovato in Resources/ClusterDriveMode/");

			if (_sportBackgroundSprite == null)
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ ClusterSPORT.png non trovato in Resources/ClusterDriveMode/");
		}

		/// <summary>
		/// 🆕 NUOVO: Applica il background per la modalità specificata
		/// Gestisce il cambio con animazione smooth se richiesto
		/// </summary>
		private void ApplyBackgroundForMode(DriveMode mode, bool animated = false)
		{
			if (_backgroundImage == null)
			{
				Debug.LogWarning("[CLUSTER DRIVE MODE] ⚠️ _backgroundImage non assegnato nel prefab!");
				return;
			}

			if (!_backgroundResourcesLoaded)
			{
				Debug.LogWarning("[CLUSTER DRIVE MODE] ⚠️ Risorse background non caricate, skip aggiornamento");
				return;
			}

			Debug.Log($"[CLUSTER DRIVE MODE] 🖼️ Applico background per modalità: {mode}");

			// Determina quale sprite usare
			Sprite newBackgroundSprite = mode switch
			{
				DriveMode.Eco => _ecoBackgroundSprite,
				DriveMode.Comfort => _comfortBackgroundSprite,
				DriveMode.Sport => _sportBackgroundSprite,
				_ => _comfortBackgroundSprite // Default fallback
			};

			if (newBackgroundSprite == null)
			{
				Debug.LogError($"[CLUSTER DRIVE MODE] ❌ Background sprite per modalità {mode} è null!");
				return;
			}

			// Applica il cambio
			if (animated)
			{
				// Cambio animato (con fade)
				StartLayeredBackgroundTransition(newBackgroundSprite);
			}
			else
			{
				// Cambio immediato
				_backgroundImage.sprite = newBackgroundSprite;
				Debug.Log($"[CLUSTER DRIVE MODE] ✅ Background cambiato immediatamente a: {newBackgroundSprite.name}");
			}
		}

		/// <summary>
		/// 🆕 NUOVO: Avvia transizione layered con overlay temporaneo
		/// </summary>
		private void StartLayeredBackgroundTransition(Sprite newSprite)
		{
			// Stoppa animazione precedente se attiva
			if (_backgroundTransitionCoroutine != null)
			{
				StopCoroutine(_backgroundTransitionCoroutine);
			}

			// Avvia nuova animazione layered
			_backgroundTransitionCoroutine = StartCoroutine(AnimateLayeredBackgroundTransition(newSprite));
		}

		/// <summary>
		/// 🆕 NUOVO: Animazione layered background transition
		/// EFFETTO: Nuovo background appare sopra sfumando → Vecchio background scompare dietro
		/// </summary>
		private IEnumerator AnimateLayeredBackgroundTransition(Sprite newSprite)
		{
			Debug.Log($"[CLUSTER DRIVE MODE] 🎬 Inizio layered background transition");

			float transitionDuration = 0.4f; // Durata totale transizione

			// 🆕 STEP 1: Crea Image temporanea per overlay
			GameObject overlayObject = CreateTemporaryOverlay(newSprite);
			if (overlayObject == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] ❌ Impossibile creare overlay temporaneo");
				yield break;
			}

			Image overlayImage = overlayObject.GetComponent<Image>();
			Color overlayColor = overlayImage.color;

			// 🆕 STEP 2: Overlay inizia trasparente
			overlayColor.a = 0f;
			overlayImage.color = overlayColor;

			Debug.Log($"[CLUSTER DRIVE MODE] 📱 Overlay creato per: {newSprite.name}");

			// 🆕 STEP 3: Fade IN dell'overlay (nuovo background appare sopra)
			float elapsed = 0f;
			while (elapsed < transitionDuration)
			{
				elapsed += Time.deltaTime;

				// Smooth ease-in per apparizione graduale
				float t = EaseInOut(elapsed / transitionDuration);

				// Fade in overlay
				overlayColor.a = Mathf.Lerp(0f, 1f, t);
				overlayImage.color = overlayColor;

				yield return null;
			}

			// 🆕 STEP 4: Overlay completamente opaco
			overlayColor.a = 1f;
			overlayImage.color = overlayColor;

			Debug.Log($"[CLUSTER DRIVE MODE] ✅ Overlay fade-in completato");

			// 🆕 STEP 5: Sostituisci background principale (ora invisibile dietro overlay)
			_backgroundImage.sprite = newSprite;

			Debug.Log($"[CLUSTER DRIVE MODE] 🔄 Background principale sostituito con: {newSprite.name}");

			// 🆕 STEP 6: Rimuovi overlay (background principale ora visibile)
			DestroyTemporaryOverlay(overlayObject);

			Debug.Log($"[CLUSTER DRIVE MODE] ✅ Layered transition completata per: {newSprite.name}");
			_backgroundTransitionCoroutine = null;
		}

		/// <summary>
		/// 🆕 NUOVO: Crea Image temporanea per effetto overlay
		/// </summary>
		private GameObject CreateTemporaryOverlay(Sprite sprite)
		{
			try
			{
				// Trova il parent Canvas del background
				Canvas parentCanvas = _backgroundImage.GetComponentInParent<Canvas>();
				if (parentCanvas == null)
				{
					Debug.LogError("[CLUSTER DRIVE MODE] ❌ Impossibile trovare parent Canvas");
					return null;
				}

				// Crea GameObject overlay
				GameObject overlayObject = new GameObject("BackgroundOverlay_Temp");
				overlayObject.transform.SetParent(parentCanvas.transform, false);

				// Aggiungi Image component
				Image overlayImage = overlayObject.AddComponent<Image>();
				overlayImage.sprite = sprite;
				overlayImage.color = Color.white; // Alpha sarà controllata dall'animazione

				// Configura RectTransform per coprire tutto
				RectTransform overlayRect = overlayObject.GetComponent<RectTransform>();
				RectTransform backgroundRect = _backgroundImage.GetComponent<RectTransform>();

				// Copia esattamente posizione e dimensioni del background originale
				overlayRect.anchorMin = backgroundRect.anchorMin;
				overlayRect.anchorMax = backgroundRect.anchorMax;
				overlayRect.anchoredPosition = backgroundRect.anchoredPosition;
				overlayRect.sizeDelta = backgroundRect.sizeDelta;
				overlayRect.localScale = backgroundRect.localScale;

				// 🆕 IMPORTANTE: Posiziona SOPRA il background originale
				overlayObject.transform.SetSiblingIndex(_backgroundImage.transform.GetSiblingIndex() + 1);

				Debug.Log("[CLUSTER DRIVE MODE] ✅ Overlay temporaneo creato e posizionato");
				return overlayObject;
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[CLUSTER DRIVE MODE] ❌ Errore creazione overlay: {ex.Message}");
				return null;
			}
		}

		/// <summary>
		/// 🆕 NUOVO: Rimuove overlay temporaneo
		/// </summary>
		private void DestroyTemporaryOverlay(GameObject overlayObject)
		{
			if (overlayObject != null)
			{
				Debug.Log("[CLUSTER DRIVE MODE] 🗑️ Rimozione overlay temporaneo");
				Destroy(overlayObject);
			}
		}

		/// <summary>
		/// 🆕 NUOVO: Easing function per transizione smooth
		/// </summary>
		private float EaseInOut(float t)
		{
			return t < 0.5f
				? 2 * t * t
				: 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
		}

		/// <summary>
		/// 🆕 NUOVO: Coroutine per animazione cambio background
		/// Effetto: Fade out → Cambio sprite → Fade in
		/// </summary>
		private IEnumerator AnimateBackgroundTransition(Sprite newSprite)
		{
			Debug.Log($"[CLUSTER DRIVE MODE] 🎬 Inizio animazione background transition");

			float transitionDuration = 0.3f; // Transizione veloce ma smooth
			Color originalColor = _backgroundImage.color;

			// FASE 1: Fade Out
			float elapsed = 0f;
			while (elapsed < transitionDuration)
			{
				elapsed += Time.deltaTime;
				float alpha = Mathf.Lerp(1f, 0f, elapsed / transitionDuration);

				Color fadeColor = originalColor;
				fadeColor.a = alpha;
				_backgroundImage.color = fadeColor;

				yield return null;
			}

			// FASE 2: Cambio Sprite (quando invisibile)
			_backgroundImage.sprite = newSprite;
			Debug.Log($"[CLUSTER DRIVE MODE] 🔄 Background sprite cambiato a: {newSprite.name}");

			// FASE 3: Fade In
			elapsed = 0f;
			while (elapsed < transitionDuration)
			{
				elapsed += Time.deltaTime;
				float alpha = Mathf.Lerp(0f, 1f, elapsed / transitionDuration);

				Color fadeColor = originalColor;
				fadeColor.a = alpha;
				_backgroundImage.color = fadeColor;

				yield return null;
			}

			// Ripristina colore originale
			_backgroundImage.color = originalColor;

			Debug.Log($"[CLUSTER DRIVE MODE] ✅ Animazione background completata per: {newSprite.name}");
			_backgroundTransitionCoroutine = null;
		}

		#endregion


		#region Mode Display Methods

		/// <summary>
		/// Aggiorna display modalità con animazione
		/// CORRETTO per gestire correttamente l'inizializzazione
		/// </summary>
		private void UpdateModeDisplay(DriveMode activeMode)
		{
			Debug.Log($"[CLUSTER DRIVE MODE] 🔄 Aggiornamento display: {_currentDriveMode} → {activeMode}");

			var previousMode = _currentDriveMode;
			_currentDriveMode = activeMode;

			// Stop animazione precedente
			if (_modeTransitionCoroutine != null)
			{
				StopCoroutine(_modeTransitionCoroutine);
			}

			// Avvia animazione transizione
			_modeTransitionCoroutine = StartCoroutine(AnimateModeTransition(activeMode));

			Debug.Log($"[CLUSTER DRIVE MODE] ✅ Display aggiornato da {previousMode} a {activeMode}");
		}

		/// <summary>
		/// Animazione transizione modalità con easing
		/// </summary>
		private IEnumerator AnimateModeTransition(DriveMode activeMode)
		{
			Debug.Log($"[CLUSTER DRIVE MODE] 🎬 INIZIO animazione transizione verso: {activeMode}");

			float duration = ClusterDriveModeData.ANIMATION_DURATION;
			float elapsed = 0f;

			// Determina colori target
			Color ecoTarget = activeMode == DriveMode.Eco ? ClusterDriveModeData.ECO_COLOR : ClusterDriveModeData.INACTIVE_COLOR;
			Color comfortTarget = activeMode == DriveMode.Comfort ? ClusterDriveModeData.COMFORT_COLOR : ClusterDriveModeData.INACTIVE_COLOR;
			Color sportTarget = activeMode == DriveMode.Sport ? ClusterDriveModeData.SPORT_COLOR : ClusterDriveModeData.INACTIVE_COLOR;

			// Colori iniziali
			Color ecoStart = _ecoModeIndicator?.color ?? ClusterDriveModeData.INACTIVE_COLOR;
			Color comfortStart = _comfortModeIndicator?.color ?? ClusterDriveModeData.INACTIVE_COLOR;
			Color sportStart = _sportModeIndicator?.color ?? ClusterDriveModeData.INACTIVE_COLOR;

			// Animazione
			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

				// Aggiorna colori con interpolazione smooth
				if (_ecoModeIndicator != null)
					_ecoModeIndicator.color = Color.Lerp(ecoStart, ecoTarget, t);

				if (_comfortModeIndicator != null)
					_comfortModeIndicator.color = Color.Lerp(comfortStart, comfortTarget, t);

				if (_sportModeIndicator != null)
					_sportModeIndicator.color = Color.Lerp(sportStart, sportTarget, t);

				yield return null;
			}

			// Assicura colori finali
			if (_ecoModeIndicator != null) _ecoModeIndicator.color = ecoTarget;
			if (_comfortModeIndicator != null) _comfortModeIndicator.color = comfortTarget;
			if (_sportModeIndicator != null) _sportModeIndicator.color = sportTarget;

			Debug.Log($"[CLUSTER DRIVE MODE] ✅ Transizione completata per modalità: {activeMode}");

			// Aggiorna tema generale
			ApplyModeTheme(activeMode);

			_modeTransitionCoroutine = null;
		}

		#endregion

		#region Event System Methods

		/// <summary>
		/// Sottoscrizione agli eventi del sistema
		/// </summary>
		private void SubscribeToEvents()
		{
			Debug.Log("[CLUSTER DRIVE MODE] 📡 Sottoscrizione eventi...");

			// Eventi Drive Mode esistenti (dai tuoi stati FSM)
			_broadcaster.Add<DriveModeChangedEvent>(OnDriveModeChanged);
			_broadcaster.Add<ApplyThemeEvent>(OnThemeChanged);

			// Eventi display configuration esistenti (dai tuoi stati FSM)
			_broadcaster.Add<DisplayConfigEvent>(OnDisplayConfigChanged);

			// Eventi metriche specializzate (dai tuoi stati FSM)
			_broadcaster.Add<EcoMetricsUpdateEvent>(OnEcoMetricsUpdate);
			_broadcaster.Add<ComfortMetricsUpdateEvent>(OnComfortMetricsUpdate);
			_broadcaster.Add<SportMetricsUpdateEvent>(OnSportMetricsUpdate);

			Debug.Log("[CLUSTER DRIVE MODE] ✅ Eventi sottoscritti");
		}

		/// <summary>
		/// Rimozione sottoscrizioni eventi
		/// </summary>
		private void UnsubscribeFromEvents()
		{
			Debug.Log("[CLUSTER DRIVE MODE] 📡 Rimozione sottoscrizioni eventi...");

			// Rimuovi tutti gli eventi sottoscritti
			_broadcaster.Remove<DriveModeChangedEvent>(OnDriveModeChanged);
			_broadcaster.Remove<ApplyThemeEvent>(OnThemeChanged);
			_broadcaster.Remove<DisplayConfigEvent>(OnDisplayConfigChanged);
			_broadcaster.Remove<EcoMetricsUpdateEvent>(OnEcoMetricsUpdate);
			_broadcaster.Remove<ComfortMetricsUpdateEvent>(OnComfortMetricsUpdate);
			_broadcaster.Remove<SportMetricsUpdateEvent>(OnSportMetricsUpdate);

			Debug.Log("[CLUSTER DRIVE MODE] ✅ Sottoscrizioni rimosse");
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Gestisce cambio modalità di guida
		/// Aggiorna UI quando gli stati FSM cambiano modalità
		/// </summary>
		private void OnDriveModeChanged(DriveModeChangedEvent e)
		{
			Debug.Log($"[CLUSTER DRIVE MODE] 📢 Modalità cambiata via FSM: {e.NewMode}");

			// Aggiorna display modalità
			UpdateModeDisplay(e.NewMode);

			// Aggiorna background con animazione
			ApplyBackgroundForMode(e.NewMode, animated: true);
		}

		/// <summary>
		/// Gestisce cambio tema
		/// Applica i colori quando gli stati FSM cambiano tema
		/// </summary>
		private void OnThemeChanged(ApplyThemeEvent e)
		{
			Debug.Log($"[CLUSTER DRIVE MODE] 🎨 Tema cambiato: {e.ThemeName}");

			// Applica tema personalizzato se fornito
			ApplyThemeColors(e);
		}

		/// <summary>
		/// Gestisce configurazione display
		/// Adatta UI alle impostazioni degli stati FSM
		/// </summary>
		private void OnDisplayConfigChanged(DisplayConfigEvent e)
		{
			Debug.Log($"[CLUSTER DRIVE MODE] ⚙️ Display config aggiornata");

			// Applica configurazione display
			ApplyDisplayConfiguration(e);
		}

		/// <summary>
		/// Gestisce metriche ECO - Senza log continui
		/// </summary>
		private void OnEcoMetricsUpdate(EcoMetricsUpdateEvent e)
		{
			if (_currentDriveMode == DriveMode.Eco)
			{
				UpdateEcoSpecificDisplay(e);
			}
		}

		/// <summary>
		/// Gestisce metriche COMFORT - Senza log continui
		/// </summary>
		private void OnComfortMetricsUpdate(ComfortMetricsUpdateEvent e)
		{
			if (_currentDriveMode == DriveMode.Comfort)
			{
				UpdateComfortSpecificDisplay(e);
			}
		}

		/// <summary>
		/// Gestisce metriche SPORT - Senza log continui
		/// </summary>
		private void OnSportMetricsUpdate(SportMetricsUpdateEvent e)
		{
			if (_currentDriveMode == DriveMode.Sport)
			{
				UpdateSportSpecificDisplay(e);
			}
		}

		#endregion

		#region UI Update Methods

		/// <summary>
		/// Aggiorna informazioni veicolo in tempo reale
		/// </summary>
		private void UpdateVehicleInfo()
		{
			if (_vehicleDataService == null) return;

			// Aggiorna display velocità
			if (_speedText != null)
				_speedText.text = $"{_vehicleDataService.CurrentSpeed:F0} km/h";

			// Aggiorna display RPM
			if (_rpmText != null)
				_rpmText.text = $"{_vehicleDataService.CurrentRPM:F0} RPM";

			// Aggiorna display marcia
			if (_gearText != null)
			{
				string gearDisplay = _vehicleDataService.CurrentGear switch
				{
					-1 => "R",    // Retromarcia
					0 => "P",     // Parcheggio
					_ => _vehicleDataService.CurrentGear.ToString()
				};
				_gearText.text = gearDisplay;
			}

			// Aggiorna modalità corrente
			if (_currentModeText != null)
				_currentModeText.text = _currentDriveMode.ToString().ToUpper();
		}

		/// <summary>
		/// Applica tema per modalità corrente
		/// </summary>
		private void ApplyModeTheme(DriveMode mode)
		{
			Color primaryColor = mode switch
			{
				DriveMode.Eco => ClusterDriveModeData.ECO_COLOR,
				DriveMode.Comfort => ClusterDriveModeData.COMFORT_COLOR,
				DriveMode.Sport => ClusterDriveModeData.SPORT_COLOR,
				_ => ClusterDriveModeData.COMFORT_COLOR
			};

			//// Applica colore primario ai testi delle info veicolo
			//if (_speedText != null) _speedText.color = primaryColor;
			//if (_rpmText != null) _rpmText.color = primaryColor;
			//if (_gearText != null) _gearText.color = primaryColor;
			//if (_currentModeText != null)
			//{
			//	_currentModeText.color = primaryColor;
			//	_currentModeText.text = mode.ToString().ToUpper();
			//}

			Debug.Log($"[CLUSTER DRIVE MODE] ✅ Tema applicato per modalità: {mode}");
		}

		/// <summary>
		/// Applica colori da evento tema personalizzato
		/// </summary>
		private void ApplyThemeColors(ApplyThemeEvent themeEvent)
		{
			// Applica tema personalizzato dagli stati FSM
			if (_currentModeText != null)
				_currentModeText.color = themeEvent.PrimaryColor;

			// Applica colore secondario se disponibile
			//if (themeEvent.SecondaryColor != Color.clear)
			//{
			//	if (_speedText != null) _speedText.color = themeEvent.SecondaryColor;
			//	if (_rpmText != null) _rpmText.color = themeEvent.SecondaryColor;
			//	if (_gearText != null) _gearText.color = themeEvent.SecondaryColor;
			//}
		}

		/// <summary>
		/// Applica configurazione display dagli stati FSM
		/// </summary>
		private void ApplyDisplayConfiguration(DisplayConfigEvent config)
		{
			// Configura visibility e stili basati su configurazione modalità

			// Modalità ECO: Mostra info consumo
			if (_currentDriveMode == DriveMode.Eco && config.ShowConsumption)
			{
				ShowConsumptionInfo(true);
			}
			else if (!config.ShowConsumption)
			{
				ShowConsumptionInfo(false);
			}

			// Modalità SPORT: Mostra metriche avanzate
			if (_currentDriveMode == DriveMode.Sport && config.ShowSportMetrics)
			{
				ShowAdvancedSportMetrics(true);
			}
			else if (!config.ShowSportMetrics)
			{
				ShowAdvancedSportMetrics(false);
			}

			// Applica smoothing per modalità COMFORT
			if (_currentDriveMode == DriveMode.Comfort && config.SmoothingEnabled)
			{
				EnableSmoothDataDisplay(true);
			}
			else
			{
				EnableSmoothDataDisplay(false);
			}
		}

		#endregion

		#region Mode Specific Display Methods

		/// <summary>
		/// Aggiorna display specifico ECO - Senza log continui
		/// </summary>
		private void UpdateEcoSpecificDisplay(EcoMetricsUpdateEvent ecoMetrics)
		{
			if (_currentModeText != null)
			{
				_currentModeText.text = $"ECO - {ecoMetrics.EcoScore}% EFF";
			}
		}

		/// <summary>
		/// Aggiorna display specifico COMFORT - Senza log continui
		/// </summary>
		private void UpdateComfortSpecificDisplay(ComfortMetricsUpdateEvent comfortMetrics)
		{
			if (_currentModeText != null)
			{
				_currentModeText.text = $"COMFORT - {comfortMetrics.ComfortScore:F1}";
			}
		}

		/// <summary>
		/// Aggiorna display specifico SPORT - Senza log continui
		/// </summary>
		private void UpdateSportSpecificDisplay(SportMetricsUpdateEvent sportMetrics)
		{
			if (_currentModeText != null)
			{
				_currentModeText.text = $"SPORT - {sportMetrics.PerformanceScore:F0}%";
			}
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Mostra/nasconde informazioni consumo per modalità ECO
		/// </summary>
		private void ShowConsumptionInfo(bool show)
		{
			// TODO: Implementare quando aggiungeremo UI consumo specifica
		}

		/// <summary>
		/// Mostra/nasconde metriche avanzate per modalità SPORT
		/// </summary>
		private void ShowAdvancedSportMetrics(bool show)
		{
			// TODO: Implementare quando aggiungeremo UI sport metrics specifica
		}

		/// <summary>
		/// Abilita/disabilita smoothing dati per modalità COMFORT
		/// </summary>
		private void EnableSmoothDataDisplay(bool enable)
		{
			// TODO: Implementare smoothing dei dati quando necessario
		}

		#endregion
	}
}