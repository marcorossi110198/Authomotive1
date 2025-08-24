using ClusterAudi;

/// <summary>
/// Stato base per tutti gli stati del cluster.
/// IDENTICO a DashboardBaseState.cs del progetto base.
/// </summary>
public abstract class ClusterBaseState : IState
{
	protected ClusterStateContext _context;

	public ClusterBaseState(ClusterStateContext context)
	{
		_context = context;
	}

	public abstract void StateOnEnter();
	public abstract void StateOnExit();
	public abstract void StateOnUpdate();
}