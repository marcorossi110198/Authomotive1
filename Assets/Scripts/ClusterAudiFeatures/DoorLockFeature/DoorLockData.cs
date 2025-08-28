using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Configurazioni per DoorLock Feature - VERSIONE SEMPLICE
	/// IDENTICO al pattern ClockData.cs che hai appena implementato
	/// </summary>
	public static class DoorLockData
	{
		#region Asset Paths - IDENTICO Mercedes Pattern

		/// <summary>
		/// Path del prefab DoorLock - IDENTICO al pattern Mercedes
		/// </summary>
		public const string DOOR_LOCK_PREFAB_PATH = "DoorLock/DoorLockPrefab";

		#endregion

		#region Audio Paths - Per AudioFeature esistente

		/// <summary>
		/// Path suono lucchetto che si chiude
		/// </summary>
		public const string LOCK_SOUND_PATH = "Audio/SFX/DoorLock/LockSound";

		/// <summary>
		/// Path suono lucchetto che si apre  
		/// </summary>
		public const string UNLOCK_SOUND_PATH = "Audio/SFX/DoorLock/UnlockSound";

		#endregion

		#region Image Paths - Per le tue immagini

		/// <summary>
		/// Path immagine lucchetto chiuso
		/// </summary>
		public const string LOCKED_IMAGE_PATH = "DoorLock/img/Lock";

		/// <summary>
		/// Path immagine lucchetto aperto
		/// </summary>
		public const string UNLOCKED_IMAGE_PATH = "DoorLock/img/Unlock";

		#endregion

		#region Audio Configuration - Per integrazione AudioFeature

		/// <summary>
		/// Volume suoni door lock
		/// </summary>
		public const float DOOR_LOCK_VOLUME = 0.8f;

		/// <summary>
		/// Priorità audio door lock (alta per safety)
		/// </summary>
		public const int DOOR_LOCK_AUDIO_PRIORITY = 4;

		#endregion

		#region UI Constants - Semplice

		/// <summary>
		/// Scala immagine lucchetto
		/// </summary>
		public const float LOCK_IMAGE_SCALE = 1.0f;

		#endregion

		#region Keyboard Controls

		/// <summary>
		/// Tasto per toggle door lock
		/// </summary>
		public static readonly KeyCode DOOR_LOCK_TOGGLE_KEY = KeyCode.L;

		#endregion

		#region Initial State

		/// <summary>
		/// Stato iniziale (locked = true)
		/// </summary>
		public const bool DEFAULT_LOCKED_STATE = true;

		#endregion
	}
}