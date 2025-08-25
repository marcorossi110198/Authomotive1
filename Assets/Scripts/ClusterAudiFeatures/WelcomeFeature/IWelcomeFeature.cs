using System.Threading.Tasks;
using ClusterAudi;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia pubblica per la WelcomeFeature.
	/// Segue ESATTAMENTE il pattern di IDashboardFeature.cs del progetto Mercedes.
	/// Definisce COSA la feature può fare (contratto pubblico).
	/// </summary>
	public interface IWelcomeFeature : IFeature
	{
		/// <summary>
		/// Istanzia la Welcome Feature e tutti i suoi componenti UI.
		/// Metodo principale per avviare la schermata di benvenuto.
		/// IDENTICO al pattern InstantiateDashboardFeature() del progetto Mercedes.
		/// </summary>
		/// <returns>Task che completa quando l'istanziazione è terminata</returns>
		Task InstantiateWelcomeFeature();
	}
}