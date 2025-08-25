using ClusterAudi;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia interna per la WelcomeFeature.
	/// Segue ESATTAMENTE il pattern di IDashboardFeatureInternal.cs del progetto Mercedes.
	/// Definisce COME i MonoBehaviour accedono alla feature (contratto interno).
	/// </summary>
	public interface IWelcomeFeatureInternal : IFeatureInternal
	{
		/// <summary>
		/// Ottiene il riferimento al Client per accesso ai servizi.
		/// IDENTICO al pattern GetClient() del progetto Mercedes.
		/// Utilizzato dai MonoBehaviour per accedere a Services, Broadcaster, etc.
		/// </summary>
		/// <returns>Istanza del Client</returns>
		Client GetClient();
	}
}