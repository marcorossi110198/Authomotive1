using ClusterAudi;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ClusterAudiFeatures
{
	public class WelcomeScreenBehaviour : BaseMonoBehaviour<IWelcomeFeatureInternal>
	{
		[Header("Welcome Screen UI")]
		[SerializeField] private CanvasGroup _welcomeCanvasGroup;
		[SerializeField] private Image _audiLogo;
		[SerializeField] private TextMeshProUGUI _welcomeText;
		[SerializeField] private TextMeshProUGUI _timerText;
		[SerializeField] private Slider _progressSlider;

		private float _welcomeTimer = 0f;
		private bool _isActive = true;
		private IBroadcaster _broadcaster;

		protected override void ManagedAwake()
		{
			Debug.Log("[WELCOME SCREEN] 🎉 WelcomeScreenBehaviour inizializzato");

			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();

			SetupUI();
		}

		protected override void ManagedStart()
		{
			Debug.Log("[WELCOME SCREEN] ▶️ Avvio Welcome Screen...");
			StartCoroutine(WelcomeSequence());
		}

		protected override void ManagedUpdate()
		{
			if (!_isActive) return;

			// Update progress slider
			if (_progressSlider != null)
				_progressSlider.value = _welcomeTimer;

			// Debug input
			HandleInput();
		}

		private void SetupUI()
		{
			// CanvasGroup setup
			if (_welcomeCanvasGroup == null)
				_welcomeCanvasGroup = GetComponentInParent<CanvasGroup>();

			if (_welcomeCanvasGroup != null)
				_welcomeCanvasGroup.alpha = 1f;

			// Logo setup - inizia trasparente
			if (_audiLogo != null)
			{
				var color = _audiLogo.color;
				color.a = 0f;
				_audiLogo.color = color;
			}

			// Progress slider setup
			if (_progressSlider != null)
			{
				_progressSlider.minValue = 0f;
				_progressSlider.maxValue = WelcomeData.WELCOME_SCREEN_DURATION;
				_progressSlider.value = 0f;
			}
		}

		private IEnumerator WelcomeSequence()
		{
			// Logo fade in
			yield return StartCoroutine(FadeLogo(0f, 1f, 1f));

			// Timer principale
			while (_welcomeTimer < WelcomeData.WELCOME_SCREEN_DURATION && _isActive)
			{
				_welcomeTimer += Time.deltaTime;

				// Animazione logo semplice (fade in/out ogni secondo)
				if (Mathf.FloorToInt(_welcomeTimer) != Mathf.FloorToInt(_welcomeTimer - Time.deltaTime))
				{
					StartCoroutine(SimpleLogoPulse());
				}

				yield return null;
			}

			// Transizione automatica
			if (_isActive)
			{
				Debug.Log("[WELCOME SCREEN] ⏰ Timer completato - transizione automatica");
				TransitionToComfort();
			}
		}

		private IEnumerator FadeLogo(float fromAlpha, float toAlpha, float duration)
		{
			if (_audiLogo == null) yield break;

			float elapsed = 0f;
			Color color = _audiLogo.color;

			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				color.a = Mathf.Lerp(fromAlpha, toAlpha, elapsed / duration);
				_audiLogo.color = color;
				yield return null;
			}

			color.a = toAlpha;
			_audiLogo.color = color;
		}

		private IEnumerator SimpleLogoPulse()
		{
			// Pulse semplice - fade out e in rapidamente
			yield return StartCoroutine(FadeLogo(1f, 0.3f, 0.2f));
			yield return StartCoroutine(FadeLogo(0.3f, 1f, 0.2f));
		}

		private void TransitionToComfort()
		{
			if (!_isActive) return;
			_isActive = false;

			Debug.Log("[WELCOME SCREEN] 🔄 Transizione a Comfort Mode");
			_broadcaster.Broadcast(new WelcomeTransitionEvent(WelcomeData.COMFORT_MODE_STATE));

			StartCoroutine(FadeOutAndDestroy());
		}

		private IEnumerator FadeOutAndDestroy()
		{
			// Fade out canvas
			if (_welcomeCanvasGroup != null)
			{
				float elapsed = 0f;
				float duration = 1f;

				while (elapsed < duration)
				{
					elapsed += Time.deltaTime;
					_welcomeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
					yield return null;
				}
			}

			Debug.Log("[WELCOME SCREEN] 👋 Welcome Screen completato");
			Destroy(gameObject);
		}

		private void HandleInput()
		{
			// Debug keys per test rapidi
			if (Input.GetKeyDown(KeyCode.F1))
			{
				Debug.Log("[WELCOME SCREEN] F1 -> Eco Mode");
				_broadcaster.Broadcast(new WelcomeTransitionEvent(WelcomeData.ECO_MODE_STATE));
				StartCoroutine(FadeOutAndDestroy());
			}
			else if (Input.GetKeyDown(KeyCode.F2))
			{
				Debug.Log("[WELCOME SCREEN] F2 -> Comfort Mode");
				_broadcaster.Broadcast(new WelcomeTransitionEvent(WelcomeData.COMFORT_MODE_STATE));
				StartCoroutine(FadeOutAndDestroy());
			}
			else if (Input.GetKeyDown(KeyCode.F3))
			{
				Debug.Log("[WELCOME SCREEN] F3 -> Sport Mode");
				_broadcaster.Broadcast(new WelcomeTransitionEvent(WelcomeData.SPORT_MODE_STATE));
				StartCoroutine(FadeOutAndDestroy());
			}
		}
	}
}