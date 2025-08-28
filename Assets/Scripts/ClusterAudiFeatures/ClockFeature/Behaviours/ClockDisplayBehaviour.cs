using ClusterAudi;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// MonoBehaviour per Clock Display - VERSIONE PREFAB
	/// IDENTICO al pattern SeatBeltBehaviour.cs che hai già implementato
	/// USA SOLO COMPONENTI ASSEGNATI NEL PREFAB
	/// </summary>
	public class ClockDisplayBehaviour : BaseMonoBehaviour<IClockFeatureInternal>
	{
		#region Serialized Fields - DA ASSEGNARE NEL PREFAB

		[Header("Clock Display UI - ASSIGN IN PREFAB")]
		[SerializeField] private TextMeshProUGUI _clockText;

		#endregion

		#region Private Fields

		// Servizi
		private IBroadcaster _broadcaster;

		// Update timer
		private Coroutine _clockUpdateCoroutine;
		private string _currentTimeString = "";

		#endregion

		#region BaseMonoBehaviour Override

		protected override void ManagedAwake()
		{
			Debug.Log("[CLOCK DISPLAY] 🕐 ClockDisplayBehaviour inizializzato");

			// Ottieni servizi
			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();

			// Valida componenti del prefab
			ValidateUIComponents();

			// Setup UI
			SetupClockDisplay();
		}

		protected override void ManagedStart()
		{
			Debug.Log("[CLOCK DISPLAY] ▶️ Clock Display avviato");

			// Avvia aggiornamento continuo
			StartClockUpdates();
		}

		protected override void ManagedUpdate()
		{
			// Non serve update continuo, usa Coroutine timer
		}

		protected override void ManagedOnDestroy()
		{
			Debug.Log("[CLOCK DISPLAY] 🗑️ Clock Display distrutto");

			// Stop coroutine
			if (_clockUpdateCoroutine != null)
			{
				StopCoroutine(_clockUpdateCoroutine);
			}
		}

		#endregion

		#region UI Setup - VERSIONE PREFAB

		/// <summary>
		/// Valida che tutti i SerializeField siano assegnati nel prefab
		/// IDENTICO al pattern delle tue altre features
		/// </summary>
		private void ValidateUIComponents()
		{
			int missingComponents = 0;

			if (_clockText == null)
			{
				Debug.LogError("[CLOCK DISPLAY] ❌ _clockText non assegnato nel prefab!");
				missingComponents++;
			}

			if (missingComponents == 0)
			{
				Debug.Log("[CLOCK DISPLAY] ✅ Tutti i SerializeField assegnati correttamente");
			}
			else
			{
				Debug.LogError($"[CLOCK DISPLAY] ❌ {missingComponents} componenti mancanti! Configura il prefab ClockDisplayPrefab");
			}
		}

		/// <summary>
		/// Setup iniziale del display orologio - USA PREFAB
		/// </summary>
		private void SetupClockDisplay()
		{
			if (_clockText == null)
			{
				Debug.LogError("[CLOCK DISPLAY] ❌ _clockText non configurato nel prefab!");
				return;
			}

			// Configura stile del testo (mantiene quello del prefab ma aggiunge configurazione)
			_clockText.text = "--:--";

			Debug.Log("[CLOCK DISPLAY] ✅ Clock display configurato da prefab");
		}

		#endregion

		#region Clock Updates

		/// <summary>
		/// Avvia gli aggiornamenti dell'orologio
		/// </summary>
		private void StartClockUpdates()
		{
			if (_clockText == null)
			{
				Debug.LogError("[CLOCK DISPLAY] ❌ Impossibile avviare updates: _clockText non configurato!");
				return;
			}

			_clockUpdateCoroutine = StartCoroutine(ClockUpdateCoroutine());
		}

		/// <summary>
		/// Coroutine per aggiornamento continuo orologio
		/// </summary>
		private IEnumerator ClockUpdateCoroutine()
		{
			while (true)
			{
				UpdateClockDisplay();
				yield return new WaitForSeconds(ClockData.UPDATE_INTERVAL);
			}
		}

		/// <summary>
		/// Aggiorna il display dell'orologio
		/// </summary>
		private void UpdateClockDisplay()
		{
			try
			{
				// Ottieni orario corrente
				DateTime currentTime = DateTime.Now;
				string newTimeString = currentTime.ToString(ClockData.TIME_FORMAT);

				// Aggiorna solo se cambiato
				if (newTimeString != _currentTimeString)
				{
					_currentTimeString = newTimeString;

					// Aggiorna UI
					if (_clockText != null)
					{
						_clockText.text = _currentTimeString;
					}

					// Broadcast evento (per debug o altre features)
					_broadcaster?.Broadcast(new ClockTimeUpdateEvent(_currentTimeString, currentTime));

					Debug.Log($"[CLOCK DISPLAY] 🕐 Orario aggiornato: {_currentTimeString}");
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[CLOCK DISPLAY] ❌ Errore aggiornamento orologio: {ex.Message}");
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Forza aggiornamento immediato
		/// </summary>
		public void ForceUpdate()
		{
			UpdateClockDisplay();
		}

		/// <summary>
		/// Mostra/nasconde orologio
		/// </summary>
		public void SetVisible(bool visible)
		{
			if (_clockText != null)
			{
				_clockText.gameObject.SetActive(visible);
			}
		}

		#endregion
	}
}