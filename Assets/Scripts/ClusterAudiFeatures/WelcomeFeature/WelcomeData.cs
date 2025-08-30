using UnityEngine;

namespace ClusterAudiFeatures
{
	public static class WelcomeData
	{
		// Durata schermata welcome
		public const float WELCOME_SCREEN_DURATION = 5f;

		// Path del prefab
		public const string WELCOME_SCREEN_PREFAB_PATH = "WelcomeScreen/WelcomeScreenPrefab";

		// Stati FSM
		public const string WELCOME_STATE = "WelcomeState";
		public const string COMFORT_MODE_STATE = "ComfortModeState";
		public const string ECO_MODE_STATE = "EcoModeState";
		public const string SPORT_MODE_STATE = "SportModeState";

		// Tasti debug
		public static readonly KeyCode DEBUG_ECO_MODE_KEY = KeyCode.F1;
		public static readonly KeyCode DEBUG_COMFORT_MODE_KEY = KeyCode.F2;
		public static readonly KeyCode DEBUG_SPORT_MODE_KEY = KeyCode.F3;

		/// <summary>
		/// Verifica validità stato
		/// </summary>
		public static bool IsValidState(string state)
		{
			return state == WELCOME_STATE || state == COMFORT_MODE_STATE ||
				   state == ECO_MODE_STATE || state == SPORT_MODE_STATE;
		}
	}
}