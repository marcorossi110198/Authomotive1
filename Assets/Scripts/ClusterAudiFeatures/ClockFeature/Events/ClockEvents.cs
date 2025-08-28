using System;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Eventi per Clock Feature - VERSIONE SEMPLICE
	/// Pattern IDENTICO agli eventi che hai già implementato
	/// </summary>

	/// <summary>
	/// Evento aggiornamento orario
	/// </summary>
	public class ClockTimeUpdateEvent
	{
		public string CurrentTime { get; }
		public DateTime DateTime { get; }

		public ClockTimeUpdateEvent(string currentTime, DateTime dateTime)
		{
			CurrentTime = currentTime;
			DateTime = dateTime;
		}
	}
}