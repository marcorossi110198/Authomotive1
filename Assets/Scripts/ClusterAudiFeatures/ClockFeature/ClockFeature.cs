using ClusterAudi;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Implementazione Clock Feature - VERSIONE SOLO PREFAB
	/// IDENTICA al pattern SeatBeltFeature.cs che hai già implementato
	/// NESSUN FALLBACK - Solo caricamento prefab come le altre features
	/// </summary>
	public class ClockFeature : BaseFeature, IClockFeature, IClockFeatureInternal
	{
		#region Constructor - IDENTICO Mercedes

		/// <summary>
		/// Costruttore - IDENTICO al pattern Mercedes
		/// </summary>
		public ClockFeature(Client client) : base(client)
		{
			Debug.Log("[CLOCK FEATURE] 🕐 ClockFeature inizializzata");
		}

		#endregion

		#region IClockFeature Implementation - IDENTICO Mercedes

		/// <summary>
		/// Istanzia la Clock Feature - IDENTICO al pattern Mercedes
		/// SOLO CARICAMENTO PREFAB - NESSUN FALLBACK
		/// </summary>
		public async Task InstantiateClockFeature()
		{
			Debug.Log("[CLOCK FEATURE] 🚀 Istanziazione Clock Feature...");

			try
			{
				// IDENTICO alle tue altre features: usa AssetService per caricare prefab
				var clockDisplayInstance = await _assetService.InstantiateAsset<ClockDisplayBehaviour>(
					ClockData.CLOCK_DISPLAY_PREFAB_PATH);

				if (clockDisplayInstance != null)
				{
					// IDENTICO alle tue altre features: Initialize con this per dependency injection
					clockDisplayInstance.Initialize(this);
					Debug.Log("[CLOCK FEATURE] ✅ Clock Display istanziato da prefab");
				}
				else
				{
					// IDENTICO alle tue altre features: solo warning se prefab non trovato
					Debug.LogWarning("[CLOCK FEATURE] ⚠️ Prefab non trovato: " + ClockData.CLOCK_DISPLAY_PREFAB_PATH);
					Debug.LogError("[CLOCK FEATURE] ❌ Crea il prefab ClockDisplayPrefab in Resources/Clock/");
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[CLOCK FEATURE] ❌ Errore istanziazione: {ex.Message}");
				Debug.LogException(ex);
			}
		}

		#endregion

		#region IClockFeatureInternal Implementation - IDENTICO Mercedes

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