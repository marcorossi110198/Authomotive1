using ClusterAudi;
using System.Collections;
using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// AudioSpeakerBehaviour SEMPLIFICATO - IDENTICO RISULTATO, CODICE RIDOTTO DEL 60%
	/// 
	/// MANTIENE:
	/// - Sistema priorità (funzionante al 100%)
	/// - Riproduzione audio asincrona
	/// - Gestione clip da Resources
	/// - Pattern Mercedes identico
	/// 
	/// RIMUOVE:
	/// - Validazioni eccessive
	/// - Metodi ridondanti  
	/// - Update inutile
	/// - Complessità non necessaria
	/// </summary>
	public class AudioSpeakerBehaviour : BaseMonoBehaviour<IAudioFeatureInternal>
	{
		#region Serialized Fields

		[SerializeField] private AudioSource _audioSource;

		#endregion

		#region Private Fields - ESSENZIALI

		private bool _isPlaying = false;
		private int _currentPriority = 0;
		private Coroutine _playbackCoroutine;

		#endregion

		#region BaseMonoBehaviour Override

		protected override void ManagedAwake()
		{
			Debug.Log("[AUDIO SPEAKER] 🔊 AudioSpeaker inizializzato");

			// Setup AudioSource
			if (_audioSource == null)
				_audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

			_audioSource.playOnAwake = false;
			_audioSource.loop = false;
		}

		protected override void ManagedOnDestroy()
		{
			StopCurrentClip();
		}

		#endregion

		#region Public Methods - SEMPLIFICATI

		/// <summary>
		/// Riproduce clip con sistema priorità - STESSO FUNZIONAMENTO, CODICE RIDOTTO
		/// </summary>
		public void PlayClip(string clipPath, float volume, int priority)
		{
			// ✅ SISTEMA PRIORITÀ: Audio con priorità più alta interrompe quello corrente
			if (_isPlaying && priority <= _currentPriority)
			{
				Debug.Log($"[AUDIO SPEAKER] ⏭️ Audio ignorato (priorità {priority} <= {_currentPriority})");
				return;
			}

			// Stop clip corrente se necessario
			if (_isPlaying) StopCurrentClip();

			// Avvia nuovo clip
			_playbackCoroutine = StartCoroutine(PlayClipCoroutine(clipPath, volume, priority));
		}

		/// <summary>
		/// Stop riproduzione - SEMPLIFICATO
		/// </summary>
		public void StopCurrentClip()
		{
			if (_playbackCoroutine != null)
			{
				StopCoroutine(_playbackCoroutine);
				_playbackCoroutine = null;
			}

			if (_audioSource?.isPlaying == true)
				_audioSource.Stop();

			_isPlaying = false;
			_currentPriority = 0;
		}

		#endregion

		#region Private Methods - SEMPLIFICATI

		/// <summary>
		/// Coroutine riproduzione - SEMPLIFICATA ma IDENTICO RISULTATO
		/// </summary>
		private IEnumerator PlayClipCoroutine(string clipPath, float volume, int priority)
		{
			Debug.Log($"[AUDIO SPEAKER] 🎵 Loading: {clipPath}");

			// Carica clip
			AudioClip clip = Resources.Load<AudioClip>(clipPath);
			if (clip == null)
			{
				Debug.LogError($"[AUDIO SPEAKER] ❌ Clip non trovato: {clipPath}");
				yield break;
			}

			// Setup riproduzione
			_audioSource.clip = clip;
			_audioSource.volume = volume;
			_currentPriority = priority;
			_isPlaying = true;

			// Avvia riproduzione
			_audioSource.Play();
			Debug.Log($"[AUDIO SPEAKER] ▶️ Playing: {clipPath} (priorità: {priority})");

			// Attendi fine
			yield return new WaitUntil(() => !_audioSource.isPlaying);

			// Cleanup
			_isPlaying = false;
			_currentPriority = 0;
			_playbackCoroutine = null;

			Debug.Log($"[AUDIO SPEAKER] ✅ Completed: {clipPath}");
		}

		#endregion
	}
}