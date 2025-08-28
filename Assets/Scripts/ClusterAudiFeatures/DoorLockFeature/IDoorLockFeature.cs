using ClusterAudi;
using System.Threading.Tasks;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia pubblica per DoorLock Feature - SEMPLICE
	/// IDENTICA al pattern IClockFeature che hai appena implementato
	/// </summary>
	public interface IDoorLockFeature : IFeature
	{
		/// <summary>
		/// Istanzia la DoorLock Feature - IDENTICO pattern Mercedes
		/// </summary>
		Task InstantiateDoorLockFeature();

		/// <summary>
		/// Ottiene stato corrente (locked/unlocked)
		/// </summary>
		bool IsLocked { get; }

		/// <summary>
		/// Toggle manuale lock/unlock
		/// </summary>
		void ToggleLock();
	}
}