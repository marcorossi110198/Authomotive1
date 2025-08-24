namespace ClusterAudi
{
	/// <summary>
	/// Client concreto per dashboard automotive.
	/// Segue ESATTAMENTE il pattern di DashboardClient.cs del progetto base.
	/// </summary>
	public class ClusterClient : Client
	{
		public override void InitFeatures()
		{
			// Per ora vuoto - aggiungeremo le features step by step
			// Seguirà il pattern:
			// Features.Add<IWelcomeFeature>(new WelcomeFeature(this));
			// Features.Add<IDriveModeFeature>(new DriveModeFeature(this));
			// etc.
		}

		public override void StartClient()
		{
			// Per ora vuoto - qui useremo ClusterStartUpFlow
			// ClusterStartUpFlow myFlow = new ClusterStartUpFlow();
			// myFlow.BeginStartUp(this);
		}
	}
}