using ClusterAudi;
using ClusterAudiFeatures;
using UnityEngine;

/// <summary>
/// Stato di benvenuto del cluster - VERSIONE CORRETTA FINALE
/// ESC COMPLETAMENTE RIMOSSO da qui
/// </summary>
public class WelcomeState : ClusterBaseState
{
	private IBroadcaster _broadcaster;
	private IWelcomeFeature _welcomeFeature;

	// Flag per evitare doppia istanziazione
	private bool _isWelcomeFeatureInstantiated = false;

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

		// Istanzia solo se non già fatto
		StartWelcomeFeatureOnce();
	}

	public override void StateOnExit()
	{
		Debug.Log("[WELCOME STATE] 👋 Uscita da Welcome State");

		// Rimuovi sottoscrizione eventi
		_broadcaster.Remove<WelcomeTransitionEvent>(OnWelcomeTransition);

		// Cleanup flag quando esci dallo stato
		_isWelcomeFeatureInstantiated = false;
	}

	public override void StateOnUpdate()
	{
		// Gestione debug keys SOLO F1-F4 (ESC COMPLETAMENTE RIMOSSO)
		HandleDebugInput();
	}

	/// <summary>
	/// Istanzia WelcomeFeature solo una volta
	/// </summary>
	private async void StartWelcomeFeatureOnce()
	{
		if (_isWelcomeFeatureInstantiated)
		{
			Debug.Log("[WELCOME STATE] ⚠️ WelcomeFeature già istanziata - skip");
			return;
		}

		try
		{
			Debug.Log("[WELCOME STATE] 🚀 Istanziazione WelcomeFeature...");

			await _welcomeFeature.InstantiateWelcomeFeature();

			_isWelcomeFeatureInstantiated = true;
			Debug.Log("[WELCOME STATE] ✅ WelcomeFeature istanziata correttamente");
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"[WELCOME STATE] ❌ Errore istanziazione WelcomeFeature: {ex.Message}");
			Debug.LogException(ex);
		}
	}

	private void OnWelcomeTransition(WelcomeTransitionEvent e)
	{
		Debug.Log($"[WELCOME STATE] 🔄 Richiesta transizione a: {e.TargetState}");

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
	/// SOLO F1-F3 per debug - ESC COMPLETAMENTE RIMOSSO
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

		// ESC COMPLETAMENTE RIMOSSO DA QUI
		// Verrà gestito centralmente negli altri stati
	}
}