using ClusterAudi;
using ClusterAudiFeatures;
using UnityEngine;

/// <summary>
/// Stato di benvenuto del cluster.
/// Gestisce la schermata di benvenuto e le transizioni alle modalità di guida.
/// Segue il pattern degli stati del progetto Mercedes.
/// </summary>
public class WelcomeState : ClusterBaseState
{
	private IBroadcaster _broadcaster;
	private IWelcomeFeature _welcomeFeature;

	public WelcomeState(ClusterStateContext context) : base(context)
	{
		_broadcaster = context.Client.Services.Get<IBroadcaster>();
		_welcomeFeature = context.Client.Features.Get<IWelcomeFeature>();
	}

	public override void StateOnEnter()
	{
		Debug.Log("[WELCOME STATE] 🎉 Ingresso in Welcome State");

		// Sottoscrivi agli eventi di transizione
		_broadcaster.Add<WelcomeTransitionEvent>(OnWelcomeTransition);

		// Avvia Welcome Feature (se non già avviata)
		StartWelcomeFeature();
	}

	public override void StateOnExit()
	{
		Debug.Log("[WELCOME STATE] 👋 Uscita da Welcome State");

		// Rimuovi sottoscrizione eventi
		_broadcaster.Remove<WelcomeTransitionEvent>(OnWelcomeTransition);
	}

	public override void StateOnUpdate()
	{
		// Gestione debug keys F1-F4
		HandleDebugInput();
	}

	/// <summary>
	/// Avvia la Welcome Feature se necessario
	/// </summary>
	private async void StartWelcomeFeature()
	{
		try
		{
			// La WelcomeFeature potrebbe essere già istanziata dal ClusterStartUpFlow
			// Questo è un backup nel caso non lo fosse
			await _welcomeFeature.InstantiateWelcomeFeature();
		}
		catch (System.Exception ex)
		{
			Debug.LogWarning($"[WELCOME STATE] WelcomeFeature già istanziata o errore: {ex.Message}");
		}
	}

	/// <summary>
	/// Gestisce le transizioni dalla Welcome Screen
	/// </summary>
	private void OnWelcomeTransition(WelcomeTransitionEvent e)
	{
		Debug.Log($"[WELCOME STATE] 🔄 Richiesta transizione a: {e.TargetState}");

		// Valida che lo stato target sia valido
		if (WelcomeData.IsValidState(e.TargetState))
		{
			_context.ClusterStateMachine.GoTo(e.TargetState);
		}
		else
		{
			Debug.LogWarning($"[WELCOME STATE] ❌ Stato non valido: {e.TargetState}");
		}
	}

	/// <summary>
	/// Gestisce input debug per transizioni manuali
	/// </summary>
	private void HandleDebugInput()
	{
		if (Input.GetKeyDown(WelcomeData.DEBUG_ECO_MODE_KEY))
		{
			Debug.Log("[WELCOME STATE] 🟢 Debug: F1 -> Eco Mode");
			_context.ClusterStateMachine.GoTo(WelcomeData.ECO_MODE_STATE);
		}
		else if (Input.GetKeyDown(WelcomeData.DEBUG_COMFORT_MODE_KEY))
		{
			Debug.Log("[WELCOME STATE] 🔵 Debug: F2 -> Comfort Mode");
			_context.ClusterStateMachine.GoTo(WelcomeData.COMFORT_MODE_STATE);
		}
		else if (Input.GetKeyDown(WelcomeData.DEBUG_SPORT_MODE_KEY))
		{
			Debug.Log("[WELCOME STATE] 🔴 Debug: F3 -> Sport Mode");
			_context.ClusterStateMachine.GoTo(WelcomeData.SPORT_MODE_STATE);
		}

		// ESC per tornare a Welcome (utile per testing)
		else if (Input.GetKeyDown(KeyCode.Escape))
		{
			Debug.Log("[WELCOME STATE] 🔄 Reset: ESC -> Welcome State");
			// Già in Welcome, riavvia la feature
			StartWelcomeFeature();
		}
	}
}