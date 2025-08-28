using ClusterAudi;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia interna per DoorLock Feature - SEMPLICE
	/// IDENTICA al pattern IClockFeatureInternal che hai appena implementato
	/// </summary>
	public interface IDoorLockFeatureInternal : IFeatureInternal
	{
		/// <summary>
		/// Ottiene il Client per accesso ai servizi
		/// IDENTICO al pattern GetClient() del Mercedes
		/// </summary>
		Client GetClient();

		/// <summary>
		/// Ottiene stato locked corrente
		/// </summary>
		bool IsLocked { get; }

		/// <summary>
		/// Imposta stato locked
		/// </summary>
		void SetLocked(bool locked);
	}
}