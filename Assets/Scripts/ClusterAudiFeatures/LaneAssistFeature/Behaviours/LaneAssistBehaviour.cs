using ClusterAudi;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// LaneAssist Behaviour - VERSIONE SEMPLIFICATA
	/// Rimossi: Debug Panel, DebugText, validazioni eccessive
	/// Mantenuti: Solo funzionalità core con stesso risultato
	/// </summary>
	public class LaneAssistBehaviour : BaseMonoBehaviour<ILaneAssistFeatureInternal>
	{
		#region Serialized Fields - SOLO ESSENZIALI

		[Header("Lane Assist Components")]
		[SerializeField] private Image _leftLaneLines;
		[SerializeField] private Image _rightLaneLines;
		[SerializeField] private Image _carIcon;
		[SerializeField] private TextMeshProUGUI _statusText;

		#endregion

		#region Private Fields - SEMPLIFICATI

		private IBroadcaster _broadcaster;
		private IVehicleDataService _vehicleDataService;

		// Lane departure detection
		private float _leftKeyHoldTime = 0f;
		private float _rightKeyHoldTime = 0f;
		private bool _isLeftKeyHeld = false;
		private bool _isRightKeyHeld = false;

		// State
		private LaneAssistData.LaneDepartureType _currentDeparture = LaneAssistData.LaneDepartureType.None;
		private bool _isSystemActive = false;
		private float _autoResetTimer = 0f;

		// Visual
		private Vector3 _originalCarIconPosition;

		// Audio ripetuto
		private Coroutine _audioCoroutine;

		#endregion

		#region BaseMonoBehaviour Override

		protected override void ManagedAwake()
		{
			Debug.Log("[LANE ASSIST UI] 🛣️ LaneAssistBehaviour inizializzato");

			// Setup servizi
			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();
			_vehicleDataService = client.Services.Get<IVehicleDataService>();

			// Setup iniziale
			InitializeUI();
		}

		protected override void ManagedUpdate()
		{
			// Update sistema basato su velocità
			UpdateSystemState();

			// Monitor A/D keys se sistema attivo
			if (_isSystemActive)
			{
				MonitorLaneDepartureInput();
			}

			// Gestisce auto-reset
			if (_currentDeparture != LaneAssistData.LaneDepartureType.None)
			{
				HandleAutoReset();
			}
		}

		protected override void ManagedOnDestroy()
		{
			Debug.Log("[LANE ASSIST UI] 🗑️ LaneAssist UI distrutta");
			StopAudio();
		}

		#endregion

		#region Core Logic - SEMPLIFICATO

		/// <summary>
		/// Inizializza UI
		/// </summary>
		private void InitializeUI()
		{
			if (_carIcon != null)
				_originalCarIconPosition = _carIcon.transform.localPosition;

			SetLaneLineColors(LaneAssistData.NORMAL_LANE_COLOR);
			UpdateStatusText("Lane Assist: OFF");

			Debug.Log("[LANE ASSIST UI] ✅ UI inizializzata");
		}

		/// <summary>
		/// Update stato sistema
		/// </summary>
		private void UpdateSystemState()
		{
			float currentSpeed = _vehicleDataService.CurrentSpeed;
			bool shouldBeActive = LaneAssistData.CanActivateLaneAssist(currentSpeed);

			if (_isSystemActive != shouldBeActive)
			{
				_isSystemActive = shouldBeActive;
				UpdateStatusText(_isSystemActive ? "Lane Assist: ON" : "Lane Assist: OFF");

				if (!_isSystemActive && _currentDeparture != LaneAssistData.LaneDepartureType.None)
				{
					ResetLaneDeparture();
				}
			}
		}

		/// <summary>
		/// Monitor input A/D - CORE LOGIC SEMPLIFICATO
		/// </summary>
		private void MonitorLaneDepartureInput()
		{
			// A KEY (LEFT)
			if (Input.GetKey(LaneAssistData.LEFT_DEPARTURE_KEY))
			{
				_isLeftKeyHeld = true;
				_leftKeyHoldTime += Time.deltaTime;

				if (_currentDeparture != LaneAssistData.LaneDepartureType.Left && 
					LaneAssistData.IsLaneDeparture(_leftKeyHoldTime))
				{
					TriggerLaneDeparture(LaneAssistData.LaneDepartureType.Left);
				}
			}
			else if (_isLeftKeyHeld)
			{
				_isLeftKeyHeld = false;
				_leftKeyHoldTime = 0f;
			}

			// D KEY (RIGHT)
			if (Input.GetKey(LaneAssistData.RIGHT_DEPARTURE_KEY))
			{
				_isRightKeyHeld = true;
				_rightKeyHoldTime += Time.deltaTime;

				if (_currentDeparture != LaneAssistData.LaneDepartureType.Right && 
					LaneAssistData.IsLaneDeparture(_rightKeyHoldTime))
				{
					TriggerLaneDeparture(LaneAssistData.LaneDepartureType.Right);
				}
			}
			else if (_isRightKeyHeld)
			{
				_isRightKeyHeld = false;
				_rightKeyHoldTime = 0f;
			}
		}

		/// <summary>
		/// Trigger lane departure
		/// </summary>
		private void TriggerLaneDeparture(LaneAssistData.LaneDepartureType departureType)
		{
			Debug.Log($"[LANE ASSIST UI] 🚨 LANE DEPARTURE: {departureType}");

			_currentDeparture = departureType;
			_autoResetTimer = 0f;

			// Update visuals
			UpdateStatusText("Lane Assist: WARNING");
			SetLaneLineColors(LaneAssistData.DEPARTURE_LANE_COLOR);
			SetCarIconShift(departureType == LaneAssistData.LaneDepartureType.Left ? -20f : 20f);

			// Avvia audio ripetuto
			StartRepeatingAudio(departureType);

			// Broadcast evento
			_broadcaster.Broadcast(new LaneDepartureDetectedEvent(departureType, 2f));
		}

		/// <summary>
		/// Auto-reset handler
		/// </summary>
		private void HandleAutoReset()
		{
			if (!_isLeftKeyHeld && !_isRightKeyHeld)
			{
				_autoResetTimer += Time.deltaTime;

				if (_autoResetTimer >= LaneAssistData.AUTO_RESET_TIME)
				{
					ResetLaneDeparture();
				}
			}
			else
			{
				_autoResetTimer = 0f;
			}
		}

		/// <summary>
		/// Reset lane departure
		/// </summary>
		private void ResetLaneDeparture()
		{
			Debug.Log("[LANE ASSIST UI] ✅ Lane departure reset");

			_currentDeparture = LaneAssistData.LaneDepartureType.None;
			_autoResetTimer = 0f;

			// Reset visuals
			UpdateStatusText(_isSystemActive ? "Lane Assist: ON" : "Lane Assist: OFF");
			SetLaneLineColors(LaneAssistData.NORMAL_LANE_COLOR);
			ResetCarIconPosition();

			// Stop audio
			StopAudio();

			// Broadcast evento
			_broadcaster.Broadcast(new LaneDepartureResetEvent());
		}

		#endregion

		#region Audio System - SEMPLIFICATO

		/// <summary>
		/// Avvia audio ripetuto
		/// </summary>
		private void StartRepeatingAudio(LaneAssistData.LaneDepartureType departureType)
		{
			StopAudio();
			_audioCoroutine = StartCoroutine(AudioCoroutine(departureType));
		}

		/// <summary>
		/// Stop audio
		/// </summary>
		private void StopAudio()
		{
			if (_audioCoroutine != null)
			{
				StopCoroutine(_audioCoroutine);
				_audioCoroutine = null;
			}
		}

		/// <summary>
		/// Coroutine audio ripetuto
		/// </summary>
		private IEnumerator AudioCoroutine(LaneAssistData.LaneDepartureType departureType)
		{
			string audioPath = departureType == LaneAssistData.LaneDepartureType.Left 
				? LaneAssistData.LANE_DEPARTURE_LEFT_AUDIO_PATH 
				: LaneAssistData.LANE_DEPARTURE_RIGHT_AUDIO_PATH;

			while (_currentDeparture == departureType)
			{
				_broadcaster.Broadcast(new LaneAssistAudioRequestEvent(audioPath, departureType));
				yield return new WaitForSeconds(2f);
			}
		}

		#endregion

		#region Visual Methods - SEMPLIFICATI

		private void SetLaneLineColors(Color color)
		{
			if (_leftLaneLines != null) _leftLaneLines.color = color;
			if (_rightLaneLines != null) _rightLaneLines.color = color;
		}

		private void SetCarIconShift(float shiftAmount)
		{
			if (_carIcon != null)
				_carIcon.transform.localPosition = _originalCarIconPosition + new Vector3(shiftAmount, 0f, 0f);
		}

		private void ResetCarIconPosition()
		{
			if (_carIcon != null)
				_carIcon.transform.localPosition = _originalCarIconPosition;
		}

		private void UpdateStatusText(string message)
		{
			if (_statusText != null)
				_statusText.text = message;
		}

		#endregion
	}
}