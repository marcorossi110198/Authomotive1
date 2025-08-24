using ClusterAudi;

/// <summary>
/// Stato modalità Comfort del cluster.
/// Gestisce UI e comportamenti specifici per modalità Comfort.
/// </summary>
public class ComfortModeState : ClusterBaseState
{
	public ComfortModeState(ClusterStateContext context) : base(context)
	{
	}

	public override void StateOnEnter()
	{
		// Logica ingresso modalità Comfort:
		// - Imposta tema UI blu
		// - Configura parametri veicolo bilanciati
		// - Attiva indicatori Comfort
		// - Broadcast cambio modalità
		_context.VehicleData.SetDriveMode(DriveMode.Comfort);
	}

	public override void StateOnExit()
	{
		// Cleanup modalità Comfort:
		// - Reset tema UI
		// - Salva preferenze Comfort
	}

	public override void StateOnUpdate()
	{
		// Update modalità Comfort:
		// - Gestione debug keys per cambio modalità
		// - Bilanciamento prestazioni/efficienza
		// - Update UI elementi specifici Comfort
	}
}