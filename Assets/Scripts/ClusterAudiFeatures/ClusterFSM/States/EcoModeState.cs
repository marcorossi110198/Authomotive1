using ClusterAudi;

/// <summary>
/// Stato modalità Eco del cluster.
/// Gestisce UI e comportamenti specifici per modalità Eco.
/// </summary>
public class EcoModeState : ClusterBaseState
{
	public EcoModeState(ClusterStateContext context) : base(context)
	{
	}

	public override void StateOnEnter()
	{
		// Logica ingresso modalità Eco:
		// - Imposta tema UI verde
		// - Configura parametri veicolo per efficienza
		// - Attiva indicatori Eco
		// - Broadcast cambio modalità
		_context.VehicleData.SetDriveMode(DriveMode.Eco);
	}

	public override void StateOnExit()
	{
		// Cleanup modalità Eco:
		// - Reset tema UI
		// - Salva statistiche Eco
	}

	public override void StateOnUpdate()
	{
		// Update modalità Eco:
		// - Gestione debug keys per cambio modalità
		// - Monitoraggio efficienza
		// - Update UI elementi specifici Eco
	}
}