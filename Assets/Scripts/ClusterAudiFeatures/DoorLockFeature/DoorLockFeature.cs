using ClusterAudi;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Implementazione DoorLock Feature - VERSIONE SEMPLICE
	/// IDENTICA al pattern ClockFeature.cs che hai appena implementato
	/// </summary>
	public class DoorLockFeature : BaseFeature, IDoorLockFeature, IDoorLockFeatureInternal
	{
		#region Private Fields

		/// <summary>
		/// Stato corrente door lock
		/// </summary>
		private bool _isLocked = DoorLockData.DEFAULT_LOCKED_STATE;

		#endregion

		#region Constructor - IDENTICO Mercedes

		/// <summary>
		/// Costruttore - IDENTICO al pattern Mercedes
		/// </summary>
		public DoorLockFeature(Client client) : base(client)
		{
			Debug.Log("[DOOR LOCK FEATURE] 🔒 DoorLockFeature inizializzata");
		}

		#endregion

		#region IDoorLockFeature Implementation - IDENTICO Mercedes

		/// <summary>
		/// Istanzia la DoorLock Feature - IDENTICO al pattern Mercedes
		/// </summary>
		public async Task InstantiateDoorLockFeature()
		{
			Debug.Log("[DOOR LOCK FEATURE] 🚀 Istanziazione DoorLock Feature...");

			try
			{
				// IDENTICO ClockFeature: usa AssetService per caricare prefab
				var doorLockInstance = await _assetService.InstantiateAsset<DoorLockBehaviour>(
					DoorLockData.DOOR_LOCK_PREFAB_PATH);

				if (doorLockInstance != null)
				{
					// IDENTICO ClockFeature: Initialize con this per dependency injection
					doorLockInstance.Initialize(this);
					Debug.Log("[DOOR LOCK FEATURE] ✅ DoorLock istanziato da prefab");
				}
				else
				{
					// IDENTICO ClockFeature: solo warning se prefab non trovato
					Debug.LogWarning("[DOOR LOCK FEATURE] ⚠️ Prefab non trovato: " + DoorLockData.DOOR_LOCK_PREFAB_PATH);
					Debug.LogError("[DOOR LOCK FEATURE] ❌ Crea il prefab DoorLockPrefab in Resources/DoorLock/");
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[DOOR LOCK FEATURE] ❌ Errore istanziazione: {ex.Message}");
				Debug.LogException(ex);
			}
		}

		/// <summary>
		/// Stato corrente locked
		/// </summary>
		public bool IsLocked => _isLocked;

		/// <summary>
		/// Toggle manuale lock/unlock
		/// </summary>
		public void ToggleLock()
		{
			bool previousState = _isLocked;
			_isLocked = !_isLocked;

			Debug.Log($"[DOOR LOCK FEATURE] 🔄 Door lock toggled: {previousState} → {_isLocked}");

			// Broadcast cambio stato
			_broadcaster.Broadcast(new DoorLockStateChangedEvent(_isLocked, previousState));

			// Richiedi audio tramite AudioFeature esistente
			RequestLockAudio(_isLocked);
		}

		#endregion

		#region IDoorLockFeatureInternal Implementation - IDENTICO Mercedes

		/// <summary>
		/// Ottiene il Client - IDENTICO al pattern Mercedes
		/// </summary>
		public Client GetClient()
		{
			return _client;
		}

		/// <summary>
		/// Stato locked per MonoBehaviour
		/// </summary>
		bool IDoorLockFeatureInternal.IsLocked => _isLocked;

		/// <summary>
		/// Imposta stato locked (chiamato da MonoBehaviour)
		/// </summary>
		public void SetLocked(bool locked)
		{
			if (_isLocked != locked)
			{
				bool previousState = _isLocked;
				_isLocked = locked;

				Debug.Log($"[DOOR LOCK FEATURE] 🔧 Door lock state set: {previousState} → {_isLocked}");

				// Broadcast cambio stato
				_broadcaster.Broadcast(new DoorLockStateChangedEvent(_isLocked, previousState));

				// Richiedi audio
				RequestLockAudio(_isLocked);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Richiede audio tramite la tua AudioFeature esistente
		/// </summary>
		private void RequestLockAudio(bool isLocking)
		{
			try
			{
				// Determina quale suono riprodurre
				string audioPath = isLocking ? DoorLockData.LOCK_SOUND_PATH : DoorLockData.UNLOCK_SOUND_PATH;

				Debug.Log($"[DOOR LOCK FEATURE] 🔊 Richiesta audio: {audioPath}");

				// Broadcast evento audio per la tua AudioFeature
				var audioEvent = new DoorLockAudioRequestEvent(
					audioPath,
					DoorLockData.DOOR_LOCK_VOLUME,
					DoorLockData.DOOR_LOCK_AUDIO_PRIORITY,
					isLocking);

				_broadcaster.Broadcast(audioEvent);

				// ALTERNATIVO: Se vuoi usare direttamente AudioFeature (opzionale)
				// var audioFeature = _client.Features.Get<IAudioFeature>();
				// audioFeature.PlayAudioClip(audioPath, DoorLockData.DOOR_LOCK_VOLUME, DoorLockData.DOOR_LOCK_AUDIO_PRIORITY);

			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[DOOR LOCK FEATURE] ❌ Errore richiesta audio: {ex.Message}");
			}
		}

		#endregion
	}
}