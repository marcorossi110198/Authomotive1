using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Eventi specifici per Audio Feature
	/// Pattern IDENTICO a Mercedes per comunicazione event-driven
	/// 
	/// RESPONSABILITÀ:
	/// - Eventi generali del sistema audio (non specifici SeatBelt)
	/// - Stati riproduzione audio
	/// - Controlli volume e configurazione
	/// - Debug e monitoring audio system
	/// </summary>

	#region General Audio Events

	/// <summary>
	/// Evento richiesta riproduzione audio generica
	/// Per audio non-SeatBelt (musica, effetti generali, etc.)
	/// </summary>
	public class PlayGeneralAudioEvent
	{
		public string AudioClipPath { get; }
		public float Volume { get; }
		public int Priority { get; }
		public bool Loop { get; }
		public string AudioCategory { get; }
		public System.DateTime Timestamp { get; }

		public PlayGeneralAudioEvent(
			string audioClipPath,
			float volume = 1f,
			int priority = 1,
			bool loop = false,
			string audioCategory = "General")
		{
			AudioClipPath = audioClipPath;
			Volume = volume;
			Priority = priority;
			Loop = loop;
			AudioCategory = audioCategory;
			Timestamp = System.DateTime.Now;
		}
	}

	/// <summary>
	/// Evento stop audio generale
	/// </summary>
	public class StopGeneralAudioEvent
	{
		public string AudioCategory { get; }
		public System.DateTime Timestamp { get; }
		public string Reason { get; }

		public StopGeneralAudioEvent(string audioCategory = "All", string reason = "Manual stop")
		{
			AudioCategory = audioCategory;
			Timestamp = System.DateTime.Now;
			Reason = reason;
		}
	}

	#endregion

	#region Audio System Status Events

	/// <summary>
	/// Evento cambio stato riproduzione audio
	/// </summary>
	public class AudioPlaybackStateChangedEvent
	{
		public AudioPlaybackState PreviousState { get; }
		public AudioPlaybackState NewState { get; }
		public string CurrentClipPath { get; }
		public float CurrentVolume { get; }
		public int CurrentPriority { get; }

		public AudioPlaybackStateChangedEvent(
			AudioPlaybackState previousState,
			AudioPlaybackState newState,
			string currentClipPath = "",
			float currentVolume = 0f,
			int currentPriority = 0)
		{
			PreviousState = previousState;
			NewState = newState;
			CurrentClipPath = currentClipPath;
			CurrentVolume = currentVolume;
			CurrentPriority = currentPriority;
		}
	}

	/// <summary>
	/// Stati di riproduzione audio
	/// </summary>
	public enum AudioPlaybackState
	{
		Stopped,    // Nessun audio in riproduzione
		Playing,    // Audio in riproduzione
		Paused,     // Audio in pausa
		Loading,    // Caricamento audio
		Error       // Errore riproduzione
	}

	/// <summary>
	/// Evento completamento riproduzione audio
	/// </summary>
	public class AudioPlaybackCompletedEvent
	{
		public string CompletedClipPath { get; }
		public float TotalDuration { get; }
		public string AudioCategory { get; }
		public System.DateTime CompletionTime { get; }

		public AudioPlaybackCompletedEvent(
			string completedClipPath,
			float totalDuration,
			string audioCategory = "General")
		{
			CompletedClipPath = completedClipPath;
			TotalDuration = totalDuration;
			AudioCategory = audioCategory;
			CompletionTime = System.DateTime.Now;
		}
	}

	#endregion

	#region Volume & Configuration Events

	/// <summary>
	/// Evento cambio volume master
	/// </summary>
	public class AudioMasterVolumeChangedEvent
	{
		public float PreviousVolume { get; }
		public float NewVolume { get; }
		public string ChangeReason { get; }

		public AudioMasterVolumeChangedEvent(float previousVolume, float newVolume, string reason = "User")
		{
			PreviousVolume = previousVolume;
			NewVolume = newVolume;
			ChangeReason = reason;
		}
	}

	/// <summary>
	/// Evento cambio configurazione audio per modalità guida
	/// </summary>
	public class AudioDriveModeConfigEvent
	{
		public ClusterAudi.DriveMode DriveMode { get; }
		public float VolumeMultiplier { get; }
		public AudioProfile AudioProfile { get; }

		public AudioDriveModeConfigEvent(
			ClusterAudi.DriveMode driveMode,
			float volumeMultiplier,
			AudioProfile audioProfile)
		{
			DriveMode = driveMode;
			VolumeMultiplier = volumeMultiplier;
			AudioProfile = audioProfile;
		}
	}

	/// <summary>
	/// Profili audio per modalità guida
	/// </summary>
	public enum AudioProfile
	{
		Quiet,      // Eco mode - audio ridotto
		Balanced,   // Comfort mode - audio bilanciato  
		Enhanced    // Sport mode - audio potenziato
	}

	#endregion

	#region Audio System Management Events

	/// <summary>
	/// Evento abilitazione/disabilitazione sistema audio
	/// </summary>
	public class AudioSystemEnabledEvent
	{
		public bool IsEnabled { get; }
		public string Reason { get; }
		public System.DateTime Timestamp { get; }

		public AudioSystemEnabledEvent(bool isEnabled, string reason = "")
		{
			IsEnabled = isEnabled;
			Reason = reason;
			Timestamp = System.DateTime.Now;
		}
	}

	/// <summary>
	/// Evento errore sistema audio
	/// </summary>
	public class AudioSystemErrorEvent
	{
		public AudioErrorType ErrorType { get; }
		public string ErrorMessage { get; }
		public string FailedClipPath { get; }
		public System.Exception Exception { get; }

		public AudioSystemErrorEvent(
			AudioErrorType errorType,
			string errorMessage,
			string failedClipPath = "",
			System.Exception exception = null)
		{
			ErrorType = errorType;
			ErrorMessage = errorMessage;
			FailedClipPath = failedClipPath;
			Exception = exception;
		}
	}

	/// <summary>
	/// Tipi di errori audio
	/// </summary>
	public enum AudioErrorType
	{
		ClipNotFound,       // File audio non trovato
		LoadingFailed,      // Caricamento fallito
		PlaybackFailed,     // Riproduzione fallita
		ComponentMissing,   // AudioSource mancante
		SystemDisabled      // Sistema audio disabilitato
	}

	#endregion

	#region Audio Performance & Debug Events

	/// <summary>
	/// Evento performance monitoring audio
	/// </summary>
	public class AudioPerformanceEvent
	{
		public float AudioLatency { get; }
		public int ActiveClipsCount { get; }
		public float MemoryUsage { get; }
		public AudioPerformanceLevel PerformanceLevel { get; }

		public AudioPerformanceEvent(
			float audioLatency,
			int activeClipsCount,
			float memoryUsage,
			AudioPerformanceLevel performanceLevel)
		{
			AudioLatency = audioLatency;
			ActiveClipsCount = activeClipsCount;
			MemoryUsage = memoryUsage;
			PerformanceLevel = performanceLevel;
		}
	}

	/// <summary>
	/// Livelli performance audio
	/// </summary>
	public enum AudioPerformanceLevel
	{
		Optimal,    // Performance ottimale
		Good,       // Performance buona
		Warning,    // Performance in calo
		Critical    // Performance critiche
	}

	/// <summary>
	/// Evento debug audio system
	/// </summary>
	public class AudioDebugEvent
	{
		public string DebugMessage { get; }
		public AudioDebugType DebugType { get; }
		public object DebugData { get; }
		public System.DateTime Timestamp { get; }

		public AudioDebugEvent(string message, AudioDebugType type, object data = null)
		{
			DebugMessage = message;
			DebugType = type;
			DebugData = data;
			Timestamp = System.DateTime.Now;
		}
	}

	/// <summary>
	/// Tipi debug audio
	/// </summary>
	public enum AudioDebugType
	{
		SystemInitialized,
		ClipLoaded,
		PlaybackStarted,
		PlaybackStopped,
		VolumeChanged,
		ConfigurationChanged,
		ErrorOccurred,
		PerformanceWarning
	}

	#endregion

	#region Development Notes

	/*
     * AUDIO EVENTS - SISTEMA COMPLETO
     * 
     * Pattern seguito IDENTICO a Mercedes per event-driven architecture:
     * 
     * GENERAL AUDIO EVENTS:
     * - PlayGeneralAudioEvent: Riproduzione audio generale (non SeatBelt)
     * - StopGeneralAudioEvent: Stop audio generale
     * 
     * AUDIO SYSTEM STATUS EVENTS:
     * - AudioPlaybackStateChangedEvent: Cambio stati riproduzione
     * - AudioPlaybackCompletedEvent: Fine riproduzione
     * 
     * VOLUME & CONFIGURATION EVENTS:
     * - AudioMasterVolumeChangedEvent: Cambio volume master
     * - AudioDriveModeConfigEvent: Config audio per modalità guida
     * 
     * AUDIO SYSTEM MANAGEMENT EVENTS:
     * - AudioSystemEnabledEvent: Enable/disable sistema
     * - AudioSystemErrorEvent: Gestione errori
     * 
     * PERFORMANCE & DEBUG EVENTS:
     * - AudioPerformanceEvent: Monitoring performance
     * - AudioDebugEvent: Debug e testing
     * 
     * SEPARAZIONE RESPONSABILITÀ:
     * - AudioEvents.cs: Eventi generali sistema audio
     * - SeatBeltEvents.cs: Eventi specifici SeatBelt (PlaySeatBeltAudioEvent, etc.)
     * 
     * Questo permette:
     * 1. Sistema audio modulare e estensibile
     * 2. SeatBelt audio come caso specifico
     * 3. Possibilità di aggiungere altre categorie audio (Navigation, Phone, etc.)
     * 4. Monitoring e debugging completo
     * 
     * Perfettamente integrato nell'architettura Mercedes esistente.
     */

	#endregion
}