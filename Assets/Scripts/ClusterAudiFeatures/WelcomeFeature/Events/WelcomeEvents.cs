namespace ClusterAudiFeatures
{
	public class WelcomeTransitionEvent
	{
		public string TargetState { get; }

		public WelcomeTransitionEvent(string targetState)
		{
			TargetState = targetState;
		}
	}
}