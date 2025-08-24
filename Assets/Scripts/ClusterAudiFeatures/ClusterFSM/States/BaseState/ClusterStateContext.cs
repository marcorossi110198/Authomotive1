using ClusterAudi;

/// <summary>
/// Contesto condiviso tra tutti gli stati del cluster.
/// IDENTICO a DashboardStateContext.cs del progetto base.
/// </summary>
public class ClusterStateContext
{
	public Client Client;
	public ClusterFSM ClusterStateMachine;

	// Riferimenti ai servizi principali
	public IVehicleDataService VehicleData;
	public IBroadcaster Broadcaster;

	// UI references che aggiungeremo quando necessario
	// (seguendo il pattern del progetto base)
}