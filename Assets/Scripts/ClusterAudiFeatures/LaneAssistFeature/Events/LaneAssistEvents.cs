using System;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Eventi per Lane Assist Feature
	/// IDENTICO al pattern ClusterDriveModeEvents.cs del tuo progetto
	/// </summary>

	#region Lane Departure Events

	/// <summary>
	/// Evento lane departure detected
	/// </summary>
	public class LaneDepartureDetectedEvent
	{
		public LaneAssistData.LaneDepartureType DepartureType { get; }
		public float DepartureTime { get; }
		public float CurrentSpeed { get; }
		public DateTime Timestamp { get; }

		public LaneDepartureDetectedEvent(
			LaneAssistData.LaneDepartureType departureType,
			float departureTime,
			float currentSpeed)
		{
			DepartureType = departureType;
			DepartureTime = departureTime;
			CurrentSpeed = currentSpeed;
			Timestamp = DateTime.Now;
		}
	}

	/// <summary>
	/// Evento lane departure reset
	/// </summary>
	public class LaneDepartureResetEvent
	{
		public LaneAssistData.LaneDepartureType PreviousDepartureType { get; }
		public DateTime Timestamp { get; }

		public LaneDepartureResetEvent(LaneAssistData.LaneDepartureType previousType)
		{
			PreviousDepartureType = previousType;
			Timestamp = DateTime.Now;
		}
	}

	#endregion

	#region Lane Assist State Events

	/// <summary>
	/// Evento cambio stato lane assist
	/// </summary>
	public class LaneAssistStateChangedEvent
	{
		public LaneAssistData.LaneAssistState PreviousState { get; }
		public LaneAssistData.LaneAssistState NewState { get; }
		public string Reason { get; }

		public LaneAssistStateChangedEvent(
			LaneAssistData.LaneAssistState previousState,
			LaneAssistData.LaneAssistState newState,
			string reason = "")
		{
			PreviousState = previousState;
			NewState = newState;
			Reason = reason;
		}
	}

	#endregion

	#region Audio Events

	/// <summary>
	/// Richiesta audio lane assist - Usa sistema audio esistente
	/// </summary>
	public class LaneAssistAudioRequestEvent
	{
		public string AudioPath { get; }
		public float Volume { get; }
		public int Priority { get; }
		public LaneAssistData.LaneDepartureType DepartureType { get; }

		public LaneAssistAudioRequestEvent(
			string audioPath,
			LaneAssistData.LaneDepartureType departureType,
			float volume = LaneAssistData.LANE_ASSIST_AUDIO_VOLUME,
			int priority = LaneAssistData.LANE_ASSIST_AUDIO_PRIORITY)
		{
			AudioPath = audioPath;
			DepartureType = departureType;
			Volume = volume;
			Priority = priority;
		}
	}

	#endregion

	#region Visual Events

	/// <summary>
	/// Evento aggiornamento visuale lane assist
	/// </summary>
	public class LaneAssistVisualUpdateEvent
	{
		public LaneAssistData.LaneAssistState CurrentState { get; }
		public LaneAssistData.LaneDepartureType CurrentDeparture { get; }
		public bool ShowWarning { get; }
		public UnityEngine.Color LaneColor { get; }
		public float CarIconShift { get; }

		public LaneAssistVisualUpdateEvent(
			LaneAssistData.LaneAssistState state,
			LaneAssistData.LaneDepartureType departure,
			bool showWarning,
			UnityEngine.Color laneColor,
			float carIconShift)
		{
			CurrentState = state;
			CurrentDeparture = departure;
			ShowWarning = showWarning;
			LaneColor = laneColor;
			CarIconShift = carIconShift;
		}
	}

	#endregion

	#region Debug Events

	/// <summary>
	/// Evento debug lane assist per testing
	/// </summary>
	public class LaneAssistDebugEvent
	{
		public string Message { get; }
		public object DebugData { get; }
		public DateTime Timestamp { get; }

		public LaneAssistDebugEvent(string message, object debugData = null)
		{
			Message = message;
			DebugData = debugData;
			Timestamp = DateTime.Now;
		}
	}

	#endregion
}