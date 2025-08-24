namespace ClusterAudi
{
	/// <summary>
	/// MonoBehaviour che si auto-inietta le features dal Client.
	/// IDENTICO al BaseSelfInjectedBehaviour del progetto originale.
	/// </summary>
	public class BaseSelfInjectedBehaviour<TFeatureInternal, TFeature> : BaseMonoBehaviour<TFeatureInternal>
		where TFeatureInternal : IFeatureInternal
		where TFeature : IFeature
	{
		private void Awake()
		{
			Client client = Client.Instance;

			if (client == null)
			{
				return;
			}

			var myFeature = client.Features.Get<TFeature>();

			if (myFeature is TFeatureInternal myFeatureInternal)
			{
				Initialize(myFeatureInternal);
			}
		}
	}
}