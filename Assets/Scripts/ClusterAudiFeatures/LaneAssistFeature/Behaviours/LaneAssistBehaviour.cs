using ClusterAudi;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// MonoBehaviour per Lane Assist UI
	/// IDENTICO al pattern ClusterDriveModeBehaviour del tuo progetto
	/// 
	/// RESPONSABILITÀ:
	/// - Monitora input tasti A/D per lane departure
	/// - Visualizza lane lines e car icon
	/// - Gestisce timers per detection e auto-reset
	/// - Audio ripetuto durante warning
	/// - Status text semplificato: ON/OFF/WARNING
	/// </summary>
	public class LaneAssistBehaviour : BaseMonoBehaviour<ILaneAssistFeatureInternal>
	{
		#region Serialized Fields - UI COMPONENTS

		[Header("Lane Assist Visual Components - ASSIGN IN PREFAB")]
		[Tooltip("Image delle lane lines sinistre")]
		[SerializeField] private Image _leftLaneLines;

		[Tooltip("Image delle lane lines destre")]
		[SerializeField] private Image _rightLaneLines;

		[Tooltip("Image dell'icona auto")]
		[SerializeField] private Image _carIcon;

		[Tooltip("Testo stato lane assist")]
		[SerializeField] private TextMeshProUGUI _statusText;

		[Header("Debug Components (Optional)")]
		[Tooltip("Panel per debug info (opzionale)")]
		[SerializeField] private GameObject _debugPanel;

		[Tooltip("Testo debug info (opzionale)")]
		[SerializeField] private TextMeshProUGUI _debugText;

		#endregion

		#region Private Fields

		// Servizi (ottenuti dal Client via feature)
		private IBroadcaster _broadcaster;
		private IVehicleDataService _vehicleDataService;

		// Stato lane departure detection
		private float _leftKeyHoldTime = 0f;
		private float _rightKeyHoldTime = 0f;
		private bool _isLeftKeyHeld = false;
		private bool _isRightKeyHeld = false;

		// Lane departure state
		private LaneAssistData.LaneDepartureType _currentDeparture = LaneAssistData.LaneDepartureType.None;
		private float _departureStartTime = 0f;
		private float _autoResetTimer = 0f;

		// Sistema state
		private LaneAssistData.LaneAssistState _currentState = LaneAssistData.LaneAssistState.Disabled;
		private bool _isSystemActive = false;

		// Visual state
		private Vector3 _originalCarIconPosition;
		private Color _originalLaneColor;

		// 🆕 NUOVO: Audio ripetuto durante warning
		private Coroutine _audioWarningCoroutine;
		private const float AUDIO_REPEAT_INTERVAL = 2.0f; // Ripeti ogni 2 secondi

		#endregion

		#region BaseMonoBehaviour Override

		/// <summary>
		/// Inizializzazione
		/// </summary>
		protected override void ManagedAwake()
		{
			Debug.Log("[LANE ASSIST BEHAVIOUR] 🛣️ LaneAssistBehaviour inizializzato");

			// Ottieni servizi dal Client via feature
			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();
			_vehicleDataService = client.Services.Get<IVehicleDataService>();

			// Setup iniziale
			ValidateUIComponents();
			SubscribeToEvents();
			InitializeVisualState();
		}

		/// <summary>
		/// Avvio
		/// </summary>
		protected override void ManagedStart()
		{
			Debug.Log("[LANE ASSIST BEHAVIOUR] ▶️ Lane Assist UI avviata");

			// Update iniziale stato
			UpdateSystemState();
		}

		/// <summary>
		/// Update continuo - Qui il core logic!
		/// </summary>
		protected override void ManagedUpdate()
		{
			// 1. Update stato sistema basato su velocità
			UpdateSystemState();

			// 2. Monitora input A/D keys se sistema attivo
			if (_isSystemActive)
			{
				MonitorLaneDepartureInput();
			}

			// 3. Gestisce auto-reset se in departure
			if (_currentDeparture != LaneAssistData.LaneDepartureType.None)
			{
				HandleAutoReset();
			}

			// 4. Update debug info
			UpdateDebugInfo();
		}

		/// <summary>
		/// Cleanup
		/// </summary>
		protected override void ManagedOnDestroy()
		{
			Debug.Log("[LANE ASSIST BEHAVIOUR] 🗑️ Lane Assist UI distrutta");

			// Stop audio ripetuto se attivo
			StopRepeatingAudio();

			// Cleanup eventi
			UnsubscribeFromEvents();
		}

		#endregion

		#region UI Setup Methods

		/// <summary>
		/// Valida che tutti i SerializeField siano assegnati nel prefab
		/// </summary>
		private void ValidateUIComponents()
		{
			int missingComponents = 0;

			if (_leftLaneLines == null)
			{
				Debug.LogError("[LANE ASSIST BEHAVIOUR] ❌ _leftLaneLines non assegnato nel prefab!");
				missingComponents++;
			}
			if (_rightLaneLines == null)
			{
				Debug.LogError("[LANE ASSIST BEHAVIOUR] ❌ _rightLaneLines non assegnato nel prefab!");
				missingComponents++;
			}
			if (_carIcon == null)
			{
				Debug.LogError("[LANE ASSIST BEHAVIOUR] ❌ _carIcon non assegnato nel prefab!");
				missingComponents++;
			}
			if (_statusText == null)
			{
				Debug.LogError("[LANE ASSIST BEHAVIOUR] ❌ _statusText non assegnato nel prefab!");
				missingComponents++;
			}

			if (missingComponents == 0)
			{
				Debug.Log("[LANE ASSIST BEHAVIOUR] ✅ Tutti i SerializeField assegnati correttamente");
			}
			else
			{
				Debug.LogError($"[LANE ASSIST BEHAVIOUR] ❌ {missingComponents} componenti mancanti! " +
							   "Configura il prefab LaneAssistPrefab");
			}
		}

		/// <summary>
		/// Inizializza stato visuale
		/// </summary>
		private void InitializeVisualState()
		{
			// Salva posizione originale car icon
			if (_carIcon != null)
			{
				_originalCarIconPosition = _carIcon.transform.localPosition;
			}

			// Salva colore originale lane lines
			if (_leftLaneLines != null)
			{
				_originalLaneColor = _leftLaneLines.color;
			}

			// Configura stato iniziale
			SetLaneLineColors(LaneAssistData.NORMAL_LANE_COLOR);
			ResetCarIconPosition();
			UpdateStatusText("Lane Assist: OFF"); // 🆕 NUOVO: Status semplificato

			Debug.Log("[LANE ASSIST BEHAVIOUR] ✅ Stato visuale inizializzato");
		}

		#endregion

		#region Event System Methods

		/// <summary>
		/// Sottoscrizione agli eventi del sistema
		/// </summary>
		private void SubscribeToEvents()
		{
			Debug.Log("[LANE ASSIST BEHAVIOUR] 📡 Sottoscrizione eventi...");

			// Eventi lane assist state
			_broadcaster.Add<LaneAssistStateChangedEvent>(OnLaneAssistStateChanged);
			_broadcaster.Add<LaneAssistVisualUpdateEvent>(OnVisualUpdate);

			Debug.Log("[LANE ASSIST BEHAVIOUR] ✅ Eventi sottoscritti");
		}

		/// <summary>
		/// Rimozione sottoscrizioni eventi
		/// </summary>
		private void UnsubscribeFromEvents()
		{
			Debug.Log("[LANE ASSIST BEHAVIOUR] 📡 Rimozione sottoscrizioni eventi...");

			_broadcaster.Remove<LaneAssistStateChangedEvent>(OnLaneAssistStateChanged);
			_broadcaster.Remove<LaneAssistVisualUpdateEvent>(OnVisualUpdate);

			Debug.Log("[LANE ASSIST BEHAVIOUR] ✅ Sottoscrizioni rimosse");
		}

		#endregion

		#region Core Lane Departure Detection Logic

		/// <summary>
		/// Update stato sistema basato su velocità
		/// </summary>
		private void UpdateSystemState()
		{
			float currentSpeed = _vehicleDataService.CurrentSpeed;
			bool shouldBeActive = LaneAssistData.CanActivateLaneAssist(currentSpeed);

			if (_isSystemActive != shouldBeActive)
			{
				_isSystemActive = shouldBeActive;

				if (_isSystemActive)
				{
					Debug.Log($"[LANE ASSIST BEHAVIOUR] ✅ Sistema ATTIVO - Velocità: {currentSpeed:F1} km/h");
					UpdateStatusText("Lane Assist: ON"); // 🆕 NUOVO: Status semplificato
				}
				else
				{
					Debug.Log($"[LANE ASSIST BEHAVIOUR] ⚠️ Sistema INATTIVO - Velocità: {currentSpeed:F1} km/h < {LaneAssistData.MIN_ACTIVATION_SPEED} km/h");
					UpdateStatusText("Lane Assist: OFF"); // 🆕 NUOVO: Status semplificato

					// Reset departure se sistema diventa inattivo
					if (_currentDeparture != LaneAssistData.LaneDepartureType.None)
					{
						ResetLaneDeparture("Speed too low");
					}
				}
			}
		}

		/// <summary>
		/// Monitora input A/D per lane departure detection - CORE LOGIC
		/// </summary>
		private void MonitorLaneDepartureInput()
		{
			float deltaTime = Time.deltaTime;

			// === MONITOR A KEY (LEFT DEPARTURE) ===
			if (Input.GetKey(LaneAssistData.LEFT_DEPARTURE_KEY))
			{
				_isLeftKeyHeld = true;
				_leftKeyHoldTime += deltaTime;

				// Check per lane departure threshold
				if (!IsInDeparture(LaneAssistData.LaneDepartureType.Left) &&
					LaneAssistData.IsLaneDeparture(_leftKeyHoldTime))
				{
					TriggerLaneDeparture(LaneAssistData.LaneDepartureType.Left, _leftKeyHoldTime);
				}
			}
			else if (_isLeftKeyHeld)
			{
				// A key rilasciato
				_isLeftKeyHeld = false;

				if (IsInDeparture(LaneAssistData.LaneDepartureType.Left))
				{
					Debug.Log($"[LANE ASSIST BEHAVIOUR] 🔄 A key rilasciato dopo {_leftKeyHoldTime:F1}s - Inizio auto-reset");
				}

				_leftKeyHoldTime = 0f;
			}

			// === MONITOR D KEY (RIGHT DEPARTURE) ===
			if (Input.GetKey(LaneAssistData.RIGHT_DEPARTURE_KEY))
			{
				_isRightKeyHeld = true;
				_rightKeyHoldTime += deltaTime;

				// Check per lane departure threshold
				if (!IsInDeparture(LaneAssistData.LaneDepartureType.Right) &&
					LaneAssistData.IsLaneDeparture(_rightKeyHoldTime))
				{
					TriggerLaneDeparture(LaneAssistData.LaneDepartureType.Right, _rightKeyHoldTime);
				}
			}
			else if (_isRightKeyHeld)
			{
				// D key rilasciato
				_isRightKeyHeld = false;

				if (IsInDeparture(LaneAssistData.LaneDepartureType.Right))
				{
					Debug.Log($"[LANE ASSIST BEHAVIOUR] 🔄 D key rilasciato dopo {_rightKeyHoldTime:F1}s - Inizio auto-reset");
				}

				_rightKeyHoldTime = 0f;
			}
		}

		/// <summary>
		/// Trigger lane departure detection
		/// </summary>
		private void TriggerLaneDeparture(LaneAssistData.LaneDepartureType departureType, float holdTime)
		{
			Debug.Log($"[LANE ASSIST BEHAVIOUR] 🚨 LANE DEPARTURE DETECTED: {departureType} dopo {holdTime:F1}s");

			_currentDeparture = departureType;
			_departureStartTime = Time.time;
			_autoResetTimer = 0f;

			float currentSpeed = _vehicleDataService.CurrentSpeed;

			// 🆕 NUOVO: Update status text a WARNING
			UpdateStatusText($"Lane Assist: ATTENZIONE");

			// 🆕 NUOVO: Avvia audio ripetuto
			StartRepeatingAudio(departureType);

			// Broadcast lane departure event
			var departureEvent = new LaneDepartureDetectedEvent(departureType, holdTime, currentSpeed);
			_broadcaster.Broadcast(departureEvent);
		}

		/// <summary>
		/// Gestisce auto-reset quando keys non sono più tenuti
		/// </summary>
		private void HandleAutoReset()
		{
			// Auto-reset solo se nessun key è tenuto premuto
			bool anyKeyHeld = _isLeftKeyHeld || _isRightKeyHeld;

			if (!anyKeyHeld)
			{
				_autoResetTimer += Time.deltaTime;

				if (_autoResetTimer >= LaneAssistData.AUTO_RESET_TIME)
				{
					Debug.Log($"[LANE ASSIST BEHAVIOUR] ⏰ Auto-reset lane departure dopo {_autoResetTimer:F1}s");
					ResetLaneDeparture("Auto-reset timeout");
				}
			}
			else
			{
				// Reset timer se key è ancora premuto
				_autoResetTimer = 0f;
			}
		}

		/// <summary>
		/// Reset lane departure
		/// </summary>
		private void ResetLaneDeparture(string reason)
		{
			if (_currentDeparture != LaneAssistData.LaneDepartureType.None)
			{
				var previousDeparture = _currentDeparture;
				_currentDeparture = LaneAssistData.LaneDepartureType.None;
				_departureStartTime = 0f;
				_autoResetTimer = 0f;

				Debug.Log($"[LANE ASSIST BEHAVIOUR] ✅ Lane departure reset: {previousDeparture} ({reason})");

				// 🆕 NUOVO: Stop audio ripetuto e ritorna a status normale
				StopRepeatingAudio();
				UpdateStatusText(_isSystemActive ? "Lane Assist: ON" : "Lane Assist: OFF");

				// Broadcast reset event
				var resetEvent = new LaneDepartureResetEvent(previousDeparture);
				_broadcaster.Broadcast(resetEvent);
			}
		}

		/// <summary>
		/// Verifica se è attualmente in lane departure
		/// </summary>
		private bool IsInDeparture(LaneAssistData.LaneDepartureType departureType)
		{
			return _currentDeparture == departureType;
		}

		#endregion

		#region 🆕 NUOVO: Repeating Audio System

		/// <summary>
		/// 🆕 NUOVO: Avvia audio warning ripetuto durante lane departure
		/// </summary>
		private void StartRepeatingAudio(LaneAssistData.LaneDepartureType departureType)
		{
			// Stop previous audio se attivo
			StopRepeatingAudio();

			// Avvia coroutine audio ripetuto
			_audioWarningCoroutine = StartCoroutine(RepeatAudioWarning(departureType));

			Debug.Log($"[LANE ASSIST BEHAVIOUR] 🔊 Audio ripetuto avviato per {departureType}");
		}

		/// <summary>
		/// 🆕 NUOVO: Stop audio warning ripetuto
		/// </summary>
		private void StopRepeatingAudio()
		{
			if (_audioWarningCoroutine != null)
			{
				StopCoroutine(_audioWarningCoroutine);
				_audioWarningCoroutine = null;

				Debug.Log("[LANE ASSIST BEHAVIOUR] 🔇 Audio ripetuto fermato");
			}
		}

		/// <summary>
		/// 🆕 NUOVO: Coroutine per audio ripetuto
		/// </summary>
		private IEnumerator RepeatAudioWarning(LaneAssistData.LaneDepartureType departureType)
		{
			while (_currentDeparture == departureType)
			{
				// Richiedi audio warning
				RequestAudioWarning(departureType);

				// Aspetta prima della prossima ripetizione
				yield return new WaitForSeconds(AUDIO_REPEAT_INTERVAL);
			}

			Debug.Log($"[LANE ASSIST BEHAVIOUR] 🔇 Audio ripetuto terminato per {departureType}");
		}

		/// <summary>
		/// Richiede audio warning per lane departure
		/// </summary>
		private void RequestAudioWarning(LaneAssistData.LaneDepartureType departureType)
		{
			string audioPath = departureType switch
			{
				LaneAssistData.LaneDepartureType.Left => LaneAssistData.LANE_DEPARTURE_LEFT_AUDIO_PATH,
				LaneAssistData.LaneDepartureType.Right => LaneAssistData.LANE_DEPARTURE_RIGHT_AUDIO_PATH,
				_ => ""
			};

			if (!string.IsNullOrEmpty(audioPath))
			{
				var audioRequest = new LaneAssistAudioRequestEvent(audioPath, departureType);
				_broadcaster.Broadcast(audioRequest);

				Debug.Log($"[LANE ASSIST BEHAVIOUR] 🔊 Audio warning richiesto: {audioPath}");
			}
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Gestisce cambio stato lane assist
		/// </summary>
		private void OnLaneAssistStateChanged(LaneAssistStateChangedEvent e)
		{
			Debug.Log($"[LANE ASSIST BEHAVIOUR] 🔄 Stato cambiato: {e.PreviousState} → {e.NewState}");

			_currentState = e.NewState;

			// 🆕 NUOVO: Update status text semplificato solo se non in warning
			if (_currentDeparture == LaneAssistData.LaneDepartureType.None)
			{
				string statusText = e.NewState switch
				{
					LaneAssistData.LaneAssistState.Active => "Lane Assist: ON",
					LaneAssistData.LaneAssistState.Disabled => "Lane Assist: OFF",
					LaneAssistData.LaneAssistState.Unavailable => "Lane Assist: OFF",
					_ => "Lane Assist: OFF"
				};

				UpdateStatusText(statusText);
			}
			// Se in warning, il testo è già impostato in TriggerLaneDeparture
		}

		/// <summary>
		/// Gestisce update visuali
		/// </summary>
		private void OnVisualUpdate(LaneAssistVisualUpdateEvent e)
		{
			Debug.Log($"[LANE ASSIST BEHAVIOUR] 🎨 Visual update: {e.CurrentDeparture}, Warning: {e.ShowWarning}");

			// Update lane lines color
			SetLaneLineColors(e.LaneColor);

			// Update car icon position
			SetCarIconShift(e.CarIconShift);
		}

		#endregion

		#region Visual Update Methods

		/// <summary>
		/// Imposta colore lane lines
		/// </summary>
		private void SetLaneLineColors(Color color)
		{
			if (_leftLaneLines != null)
				_leftLaneLines.color = color;

			if (_rightLaneLines != null)
				_rightLaneLines.color = color;
		}

		/// <summary>
		/// Imposta shift car icon per departure visual
		/// </summary>
		private void SetCarIconShift(float shiftAmount)
		{
			if (_carIcon != null)
			{
				Vector3 newPosition = _originalCarIconPosition + new Vector3(shiftAmount, 0f, 0f);
				_carIcon.transform.localPosition = newPosition;
			}
		}

		/// <summary>
		/// Reset car icon alla posizione originale
		/// </summary>
		private void ResetCarIconPosition()
		{
			if (_carIcon != null)
			{
				_carIcon.transform.localPosition = _originalCarIconPosition;
			}
		}

		/// <summary>
		/// Update testo stato
		/// </summary>
		private void UpdateStatusText(string message)
		{
			if (_statusText != null)
			{
				_statusText.text = message;
			}
		}

		#endregion

		#region Debug Methods

		/// <summary>
		/// Update info debug se panel attivo
		/// </summary>
		private void UpdateDebugInfo()
		{
			if (_debugPanel != null && _debugPanel.activeSelf && _debugText != null)
			{
				float currentSpeed = _vehicleDataService.CurrentSpeed;

				string debugInfo = $"=== LANE ASSIST DEBUG ===\n" +
								  $"Speed: {currentSpeed:F1} km/h\n" +
								  $"System Active: {_isSystemActive}\n" +
								  $"Current State: {_currentState}\n" +
								  $"Current Departure: {_currentDeparture}\n" +
								  $"Left Key Hold: {_leftKeyHoldTime:F1}s\n" +
								  $"Right Key Hold: {_rightKeyHoldTime:F1}s\n" +
								  $"Auto Reset Timer: {_autoResetTimer:F1}s\n" +
								  $"Audio Repeating: {(_audioWarningCoroutine != null ? "YES" : "NO")}\n" +
								  $"=== CONTROLS ===\n" +
								  $"A = Left Departure\n" +
								  $"D = Right Departure\n" +
								  $"Hold 2s+ for trigger\n" +
								  $"Speed > 30 km/h to activate";

				_debugText.text = debugInfo;
			}
		}

		#endregion
	}
}