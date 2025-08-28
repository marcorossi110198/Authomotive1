using ClusterAudi;
using UnityEngine;
using UnityEngine.UI;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// MonoBehaviour per DoorLock Display - CON CLICK
	/// IDENTICO al pattern ClockDisplayBehaviour che hai appena implementato
	/// USA SOLO COMPONENTI ASSEGNATI NEL PREFAB + CLICK HANDLER
	/// </summary>
	public class DoorLockBehaviour : BaseMonoBehaviour<IDoorLockFeatureInternal>
	{
		#region Serialized Fields - DA ASSEGNARE NEL PREFAB

		[Header("DoorLock UI - ASSIGN IN PREFAB")]
		[SerializeField] private Button _lockButton;
		[SerializeField] private Image _lockImage;

		#endregion

		#region Private Fields

		// Servizi
		private IBroadcaster _broadcaster;

		// Immagini cached
		private Sprite _lockedSprite;
		private Sprite _unlockedSprite;
		private bool _imagesLoaded = false;

		#endregion

		#region BaseMonoBehaviour Override

		protected override void ManagedAwake()
		{
			Debug.Log("[DOOR LOCK] 🔒 DoorLockBehaviour inizializzato");

			// Ottieni servizi
			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();

			// Valida componenti del prefab
			ValidateUIComponents();

			// Carica immagini
			LoadLockImages();

			// Setup UI
			SetupLockDisplay();

			// Setup eventi
			SubscribeToEvents();
		}

		protected override void ManagedStart()
		{
			Debug.Log("[DOOR LOCK] ▶️ DoorLock avviato");

			// Aggiorna display con stato corrente
			UpdateLockDisplay();
		}

		protected override void ManagedUpdate()
		{
			// Gestione input tastiera
			HandleKeyboardInput();
		}

		protected override void ManagedOnDestroy()
		{
			Debug.Log("[DOOR LOCK] 🗑️ DoorLock distrutto");

			// Cleanup eventi
			UnsubscribeFromEvents();
		}

		#endregion

		#region UI Setup - VERSIONE PREFAB

		/// <summary>
		/// Valida che tutti i SerializeField siano assegnati nel prefab
		/// IDENTICO al pattern ClockDisplayBehaviour
		/// </summary>
		private void ValidateUIComponents()
		{
			int missingComponents = 0;

			if (_lockButton == null)
			{
				Debug.LogError("[DOOR LOCK] ❌ _lockButton non assegnato nel prefab!");
				missingComponents++;
			}

			if (_lockImage == null)
			{
				Debug.LogError("[DOOR LOCK] ❌ _lockImage non assegnato nel prefab!");
				missingComponents++;
			}

			if (missingComponents == 0)
			{
				Debug.Log("[DOOR LOCK] ✅ Tutti i SerializeField assegnati correttamente");
			}
			else
			{
				Debug.LogError($"[DOOR LOCK] ❌ {missingComponents} componenti mancanti! Configura il prefab DoorLockPrefab");
			}
		}

		/// <summary>
		/// Carica le immagini locked/unlocked da Resources
		/// </summary>
		private void LoadLockImages()
		{
			Debug.Log("[DOOR LOCK] 🖼️ Caricamento immagini lock...");

			try
			{
				_lockedSprite = Resources.Load<Sprite>(DoorLockData.LOCKED_IMAGE_PATH);
				_unlockedSprite = Resources.Load<Sprite>(DoorLockData.UNLOCKED_IMAGE_PATH);

				if (_lockedSprite != null && _unlockedSprite != null)
				{
					_imagesLoaded = true;
					Debug.Log("[DOOR LOCK] ✅ Immagini lock caricate con successo");
				}
				else
				{
					Debug.LogError("[DOOR LOCK] ❌ Impossibile caricare immagini lock da Resources/DoorLock/img/");
					LogMissingImages();
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[DOOR LOCK] ❌ Errore caricamento immagini: {ex.Message}");
				_imagesLoaded = false;
			}
		}

		/// <summary>
		/// Log dettagliato per immagini mancanti
		/// </summary>
		private void LogMissingImages()
		{
			if (_lockedSprite == null)
				Debug.LogError("[DOOR LOCK] ❌ LucchettoCHIUSO.png non trovato in Resources/DoorLock/img/");

			if (_unlockedSprite == null)
				Debug.LogError("[DOOR LOCK] ❌ LucchettoAPERTO.png non trovato in Resources/DoorLock/img/");
		}

		/// <summary>
		/// Setup iniziale del display door lock
		/// </summary>
		private void SetupLockDisplay()
		{
			if (_lockButton == null || _lockImage == null)
			{
				Debug.LogError("[DOOR LOCK] ❌ Componenti UI non configurati nel prefab!");
				return;
			}

			// Setup click handler
			_lockButton.onClick.AddListener(OnLockButtonClicked);

			Debug.Log("[DOOR LOCK] ✅ Lock display configurato da prefab");
		}

		#endregion

		#region Event System

		/// <summary>
		/// Sottoscrizione agli eventi
		/// </summary>
		private void SubscribeToEvents()
		{
			_broadcaster.Add<DoorLockStateChangedEvent>(OnLockStateChanged);
		}

		/// <summary>
		/// Rimozione sottoscrizioni eventi
		/// </summary>
		private void UnsubscribeFromEvents()
		{
			_broadcaster.Remove<DoorLockStateChangedEvent>(OnLockStateChanged);
		}

		/// <summary>
		/// Handler cambio stato lock
		/// </summary>
		private void OnLockStateChanged(DoorLockStateChangedEvent e)
		{
			Debug.Log($"[DOOR LOCK] 🔄 Stato cambiato: {e.PreviousState} → {e.IsLocked}");
			UpdateLockDisplay();
		}

		#endregion

		#region Input Handling

		/// <summary>
		/// Gestisce input da tastiera - PATTERN IDENTICO agli stati FSM
		/// </summary>
		private void HandleKeyboardInput()
		{
			// L = Toggle Door Lock
			if (Input.GetKeyDown(DoorLockData.DOOR_LOCK_TOGGLE_KEY))
			{
				Debug.Log("[DOOR LOCK] ⌨️ Tasto L premuto - Toggle door lock");
				OnLockButtonClicked(); // Usa stesso handler del click
			}
		}

		#endregion

		#region Click Handler

		/// <summary>
		/// Handler click bottone lock - QUESTO È IL CUORE DELLA FEATURE
		/// </summary>
		private void OnLockButtonClicked()
		{
			Debug.Log("[DOOR LOCK] 🖱️ Click su lock button");

			// Toggle stato tramite feature
			var currentState = _feature.IsLocked;
			_feature.SetLocked(!currentState);

			Debug.Log($"[DOOR LOCK] 🔄 Toggle richiesto: {currentState} → {!currentState}");
		}

		#endregion

		#region Display Update

		/// <summary>
		/// Aggiorna il display del lock
		/// </summary>
		private void UpdateLockDisplay()
		{
			if (_lockImage == null || !_imagesLoaded)
			{
				Debug.LogWarning("[DOOR LOCK] ⚠️ Impossibile aggiornare display: componenti mancanti");
				return;
			}

			// Ottieni stato corrente dalla feature
			bool isLocked = _feature.IsLocked;

			// Aggiorna immagine
			_lockImage.sprite = isLocked ? _lockedSprite : _unlockedSprite;

			// Scala immagine se necessario
			_lockImage.transform.localScale = Vector3.one * DoorLockData.LOCK_IMAGE_SCALE;

			Debug.Log($"[DOOR LOCK] 🖼️ Display aggiornato: {(isLocked ? "LOCKED" : "UNLOCKED")}");
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Toggle manuale (per debug/testing)
		/// </summary>
		public void ToggleLock()
		{
			OnLockButtonClicked();
		}

		/// <summary>
		/// Forza aggiornamento display
		/// </summary>
		public void ForceUpdate()
		{
			UpdateLockDisplay();
		}

		#endregion
	}
}