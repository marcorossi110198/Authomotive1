using ClusterAudi;
using System.Collections;
using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// MonoBehaviour per Audio Speaker
	/// IDENTICO al pattern SeatBeltBehaviour seguendo modello Mercedes
	/// </summary>
	public class AudioSpeakerBehaviour : BaseMonoBehaviour<IAudioFeatureInternal>
	{
		#region Serialized Fields

		[Header("Audio Components - ASSIGN IN PREFAB")]
		[SerializeField] private AudioSource _audioSource;

		#endregion

		#region Private Fields

		// Servizi
		private IBroadcaster _broadcaster;

		// Stato corrente
		private bool _isPlaying = false;
		private string _currentClipPath = "";
		private int _currentPriority = 0;

		// Performance
		private Coroutine _playbackCoroutine;

		#endregion

		#region BaseMonoBehaviour Override

		protected override void ManagedAwake()
		{
			Debug.Log("[AUDIO SPEAKER] 🔊 AudioSpeakerBehaviour inizializzato");

			// Ottieni servizi
			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();

			// Valida componenti
			ValidateAudioComponents();
		}

		protected override void ManagedStart()
		{
			Debug.Log("[AUDIO SPEAKER] ▶️ Audio Speaker avviato");
		}

		protected override void ManagedUpdate()
		{
			// Update stato riproduzione
			UpdatePlaybackState();
		}

		protected override void ManagedOnDestroy()
		{
			Debug.Log("[AUDIO SPEAKER] 🗑️ Audio Speaker distrutto");

			// Stop riproduzione
			StopCurrentClip();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Riproduce clip audio con priorità
		/// </summary>
		public void PlayClip(string clipPath, float volume, int priority)
		{
			// Controlla priorità - audio con priorità più alta interrompe quello corrente
			if (_isPlaying && priority <= _currentPriority)
			{
				Debug.Log($"[AUDIO SPEAKER] ⏭️ Audio ignorato (priorità {priority} <= {_currentPriority})");
				return;
			}

			// Stop clip corrente se necessario
			if (_isPlaying)
			{
				StopCurrentClip();
			}

			// Avvia riproduzione nuovo clip
			StartClipPlayback(clipPath, volume, priority);
		}

		/// <summary>
		/// Ferma riproduzione corrente
		/// </summary>
		public void StopCurrentClip()
		{
			if (_playbackCoroutine != null)
			{
				StopCoroutine(_playbackCoroutine);
				_playbackCoroutine = null;
			}

			if (_audioSource != null && _audioSource.isPlaying)
			{
				_audioSource.Stop();
			}

			_isPlaying = false;
			_currentClipPath = "";
			_currentPriority = 0;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Valida componenti audio
		/// </summary>
		private void ValidateAudioComponents()
		{
			// Auto-ottieni AudioSource se non assegnato
			if (_audioSource == null)
			{
				_audioSource = GetComponent<AudioSource>();
			}

			// Crea AudioSource se non esiste
			if (_audioSource == null)
			{
				_audioSource = gameObject.AddComponent<AudioSource>();
				Debug.Log("[AUDIO SPEAKER] ➕ AudioSource creato dinamicamente");
			}

			// Configura AudioSource
			_audioSource.playOnAwake = false;
			_audioSource.loop = false;

			Debug.Log("[AUDIO SPEAKER] ✅ Componenti audio validati");
		}

		/// <summary>
		/// Avvia riproduzione clip
		/// </summary>
		private void StartClipPlayback(string clipPath, float volume, int priority)
		{
			_playbackCoroutine = StartCoroutine(PlayClipCoroutine(clipPath, volume, priority));
		}

		/// <summary>
		/// Coroutine riproduzione clip
		/// </summary>
		private IEnumerator PlayClipCoroutine(string clipPath, float volume, int priority)
		{
			Debug.Log($"[AUDIO SPEAKER] 🎵 Caricamento audio: {clipPath}");

			// Carica clip da Resources
			AudioClip clip = Resources.Load<AudioClip>(clipPath);

			if (clip == null)
			{
				Debug.LogError($"[AUDIO SPEAKER] ❌ Audio clip non trovato: {clipPath}");
				yield break;
			}

			// Configura riproduzione
			_audioSource.clip = clip;
			_audioSource.volume = volume;
			_currentClipPath = clipPath;
			_currentPriority = priority;
			_isPlaying = true;

			// Avvia riproduzione
			_audioSource.Play();
			Debug.Log($"[AUDIO SPEAKER] ▶️ Riproduzione avviata: {clipPath} (priorità: {priority})");

			// Attendi fine riproduzione
			while (_audioSource.isPlaying)
			{
				yield return null;
			}

			// Cleanup
			_isPlaying = false;
			_currentClipPath = "";
			_currentPriority = 0;
			_playbackCoroutine = null;

			Debug.Log($"[AUDIO SPEAKER] ✅ Riproduzione completata: {clipPath}");
		}

		/// <summary>
		/// Update stato riproduzione
		/// </summary>
		private void UpdatePlaybackState()
		{
			// Rileva fine riproduzione per cleanup
			if (_isPlaying && _audioSource != null && !_audioSource.isPlaying && _playbackCoroutine == null)
			{
				_isPlaying = false;
				_currentClipPath = "";
				_currentPriority = 0;
			}
		}

		#endregion
	}
}