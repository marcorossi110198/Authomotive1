using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Configurazioni per Clock Feature - VERSIONE SEMPLICE
	/// IDENTICO al pattern ClusterDriveModeData.cs che hai già implementato
	/// </summary>
	public static class ClockData
	{
		#region Asset Paths - IDENTICO Mercedes Pattern

		/// <summary>
		/// Path del prefab ClockDisplay - IDENTICO al pattern Mercedes
		/// </summary>
		public const string CLOCK_DISPLAY_PREFAB_PATH = "Clock/ClockDisplayPrefab";

		#endregion

		#region Clock Configuration - SEMPLICE

		/// <summary>
		/// Formato orario (HH:mm)
		/// </summary>
		public const string TIME_FORMAT = "HH:mm";

		/// <summary>
		/// Intervallo aggiornamento (1 secondo)
		/// </summary>
		public const float UPDATE_INTERVAL = 1.0f;

		#endregion
	}
}