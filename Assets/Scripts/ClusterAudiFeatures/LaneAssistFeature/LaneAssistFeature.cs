using System.Threading.Tasks;
using ClusterAudi;
using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// LaneAssist Feature - VERSIONE SEMPLIFICATA
	/// Rimossa complessità eccessiva, mantenute solo funzionalità essenziali
	/// </summary>
	public class LaneAssistFeature : BaseFeature, ILaneAssistFeature, ILaneAssistFeatureInternal
	{
		#region Private Fields - SEMPLIFICATI

		private LaneAssistData.LaneAssistState _currentState = LaneAssistData.LaneAssistState.Disabled;
		private bool _isSystemEnabled = true;
		private LaneAssistBehaviour _behaviour;

		#endregion

		#region Constructor

		public LaneAssistFeature(Client client) : base(client)
		{
			Debug.Log("[LANE ASSIST FEATURE] 🛣️ LaneAssistFeature inizializzata");

			// Sottoscrivi eventi
			_broadcaster.Add<LaneAssistAudioRequestEvent>(OnAudioRequest);
		}

		#endregion

		#region ILaneAssistFeature Implementation

		public async Task InstantiateLaneAssistFeature()
		{
			Debug.Log("[LANE ASSIST FEATURE] 🚀 Istanziazione LaneAssist...");

			try
			{
				var instance = await _assetService.InstantiateAsset<LaneAssistBehaviour>(
					LaneAssistData.LANE_ASSIST_PREFAB_PATH);

				if (instance != null)
				{
					instance.Initialize(this);
					_behaviour = instance;
					Debug.Log("[LANE ASSIST FEATURE] ✅ UI istanziata da prefab");
				}
				else
				{
					await CreateDynamicFallback();
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[LANE ASSIST FEATURE] ❌ Errore: {ex.Message}");
				await CreateDynamicFallback();
			}
		}

		public void SetLaneAssistEnabled(bool enabled)
		{
			_isSystemEnabled = enabled;
			_currentState = enabled ? LaneAssistData.LaneAssistState.Active : LaneAssistData.LaneAssistState.Disabled;
			Debug.Log($"[LANE ASSIST FEATURE] 🔧 Sistema: {(_isSystemEnabled ? "ON" : "OFF")}");
		}

		public LaneAssistData.LaneAssistState GetCurrentState()
		{
			return _currentState;
		}

		public void ForceResetLaneDeparture()
		{
			Debug.Log("[LANE ASSIST FEATURE] 🔄 Force reset lane departure");
			_broadcaster.Broadcast(new LaneDepartureResetEvent());
		}

		#endregion

		#region ILaneAssistFeatureInternal Implementation

		public Client GetClient() => _client;

		#endregion

		#region Event Handlers

		/// <summary>
		/// Gestisce richieste audio (invia ad AudioFeature esistente)
		/// </summary>
		private void OnAudioRequest(LaneAssistAudioRequestEvent e)
		{
			// Qui dovresti inviare l'evento al tuo sistema audio esistente
			// Es: _broadcaster.Broadcast(new PlayAudioEvent(e.AudioPath, e.Volume, e.Priority));

			Debug.Log($"[LANE ASSIST FEATURE] 🔊 Audio request: {e.AudioPath}");
		}

		#endregion

		#region Fallback Creation

		private async Task CreateDynamicFallback()
		{
			Debug.Log("[LANE ASSIST FEATURE] 🔧 Creazione dinamica fallback...");
			await Task.Delay(100);

			var laneAssistObj = new GameObject("LaneAssist");
			_behaviour = laneAssistObj.AddComponent<LaneAssistBehaviour>();
			_behaviour.Initialize(this);

			Debug.Log("[LANE ASSIST FEATURE] ✅ Fallback dinamico creato");
		}

		#endregion
	}
}