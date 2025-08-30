using System;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Eventi per Clock Feature
	/// </summary>
	public class ClockTimeUpdateEvent
	{
		public string TimeString { get; }
		public DateTime CurrentTime { get; }

		public ClockTimeUpdateEvent(string timeString, DateTime currentTime)
		{
			TimeString = timeString;
			CurrentTime = currentTime;
		}
	}
}