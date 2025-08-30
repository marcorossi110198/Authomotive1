using ClusterAudi;
using ClusterAudiFeatures;
using UnityEngine;

/// <summary>
/// Stato di benvenuto del cluster - VERSIONE SEMPLIFICATA
/// Rimossa logica ridondante, mantiene solo l'essenziale
/// </summary>
public class WelcomeState : ClusterBaseState
{
	private IBroadcaster _broadcaster;
	private IWelcomeFeature _welcomeFeature;
	private bool _isInstantiated = false;

	public WelcomeState(ClusterStateContext context) : base(context)
	{
		_broadcaster = context.Client.Services.Get<IBroadcaster>();
		_welcomeFeature = context.Client.Features.Get<IWelcomeFeature>();
	}

	public override void StateOnEnter()
	{
		Debug.Log("[WELCOME STATE] 🎉 Ingresso in Welcome State");

		_broadcaster.Add<WelcomeTransitionEvent>(OnWelcomeTransition);
		InstantiateWelcome();
	}

	public override void StateOnExit()
	{
		Debug.Log("[WELCOME STATE] 👋 Uscita da Welcome State");
		_broadcaster.Remove<WelcomeTransitionEvent>(OnWelcomeTransition);
		_isInstantiated = false;
	}

	public override void StateOnUpdate()
	{
		// Debug input per test rapidi
		if (Input.GetKeyDown(KeyCode.F1))
			_context.ClusterStateMachine.GoTo(WelcomeData.ECO_MODE_STATE);
		else if (Input.GetKeyDown(KeyCode.F2))
			_context.ClusterStateMachine.GoTo(WelcomeData.COMFORT_MODE_STATE);
		else if (Input.GetKeyDown(KeyCode.F3))
			_context.ClusterStateMachine.GoTo(WelcomeData.SPORT_MODE_STATE);
	}

	private async void InstantiateWelcome()
	{
		if (_isInstantiated) return;

		try
		{
			Debug.Log("[WELCOME STATE] 🚀 Istanziazione WelcomeFeature...");
			await _welcomeFeature.InstantiateWelcomeFeature();
			_isInstantiated = true;
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"[WELCOME STATE] ❌ Errore: {ex.Message}");
		}
	}

	private void OnWelcomeTransition(WelcomeTransitionEvent e)
	{
		Debug.Log($"[WELCOME STATE] 🔄 Transizione a: {e.TargetState}");

		if (WelcomeData.IsValidState(e.TargetState))
			_context.ClusterStateMachine.GoTo(e.TargetState);
		else
			Debug.LogWarning($"[WELCOME STATE] ❌ Stato non valido: {e.TargetState}");
	}
}