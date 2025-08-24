using ClusterAudi;

/// <summary>
/// Stato modalit� Eco del cluster.
/// Gestisce UI e comportamenti specifici per modalit� Eco.
/// </summary>
public class EcoModeState : ClusterBaseState
{
	public EcoModeState(ClusterStateContext context) : base(context)
	{
	}

	public override void StateOnEnter()
	{
		// Logica ingresso modalit� Eco:
		// - Imposta tema UI verde
		// - Configura parametri veicolo per efficienza
		// - Attiva indicatori Eco
		// - Broadcast cambio modalit�
		_context.VehicleData.SetDriveMode(DriveMode.Eco);
	}

	public override void StateOnExit()
	{
		// Cleanup modalit� Eco:
		// - Reset tema UI
		// - Salva statistiche Eco
	}

	public override void StateOnUpdate()
	{
		// Update modalit� Eco:
		// - Gestione debug keys per cambio modalit�
		// - Monitoraggio efficienza
		// - Update UI elementi specifici Eco
	}
}