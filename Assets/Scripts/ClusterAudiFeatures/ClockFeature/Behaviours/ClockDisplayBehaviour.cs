using ClusterAudi;
using System.Collections;
using TMPro;
using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// MonoBehaviour per Clock Display - VERSIONE SEMPLIFICATA
	/// </summary>
	public class ClockDisplayBehaviour : BaseMonoBehaviour<IClockFeatureInternal>
	{
		[Header("Clock Display UI")]
		[SerializeField] private TextMeshProUGUI _clockText;

		private Coroutine _clockUpdateCoroutine;

		protected override void ManagedStart()
		{
			// Avvia aggiornamento continuo
			_clockUpdateCoroutine = StartCoroutine(ClockUpdateCoroutine());
		}

		protected override void ManagedOnDestroy()
		{
			if (_clockUpdateCoroutine != null)
			{
				StopCoroutine(_clockUpdateCoroutine);
			}
		}

		/// <summary>
		/// Coroutine per aggiornamento continuo orologio
		/// </summary>
		private IEnumerator ClockUpdateCoroutine()
		{
			while (true)
			{
				if (_clockText != null)
				{
					_clockText.text = System.DateTime.Now.ToString("HH:mm");
				}

				yield return new WaitForSeconds(1f);
			}
		}
	}
}