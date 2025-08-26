using System.Threading.Tasks;
using ClusterAudi;
using UnityEngine;

/// <summary>
/// CLUSTER DRIVE MODE FEATURE - Pattern Mercedes ESATTO
/// 
/// STRUTTURA NOMENCLATURA CORRETTA:
/// - ClusterDriveModeFeature (gestisce UI modalità guida)
/// - ClusterDriveModeBehaviour (MonoBehaviour UI)
/// - ClusterDriveModeData (costanti)
/// - Eventi specifici per drive modes
/// </summary>

#region 1. INTERFACCE - IClusterDriveModeFeature.cs

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia pubblica per Cluster Drive Mode Feature
	/// IDENTICA al pattern IDashboardFeature del progetto Mercedes
	/// </summary>
	public interface IClusterDriveModeFeature : IFeature
	{
		/// <summary>
		/// Istanzia l'UI per le modalità di guida del cluster
		/// IDENTICO al pattern InstantiateDashboardFeature() del Mercedes
		/// </summary>
		Task InstantiateClusterDriveModeFeature();
	}
}

#endregion

#region 2. INTERFACCE INTERNE - IClusterDriveModeFeatureInternal.cs

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia interna per Cluster Drive Mode Feature  
	/// IDENTICA al pattern IDashboardFeatureInternal del progetto Mercedes
	/// </summary>
	public interface IClusterDriveModeFeatureInternal : IFeatureInternal
	{
		/// <summary>
		/// Ottiene il Client per accesso ai servizi
		/// IDENTICO al pattern GetClient() del Mercedes
		/// </summary>
		Client GetClient();
	}
}

#endregion

#region 3. DATA CONSTANTS - ClusterDriveModeData.cs

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
	}
}

#endregion

#region 4. FEATURE IMPLEMENTATION - ClusterDriveModeFeature.cs



namespace ClusterAudiFeatures
{
	/// <summary>
	/// Implementazione Cluster Drive Mode Feature
	/// IDENTICA al pattern DashboardFeature.cs del progetto Mercedes
	/// 
	/// RESPONSABILITÀ:
	/// - Gestisce l'istanziazione UI modalità guida
	/// - Fornisce accesso al Client per MonoBehaviour
	/// - Segue esattamente il pattern BaseFeature del Mercedes
	/// </summary>
	public class ClusterDriveModeFeature : BaseFeature, IClusterDriveModeFeature, IClusterDriveModeFeatureInternal
	{
		#region Constructor - IDENTICO Mercedes

		/// <summary>
		/// Costruttore - IDENTICO al pattern Mercedes
		/// </summary>
		public ClusterDriveModeFeature(Client client) : base(client)
		{
			Debug.Log("[CLUSTER DRIVE MODE FEATURE] 🎛️ ClusterDriveModeFeature inizializzata");
		}

		#endregion

		#region IClusterDriveModeFeature Implementation - IDENTICO Mercedes

		/// <summary>
		/// Istanzia la UI Drive Mode - IDENTICO al pattern Mercedes
		/// </summary>
		public async Task InstantiateClusterDriveModeFeature()
		{
			Debug.Log("[CLUSTER DRIVE MODE FEATURE] 🚀 Istanziazione Cluster Drive Mode UI...");

			try
			{
				// IDENTICO Mercedes: usa AssetService per caricare prefab
				var clusterDriveModeInstance = await _assetService.InstantiateAsset<ClusterDriveModeBehaviour>(
					ClusterDriveModeData.CLUSTER_DRIVE_MODE_PREFAB_PATH);

				if (clusterDriveModeInstance != null)
				{
					// IDENTICO Mercedes: Initialize con this per dependency injection
					clusterDriveModeInstance.Initialize(this);
					Debug.Log("[CLUSTER DRIVE MODE FEATURE] ✅ Drive Mode UI istanziata da prefab");
				}
				else
				{
					// IDENTICO Mercedes: log warning se prefab non trovato
					Debug.LogWarning("[CLUSTER DRIVE MODE FEATURE] ⚠️ Prefab non trovato: " +
						ClusterDriveModeData.CLUSTER_DRIVE_MODE_PREFAB_PATH);

					// Mercedes non crea dinamicamente, noi seguiamo stesso pattern
					Debug.LogError("[CLUSTER DRIVE MODE FEATURE] ❌ Creazione dinamica non implementata. Crea il prefab!");
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[CLUSTER DRIVE MODE FEATURE] ❌ Errore istanziazione: {ex.Message}");
				Debug.LogException(ex);
			}
		}

		#endregion

		#region IClusterDriveModeFeatureInternal Implementation - IDENTICO Mercedes

		/// <summary>
		/// Ottiene il Client - IDENTICO al pattern Mercedes
		/// </summary>
		public Client GetClient()
		{
			return _client;
		}

		#endregion
	}
}

#endregion

#region 5. EVENTS - Events/ClusterDriveModeEvents.cs

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Eventi per Cluster Drive Mode Feature
	/// IDENTICO al pattern eventi Mercedes
	/// </summary>

	/// <summary>
	/// Richiesta transizione stato dal Drive Mode UI
	/// IDENTICO al pattern eventi Mercedes per comunicazione UI -> FSM
	/// </summary>
	public class ClusterDriveModeStateTransitionRequest
	{
		public string TargetState { get; }

		public ClusterDriveModeStateTransitionRequest(string targetState)
		{
			TargetState = targetState;
		}
	}

	/// <summary>
	/// Evento aggiornamento tema Drive Mode
	/// IDENTICO al pattern eventi Mercedes per theming
	/// </summary>
	public class ClusterDriveModeThemeUpdateEvent
	{
		public DriveMode CurrentMode { get; }
		public Color PrimaryColor { get; }
		public Color SecondaryColor { get; }

		public ClusterDriveModeThemeUpdateEvent(DriveMode mode, Color primary, Color secondary)
		{
			CurrentMode = mode;
			PrimaryColor = primary;
			SecondaryColor = secondary;
		}
	}
}

#endregion