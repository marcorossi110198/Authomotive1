using ClusterAudi;
using System.Collections;
using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// MonoBehaviour per Audio Speaker - SEMPLIFICATO
	/// </summary>
	public class AudioSpeakerBehaviour : BaseMonoBehaviour<IAudioFeatureInternal>
	{
		[Header("Audio Components")]
		[SerializeField] private AudioSource _audioSource;

		private bool _isPlaying = false;
		private int _currentPriority = 0;

		protected override void ManagedAwake()
		{
			// Auto-ottieni AudioSource se non assegnato
			if (_audioSource == null)
			{
				_audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
			}

			_audioSource.playOnAwake = false;
			_audioSource.loop = false;
		}

		public void PlayClip(string clipPath, float volume, int priority)
		{
			// Sistema priorità: audio con priorità più alta interrompe quello corrente
			if (_isPlaying && priority <= _currentPriority)
			{
				return; // Ignora audio con priorità più bassa
			}

			StopCurrentClip();
			StartCoroutine(PlayClipCoroutine(clipPath, volume, priority));
		}

		public void StopCurrentClip()
		{
			StopAllCoroutines();
			if (_audioSource.isPlaying)
			{
				_audioSource.Stop();
			}
			_isPlaying = false;
			_currentPriority = 0;
		}

		private IEnumerator PlayClipCoroutine(string clipPath, float volume, int priority)
		{
			AudioClip clip = Resources.Load<AudioClip>(clipPath);
			if (clip == null)
			{
				Debug.LogError($"[AUDIO SPEAKER] Audio clip non trovato: {clipPath}");
				yield break;
			}

			_audioSource.clip = clip;
			_audioSource.volume = volume;
			_currentPriority = priority;
			_isPlaying = true;

			_audioSource.Play();

			// Attendi fine riproduzione
			while (_audioSource.isPlaying)
			{
				yield return null;
			}

			// Cleanup
			_isPlaying = false;
			_currentPriority = 0;
		}
	}
}