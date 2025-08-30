using ClusterAudi;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudiFeatures
{
	public class WelcomeFeature : BaseFeature, IWelcomeFeature, IWelcomeFeatureInternal
	{
		public WelcomeFeature(Client client) : base(client)
		{
			Debug.Log("[WELCOME FEATURE] 🎉 WelcomeFeature inizializzata");
		}

		public async Task InstantiateWelcomeFeature()
		{
			Debug.Log("[WELCOME FEATURE] 🚀 Inizio istanziazione Welcome Feature...");

			try
			{
				// Carica prefab via AssetService (pattern identico a Mercedes)
				var welcomeScreenInstance = await _assetService.InstantiateAsset<WelcomeScreenBehaviour>(WelcomeData.WELCOME_SCREEN_PREFAB_PATH);

				if (welcomeScreenInstance != null)
				{
					welcomeScreenInstance.Initialize(this);
					Debug.Log("[WELCOME FEATURE] 🎉 Welcome Feature istanziata!");
				}
				else
				{
					Debug.LogError("[WELCOME FEATURE] ❌ Impossibile caricare prefab: " + WelcomeData.WELCOME_SCREEN_PREFAB_PATH);
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[WELCOME FEATURE] ❌ Errore durante istanziazione: {ex.Message}");
				Debug.LogException(ex);
			}
		}

		public Client GetClient()
		{
			return _client;
		}
	}
}