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
	/// - Debug keys ESC per testing
	/// - Integration completa con WelcomeFeature
	/// - USA SOLO COMPONENTI DEL PREFAB
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

			// Update progress slider (SOLO la progress bar viene aggiornata dal codice)
			UpdateProgressSlider();
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
		/// Setup iniziale dell'UI - VERSIONE SOLO PREFAB
		/// </summary>
		private void SetupInitialUI()
		{
			Debug.Log("[WELCOME SCREEN] 🔧 Setup UI da prefab...");

			// 1. OTTIENI CANVAS GROUP (dovrebbe essere sul root Canvas del prefab)
			_welcomeCanvasGroup = GetComponentInParent<CanvasGroup>();
			if (_welcomeCanvasGroup == null)
			{
				Debug.LogWarning("[WELCOME SCREEN] ⚠️ CanvasGroup non trovato sul Canvas root");
				// Cerca in children o aggiungi al Canvas parent
				var canvas = GetComponentInParent<Canvas>();
				if (canvas != null)
				{
					_welcomeCanvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
				}
			}

			if (_welcomeCanvasGroup != null)
			{
				_welcomeCanvasGroup.alpha = 1f; // Inizia visibile
				Debug.Log("[WELCOME SCREEN] ✅ CanvasGroup configurato");
			}

			// 2. VERIFICA COMPONENTI PREFAB
			ValidateUIComponents();

			// 3. CONFIGURA COMPONENTI (solo valori iniziali, NON modifica testi)
			ConfigureUIComponents();

			Debug.Log("[WELCOME SCREEN] ✅ UI configurata da prefab");
		}

		/// <summary>
		/// Verifica che tutti i SerializeField siano assegnati nel prefab
		/// </summary>
		private void ValidateUIComponents()
		{
			int missingComponents = 0;

			if (_audiLogo == null)
			{
				Debug.LogError("[WELCOME SCREEN] ❌ _audiLogo non assegnato nell'Inspector!");
				missingComponents++;
			}

			if (_welcomeText == null)
			{
				Debug.LogError("[WELCOME SCREEN] ❌ _welcomeText non assegnato nell'Inspector!");
				missingComponents++;
			}

			if (_timerText == null)
			{
				Debug.LogError("[WELCOME SCREEN] ❌ _timerText non assegnato nell'Inspector!");
				missingComponents++;
			}

			if (_progressSlider == null)
			{
				Debug.LogError("[WELCOME SCREEN] ❌ _progressSlider non assegnato nell'Inspector!");
				missingComponents++;
			}

			if (missingComponents == 0)
			{
				Debug.Log("[WELCOME SCREEN] ✅ Tutti i SerializeField assegnati correttamente");
			}
			else
			{
				Debug.LogError($"[WELCOME SCREEN] ❌ {missingComponents} componenti non assegnati! Configura il prefab.");
			}
		}

		/// <summary>
		/// Configura valori iniziali dei componenti
		/// NON modifica testi - mantiene tutto dal prefab
		/// </summary>
		private void ConfigureUIComponents()
		{
			// Logo: inizia trasparente per animazione fade-in
			if (_audiLogo != null)
			{
				var logoColor = _audiLogo.color;
				logoColor.a = 0f; // Trasparente per fade-in
				_audiLogo.color = logoColor;
				_audiLogo.transform.localScale = Vector3.one;
				Debug.Log("[WELCOME SCREEN] ✅ AudiLogo configurato");
			}

			// Welcome Text: MANTIENI COMPLETAMENTE dal prefab
			if (_welcomeText != null)
			{
				Debug.Log($"[WELCOME SCREEN] ✅ WelcomeText: '{_welcomeText.text}' (dal prefab)");
			}

			// Timer Text: MANTIENI COMPLETAMENTE dal prefab
			if (_timerText != null)
			{
				Debug.Log($"[WELCOME SCREEN] ✅ TimerText: '{_timerText.text}' (dal prefab)");
			}

			// Progress Slider: SOLO configurazione valori, mantieni stile da prefab
			if (_progressSlider != null)
			{
				_progressSlider.minValue = 0f;
				_progressSlider.maxValue = WelcomeData.WELCOME_SCREEN_DURATION;
				_progressSlider.value = 0f;
				_progressSlider.interactable = false;
				Debug.Log("[WELCOME SCREEN] ✅ ProgressSlider configurato");
			}
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
			// yield return StartCoroutine(FadeInCanvas());

			// 2. Fade In Logo iniziale
			yield return StartCoroutine(FadeInLogo());

			// 3. Logo Fade In/Out Loop (NUOVO - sostituisce pulse)
			float loopStartTime = Time.time;
			while (_isWelcomeActive && !_isTransitioning)
			{
				// Fade In/Out rapido e graduale
				yield return StartCoroutine(FadeLogoInOut());

				// Pausa breve tra i cicli
				yield return new WaitForSeconds(0.2f);

				// Esci se è passato troppo tempo o se stiamo transitioning
				if (Time.time - loopStartTime > WelcomeData.WELCOME_SCREEN_DURATION - 1f)
					break;
			}

			// 4. Fade Out finale (se non già in transizione)
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

		/// Fade in del logo iniziale - INVARIATO (già perfetto)
		/// </summary>
		private IEnumerator FadeInLogo()
		{
			if (_audiLogo == null) yield break;

			float duration = 1.0f;
			float elapsedTime = 0f;
			Color originalColor = _audiLogo.color;

			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				// Curve easing per smooth fade in
				float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);

				Color newColor = originalColor;
				newColor.a = Mathf.Lerp(0f, 1f, t);
				_audiLogo.color = newColor;

				yield return null;
			}

			Color finalColor = originalColor;
			finalColor.a = 1f;
			_audiLogo.color = finalColor;
		}

		// <summary>
		/// NUOVA: Fade In/Out ciclico rapido del logo - sostituisce PulseLogo()
		/// Effetto: Logo scompare e riappare gradualmente e velocemente
		/// </summary>
		private IEnumerator FadeLogoInOut()
		{
			if (_audiLogo == null) yield break;

			Color originalColor = _audiLogo.color;

			// FASE 1: Fade Out rapido (0.4 secondi)
			float fadeOutDuration = 1f;
			float elapsedTime = 0f;

			while (elapsedTime < fadeOutDuration)
			{
				elapsedTime += Time.deltaTime;
				// Easing out per fade smooth
				float t = EaseOut(elapsedTime / fadeOutDuration);

				Color newColor = originalColor;
				newColor.a = Mathf.Lerp(1f, 0.15f, t); // Non completamente trasparente
				_audiLogo.color = newColor;

				yield return null;
			}

			// FASE 2: Breve pausa al minimo alpha
			yield return new WaitForSeconds(0.1f);

			// FASE 3: Fade In rapido (0.4 secondi) 
			float fadeInDuration = 0.4f;
			elapsedTime = 0f;

			while (elapsedTime < fadeInDuration)
			{
				elapsedTime += Time.deltaTime;
				// Easing in per fade smooth
				float t = EaseIn(elapsedTime / fadeInDuration);

				Color newColor = originalColor;
				newColor.a = Mathf.Lerp(0.15f, 1f, t);
				_audiLogo.color = newColor;

				yield return null;
			}

			// Ripristina colore originale
			_audiLogo.color = originalColor;
		}

		/// <summary>
		/// Fade out finale del logo - MIGLIORATO con easing
		/// </summary>
		private IEnumerator FadeOutLogo()
		{
			if (_audiLogo == null) yield break;

			float duration = 0.8f;
			float elapsedTime = 0f;
			Color originalColor = _audiLogo.color;

			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				// Easing out per smooth fade out finale
				float t = EaseOut(elapsedTime / duration);

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
		/// Aggiorna SOLO la progress bar (NON modifica i testi)
		/// </summary>
		private void UpdateProgressSlider()
		{
			if (!_isWelcomeActive) return;

			// Aggiorna SOLO progress slider (i testi rimangono dal prefab)
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

			Debug.Log("[WELCOME SCREEN] 🔄 Transizione a Comfort Mode");

			// Stoppa coroutine prima di transizionare
			StopAllWelcomeCoroutines();

			_isTransitioning = true;
			_isWelcomeActive = false;

			// Broadcast evento transizione
			_broadcaster.Broadcast(new WelcomeTransitionEvent(WelcomeData.COMFORT_MODE_STATE));

			// Fade out normale per transizione automatica
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
		/// Gestisce input debug per testing - SOLO ESC PERMESSO
		/// </summary>
		private void HandleDebugInput()
		{
			if (!_isWelcomeActive || _isTransitioning) return;

		}

		/// <summary>
		/// Stoppa tutte le coroutine del Welcome Screen
		/// </summary>
		private void StopAllWelcomeCoroutines()
		{
			// Stoppa animazione logo se attiva
			if (_logoAnimationCoroutine != null)
			{
				StopCoroutine(_logoAnimationCoroutine);
				_logoAnimationCoroutine = null;
			}

			// Stoppa timer welcome se attivo
			if (_welcomeTimerCoroutine != null)
			{
				StopCoroutine(_welcomeTimerCoroutine);
				_welcomeTimerCoroutine = null;
			}

			Debug.Log("[WELCOME SCREEN] 🛑 Tutte le coroutine Welcome stoppate");
		}

		#endregion

		#region Animation Easing Functions - NUOVE

		/// <summary>
		/// Funzione easing per smooth fade in
		/// Crea un effetto più graduale all'inizio
		/// </summary>
		private float EaseIn(float t)
		{
			return t * t;
		}

		/// <summary>
		/// Funzione easing per smooth fade out  
		/// Crea un effetto più graduale alla fine
		/// </summary>
		private float EaseOut(float t)
		{
			return 1 - Mathf.Pow(1 - t, 2);
		}

		/// <summary>
		/// Funzione easing per smooth fade in-out
		/// Combinazione di ease in e ease out per effetto bilanciato
		/// </summary>
		private float EaseInOut(float t)
		{
			if (t < 0.5f)
				return 2 * t * t;
			else
				return 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
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