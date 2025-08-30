using ClusterAudi;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Implementazione Audio Feature - SEMPLIFICATA
	/// </summary>
	public class AudioFeature : BaseFeature, IAudioFeature, IAudioFeatureInternal
	{
		private AudioSpeakerBehaviour _audioSpeakerBehaviour;

		public AudioFeature(Client client) : base(client)
		{
			// Sottoscrivi agli eventi audio
			var broadcaster = client.Services.Get<IBroadcaster>();
			broadcaster.Add<PlaySeatBeltAudioEvent>(OnPlaySeatBeltAudio);
			broadcaster.Add<DoorLockAudioRequestEvent>(OnPlayDoorLockAudio);
			broadcaster.Add<LaneAssistAudioRequestEvent>(OnPlayLaneAssistAudio);
		}

		public async Task InstantiateAudioFeature()
		{
			var audioSpeakerInstance = await _assetService.InstantiateAsset<AudioSpeakerBehaviour>(
				AudioData.AUDIO_SPEAKER_PREFAB_PATH);

			if (audioSpeakerInstance != null)
			{
				audioSpeakerInstance.Initialize(this);
				_audioSpeakerBehaviour = audioSpeakerInstance;
			}
			else
			{
				Debug.LogWarning("[AUDIO FEATURE] Prefab non trovato: " + AudioData.AUDIO_SPEAKER_PREFAB_PATH);
			}
		}

		public void PlayAudioClip(string clipPath, float volume = 1f, int priority = 1)
		{
			_audioSpeakerBehaviour?.PlayClip(clipPath, volume, priority);
		}

		public void StopCurrentAudio()
		{
			_audioSpeakerBehaviour?.StopCurrentClip();
		}

		public void SetMasterVolume(float volume)
		{
			// Implementazione se necessaria
		}

		public Client GetClient()
		{
			return _client;
		}

		// Event Handlers
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
	}
}