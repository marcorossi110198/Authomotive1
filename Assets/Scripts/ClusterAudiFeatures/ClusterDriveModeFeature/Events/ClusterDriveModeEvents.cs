using ClusterAudi;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Eventi per Cluster Drive Mode Feature
	/// IDENTICO al pattern eventi Mercedes
	/// </summary>

	/// <summary>
	/// Richiesta transizione stato dal Drive Mode UI
	/// IDENTICO al pattern eventi Mercedes per comunicazione UI -> FSM
	/// </summary>
	public class ClusterDriveModeStateTransitionRequest
	{
		public string TargetState { get; }

		public ClusterDriveModeStateTransitionRequest(string targetState)
		{
			TargetState = targetState;
		}
	}

	/// <summary>
	/// Evento aggiornamento tema Drive Mode
	/// IDENTICO al pattern eventi Mercedes per theming
	/// </summary>
	public class ClusterDriveModeThemeUpdateEvent
	{
		public DriveMode CurrentMode { get; }
		public UnityEngine.Color PrimaryColor { get; }
		public UnityEngine.Color SecondaryColor { get; }

		public ClusterDriveModeThemeUpdateEvent(DriveMode mode, UnityEngine.Color primary, UnityEngine.Color secondary)
		{
			CurrentMode = mode;
			PrimaryColor = primary;
			SecondaryColor = secondary;
		}
	}
}