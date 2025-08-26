/// <summary>
/// Richiesta transizione stato dal Drive Mode UI - NON PIÙ NECESSARIO
/// </summary>
public class ClusterDriveModeStateTransitionRequest
{
	public string TargetState { get; }

	public ClusterDriveModeStateTransitionRequest(string targetState)
	{
		TargetState = targetState;
	}
}