using UnityEngine;

namespace ClusterAudi
{
	/// <summary>
	/// Client base per Cluster Audi.
	/// Segue ESATTAMENTE il pattern di Client.cs del progetto base.
	/// </summary>
	public abstract class Client : MonoBehaviour
	{
		public static Client Instance { get; private set; }

		public Locator<IService> Services;
		public Locator<IFeature> Features;

		protected IBroadcaster _broadcaster;

		public abstract void InitFeatures();
		public abstract void StartClient();

		public void InitServices()
		{
			// Servizi base identici al progetto originale
			Services.Add<IBroadcaster>(new Broadcaster());
			Services.Add<IAssetService>(new AssetService());

			// Servizio automotive specifico
			Services.Add<IVehicleDataService>(new VehicleDataService());
		}

		private void Awake()
		{
			// Pattern IDENTICO al progetto base
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(gameObject);
				return;
			}

			// Inizializzazione IDENTICA al progetto base
			Services = new Locator<IService>();
			Features = new Locator<IFeature>();

			InitServices();
			InitFeatures();

			_broadcaster = Services.Get<IBroadcaster>();
		}

		private void Start()
		{
			StartClient();
		}

		private void OnDestroy()
		{
			// Cleanup features quando necessario
		}
	}
}