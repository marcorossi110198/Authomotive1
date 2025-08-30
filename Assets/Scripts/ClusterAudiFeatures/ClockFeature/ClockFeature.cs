using ClusterAudi;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Implementazione Clock Feature
	/// </summary>
	public class ClockFeature : BaseFeature, IClockFeature, IClockFeatureInternal
	{
		public ClockFeature(Client client) : base(client)
		{
		}

		public async Task InstantiateClockFeature()
		{
			var clockDisplayInstance = await _assetService.InstantiateAsset<ClockDisplayBehaviour>(
				ClockData.CLOCK_DISPLAY_PREFAB_PATH);

			if (clockDisplayInstance != null)
			{
				clockDisplayInstance.Initialize(this);
			}
			else
			{
				Debug.LogWarning("[CLOCK FEATURE] Prefab non trovato: " + ClockData.CLOCK_DISPLAY_PREFAB_PATH);
			}
		}

		public Client GetClient()
		{
			return _client;
		}
	}
}