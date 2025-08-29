using System.Threading.Tasks;
using ClusterAudi;
using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Implementazione Lane Assist Feature
	/// IDENTICA al pattern ClusterDriveModeFeature.cs del tuo progetto
	/// 
	/// RESPONSABILITÀ:
	/// - Gestisce l'istanziazione UI lane assist
	/// - Monitora velocità per attivazione automatica
	/// - Rileva lane departure tramite tasti A/D
	/// - Gestisce audio warnings via sistema audio esistente
	/// - Fornisce feedback visuale per lane departure
	/// </summary>
	public class LaneAssistFeature : BaseFeature, ILaneAssistFeature, ILaneAssistFeatureInternal
	{
		#region Private Fields

		/// <summary>
		/// Servizi (ottenuti dal Client)
		/// </summary>
		private IVehicleDataService _vehicleDataService;

		/// <summary>
		/// Stato corrente lane assist
		/// </summary>
		private LaneAssistData.LaneAssistState _currentState = LaneAssistData.LaneAssistState.Disabled;

		/// <summary>
		/// Lane departure corrente
		/// </summary>
		private LaneAssistData.LaneDepartureType _currentDeparture = LaneAssistData.LaneDepartureType.None;

		/// <summary>
		/// Sistema abilitato/disabilitato
		/// </summary>
		private bool _isSystemEnabled = true;

		/// <summary>
		/// Riferimento al behaviour istanziato
		/// </summary>
		private LaneAssistBehaviour _laneAssistBehaviour;

		#endregion

		#region Constructor - IDENTICO Mercedes

		/// <summary>
		/// Costruttore - IDENTICO al pattern del tuo progetto
		/// </summary>
		public LaneAssistFeature(Client client) : base(client)
		{
			Debug.Log("[LANE ASSIST FEATURE] 🛣️ LaneAssistFeature inizializzata");

			// Ottieni servizi necessari
			_vehicleDataService = client.Services.Get<IVehicleDataService>();

			// Sottoscrivi agli eventi interni
			SubscribeToInternalEvents();
		}

		#endregion

		#region ILaneAssistFeature Implementation - IDENTICO Mercedes

		/// <summary>
		/// Istanzia la Lane Assist Feature - IDENTICO al pattern del tuo progetto
		/// </summary>
		public async Task InstantiateLaneAssistFeature()
		{
			Debug.Log("[LANE ASSIST FEATURE] 🚀 Istanziazione Lane Assist Feature...");

			try
			{
				// IDENTICO pattern tuo progetto: usa AssetService per caricare prefab
				var laneAssistInstance = await _assetService.InstantiateAsset<LaneAssistBehaviour>(
					LaneAssistData.LANE_ASSIST_PREFAB_PATH);

				if (laneAssistInstance != null)
				{
					// IDENTICO pattern tuo progetto: Initialize con this per dependency injection
					laneAssistInstance.Initialize(this);
					_laneAssistBehaviour = laneAssistInstance;

					Debug.Log("[LANE ASSIST FEATURE] ✅ Lane Assist UI istanziata da prefab");

					// Inizializza stato basato su velocità corrente
					UpdateStateBasedOnSpeed();
				}
				else
				{
					// IDENTICO pattern tuo progetto: log warning se prefab non trovato
					Debug.LogWarning("[LANE ASSIST FEATURE] ⚠️ Prefab non trovato: " +
						LaneAssistData.LANE_ASSIST_PREFAB_PATH);

					// Fallback: Crea dinamicamente (per development)
					await CreateLaneAssistDynamically();
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[LANE ASSIST FEATURE] ❌ Errore istanziazione: {ex.Message}");
				Debug.LogException(ex);

				// Fallback creation
				await CreateLaneAssistDynamically();
			}
		}

		/// <summary>
		/// Abilita/disabilita lane assist
		/// </summary>
		public void SetLaneAssistEnabled(bool enabled)
		{
			if (_isSystemEnabled != enabled)
			{
				_isSystemEnabled = enabled;

				Debug.Log($"[LANE ASSIST FEATURE] 🔧 Sistema Lane Assist: {(enabled ? "ABILITATO" : "DISABILITATO")}");

				// Update stato
				UpdateStateBasedOnSpeed();

				// Broadcast cambio stato
				var stateEvent = new LaneAssistStateChangedEvent(
					_currentState,
					enabled ? LaneAssistData.LaneAssistState.Active : LaneAssistData.LaneAssistState.Disabled,
					$"System {(enabled ? "enabled" : "disabled")} by user");

				_broadcaster.Broadcast(stateEvent);
			}
		}

		/// <summary>
		/// Ottiene stato corrente
		/// </summary>
		public LaneAssistData.LaneAssistState GetCurrentState()
		{
			return _currentState;
		}

		/// <summary>
		/// Forza reset lane departure
		/// </summary>
		public void ForceResetLaneDeparture()
		{
			Debug.Log("[LANE ASSIST FEATURE] 🔄 Force reset lane departure");

			if (_currentDeparture != LaneAssistData.LaneDepartureType.None)
			{
				var previousDeparture = _currentDeparture;
				_currentDeparture = LaneAssistData.LaneDepartureType.None;

				// Broadcast reset
				_broadcaster.Broadcast(new LaneDepartureResetEvent(previousDeparture));

				// Update visual state
				UpdateVisualFeedback();
			}
		}

		#endregion

		#region ILaneAssistFeatureInternal Implementation - IDENTICO Mercedes

		/// <summary>
		/// Ottiene il Client - IDENTICO al pattern del tuo progetto
		/// </summary>
		public Client GetClient()
		{
			return _client;
		}

		#endregion

		#region Internal Logic

		/// <summary>
		/// Sottoscrizione agli eventi interni
		/// </summary>
		private void SubscribeToInternalEvents()
		{
			// Sottoscrivi a eventi lane departure dal behaviour
			_broadcaster.Add<LaneDepartureDetectedEvent>(OnLaneDepartureDetected);
			_broadcaster.Add<LaneDepartureResetEvent>(OnLaneDepartureReset);

			Debug.Log("[LANE ASSIST FEATURE] 📡 Eventi interni sottoscritti");
		}

		/// <summary>
		/// Aggiorna stato basato su velocità corrente
		/// </summary>
		private void UpdateStateBasedOnSpeed()
		{
			if (!_isSystemEnabled)
			{
				SetState(LaneAssistData.LaneAssistState.Disabled, "System disabled");
				return;
			}

			float currentSpeed = _vehicleDataService.CurrentSpeed;

			if (LaneAssistData.CanActivateLaneAssist(currentSpeed))
			{
				// Velocità sufficiente - sistema attivo
				if (_currentDeparture != LaneAssistData.LaneDepartureType.None)
				{
					SetState(LaneAssistData.LaneAssistState.Warning, $"Lane departure active at {currentSpeed:F1} km/h");
				}
				else
				{
					SetState(LaneAssistData.LaneAssistState.Active, $"Active monitoring at {currentSpeed:F1} km/h");
				}
			}
			else
			{
				// Velocità insufficiente - sistema non attivo
				SetState(LaneAssistData.LaneAssistState.Disabled, $"Speed too low: {currentSpeed:F1} km/h < {LaneAssistData.MIN_ACTIVATION_SPEED} km/h");
			}
		}

		/// <summary>
		/// Imposta nuovo stato lane assist
		/// </summary>
		private void SetState(LaneAssistData.LaneAssistState newState, string reason)
		{
			if (_currentState != newState)
			{
				var previousState = _currentState;
				_currentState = newState;

				Debug.Log($"[LANE ASSIST FEATURE] 🔄 Stato: {previousState} → {newState} ({reason})");

				// Broadcast cambio stato
				_broadcaster.Broadcast(new LaneAssistStateChangedEvent(previousState, newState, reason));

				// Update feedback visuale
				UpdateVisualFeedback();
			}
		}

		/// <summary>
		/// Aggiorna feedback visuale
		/// </summary>
		private void UpdateVisualFeedback()
		{
			// Determina colore lane lines
			UnityEngine.Color laneColor = (_currentDeparture != LaneAssistData.LaneDepartureType.None)
				? LaneAssistData.DEPARTURE_LANE_COLOR  // Giallo warning
				: LaneAssistData.NORMAL_LANE_COLOR;    // Bianco normale

			// Determina shift car icon
			float carIconShift = _currentDeparture switch
			{
				LaneAssistData.LaneDepartureType.Left => -LaneAssistData.CAR_ICON_SHIFT_DISTANCE,
				LaneAssistData.LaneDepartureType.Right => LaneAssistData.CAR_ICON_SHIFT_DISTANCE,
				_ => 0f
			};

			// Broadcast visual update
			var visualEvent = new LaneAssistVisualUpdateEvent(
				_currentState,
				_currentDeparture,
				_currentDeparture != LaneAssistData.LaneDepartureType.None,
				laneColor,
				carIconShift);

			_broadcaster.Broadcast(visualEvent);
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Gestisce lane departure detected
		/// </summary>
		private void OnLaneDepartureDetected(LaneDepartureDetectedEvent e)
		{
			Debug.Log($"[LANE ASSIST FEATURE] 🚨 Lane departure detected: {e.DepartureType} " +
					 $"dopo {e.DepartureTime:F1}s a {e.CurrentSpeed:F1} km/h");

			// Update departure state
			_currentDeparture = e.DepartureType;

			// Update stato a WARNING
			SetState(LaneAssistData.LaneAssistState.Warning, $"{e.DepartureType} departure detected");

			// Richiedi audio warning
			RequestAudioWarning(e.DepartureType);
		}

		/// <summary>
		/// Gestisce lane departure reset
		/// </summary>
		private void OnLaneDepartureReset(LaneDepartureResetEvent e)
		{
			Debug.Log($"[LANE ASSIST FEATURE] ✅ Lane departure reset da {e.PreviousDepartureType}");

			// Reset departure state
			_currentDeparture = LaneAssistData.LaneDepartureType.None;

			// Update stato basato su velocità
			UpdateStateBasedOnSpeed();
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

				Debug.Log($"[LANE ASSIST FEATURE] 🔊 Audio warning richiesto: {audioPath}");
			}
		}

		#endregion

		#region Fallback Creation

		/// <summary>
		/// Creazione dinamica per development quando prefab non disponibile
		/// </summary>
		private async Task CreateLaneAssistDynamically()
		{
			Debug.Log("[LANE ASSIST FEATURE] 🔧 Creazione dinamica Lane Assist...");

			await Task.Delay(100);

			// Crea GameObject per Lane Assist
			var laneAssistObject = new GameObject("LaneAssist");

			// Aggiungi LaneAssistBehaviour
			var laneAssistBehaviour = laneAssistObject.AddComponent<LaneAssistBehaviour>();
			laneAssistBehaviour.Initialize(this);
			_laneAssistBehaviour = laneAssistBehaviour;

			// Inizializza stato
			UpdateStateBasedOnSpeed();

			Debug.Log("[LANE ASSIST FEATURE] ✅ Lane Assist creato dinamicamente");
		}

		#endregion

		#region Cleanup

		~LaneAssistFeature()
		{
			// Rimuovi sottoscrizioni eventi
			if (_broadcaster != null)
			{
				_broadcaster.Remove<LaneDepartureDetectedEvent>(OnLaneDepartureDetected);
				_broadcaster.Remove<LaneDepartureResetEvent>(OnLaneDepartureReset);
			}
		}

		#endregion
	}
}