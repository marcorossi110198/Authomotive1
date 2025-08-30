using ClusterAudi;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudiFeatures
{
	public class DoorLockFeature : BaseFeature, IDoorLockFeature, IDoorLockFeatureInternal
	{
		private bool _isLocked = false; // Default unlocked

		public DoorLockFeature(Client client) : base(client) { }

		public async Task InstantiateDoorLockFeature()
		{
			try
			{
				var instance = await _assetService.InstantiateAsset<DoorLockBehaviour>("DoorLock/DoorLockPrefab");
				instance?.Initialize(this);
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[DOOR LOCK] Error: {ex.Message}");
			}
		}

		public bool IsLocked => _isLocked;

		public void ToggleLock() => SetLocked(!_isLocked);

		public Client GetClient() => _client;

		public void SetLocked(bool locked)
		{
			if (_isLocked == locked) return;

			bool previous = _isLocked;
			_isLocked = locked;

			_broadcaster.Broadcast(new DoorLockStateChangedEvent(_isLocked, previous));

			// Request audio
			string audioPath = locked ? "Audio/SFX/DoorLock/LockSound" : "Audio/SFX/DoorLock/UnlockSound";
			_broadcaster.Broadcast(new DoorLockAudioRequestEvent(audioPath, 0.8f, 3, locked));
		}
	}
}