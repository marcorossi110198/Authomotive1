using ClusterAudi;

/// <summary>
/// State Machine per il cluster automotive.
/// IDENTICO a DashboardFSM.cs del progetto base.
/// </summary>
public class ClusterFSM : FSM<ClusterBaseState>
{
	// Eredita tutto da FSM<T> della cartella StateMachine:
	// - AddState()
	// - RemoveState() 
	// - GoTo()
	// - UpdateState()
	// - GetCurrentState()
}