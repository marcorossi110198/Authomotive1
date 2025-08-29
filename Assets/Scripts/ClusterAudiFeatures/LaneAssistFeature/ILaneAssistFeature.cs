using ClusterAudi;
using System.Threading.Tasks;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia pubblica per Lane Assist Feature
	/// IDENTICA al pattern IClusterDriveModeFeature del tuo progetto
	/// </summary>
	public interface ILaneAssistFeature : IFeature
	{
		/// <summary>
		/// Istanzia la Lane Assist Feature - IDENTICO pattern Mercedes
		/// </summary>
		Task InstantiateLaneAssistFeature();

		/// <summary>
		/// Abilita/disabilita lane assist
		/// </summary>
		void SetLaneAssistEnabled(bool enabled);

		/// <summary>
		/// Ottiene stato corrente lane assist
		/// </summary>
		LaneAssistData.LaneAssistState GetCurrentState();

		/// <summary>
		/// Forza reset lane departure (per testing)
		/// </summary>
		void ForceResetLaneDeparture();
	}
}