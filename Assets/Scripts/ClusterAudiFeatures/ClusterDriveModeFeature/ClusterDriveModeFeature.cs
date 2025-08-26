using System.Threading.Tasks;
using ClusterAudi;
using UnityEngine;

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