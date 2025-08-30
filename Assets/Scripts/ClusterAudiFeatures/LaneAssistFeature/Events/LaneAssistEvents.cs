namespace ClusterAudiFeatures
{
	/// <summary>
	/// Eventi LaneAssist - VERSIONE SEMPLIFICATA
	/// Rimossi eventi complessi non utilizzati
	/// Mantenuti solo eventi essenziali
	/// </summary>

	#region Core Events

	/// <summary>
	/// Lane departure rilevato
	/// </summary>
	public class LaneDepartureDetectedEvent
	{
		public LaneAssistData.LaneDepartureType DepartureType { get; }
		public float DepartureTime { get; }

		public LaneDepartureDetectedEvent(LaneAssistData.LaneDepartureType departureType, float departureTime)
		{
			DepartureType = departureType;
			DepartureTime = departureTime;
		}
	}

	/// <summary>
	/// Lane departure reset
	/// </summary>
	public class LaneDepartureResetEvent
	{
		public LaneDepartureResetEvent() { }
	}

	/// <summary>
	/// Audio request
	/// </summary>
	public class LaneAssistAudioRequestEvent
	{
		public string AudioPath { get; }
		public float Volume { get; }
		public int Priority { get; }
		public LaneAssistData.LaneDepartureType DepartureType { get; }

		public LaneAssistAudioRequestEvent(string audioPath, LaneAssistData.LaneDepartureType departureType)
		{
			AudioPath = audioPath;
			Volume = LaneAssistData.LANE_ASSIST_AUDIO_VOLUME;
			Priority = LaneAssistData.LANE_ASSIST_AUDIO_PRIORITY;
			DepartureType = departureType;
		}
	}

	#endregion
}