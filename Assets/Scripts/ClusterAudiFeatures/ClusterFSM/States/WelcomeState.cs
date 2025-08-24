using ClusterAudi;

/// <summary>
/// Stato di benvenuto del cluster.
/// Corrisponde al primo stato del nostro progetto (schermata benvenuto).
/// </summary>
public class WelcomeState : ClusterBaseState
{
	public WelcomeState(ClusterStateContext context) : base(context)
	{
	}

	public override void StateOnEnter()
	{
		// Logica ingresso stato welcome:
		// - Mostra logo lampeggiante
		// - Avvia timer 5 secondi
		// - Setup debug keys
	}

	public override void StateOnExit()
	{
		// Cleanup stato welcome:
		// - Ferma animazioni logo
		// - Cleanup timer
	}

	public override void StateOnUpdate()
	{
		// Update stato welcome:
		// - Gestione timer 5 secondi
		// - Check debug keys (F1-F4)
		// - Transizione automatica a Comfort mode
	}
}