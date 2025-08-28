namespace ClusterAudiFeatures
{
	/// <summary>
	/// Configurazioni e costanti per Audio Feature
	/// IDENTICO al pattern SeatBeltData.cs che hai già implementato
	/// </summary>
	public static class AudioData
	{
		#region Asset Paths - IDENTICO Mercedes Pattern

		/// <summary>
		/// Path del prefab AudioSpeaker - IDENTICO al pattern Mercedes
		/// </summary>
		public const string AUDIO_SPEAKER_PREFAB_PATH = "Audio/AudioSpeakerPrefab";

		#endregion

		#region SeatBelt Audio Paths - PER SEATBELT FEATURE

		/// <summary>
		/// Path audio SeatBelt - Segue convenzioni Mercedes
		/// </summary>
		public const string SEATBELT_SOFT_BEEP_PATH = "Audio/SFX/SeatBelt/SoftBeep";
		public const string SEATBELT_URGENT_BEEP_PATH = "Audio/SFX/SeatBelt/UrgentBeep";
		public const string SEATBELT_CONTINUOUS_BEEP_PATH = "Audio/SFX/SeatBelt/ContinuousBeep";

		#endregion

		#region Volume Configuration

		/// <summary>
		/// Volumi standard per SeatBelt
		/// </summary>
		public const float SEATBELT_SOFT_VOLUME = 0.6f;
		public const float SEATBELT_URGENT_VOLUME = 0.8f;
		public const float SEATBELT_CONTINUOUS_VOLUME = 1.0f;

		#endregion

		#region Audio Priorities

		/// <summary>
		/// Priorità audio (numeri più alti = priorità più alta)
		/// </summary>
		public const int SEATBELT_AUDIO_PRIORITY = 5;  // Safety = alta priorità
		public const int GENERAL_SFX_PRIORITY = 2;     // SFX generali
		public const int MUSIC_PRIORITY = 1;           // Musica = bassa priorità

		#endregion
	}
}