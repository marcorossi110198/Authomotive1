using System.Threading.Tasks;
using ClusterAudi;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia pubblica SeatBelt - SEMPLIFICATA
	/// Rimossi metodi non essenziali
	/// </summary>
	public interface ISeatBeltFeature : IFeature
	{
		/// <summary>
		/// Istanzia la UI SeatBelt
		/// </summary>
		Task InstantiateSeatBeltFeature();

		/// <summary>
		/// Imposta stato di una cintura specifica
		/// </summary>
		void SetSeatBeltStatus(SeatBeltData.SeatBeltPosition position, SeatBeltData.SeatBeltStatus status);

		/// <summary>
		/// Forza controllo sistema (per debug)
		/// </summary>
		void ForceWarningCheck();
	}
}