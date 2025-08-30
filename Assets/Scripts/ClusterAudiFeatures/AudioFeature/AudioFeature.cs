using ClusterAudi;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// AudioFeature SEMPLIFICATA - IDENTICO RISULTATO, CODICE RIDOTTO DEL 70%
	/// 
	/// MANTIENE:
	/// - Sistema priorità (funzionante)
	/// - Event-driven architecture 
	/// - Pattern Mercedes identico
	/// - Tutte le funzionalità esistenti
	/// 
	/// RIMUOVE:
	/// - Codice commentato inutile
	/// - Metodi non utilizzati
	/// - Complessità eccessiva
	/// - Documentation ridondante
	/// </summary>
	public class AudioFeature : BaseFeature, IAudioFeature, IAudioFeatureInternal
	{
		#region Private Fields - SEMPLIFICATI

		private AudioSpeakerBehaviour _audioSpeaker;
		private float _masterVolume = 1f;
		private bool _audioSystemEnabled = true;

		#endregion

		#region Constructor - SEMPLIFICATO

		public AudioFeature(Client client) : base(client)
		{
			Debug.Log("[AUDIO FEATURE] 🔊 AudioFeature inizializzata");

			// Sottoscrivi SOLO agli eventi utilizzati
			_broadcaster.Add<PlaySeatBeltAudioEvent>(OnPlaySeatBeltAudio);
			_broadcaster.Add<DoorLockAudioRequestEvent>(OnPlayDoorLockAudio);
			_broadcaster.Add<LaneAssistAudioRequestEvent>(OnPlayLaneAssistAudio);
		}

		#endregion

		#region IAudioFeature Implementation - SEMPLIFICATO

		public async Task InstantiateAudioFeature()
		{
			Debug.Log("[AUDIO FEATURE] 🚀 Istanziazione Audio Speaker...");

			try
			{
				// Carica da prefab
				var instance = await _assetService.InstantiateAsset<AudioSpeakerBehaviour>(
					AudioData.AUDIO_SPEAKER_PREFAB_PATH);

				if (instance != null)
				{
					instance.Initialize(this);
					_audioSpeaker = instance;
					Debug.Log("[AUDIO FEATURE] ✅ Audio Speaker caricato da prefab");
				}
				else
				{
					// Fallback semplificato
					CreateDynamicAudioSpeaker();
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[AUDIO FEATURE] ❌ Errore: {ex.Message}");
				CreateDynamicAudioSpeaker();
			}
		}

		public void PlayAudioClip(string clipPath, float volume = 1f, int priority = 1)
		{
			if (!_audioSystemEnabled || _audioSpeaker == null) return;

			float finalVolume = volume * _masterVolume;
			_audioSpeaker.PlayClip(clipPath, finalVolume, priority);
		}

		public void StopCurrentAudio()
		{
			_audioSpeaker?.StopCurrentClip();
		}

		public void SetMasterVolume(float volume)
		{
			_masterVolume = Mathf.Clamp01(volume);
		}

		#endregion

		#region IAudioFeatureInternal Implementation

		public Client GetClient() => _client;

		#endregion

		#region Event Handlers - SEMPLIFICATI

		private void OnPlaySeatBeltAudio(PlaySeatBeltAudioEvent e)
		{
			PlayAudioClip(e.AudioClipPath, e.Volume, e.Priority);
		}

		private void OnPlayDoorLockAudio(DoorLockAudioRequestEvent e)
		{
			PlayAudioClip(e.AudioPath, e.Volume, e.Priority);
		}

		private void OnPlayLaneAssistAudio(LaneAssistAudioRequestEvent e)
		{
			PlayAudioClip(e.AudioPath, e.Volume, e.Priority);
		}

		#endregion

		#region Fallback Creation - SEMPLIFICATO

		private void CreateDynamicAudioSpeaker()
		{
			Debug.Log("[AUDIO FEATURE] 🔧 Creazione dinamica AudioSpeaker...");

			var audioObject = new GameObject("AudioSpeaker");
			audioObject.AddComponent<AudioSource>();

			_audioSpeaker = audioObject.AddComponent<AudioSpeakerBehaviour>();
			_audioSpeaker.Initialize(this);

			Debug.Log("[AUDIO FEATURE] ✅ AudioSpeaker dinamico creato");
		}

		#endregion

		#region Cleanup - SEMPLIFICATO

		~AudioFeature()
		{
			if (_broadcaster != null)
			{
				_broadcaster.Remove<PlaySeatBeltAudioEvent>(OnPlaySeatBeltAudio);
				_broadcaster.Remove<DoorLockAudioRequestEvent>(OnPlayDoorLockAudio);
				_broadcaster.Remove<LaneAssistAudioRequestEvent>(OnPlayLaneAssistAudio);
			}
		}

		#endregion
	}
}