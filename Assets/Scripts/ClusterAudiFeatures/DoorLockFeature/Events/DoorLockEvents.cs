namespace ClusterAudiFeatures
{
	/// <summary>
	/// Eventi per DoorLock Feature - VERSIONE SEMPLICE
	/// Pattern IDENTICO agli eventi che hai già implementato
	/// </summary>

	/// <summary>
	/// Evento cambio stato door lock
	/// </summary>
	public class DoorLockStateChangedEvent
	{
		public bool IsLocked { get; }
		public bool PreviousState { get; }

		public DoorLockStateChangedEvent(bool isLocked, bool previousState)
		{
			IsLocked = isLocked;
			PreviousState = previousState;
		}
	}

	/// <summary>
	/// Evento richiesta audio door lock (per AudioFeature)
	/// </summary>
	public class DoorLockAudioRequestEvent
	{
		public string AudioPath { get; }
		public float Volume { get; }
		public int Priority { get; }
		public bool IsLockSound { get; }

		public DoorLockAudioRequestEvent(string audioPath, float volume, int priority, bool isLockSound)
		{
			AudioPath = audioPath;
			Volume = volume;
			Priority = priority;
			IsLockSound = isLockSound;
		}
	}
}