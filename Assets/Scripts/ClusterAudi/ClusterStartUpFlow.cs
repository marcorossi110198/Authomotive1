using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudi
{
	/// <summary>
	/// Flusso di avvio del cluster automotive.
	/// Segue ESATTAMENTE il pattern di DashboardStartUpFlow.cs del progetto base.
	/// </summary>
	public class ClusterStartUpFlow
	{
		public void BeginStartUp(Client client)
		{
			StartUpFlowTask(client);
		}

		private async Task StartUpFlowTask(Client client)
		{
			// Per ora implementazione vuota - la riempiremo quando avremo le features
			// Seguirà il pattern del progetto base:

			// IBroadcaster clientBroadcaster = client.Services.Get<IBroadcaster>();
			// IWelcomeFeature welcomeFeature = client.Features.Get<IWelcomeFeature>();
			// IDriveModeFeature driveModeFeature = client.Features.Get<IDriveModeFeature>();

			// await welcomeFeature.InstantiateWelcomeFeature();
			// await driveModeFeature.InstantiateDriveModeFeature();

			// clientBroadcaster.Broadcast(new ClusterReadyEvent());

			await Task.Delay(1); // Placeholder per ora
		}
	}
}
