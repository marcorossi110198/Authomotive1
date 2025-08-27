using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Eventi per SeatBelt Feature
	/// IDENTICO al pattern eventi Mercedes per comunicazione event-driven
	/// </summary>

	#region Core SeatBelt Events

	/// <summary>
	/// Evento cambio stato di una singola cintura
	/// </summary>
	public class SeatBeltStatusChangedEvent
	{
		public SeatBeltData.SeatBeltPosition Position { get; }
		public SeatBeltData.SeatBeltStatus OldStatus { get; }
		public SeatBeltData.SeatBeltStatus NewStatus { get; }
		public System.DateTime Timestamp { get; }

		public SeatBeltStatusChangedEvent(
			SeatBeltData.SeatBeltPosition position,
			SeatBeltData.SeatBeltStatus oldStatus,
			SeatBeltData.SeatBeltStatus newStatus)
		{
			Position = position;
			OldStatus = oldStatus;
			NewStatus = newStatus;
			Timestamp = System.DateTime.Now;
		}
	}

	/// <summary>
	/// Evento inizio warning system
	/// </summary>
	public class SeatBeltWarningStartedEvent
	{
		public float CurrentSpeed { get; }
		public float SpeedThreshold { get; }
		public SeatBeltData.SeatBeltPosition[] UnfastenedBelts { get; }
		public ClusterAudi.DriveMode CurrentDriveMode { get; }

		public SeatBeltWarningStartedEvent(
			float currentSpeed,
			float speedThreshold,
			SeatBeltData.SeatBeltPosition[] unfastenedBelts,
			ClusterAudi.DriveMode driveMode)
		{
			CurrentSpeed = currentSpeed;
			SpeedThreshold = speedThreshold;
			UnfastenedBelts = unfastenedBelts;
			CurrentDriveMode = driveMode;
		}
	}

	/// <summary>
	/// Evento fine warning system
	/// </summary>
	public class SeatBeltWarningStoppedEvent
	{
		public float TotalWarningDuration { get; }
		public SeatBeltWarningStopReason StopReason { get; }
		public System.DateTime Timestamp { get; }

		public SeatBeltWarningStoppedEvent(float duration, SeatBeltWarningStopReason reason)
		{
			TotalWarningDuration = duration;
			StopReason = reason;
			Timestamp = System.DateTime.Now;
		}
	}

	/// <summary>
	/// Motivi di stop del warning
	/// </summary>
	public enum SeatBeltWarningStopReason
	{
		AllBeltsFastened,    // Tutte le cinture allacciate
		SpeedReduced,        // Velocità sotto soglia
		SystemDisabled,      // Sistema disabilitato
		ManualDismiss        // Dismisso manualmente
	}

	#endregion

	#region Audio Warning Events

	/// <summary>
	/// Evento escalation audio warning
	/// </summary>
	public class SeatBeltAudioEscalationEvent
	{
		public AudioEscalationLevel PreviousLevel { get; }
		public AudioEscalationLevel NewLevel { get; }
		public float WarningDuration { get; }
		public string AudioClipPath { get; }

		public SeatBeltAudioEscalationEvent(
			AudioEscalationLevel previousLevel,
			AudioEscalationLevel newLevel,
			float warningDuration,
			string audioClipPath)
		{
			PreviousLevel = previousLevel;
			NewLevel = newLevel;
			WarningDuration = warningDuration;
			AudioClipPath = audioClipPath;
		}
	}

	/// <summary>
	/// Evento richiesta riproduzione audio
	/// </summary>
	public class PlaySeatBeltAudioEvent
	{
		public string AudioClipPath { get; }
		public float Volume { get; }
		public int Priority { get; }
		public AudioEscalationLevel EscalationLevel { get; }

		public PlaySeatBeltAudioEvent(
			string audioClipPath,
			float volume = 1f,
			int priority = 2,
			AudioEscalationLevel escalationLevel = AudioEscalationLevel.Soft)
		{
			AudioClipPath = audioClipPath;
			Volume = volume;
			Priority = priority;
			EscalationLevel = escalationLevel;
		}
	}

	#endregion

	#region Configuration Events

	/// <summary>
	/// Evento aggiornamento configurazione SeatBelt
	/// </summary>
	public class SeatBeltConfigurationUpdatedEvent
	{
		public ClusterAudi.DriveMode DriveMode { get; }
		public SeatBeltConfig NewConfiguration { get; }
		public float OldSpeedThreshold { get; }
		public float NewSpeedThreshold { get; }

		public SeatBeltConfigurationUpdatedEvent(
			ClusterAudi.DriveMode driveMode,
			SeatBeltConfig newConfig,
			float oldThreshold,
			float newThreshold)
		{
			DriveMode = driveMode;
			NewConfiguration = newConfig;
			OldSpeedThreshold = oldThreshold;
			NewSpeedThreshold = newThreshold;
		}
	}

	/// <summary>
	/// Evento abilitazione/disabilitazione sistema
	/// </summary>
	public class SeatBeltSystemEnabledEvent
	{
		public bool IsEnabled { get; }
		public string Reason { get; }

		public SeatBeltSystemEnabledEvent(bool isEnabled, string reason = "")
		{
			IsEnabled = isEnabled;
			Reason = reason;
		}
	}

	#endregion

	#region Visual Warning Events

	/// <summary>
	/// Evento aggiornamento visual warning
	/// </summary>
	public class SeatBeltVisualWarningEvent
	{
		public bool ShowWarning { get; }
		public string WarningMessage { get; }
		public Color WarningColor { get; }
		public SeatBeltData.SeatBeltPosition[] AffectedPositions { get; }

		public SeatBeltVisualWarningEvent(
			bool showWarning,
			string message = "",
			Color warningColor = default,
			SeatBeltData.SeatBeltPosition[] affectedPositions = null)
		{
			ShowWarning = showWarning;
			WarningMessage = message;
			WarningColor = warningColor == default ? SeatBeltData.WARNING_TEXT_COLOR : warningColor;
			AffectedPositions = affectedPositions ?? new SeatBeltData.SeatBeltPosition[0];
		}
	}

	/// <summary>
	/// Evento richiesta flash icone
	/// </summary>
	public class SeatBeltFlashIconsEvent
	{
		public SeatBeltData.SeatBeltPosition[] PositionsToFlash { get; }
		public bool StartFlashing { get; }
		public float FlashInterval { get; }

		public SeatBeltFlashIconsEvent(
			SeatBeltData.SeatBeltPosition[] positions,
			bool startFlashing,
			float flashInterval = 0.6f)
		{
			PositionsToFlash = positions ?? new SeatBeltData.SeatBeltPosition[0];
			StartFlashing = startFlashing;
			FlashInterval = flashInterval;
		}
	}

	#endregion

	#region Debug & Testing Events

	/// <summary>
	/// Evento debug per testing sistema cinture
	/// </summary>
	public class SeatBeltDebugEvent
	{
		public string DebugMessage { get; }
		public SeatBeltDebugType DebugType { get; }
		public object DebugData { get; }

		public SeatBeltDebugEvent(string message, SeatBeltDebugType type, object data = null)
		{
			DebugMessage = message;
			DebugType = type;
			DebugData = data;
		}
	}

	/// <summary>
	/// Tipi di debug per sistema cinture
	/// </summary>
	public enum SeatBeltDebugType
	{
		StatusChanged,
		WarningTriggered,
		AudioEscalation,
		ConfigurationUpdate,
		SystemToggle,
		PerformanceWarning
	}

	#endregion

	#region Development Notes

	/*
     * SEATBELT EVENTS - SISTEMA COMPLETO
     * 
     * Pattern seguito IDENTICO a Mercedes per event-driven architecture:
     * 
     * CORE EVENTS:
     * - SeatBeltStatusChangedEvent: Cambio stato singola cintura
     * - SeatBeltWarningStartedEvent: Inizio warning system
     * - SeatBeltWarningStoppedEvent: Fine warning system
     * 
     * AUDIO EVENTS:
     * - SeatBeltAudioEscalationEvent: Escalation audio levels
     * - PlaySeatBeltAudioEvent: Richiesta riproduzione audio
     * 
     * VISUAL EVENTS:
     * - SeatBeltVisualWarningEvent: Aggiornamenti visual warning
     * - SeatBeltFlashIconsEvent: Controllo flash icone
     * 
     * CONFIGURATION EVENTS:
     * - SeatBeltConfigurationUpdatedEvent: Cambio config per modalità
     * - SeatBeltSystemEnabledEvent: Enable/disable sistema
     * 
     * DEBUG EVENTS:
     * - SeatBeltDebugEvent: Testing e performance monitoring
     * 
     * Questo sistema garantisce comunicazione loose-coupled tra:
     * - SeatBeltFeature (logic)
     * - SeatBeltBehaviour (UI)  
     * - Audio system
     * - Debug system
     * 
     * Perfettamente integrato nell'architettura Mercedes esistente.
     */

	#endregion
}