using ClusterAudi;

/// <summary>
/// Stato modalit� Sport del cluster.
/// Gestisce UI e comportamenti specifici per modalit� Sport.
/// </summary>
public class SportModeState : ClusterBaseState
{
	public SportModeState(ClusterStateContext context) : base(context)
	{
	}

	public override void StateOnEnter()
	{
		// Logica ingresso modalit� Sport:
		// - Imposta tema UI rosso
		// - Configura parametri veicolo per performance
		// - Attiva indicatori Sport
		// - Broadcast cambio modalit�
		_context.VehicleData.SetDriveMode(DriveMode.Sport);
	}

	public override void StateOnExit()
	{
		// Cleanup modalit� Sport:
		// - Reset tema UI
		// - Salva statistiche performance
	}

	public override void StateOnUpdate()
	{
		// Update modalit� Sport:
		// - Gestione debug keys per cambio modalit�
		// - Massimizzazione prestazioni
		// - Update UI elementi specifici Sport
	}
}