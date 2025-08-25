namespace ClusterAudiFeatures
{
	/// <summary>
	/// Costanti e dati per la WelcomeFeature.
	/// Segue ESATTAMENTE il pattern di DashboardData.cs del progetto Mercedes.
	/// INCLUDE anche le costanti degli stati (come nel progetto Mercedes).
	/// </summary>
	public static class WelcomeData
	{
		#region Asset Paths - Welcome Feature

		// Prefab principale della Welcome Feature
		public const string WELCOME_FEATURE_PREFAB_PATH = "Cluster/WelcomeFeaturePrefab";

		// Asset UI specifici Welcome
		public const string AUDI_LOGO_ASSET_PATH = "Cluster/UI/AudiLogo";
		public const string WELCOME_BACKGROUND_PATH = "Cluster/UI/WelcomeBackground";

		// Asset Audio Welcome
		public const string WELCOME_STARTUP_SOUND_PATH = "Cluster/Audio/WelcomeStartup";

		/// Percorso del prefab Welcome Screen nei Resources
		public const string WELCOME_SCREEN_PREFAB_PATH = "WelcomeScreen/WelcomeScreenPrefab";

		#endregion

		#region State Machine States

		// IMPORTANTE: Le costanti degli stati vanno nella PRIMA feature implementata
		// Questo segue il pattern del progetto Mercedes dove DashboardData.cs ha gli stati

		// Stati principali del cluster
		public const string WELCOME_STATE = "WelcomeState";
		public const string ECO_MODE_STATE = "EcoModeState";
		public const string COMFORT_MODE_STATE = "ComfortModeState";
		public const string SPORT_MODE_STATE = "SportModeState";

		// Stati aggiuntivi per future implementazioni
		public const string SETTINGS_STATE = "SettingsState";
		public const string NAVIGATION_STATE = "NavigationState";
		public const string DIAGNOSTICS_STATE = "DiagnosticsState";

		#endregion

		#region Welcome Screen Configuration

		// Timing Welcome Screen
		public const float WELCOME_SCREEN_DURATION = 5f; // 5 secondi prima di transizione automatica
		public const float LOGO_FADE_IN_DURATION = 1.5f; // Tempo fade-in logo
		public const float LOGO_PULSE_DURATION = 2f; // Durata pulsazione logo
		public const float LOGO_FADE_OUT_DURATION = 1f; // Tempo fade-out prima transizione

		// Configurazione animazione logo
		public const float LOGO_PULSE_MIN_SCALE = 0.95f;
		public const float LOGO_PULSE_MAX_SCALE = 1.05f;
		public const float LOGO_PULSE_SPEED = 2f; // Velocità pulsazione

		// Colori Welcome Screen
		public static readonly UnityEngine.Color WELCOME_BACKGROUND_COLOR = new(0.1f, 0.1f, 0.1f, 1f);
		public static readonly UnityEngine.Color WELCOME_TEXT_COLOR = new(1f, 1f, 1f, 0.9f);
		public static readonly UnityEngine.Color AUDI_LOGO_COLOR = new(1f, 1f, 1f, 1f);
		public static readonly UnityEngine.Color PROGRESS_BAR_COLOR = new (0.8f, 0.1f, 0.1f, 1f);

		#endregion

		#region Debug Configuration

		// Debug keys - IDENTICHE al progetto Mercedes
		public const UnityEngine.KeyCode DEBUG_ECO_MODE_KEY = UnityEngine.KeyCode.F1;
		public const UnityEngine.KeyCode DEBUG_COMFORT_MODE_KEY = UnityEngine.KeyCode.F2;
		public const UnityEngine.KeyCode DEBUG_SPORT_MODE_KEY = UnityEngine.KeyCode.F3;
		public const UnityEngine.KeyCode DEBUG_WELCOME_KEY = UnityEngine.KeyCode.F4;

		// Debug sistema (continuano dal ClusterClient)
		public const UnityEngine.KeyCode DEBUG_SYSTEM_STATUS_KEY = UnityEngine.KeyCode.F5;
		public const UnityEngine.KeyCode DEBUG_VEHICLE_TEST_KEY = UnityEngine.KeyCode.F6;
		public const UnityEngine.KeyCode DEBUG_BROADCASTER_TEST_KEY = UnityEngine.KeyCode.F7;

		#endregion

		#region UI Layout Configuration

		// Layout Welcome Screen
		public const float LOGO_SIZE_WIDTH = 300f;
		public const float LOGO_SIZE_HEIGHT = 150f;
		public const float LOGO_POSITION_Y_OFFSET = 50f; // Offset dal centro

		// Timer text configuration
		public const float TIMER_TEXT_SIZE = 24f;
		public const float TIMER_POSITION_Y_OFFSET = -100f; // Sotto il logo

		// Welcome text configuration  
		public const string WELCOME_TEXT = "Welcome to Audi Cluster";
		public const float WELCOME_TEXT_SIZE = 32f;
		public const float WELCOME_TEXT_Y_OFFSET = 150f; // Sopra il logo

		#endregion

		#region Animation Curves

		// Curve di animazione per Welcome Screen
		public static readonly UnityEngine.AnimationCurve LOGO_FADE_IN_CURVE =
			UnityEngine.AnimationCurve.EaseInOut(0, 0, 1, 1);

		public static readonly UnityEngine.AnimationCurve LOGO_PULSE_CURVE =
			new UnityEngine.AnimationCurve(
				new UnityEngine.Keyframe(0, 0),
				new UnityEngine.Keyframe(0.5f, 1),
				new UnityEngine.Keyframe(1, 0)
			);

		public static readonly UnityEngine.AnimationCurve LOGO_FADE_OUT_CURVE =
			UnityEngine.AnimationCurve.EaseInOut(0, 1, 1, 0);

		#endregion

		#region Validation Methods

		/// <summary>
		/// Valida se uno stato è valido
		/// IDENTICO al pattern del progetto Mercedes
		/// </summary>
		public static bool IsValidState(string stateName)
		{
			return stateName switch
			{
				WELCOME_STATE or
				ECO_MODE_STATE or
				COMFORT_MODE_STATE or
				SPORT_MODE_STATE => true,
				_ => false
			};
		}

		/// <summary>
		/// Ottieni lo stato di default del sistema
		/// </summary>
		public static string GetDefaultState()
		{
			return WELCOME_STATE; // Sempre iniziamo con Welcome
		}

		/// <summary>
		/// Ottieni lo stato di default dopo Welcome
		/// </summary>
		public static string GetPostWelcomeState()
		{
			return COMFORT_MODE_STATE; // Dopo welcome andiamo in Comfort
		}

		/// <summary>
		/// Verifica se uno stato è una modalità di guida
		/// </summary>
		public static bool IsDrivingModeState(string stateName)
		{
			return stateName switch
			{
				ECO_MODE_STATE or
				COMFORT_MODE_STATE or
				SPORT_MODE_STATE => true,
				_ => false
			};
		}

		#endregion

		#region Development Notes

		/*
         * NOTE PER LO SVILUPPO:
         * 
         * 1. Questo file segue ESATTAMENTE il pattern di DashboardData.cs
         * 2. INCLUDE le costanti degli stati come nel progetto Mercedes
         * 3. Le timing possono essere tweakate durante lo sviluppo
         * 4. I path degli asset verranno finalizzati quando creiamo i prefab
         * 5. Le animazioni curve possono essere modificate per migliorare l'esperienza
         * 
         * IMPORTANTE: 
         * - Questa è la PRIMA feature, quindi include le costanti degli stati
         * - Le prossime feature (DriveModeFeature, etc.) non avranno gli stati
         * - Segue il pattern del progetto Mercedes al 100%
         */

		#endregion
	}
}