using ClusterAudi;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// SeatBelt Feature - VERSIONE CORRETTA
	/// Rimosso: Warning Panel logic
	/// Mantenuto: Warning system + lampeggio icone + audio continuo
	/// </summary>
	public class SeatBeltFeature : BaseFeature, ISeatBeltFeature, ISeatBeltFeatureInternal
	{
		#region Private Fields

		private SeatBeltConfig _currentConfiguration;
		private SeatBeltData.SeatBeltStatus[] _seatBeltStates;
		private SeatBeltBehaviour _seatBeltBehaviour;

		// Warning system
		private bool _isWarningSystemActive = false;
		private float _warningStartTime = 0f;
		private AudioEscalationLevel _currentAudioLevel = AudioEscalationLevel.None;

		// Services
		private IVehicleDataService _vehicleDataService;

		// Continuous audio
		private System.Collections.IEnumerator _continuousAudioCoroutine;

		#endregion

		#region Constructor

		public SeatBeltFeature(Client client) : base(client)
		{
			Debug.Log("[SEATBELT FEATURE] 🛡️ SeatBeltFeature inizializzata");

			// Inizializza stati (tutte slacciate per testing)
			_seatBeltStates = new SeatBeltData.SeatBeltStatus[SeatBeltData.TOTAL_SEATBELTS];
			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				_seatBeltStates[i] = SeatBeltData.SeatBeltStatus.Unfastened;
			}

			// Cache servizi
			_vehicleDataService = client.Services.Get<IVehicleDataService>();
			_currentConfiguration = SeatBeltData.GetConfigForDriveMode(_vehicleDataService.CurrentDriveMode);

			// Sottoscrivi eventi
			_vehicleDataService.OnSpeedChanged += OnSpeedChanged;

			Debug.Log("[SEATBELT FEATURE] 🔧 Stati iniziali: TUTTE SLACCIATE per testing");
		}

		#endregion

		#region ISeatBeltFeature Implementation

		public async Task InstantiateSeatBeltFeature()
		{
			Debug.Log("[SEATBELT FEATURE] 🚀 Istanziazione SeatBelt UI...");

			try
			{
				var seatBeltInstance = await _assetService.InstantiateAsset<SeatBeltBehaviour>(
					SeatBeltData.SEATBELT_PREFAB_PATH);

				if (seatBeltInstance != null)
				{
					seatBeltInstance.Initialize(this);
					_seatBeltBehaviour = seatBeltInstance;
					_seatBeltBehaviour.UpdateAllSeatBeltStates(_seatBeltStates);
					Debug.Log("[SEATBELT FEATURE] ✅ UI istanziata da prefab");
				}
				else
				{
					Debug.LogWarning("[SEATBELT FEATURE] ⚠️ Prefab non trovato, creando fallback...");
					await CreateSeatBeltDynamically();
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[SEATBELT FEATURE] ❌ Errore istanziazione: {ex.Message}");
			}
		}

		public void SetSeatBeltStatus(SeatBeltData.SeatBeltPosition position, SeatBeltData.SeatBeltStatus status)
		{
			int index = (int)position;
			if (index < 0 || index >= SeatBeltData.TOTAL_SEATBELTS) return;

			var oldStatus = _seatBeltStates[index];
			if (oldStatus == status) return;

			_seatBeltStates[index] = status;

			// Broadcast evento cambio stato
			_broadcaster.Broadcast(new SeatBeltStatusChangedEvent(position, oldStatus, status));

			// Aggiorna UI
			if (_seatBeltBehaviour != null)
			{
				_seatBeltBehaviour.UpdateSeatBeltStatus(position, status);
			}

			// Rivaluta warning system
			EvaluateWarningSystem();

			Debug.Log($"[SEATBELT FEATURE] 🔄 Cintura {position}: {oldStatus} → {status}");
		}

		public void ForceWarningCheck()
		{
			Debug.Log("[SEATBELT FEATURE] 🔧 Check forzato sistema cinture");
			EvaluateWarningSystem();
		}

		#endregion

		#region ISeatBeltFeatureInternal Implementation

		public Client GetClient() => _client;

		public SeatBeltConfig GetCurrentConfiguration() => _currentConfiguration;

		public SeatBeltData.SeatBeltStatus[] GetAllSeatBeltStates()
		{
			var copy = new SeatBeltData.SeatBeltStatus[SeatBeltData.TOTAL_SEATBELTS];
			System.Array.Copy(_seatBeltStates, copy, SeatBeltData.TOTAL_SEATBELTS);
			return copy;
		}

		public bool IsWarningSystemActive() => _isWarningSystemActive;

		#endregion

		#region Warning System Core Logic

		/// <summary>
		/// Valuta se attivare warning system
		/// </summary>
		private void EvaluateWarningSystem()
		{
			float currentSpeed = _vehicleDataService.CurrentSpeed;
			float speedThreshold = _currentConfiguration.SpeedThreshold;

			bool shouldShowWarning = currentSpeed > speedThreshold && HasUnfastenedBelts();

			if (shouldShowWarning && !_isWarningSystemActive)
			{
				StartWarningSystem();
			}
			else if (!shouldShowWarning && _isWarningSystemActive)
			{
				StopWarningSystem();
			}
		}

		/// <summary>
		/// Avvia warning system con lampeggio icone + audio
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
				unfastenedBelts));

			// Attiva lampeggio icone cinture slacciate
			if (_currentConfiguration.FlashingEnabled)
			{
				_broadcaster.Broadcast(new SeatBeltFlashIconsEvent(unfastenedBelts, true, 1f)); // 1 secondo
			}

			// Avvia sistema audio continuo
			if (_currentConfiguration.EnableAudioWarning)
			{
				StartContinuousAudioWarning();
			}

			Debug.Log($"[SEATBELT FEATURE] 🚨 WARNING ATTIVATO: {unfastenedBelts.Length} cinture slacciate - Lampeggio + Audio ogni 2s");
		}

		/// <summary>
		/// Ferma warning system
		/// </summary>
		private void StopWarningSystem()
		{
			if (!_isWarningSystemActive) return;

			float totalDuration = Time.time - _warningStartTime;
			_isWarningSystemActive = false;
			_currentAudioLevel = AudioEscalationLevel.None;

			// Ferma audio continuo
			StopContinuousAudioWarning();

			// Broadcast evento fine warning
			_broadcaster.Broadcast(new SeatBeltWarningStoppedEvent(totalDuration));

			// Ferma lampeggio
			_broadcaster.Broadcast(new SeatBeltFlashIconsEvent(new SeatBeltData.SeatBeltPosition[0], false));

			Debug.Log($"[SEATBELT FEATURE] ✅ WARNING FERMATO (durata: {totalDuration:F1}s) - Audio e lampeggio fermati");
		}

		#endregion

		#region Continuous Audio System

		/// <summary>
		/// Avvia sistema audio continuo ogni 2 secondi
		/// </summary>
		private void StartContinuousAudioWarning()
		{
			if (_continuousAudioCoroutine != null)
			{
				_client.StopCoroutine(_continuousAudioCoroutine);
			}

			_continuousAudioCoroutine = ContinuousAudioCoroutine();
			_client.StartCoroutine(_continuousAudioCoroutine);
			Debug.Log("[SEATBELT FEATURE] 🔄 Audio continuo avviato (ogni 2 secondi)");
		}

		/// <summary>
		/// Ferma sistema audio continuo
		/// </summary>
		private void StopContinuousAudioWarning()
		{
			if (_continuousAudioCoroutine != null)
			{
				_client.StopCoroutine(_continuousAudioCoroutine);
				_continuousAudioCoroutine = null;
			}

			_broadcaster.Broadcast(new StopSeatBeltAudioEvent());
			Debug.Log("[SEATBELT FEATURE] 🛑 Audio continuo fermato");
		}

		/// <summary>
		/// Coroutine audio continuo con escalation
		/// </summary>
		private System.Collections.IEnumerator ContinuousAudioCoroutine()
		{
			const float AUDIO_INTERVAL = 2f;

			while (_isWarningSystemActive)
			{
				// Calcola escalation
				float warningDuration = Time.time - _warningStartTime;
				var currentLevel = SeatBeltData.GetAudioEscalationLevel(warningDuration);

				if (currentLevel != _currentAudioLevel)
				{
					_currentAudioLevel = currentLevel;
					Debug.Log($"[SEATBELT FEATURE] 📈 Audio escalation: {currentLevel} (tempo: {warningDuration:F1}s)");
				}

				// Riproduci audio
				string audioPath = GetAudioPathForLevel(_currentAudioLevel);
				_broadcaster.Broadcast(new PlaySeatBeltAudioEvent(audioPath, 1f, 5));

				yield return new WaitForSeconds(AUDIO_INTERVAL);
			}

			_continuousAudioCoroutine = null;
		}

		private string GetAudioPathForLevel(AudioEscalationLevel level)
		{
			return level switch
			{
				AudioEscalationLevel.Urgent => SeatBeltData.URGENT_BEEP_AUDIO_PATH,
				AudioEscalationLevel.Continuous => SeatBeltData.CONTINUOUS_BEEP_AUDIO_PATH,
				_ => SeatBeltData.SOFT_BEEP_AUDIO_PATH
			};
		}

		#endregion

		#region Helper Methods

		private void OnSpeedChanged(float newSpeed)
		{
			EvaluateWarningSystem();
		}

		private bool HasUnfastenedBelts()
		{
			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				if (_seatBeltStates[i] == SeatBeltData.SeatBeltStatus.Unfastened)
					return true;
			}
			return false;
		}

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

		#endregion

		#region Fallback Creation

		private async Task CreateSeatBeltDynamically()
		{
			Debug.Log("[SEATBELT FEATURE] 🔧 Creazione dinamica fallback...");
			await Task.Delay(100);

			var canvasObj = new GameObject("SeatBeltCanvas");
			var canvas = canvasObj.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 15;

			var seatBeltObj = new GameObject("SeatBelt", typeof(RectTransform));
			seatBeltObj.transform.SetParent(canvas.transform, false);

			var rectTransform = seatBeltObj.GetComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.7f, 0.1f);
			rectTransform.anchorMax = new Vector2(0.95f, 0.4f);
			rectTransform.anchoredPosition = Vector2.zero;
			rectTransform.sizeDelta = Vector2.zero;

			_seatBeltBehaviour = seatBeltObj.AddComponent<SeatBeltBehaviour>();
			_seatBeltBehaviour.Initialize(this);
			_seatBeltBehaviour.UpdateAllSeatBeltStates(_seatBeltStates);

			Debug.Log("[SEATBELT FEATURE] ✅ Fallback dinamico creato");
		}

		#endregion
	}
}