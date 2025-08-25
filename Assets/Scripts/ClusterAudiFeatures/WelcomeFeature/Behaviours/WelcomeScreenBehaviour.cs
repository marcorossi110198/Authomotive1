using ClusterAudi;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// MonoBehaviour per la schermata di benvenuto del cluster.
	/// Segue ESATTAMENTE il pattern dei MonoBehaviour del progetto Mercedes.
	/// 
	/// Funzionalità:
	/// - Logo animato (fade in + pulsazione + fade out)
	/// - Timer 5 secondi con transizione automatica
	/// - Debug keys F1-F4 per testing
	/// - Integration completa con WelcomeFeature
	/// </summary>
	public class WelcomeScreenBehaviour : BaseMonoBehaviour<IWelcomeFeatureInternal>
	{
		#region UI References

		[Header("Welcome Screen UI")]
		[SerializeField] private CanvasGroup _welcomeCanvasGroup;
		[SerializeField] private Image _audiLogo;
		[SerializeField] private TextMeshProUGUI _welcomeText;
		[SerializeField] private TextMeshProUGUI _timerText;
		[SerializeField] private Slider _progressSlider;

		#endregion

		#region Private Fields

		// Timer e stato
		private float _welcomeTimer = 0f;
		private bool _isWelcomeActive = true;
		private bool _isTransitioning = false;

		// Animation
		private Coroutine _logoAnimationCoroutine;
		private Coroutine _welcomeTimerCoroutine;

		// Servizi
		private IBroadcaster _broadcaster;
		private IVehicleDataService _vehicleDataService;

		#endregion

		#region BaseMonoBehaviour Override

		protected override void ManagedAwake()
		{
			Debug.Log("[WELCOME SCREEN] 🎉 WelcomeScreenBehaviour inizializzato");

			// Ottieni servizi dal client
			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();
			_vehicleDataService = client.Services.Get<IVehicleDataService>();

			// Setup UI iniziale
			SetupInitialUI();
		}

		protected override void ManagedStart()
		{
			Debug.Log("[WELCOME SCREEN] ▶️ Avvio Welcome Screen...");

			// Avvia sequenza di benvenuto
			StartWelcomeSequence();
		}

		protected override void ManagedUpdate()
		{
			// Gestione input debug
			HandleDebugInput();

			// Update timer UI
			UpdateTimerDisplay();
		}

		protected override void ManagedOnDestroy()
		{
			Debug.Log("[WELCOME SCREEN] 🗑️ WelcomeScreenBehaviour distrutto");

			// Cleanup coroutine
			if (_logoAnimationCoroutine != null)
			{
				StopCoroutine(_logoAnimationCoroutine);
			}

			if (_welcomeTimerCoroutine != null)
			{
				StopCoroutine(_welcomeTimerCoroutine);
			}
		}

		#endregion

		#region Welcome Sequence

		/// <summary>
		/// Setup iniziale dell'UI
		/// </summary>
		private void SetupInitialUI()
		{
			// Configura canvas group
			if (_welcomeCanvasGroup != null)
			{
				_welcomeCanvasGroup.alpha = 0f; // Inizia invisibile
			}

			// Configura logo
			if (_audiLogo != null)
			{
				_audiLogo.color = WelcomeData.AUDI_LOGO_COLOR;
				_audiLogo.transform.localScale = Vector3.one;
			}

			// Configura testi
			if (_welcomeText != null)
			{
				_welcomeText.text = WelcomeData.WELCOME_TEXT;
				_welcomeText.color = WelcomeData.WELCOME_TEXT_COLOR;
			}

			if (_timerText != null)
			{
				_timerText.color = WelcomeData.WELCOME_TEXT_COLOR;
			}

			// Configura progress slider
			if (_progressSlider != null)
			{
				_progressSlider.value = 0f;
				_progressSlider.maxValue = WelcomeData.WELCOME_SCREEN_DURATION;
			}

			Debug.Log("[WELCOME SCREEN] ✅ UI iniziale configurata");
		}

		/// <summary>
		/// Avvia la sequenza completa di benvenuto
		/// </summary>
		private void StartWelcomeSequence()
		{
			Debug.Log("[WELCOME SCREEN] 🎬 Inizio sequenza benvenuto");

			// Avvia animazione logo
			_logoAnimationCoroutine = StartCoroutine(LogoAnimationSequence());

			// Avvia timer benvenuto
			_welcomeTimerCoroutine = StartCoroutine(WelcomeTimerSequence());
		}

		#endregion

		#region Logo Animation

		/// <summary>
		/// Sequenza completa animazione logo: Fade In → Pulse → Fade Out
		/// </summary>
		private IEnumerator LogoAnimationSequence()
		{
			// 1. Fade In Canvas
			yield return StartCoroutine(FadeInCanvas());

			// 2. Fade In Logo
			yield return StartCoroutine(FadeInLogo());

			// 3. Logo Pulse Loop (per la durata del timer)
			float pulseStartTime = Time.time;
			while (_isWelcomeActive && !_isTransitioning)
			{
				yield return StartCoroutine(PulseLogo());

				// Esci se è passato troppo tempo o se stiamo transitioning
				if (Time.time - pulseStartTime > WelcomeData.WELCOME_SCREEN_DURATION)
					break;
			}

			// 4. Fade Out (se non già in transizione)
			if (!_isTransitioning)
			{
				yield return StartCoroutine(FadeOutLogo());
			}
		}

		/// <summary>
		/// Fade in del canvas principale
		/// </summary>
		private IEnumerator FadeInCanvas()
		{
			if (_welcomeCanvasGroup == null) yield break;

			float duration = 0.5f;
			float elapsedTime = 0f;

			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				float alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
				_welcomeCanvasGroup.alpha = alpha;
				yield return null;
			}

			_welcomeCanvasGroup.alpha = 1f;
		}

		/// <summary>
		/// Fade in del logo
		/// </summary>
		private IEnumerator FadeInLogo()
		{
			if (_audiLogo == null) yield break;

			float duration = WelcomeData.LOGO_FADE_IN_DURATION;
			float elapsedTime = 0f;
			Color originalColor = _audiLogo.color;

			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				float t = WelcomeData.LOGO_FADE_IN_CURVE.Evaluate(elapsedTime / duration);

				Color newColor = originalColor;
				newColor.a = Mathf.Lerp(0f, 1f, t);
				_audiLogo.color = newColor;

				yield return null;
			}

			Color finalColor = originalColor;
			finalColor.a = 1f;
			_audiLogo.color = finalColor;
		}

		/// <summary>
		/// Singola pulsazione del logo
		/// </summary>
		private IEnumerator PulseLogo()
		{
			if (_audiLogo == null) yield break;

			float duration = WelcomeData.LOGO_PULSE_DURATION;
			float elapsedTime = 0f;
			Vector3 originalScale = Vector3.one;

			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				float t = WelcomeData.LOGO_PULSE_CURVE.Evaluate(elapsedTime / duration);

				float scale = Mathf.Lerp(WelcomeData.LOGO_PULSE_MIN_SCALE, WelcomeData.LOGO_PULSE_MAX_SCALE, t);
				_audiLogo.transform.localScale = originalScale * scale;

				yield return null;
			}

			_audiLogo.transform.localScale = originalScale;
		}

		/// <summary>
		/// Fade out del logo
		/// </summary>
		private IEnumerator FadeOutLogo()
		{
			if (_audiLogo == null) yield break;

			float duration = WelcomeData.LOGO_FADE_OUT_DURATION;
			float elapsedTime = 0f;
			Color originalColor = _audiLogo.color;

			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				float t = WelcomeData.LOGO_FADE_OUT_CURVE.Evaluate(elapsedTime / duration);

				Color newColor = originalColor;
				newColor.a = Mathf.Lerp(1f, 0f, t);
				_audiLogo.color = newColor;

				yield return null;
			}

			Color finalColor = originalColor;
			finalColor.a = 0f;
			_audiLogo.color = finalColor;
		}

		#endregion

		#region Timer Management

		/// <summary>
		/// Gestisce il timer di 5 secondi e la transizione automatica
		/// </summary>
		private IEnumerator WelcomeTimerSequence()
		{
			_welcomeTimer = 0f;

			while (_welcomeTimer < WelcomeData.WELCOME_SCREEN_DURATION && _isWelcomeActive)
			{
				_welcomeTimer += Time.deltaTime;
				yield return null;
			}

			// Timer completato - transizione automatica
			if (_isWelcomeActive)
			{
				Debug.Log("[WELCOME SCREEN] ⏰ Timer completato - transizione a Comfort Mode");
				TransitionToComfortMode();
			}
		}

		/// <summary>
		/// Aggiorna il display del timer
		/// </summary>
		private void UpdateTimerDisplay()
		{
			if (!_isWelcomeActive) return;

			// Aggiorna timer text
			if (_timerText != null)
			{
				float remainingTime = WelcomeData.WELCOME_SCREEN_DURATION - _welcomeTimer;
				_timerText.text = $"Starting in {remainingTime:F1}s";
			}

			// Aggiorna progress slider
			if (_progressSlider != null)
			{
				_progressSlider.value = _welcomeTimer;
			}
		}

		#endregion

		#region Transitions

		/// <summary>
		/// Transizione automatica a Comfort Mode
		/// </summary>
		private void TransitionToComfortMode()
		{
			if (_isTransitioning) return;

			_isTransitioning = true;
			_isWelcomeActive = false;

			Debug.Log("[WELCOME SCREEN] 🔄 Transizione a Comfort Mode");

			// Broadcast evento transizione (per ora placeholder)
			_broadcaster.Broadcast(new WelcomeTransitionEvent(WelcomeData.COMFORT_MODE_STATE));

			// TODO: Quando avremo State Machine attiva, useremo:
			// var stateMachine = GetStateMachine();
			// stateMachine.GoTo(WelcomeData.COMFORT_MODE_STATE);

			// Per ora, distruggi questo GameObject dopo fade out
			StartCoroutine(DestroyAfterFadeOut());
		}

		/// <summary>
		/// Distrugge il GameObject dopo il fade out
		/// </summary>
		private IEnumerator DestroyAfterFadeOut()
		{
			// Fade out canvas
			if (_welcomeCanvasGroup != null)
			{
				float duration = 1f;
				float elapsedTime = 0f;

				while (elapsedTime < duration)
				{
					elapsedTime += Time.deltaTime;
					float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
					_welcomeCanvasGroup.alpha = alpha;
					yield return null;
				}
			}

			Debug.Log("[WELCOME SCREEN] 👋 Welcome Screen completato");

			// Distruggi GameObject
			Destroy(gameObject);
		}

		#endregion

		#region Debug Input

		/// <summary>
		/// Gestisce input debug per testing
		/// </summary>
		private void HandleDebugInput()
		{
			if (!_isWelcomeActive) return;

			// F1-F3: Transizione diretta alle modalità
			if (Input.GetKeyDown(WelcomeData.DEBUG_ECO_MODE_KEY))
			{
				Debug.Log("[WELCOME SCREEN] 🟢 Debug: Transizione a Eco Mode");
				TransitionToMode(WelcomeData.ECO_MODE_STATE);
			}
			else if (Input.GetKeyDown(WelcomeData.DEBUG_COMFORT_MODE_KEY))
			{
				Debug.Log("[WELCOME SCREEN] 🔵 Debug: Transizione a Comfort Mode");
				TransitionToMode(WelcomeData.COMFORT_MODE_STATE);
			}
			else if (Input.GetKeyDown(WelcomeData.DEBUG_SPORT_MODE_KEY))
			{
				Debug.Log("[WELCOME SCREEN] 🔴 Debug: Transizione a Sport Mode");
				TransitionToMode(WelcomeData.SPORT_MODE_STATE);
			}

			// ESCAPE: Skip welcome immediato
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Debug.Log("[WELCOME SCREEN] ⏭️ Skip Welcome Screen");
				TransitionToComfortMode();
			}
		}

		/// <summary>
		/// Transizione debug a modalità specifica
		/// </summary>
		private void TransitionToMode(string targetMode)
		{
			if (_isTransitioning) return;

			_isTransitioning = true;
			_isWelcomeActive = false;

			Debug.Log($"[WELCOME SCREEN] 🔄 Transizione debug a {targetMode}");

			// Broadcast evento transizione
			_broadcaster.Broadcast(new WelcomeTransitionEvent(targetMode));

			// TODO: Quando avremo State Machine attiva:
			// var stateMachine = GetStateMachine();
			// stateMachine.GoTo(targetMode);

			StartCoroutine(DestroyAfterFadeOut());
		}

		#endregion
	}

	#region Events

	/// <summary>
	/// Evento per transizioni dalla Welcome Screen
	/// </summary>
	public class WelcomeTransitionEvent
	{
		public string TargetState { get; }

		public WelcomeTransitionEvent(string targetState)
		{
			TargetState = targetState;
		}
	}

	#endregion
}