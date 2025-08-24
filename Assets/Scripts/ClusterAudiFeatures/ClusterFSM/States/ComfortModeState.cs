using ClusterAudi;

/// <summary>
/// Stato modalit� Comfort del cluster.
/// Gestisce UI e comportamenti specifici per modalit� Comfort.
/// </summary>
public class ComfortModeState : ClusterBaseState
{
	public ComfortModeState(ClusterStateContext context) : base(context)
	{
	}

	public override void StateOnEnter()
	{
		// Logica ingresso modalit� Comfort:
		// - Imposta tema UI blu
		// - Configura parametri veicolo bilanciati
		// - Attiva indicatori Comfort
		// - Broadcast cambio modalit�
		_context.VehicleData.SetDriveMode(DriveMode.Comfort);
	}

	public override void StateOnExit()
	{
		// Cleanup modalit� Comfort:
		// - Reset tema UI
		// - Salva preferenze Comfort
	}

	public override void StateOnUpdate()
	{
		// Update modalit� Comfort:
		// - Gestione debug keys per cambio modalit�
		// - Bilanciamento prestazioni/efficienza
		// - Update UI elementi specifici Comfort
	}
}