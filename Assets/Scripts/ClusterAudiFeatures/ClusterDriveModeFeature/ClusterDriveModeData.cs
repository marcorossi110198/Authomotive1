using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Costanti per Cluster Drive Mode Feature
	/// IDENTICO al pattern DashboardData.cs del progetto Mercedes
	/// </summary>
	public static class ClusterDriveModeData
	{
		#region Asset Paths - IDENTICO Mercedes Pattern

		/// <summary>
		/// Path del prefab principale - IDENTICO al pattern Mercedes
		/// </summary>
		public const string CLUSTER_DRIVE_MODE_PREFAB_PATH = "ClusterDriveMode/ClusterDriveModePrefab";

		#endregion

		#region State Names - IDENTICO Mercedes Pattern

		/// <summary>
		/// Nomi stati FSM - IDENTICI al pattern Mercedes
		/// </summary>
		public const string ECO_MODE_STATE = "EcoModeState";
		public const string COMFORT_MODE_STATE = "ComfortModeState";
		public const string SPORT_MODE_STATE = "SportModeState";
		public const string WELCOME_STATE = "WelcomeState";

		#endregion

		#region BACKGROUND RESOURCES PATHS

		/// <summary>
		/// Path delle risorse background per le modalità
		/// </summary>
		public const string ECO_BACKGROUND_PATH = "ClusterDriveMode/img/ClusterECO";
		public const string COMFORT_BACKGROUND_PATH = "ClusterDriveMode/img/ClusterCOMFORT";
		public const string SPORT_BACKGROUND_PATH = "ClusterDriveMode/img/ClusterSPORT";

		/// <summary>
		/// Nome file delle immagini background
		/// Per riferimento quando crei le immagini
		/// </summary>
		public const string ECO_BACKGROUND_FILENAME = "ClusterECO.png";
		public const string COMFORT_BACKGROUND_FILENAME = "ClusterCOMFORT.png";
		public const string SPORT_BACKGROUND_FILENAME = "ClusterSPORT.png";

		#endregion

		#region Debug Keys - IDENTICO Mercedes Pattern

		/// <summary>
		/// Tasti debug per transizioni - IDENTICI al Mercedes
		/// </summary>
		public static readonly KeyCode DEBUG_ECO_KEY = KeyCode.F1;
		public static readonly KeyCode DEBUG_COMFORT_KEY = KeyCode.F2;
		public static readonly KeyCode DEBUG_SPORT_KEY = KeyCode.F3;
		public static readonly KeyCode DEBUG_WELCOME_KEY = KeyCode.F4;

		#endregion

		#region UI Layout Constants

		/// <summary>
		/// Costanti layout UI
		/// </summary>
		public const float MODE_INDICATOR_SIZE = 80f;
		public const float MODE_INDICATOR_SPACING = 100f;
		public const float ANIMATION_DURATION = 0.3f;
		public const float BACKGROUND_TRANSITION_DURATION = 0.3f;

		// Posizioni UI (Screen Space)
		public static readonly Vector2 MODE_INDICATORS_POSITION = new Vector2(100, -100);
		public static readonly Vector2 VEHICLE_INFO_POSITION = new Vector2(-100, -100);

		#endregion

		#region Audi Brand Colors

		/// <summary>
		/// Palette colori Audi per modalità guida
		/// </summary>
		public static readonly Color ECO_COLOR = new Color(0.2f, 0.8f, 0.2f, 1f);        // Verde Eco
		public static readonly Color COMFORT_COLOR = new Color(0.2f, 0.5f, 0.8f, 1f);    // Blu Comfort  
		public static readonly Color SPORT_COLOR = new Color(0.9f, 0.1f, 0.1f, 1f);      // Rosso Sport
		public static readonly Color INACTIVE_COLOR = new Color(0.3f, 0.3f, 0.3f, 0.5f); // Grigio Inactive
		public static readonly Color TEXT_COLOR = new Color(0.9f, 0.9f, 0.9f, 1f);       // Bianco Testi

		#endregion

		#region UI Text Labels

		/// <summary>
		/// Etichette testo per modalità
		/// </summary>
		public const string ECO_MODE_TEXT = "ECO";
		public const string COMFORT_MODE_TEXT = "COMFORT";
		public const string SPORT_MODE_TEXT = "SPORT";

		#endregion

		#region Validation Helper

		/// <summary>
		/// Verifica se uno stato è valido - IDENTICO al pattern Mercedes
		/// </summary>
		public static bool IsValidState(string stateName)
		{
			return stateName == ECO_MODE_STATE ||
				   stateName == COMFORT_MODE_STATE ||
				   stateName == SPORT_MODE_STATE ||
				   stateName == WELCOME_STATE;
		}

		#endregion

		#region Background Utilities

		/// <summary>
		/// Ottieni path background per modalità
		/// </summary>
		public static string GetBackgroundPath(ClusterAudi.DriveMode mode)
		{
			return mode switch
			{
				ClusterAudi.DriveMode.Eco => ECO_BACKGROUND_PATH,
				ClusterAudi.DriveMode.Comfort => COMFORT_BACKGROUND_PATH,
				ClusterAudi.DriveMode.Sport => SPORT_BACKGROUND_PATH,
				_ => COMFORT_BACKGROUND_PATH // Default
			};
		}

		/// <summary>
		/// Ottieni nome file background per modalità
		/// </summary>
		public static string GetBackgroundFilename(ClusterAudi.DriveMode mode)
		{
			return mode switch
			{
				ClusterAudi.DriveMode.Eco => ECO_BACKGROUND_FILENAME,
				ClusterAudi.DriveMode.Comfort => COMFORT_BACKGROUND_FILENAME,
				ClusterAudi.DriveMode.Sport => SPORT_BACKGROUND_FILENAME,
				_ => COMFORT_BACKGROUND_FILENAME // Default
			};
		}

		/// <summary>
		/// Verifica che tutti i background siano disponibili
		/// Utility per debug e validazione
		/// </summary>
		public static bool ValidateBackgroundResources()
		{
			var ecoSprite = Resources.Load<Sprite>(ECO_BACKGROUND_PATH);
			var comfortSprite = Resources.Load<Sprite>(COMFORT_BACKGROUND_PATH);
			var sportSprite = Resources.Load<Sprite>(SPORT_BACKGROUND_PATH);

			bool allLoaded = ecoSprite != null && comfortSprite != null && sportSprite != null;

			if (!allLoaded)
			{
				Debug.LogWarning("[CLUSTER DRIVE MODE DATA] ⚠️ Alcuni background non disponibili in img/");
				if (ecoSprite == null) Debug.LogError($"❌ Manca: {ECO_BACKGROUND_PATH}");
				if (comfortSprite == null) Debug.LogError($"❌ Manca: {COMFORT_BACKGROUND_PATH}");
				if (sportSprite == null) Debug.LogError($"❌ Manca: {SPORT_BACKGROUND_PATH}");

				Debug.LogError("🔍 Verifica struttura: Resources/ClusterDriveMode/img/ClusterXXX.png");
			}
			else
			{
				Debug.Log("[CLUSTER DRIVE MODE DATA] ✅ Tutti i background disponibili in img/");
			}

			return allLoaded;
		}

		#endregion
	}
}