using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Menus
{
    public class NotificationScript : MonoBehaviour
    {
        public  GameObject    notificationBackground;
        private Text          _notificationText;
        private Image         _backgroundImage;
        private RectTransform _rectTransform;

        private Sequence _animationSequence;

        public void Start()
        {
            _rectTransform     = notificationBackground.GetComponent<RectTransform>();
            _notificationText  = notificationBackground.GetComponentInChildren<Text>();
            _backgroundImage   = notificationBackground.GetComponent<Image>();
            _animationSequence = DOTween.Sequence();

            SetupAnimation();
            _animationSequence.Pause();


            // _rectTransform.DOAnchorPosX(-25f, 5f);

            // DisplayNotification("Text Notification");
        }
        private void SetupAnimation()
        {
            _animationSequence.Append(_backgroundImage.DOFade(1.0f, 1f));
            _animationSequence.Join(_notificationText.DOFade(1.0f, 1f));
            _animationSequence.Join(_rectTransform.DOShakePosition(2f, new Vector3(28.0f, 0.0f, 28.0f)));
            _animationSequence.Append(_backgroundImage.DOFade(0.0f, 1f));
            _animationSequence.Join(_notificationText.DOFade(0.0f, 1f));
            _animationSequence.SetAutoKill(false);
            // _animationSequence.Append(_notificationText.DOFade(0.0f, 1f));
            // _animationSequence.Join(_backgroundImage.DOFade(0.0f, 1f));
        }

        public void DisplayNotification(string notificationText)
        {
            this._notificationText.text = notificationText;
            _animationSequence.Restart();

        }
    }
}