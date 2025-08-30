using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ClusterAudi;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// SeatBelt Behaviour - VERSIONE CORRETTA
	/// Rimosso: Warning Panel, System Label, Icons Container components
	/// Mantenuto: Tutto il resto + lampeggio icone in warning
	/// </summary>
	public class SeatBeltBehaviour : BaseMonoBehaviour<ISeatBeltFeatureInternal>
	{
		#region Serialized Fields - SOLO ICONE

		[Header("SeatBelt Icons")]
		[SerializeField] private Image _driverIcon;
		[SerializeField] private Image _passengerIcon;
		[SerializeField] private Image _rearLeftIcon;
		[SerializeField] private Image _rearRightIcon;

		#endregion

		#region Private Fields

		private IBroadcaster _broadcaster;
		private Image[] _seatBeltIcons;
		private SeatBeltData.SeatBeltStatus[] _displayedStates;
		private bool[] _iconFlashStates;

		// Flash system
		private bool _isFlashingActive = false;
		private Coroutine _iconFlashCoroutine;

		#endregion

		#region BaseMonoBehaviour Override

		protected override void ManagedAwake()
		{
			Debug.Log("[SEATBELT UI] 🛡️ SeatBeltBehaviour inizializzato");

			// Ottieni servizi
			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();

			// Setup arrays
			_seatBeltIcons = new Image[] { _driverIcon, _passengerIcon, _rearLeftIcon, _rearRightIcon };
			_displayedStates = new SeatBeltData.SeatBeltStatus[SeatBeltData.TOTAL_SEATBELTS];
			_iconFlashStates = new bool[SeatBeltData.TOTAL_SEATBELTS];

			// Validazione e setup
			ValidateComponents();
			SetupInitialUI();
			SubscribeToEvents();
		}

		protected override void ManagedUpdate()
		{
			HandleDebugInput();
		}

		protected override void ManagedOnDestroy()
		{
			Debug.Log("[SEATBELT UI] 🗑️ SeatBeltBehaviour distrutto");
			UnsubscribeFromEvents();
			StopAllCoroutines();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Aggiorna stato singola cintura
		/// </summary>
		public void UpdateSeatBeltStatus(SeatBeltData.SeatBeltPosition position, SeatBeltData.SeatBeltStatus status)
		{
			int index = (int)position;
			if (index < 0 || index >= SeatBeltData.TOTAL_SEATBELTS) return;

			if (_displayedStates[index] != status)
			{
				_displayedStates[index] = status;
				UpdateSingleIconVisual(position, status);
			}
		}

		/// <summary>
		/// Aggiorna tutti gli stati
		/// </summary>
		public void UpdateAllSeatBeltStates(SeatBeltData.SeatBeltStatus[] states)
		{
			if (states == null || states.Length != SeatBeltData.TOTAL_SEATBELTS) return;

			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				if (_displayedStates[i] != states[i])
				{
					_displayedStates[i] = states[i];
					UpdateSingleIconVisual((SeatBeltData.SeatBeltPosition)i, states[i]);
				}
			}
		}

		#endregion

		#region Private Methods

		private void ValidateComponents()
		{
			for (int i = 0; i < _seatBeltIcons.Length; i++)
			{
				if (_seatBeltIcons[i] == null)
				{
					Debug.LogError($"[SEATBELT UI] ❌ Icona {i} non assegnata!");
				}
			}
		}

		private void SetupInitialUI()
		{
			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				if (_seatBeltIcons[i] != null)
				{
					_displayedStates[i] = SeatBeltData.SeatBeltStatus.Unknown;
					_seatBeltIcons[i].color = SeatBeltData.GetColorForStatus(SeatBeltData.SeatBeltStatus.Unknown);
					_iconFlashStates[i] = false;
				}
			}
		}

		private void UpdateSingleIconVisual(SeatBeltData.SeatBeltPosition position, SeatBeltData.SeatBeltStatus status)
		{
			int index = (int)position;
			if (index < 0 || index >= SeatBeltData.TOTAL_SEATBELTS || _seatBeltIcons[index] == null)
				return;

			// Aggiorna colore solo se non sta flashando
			if (!_iconFlashStates[index])
			{
				_seatBeltIcons[index].color = SeatBeltData.GetColorForStatus(status);
			}
		}

		#endregion

		#region Event System

		private void SubscribeToEvents()
		{
			_broadcaster.Add<SeatBeltFlashIconsEvent>(OnFlashIcons);
			Debug.Log("[SEATBELT UI] 📡 Eventi sottoscritti");
		}

		private void UnsubscribeFromEvents()
		{
			if (_broadcaster != null)
			{
				_broadcaster.Remove<SeatBeltFlashIconsEvent>(OnFlashIcons);
			}
		}

		private void OnFlashIcons(SeatBeltFlashIconsEvent e)
		{
			if (e.StartFlashing)
			{
				StartIconFlashing(e.PositionsToFlash, e.FlashInterval);
			}
			else
			{
				StopIconFlashing();
			}
		}

		#endregion

		#region Flash System - LAMPEGGIO ICONE

		/// <summary>
		/// Avvia lampeggio icone specificate
		/// </summary>
		private void StartIconFlashing(SeatBeltData.SeatBeltPosition[] positions, float interval)
		{
			StopIconFlashing();

			if (positions == null || positions.Length == 0) return;

			_isFlashingActive = true;

			// Marca posizioni per flash
			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				_iconFlashStates[i] = false;
			}

			foreach (var pos in positions)
			{
				int index = (int)pos;
				if (index >= 0 && index < SeatBeltData.TOTAL_SEATBELTS)
				{
					_iconFlashStates[index] = true;
				}
			}

			// Avvia coroutine flash
			_iconFlashCoroutine = StartCoroutine(FlashIconsCoroutine(positions, interval));
			Debug.Log($"[SEATBELT UI] ⚡ Lampeggio avviato per {positions.Length} icone");
		}

		/// <summary>
		/// Ferma lampeggio icone
		/// </summary>
		private void StopIconFlashing()
		{
			if (_iconFlashCoroutine != null)
			{
				StopCoroutine(_iconFlashCoroutine);
				_iconFlashCoroutine = null;
			}

			_isFlashingActive = false;

			// Reset colori a stati normali
			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				if (_iconFlashStates[i] && _seatBeltIcons[i] != null)
				{
					_iconFlashStates[i] = false;
					_seatBeltIcons[i].color = SeatBeltData.GetColorForStatus(_displayedStates[i]);
				}
			}

			Debug.Log("[SEATBELT UI] ⚡ Lampeggio fermato");
		}

		/// <summary>
		/// Coroutine per lampeggio - OGNI SECONDO come richiesto
		/// </summary>
		private IEnumerator FlashIconsCoroutine(SeatBeltData.SeatBeltPosition[] positions, float interval)
		{
			while (_isFlashingActive)
			{
				// PHASE 1: Icone VISIBILI (0.5 secondi)
				foreach (var pos in positions)
				{
					int index = (int)pos;
					if (index >= 0 && index < SeatBeltData.TOTAL_SEATBELTS && _seatBeltIcons[index] != null)
					{
						_seatBeltIcons[index].color = SeatBeltData.GetColorForStatus(_displayedStates[index]);
					}
				}

				yield return new WaitForSeconds(interval * 0.5f); // 0.5 secondi visibili

				// PHASE 2: Icone INVISIBILI (0.5 secondi)  
				foreach (var pos in positions)
				{
					int index = (int)pos;
					if (index >= 0 && index < SeatBeltData.TOTAL_SEATBELTS && _seatBeltIcons[index] != null)
					{
						_seatBeltIcons[index].color = Color.clear; // Trasparente = invisibile
					}
				}

				yield return new WaitForSeconds(interval * 0.5f); // 0.5 secondi invisibili
			}
		}

		#endregion

		#region Debug Controls

		private void HandleDebugInput()
		{
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.Q)) ToggleSeatBelt(SeatBeltData.SeatBeltPosition.Driver, "DRIVER");
			if (Input.GetKeyDown(KeyCode.E)) ToggleSeatBelt(SeatBeltData.SeatBeltPosition.Passenger, "PASSENGER");
			if (Input.GetKeyDown(KeyCode.R)) ToggleSeatBelt(SeatBeltData.SeatBeltPosition.RearLeft, "REAR LEFT");
			if (Input.GetKeyDown(KeyCode.T)) ToggleSeatBelt(SeatBeltData.SeatBeltPosition.RearRight, "REAR RIGHT");

			if (Input.GetKeyDown(KeyCode.M))
			{
				var client = _feature.GetClient();
				var seatBeltFeature = client.Features.Get<ISeatBeltFeature>();
				seatBeltFeature.ForceWarningCheck();
			}

			if (Input.GetKeyDown(KeyCode.I))
			{
				LogAllSeatBeltStates();
			}
