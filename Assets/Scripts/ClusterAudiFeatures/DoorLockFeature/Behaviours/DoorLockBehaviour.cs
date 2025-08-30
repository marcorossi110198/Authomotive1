using ClusterAudi;
using UnityEngine;
using UnityEngine.UI;

namespace ClusterAudiFeatures
{
	public class DoorLockBehaviour : BaseMonoBehaviour<IDoorLockFeatureInternal>
	{
		[SerializeField] private Button _lockButton;
		[SerializeField] private Image _lockImage;

		private IBroadcaster _broadcaster;
		private Sprite _lockedSprite;
		private Sprite _unlockedSprite;

		protected override void ManagedAwake()
		{
			_broadcaster = _feature.GetClient().Services.Get<IBroadcaster>();
			LoadSprites();
			SetupButton();
			_broadcaster.Add<DoorLockStateChangedEvent>(OnStateChanged);
		}

		protected override void ManagedUpdate()
		{
			if (Input.GetKeyDown(KeyCode.L))
				_feature.ToggleLock();
		}

		protected override void ManagedOnDestroy()
		{
			_broadcaster?.Remove<DoorLockStateChangedEvent>(OnStateChanged);
		}

		private void LoadSprites()
		{
			_lockedSprite = Resources.Load<Sprite>(DoorLockData.LOCKED_IMAGE_PATH);
			_unlockedSprite = Resources.Load<Sprite>(DoorLockData.UNLOCKED_IMAGE_PATH);
		}

		private void SetupButton()
		{
			_lockButton?.onClick.AddListener(() => _feature.ToggleLock());
			UpdateDisplay();
		}

		private void OnStateChanged(DoorLockStateChangedEvent e) => UpdateDisplay();

		private void UpdateDisplay()
		{
			if (_lockImage && _lockedSprite && _unlockedSprite)
				_lockImage.sprite = _feature.IsLocked ? _lockedSprite : _unlockedSprite;
		}
	}
}