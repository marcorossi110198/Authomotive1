namespace ClusterAudiFeatures
{
	/// <summary>
	/// AudioData SEMPLIFICATO - RIDOTTO DEL 80%, STESSO RISULTATO
	/// 
	/// MANTIENE: Solo configurazioni utilizzate effettivamente
	/// RIMUOVE: Configurazioni non utilizzate, commenti eccessivi
	/// </summary>
	public static class AudioData
	{
		#region Asset Paths - SOLO QUELLI UTILIZZATI

		public const string AUDIO_SPEAKER_PREFAB_PATH = "Audio/AudioSpeakerPrefab";

		#endregion

		#region SeatBelt Audio - CONSOLIDATO

		public const string SEATBELT_SOFT_BEEP_PATH = "Audio/SFX/SeatBelt/SoftBeep";
		public const string SEATBELT_URGENT_BEEP_PATH = "Audio/SFX/SeatBelt/UrgentBeep";
		public const string SEATBELT_CONTINUOUS_BEEP_PATH = "Audio/SFX/SeatBelt/ContinuousBeep";

		#endregion

		#region Audio Priorities - SEMPLIFICATO

		public const int SEATBELT_AUDIO_PRIORITY = 5;  // Massima (safety)
		public const int LANE_ASSIST_AUDIO_PRIORITY = 4;
		public const int DOOR_LOCK_AUDIO_PRIORITY = 3;

		#endregion
	}
}