#endif
		}

		private void ToggleSeatBelt(SeatBeltData.SeatBeltPosition position, string name)
		{
			var currentState = _feature.GetAllSeatBeltStates()[(int)position];
			var newState = currentState == SeatBeltData.SeatBeltStatus.Fastened
				? SeatBeltData.SeatBeltStatus.Unfastened
				: SeatBeltData.SeatBeltStatus.Fastened;

			var client = _feature.GetClient();
			var seatBeltFeature = client.Features.Get<ISeatBeltFeature>();
			seatBeltFeature.SetSeatBeltStatus(position, newState);

			string statusIcon = newState == SeatBeltData.SeatBeltStatus.Fastened ? "✅" : "❌";
			Debug.Log($"[SEATBELT UI] 🔧 DEBUG: {name} → {statusIcon} {newState}");
		}

		private void LogAllSeatBeltStates()
		{
			var states = _feature.GetAllSeatBeltStates();
			Debug.Log("=== SEATBELT STATUS ===");
			for (int i = 0; i < states.Length; i++)
			{
				string name = ((SeatBeltData.SeatBeltPosition)i).ToString();
				string icon = states[i] == SeatBeltData.SeatBeltStatus.Fastened ? "✅" : "❌";
				Debug.Log($"{name}: {icon} {states[i]}");
			}
			Debug.Log("Controls: Q=Driver | E=Passenger | R=RearLeft | T=RearRight | M=ForceCheck | I=Info");
		}

		#endregion
	}
}