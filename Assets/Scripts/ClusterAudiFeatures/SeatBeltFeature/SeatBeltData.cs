using UnityEngine;
using ClusterAudi;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// SeatBelt Data - VERSIONE CORRETTA
	/// Rimosso: Warning Panel components (ma mantenuta logica flash)
	/// Mantenuto: Tutto il resto identico
	/// </summary>
	public static class SeatBeltData
	{
		#region Constants

		public const string SEATBELT_PREFAB_PATH = "SeatBelt/SeatBeltPrefab";
		public const int TOTAL_SEATBELTS = 4;

		// Audio paths
		public const string SOFT_BEEP_AUDIO_PATH = "Audio/SFX/SeatBelt/SoftBeep";
		public const string URGENT_BEEP_AUDIO_PATH = "Audio/SFX/SeatBelt/UrgentBeep";
		public const string CONTINUOUS_BEEP_AUDIO_PATH = "Audio/SFX/SeatBelt/ContinuousBeep";

		#endregion

		#region Enums

		public enum SeatBeltPosition
		{
			Driver = 0,
			Passenger = 1,
			RearLeft = 2,
			RearRight = 3
		}

		public enum SeatBeltStatus
		{
			Unknown,
			Fastened,
			Unfastened,
			Warning  // Stato per lampeggio
		}

		#endregion

		#region Colors

		public static readonly Color FASTENED_COLOR = Color.green;
		public static readonly Color UNFASTENED_COLOR = Color.red;
		public static readonly Color UNKNOWN_COLOR = Color.gray;
		public static readonly Color SEATBELT_WARNING_COLOR = new Color(1f, 0.5f, 0f); // Arancione per warning

		public static Color GetColorForStatus(SeatBeltStatus status)
		{
			return status switch
			{
				SeatBeltStatus.Fastened => FASTENED_COLOR,
				SeatBeltStatus.Unfastened => UNFASTENED_COLOR,
				SeatBeltStatus.Warning => SEATBELT_WARNING_COLOR,
				_ => UNKNOWN_COLOR
			};
		}

		#endregion

		#region Configuration

		public static SeatBeltConfig GetConfigForDriveMode(DriveMode mode)
		{
			return mode switch
			{
				DriveMode.Eco => new SeatBeltConfig
				{
					SpeedThreshold = 15f,
					EnableVisualWarning = true,
					EnableAudioWarning = true,
					FlashingEnabled = true
				},
				DriveMode.Sport => new SeatBeltConfig
				{
					SpeedThreshold = 25f,
					EnableVisualWarning = true,
					EnableAudioWarning = true,
					FlashingEnabled = true
				},
				_ => new SeatBeltConfig // Comfort
				{
					SpeedThreshold = 20f,
					EnableVisualWarning = true,
					EnableAudioWarning = true,
					FlashingEnabled = true
				}
			};
		}

		public static AudioEscalationLevel GetAudioEscalationLevel(float warningDurationSeconds)
		{
			return warningDurationSeconds switch
			{
				< 10f => AudioEscalationLevel.Soft,
				< 20f => AudioEscalationLevel.Urgent,
				_ => AudioEscalationLevel.Continuous
			};
		}

		#endregion
	}

	/// <summary>
	/// Configurazione SeatBelt
	/// </summary>
	public struct SeatBeltConfig
	{
		public float SpeedThreshold;
		public bool EnableVisualWarning;
		public bool EnableAudioWarning;
		public bool FlashingEnabled;
	}

	/// <summary>
	/// Livelli escalation audio
	/// </summary>
	public enum AudioEscalationLevel
	{
		None,
		Soft,
		Urgent,
		Continuous
	}
}