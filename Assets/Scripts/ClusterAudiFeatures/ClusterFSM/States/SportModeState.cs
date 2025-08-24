using ClusterAudi;

/// <summary>
/// Stato modalità Sport del cluster.
/// Gestisce UI e comportamenti specifici per modalità Sport.
/// </summary>
public class SportModeState : ClusterBaseState
{
	public SportModeState(ClusterStateContext context) : base(context)
	{
	}

	public override void StateOnEnter()
	{
		// Logica ingresso modalità Sport:
		// - Imposta tema UI rosso
		// - Configura parametri veicolo per performance
		// - Attiva indicatori Sport
		// - Broadcast cambio modalità
		_context.VehicleData.SetDriveMode(DriveMode.Sport);
	}

	public override void StateOnExit()
	{
		// Cleanup modalità Sport:
		// - Reset tema UI
		// - Salva statistiche performance
	}

	public override void StateOnUpdate()
	{
		// Update modalità Sport:
		// - Gestione debug keys per cambio modalità
		// - Massimizzazione prestazioni
		// - Update UI elementi specifici Sport
	}
}