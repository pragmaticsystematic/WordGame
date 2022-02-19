using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Menus
{
    /// <summary>
    /// This class handles the pop up notifications shown to the user
    /// </summary>
    public class NotificationScript : MonoBehaviour
    {
        // References defined in the Unity Editor

        [Tooltip("Reference to the Notification Background Image GUI element")]
        public GameObject notificationBackground;


        // Fields
        private Text          _notificationText;
        private Image         _backgroundImage;
        private RectTransform _rectTransform;
        private Sequence      _animationSequence;

        //This function is used instead of the constructor. It's called when the script is loaded.
        public void Start()
        {
            _rectTransform     = notificationBackground.GetComponent<RectTransform>();
            _notificationText  = notificationBackground.GetComponentInChildren<Text>();
            _backgroundImage   = notificationBackground.GetComponent<Image>();
            _animationSequence = DOTween.Sequence();

            SetupAnimation();
            _animationSequence.Pause(); //prevents the animation from playing right away.
        }


        /// <summary>
        /// Displays a head's up notification with the given text. The notification fades in stays for about 2 seconds
        /// and fades out.
        /// </summary>
        /// <param name="notificationText">The text the notification will contain.</param>
        public void DisplayNotification(string notificationText)
        {
            this._notificationText.text = notificationText;
            _animationSequence.Restart();
        }

        private void SetupAnimation()
        {
            /*
             * Defines the notification shaking and fade-in, fade-out animation.
             * 'Append' adds an animation after the current animation while 'Join' adds and animation at the same time
             * as the current animation.
             *
             * This sequence fades in the background and the text for 1 second (changes the color alpha from 0.0f to 1.0f
             * then it shakes the root of the notifications for 2 seconds. And then fades out the text and the background
             * by setting their alpha from 1.0f to 0.0f.
             */

            _animationSequence.Append(_backgroundImage.DOFade(1.0f, 1f));
            _animationSequence.Join(_notificationText.DOFade(1.0f, 1f));
            _animationSequence.Join(_rectTransform.DOShakePosition(2f, new Vector3(28.0f, 0.0f, 28.0f)));
            _animationSequence.Append(_backgroundImage.DOFade(0.0f, 1f));
            _animationSequence.Join(_notificationText.DOFade(0.0f, 1f));
            _animationSequence.SetAutoKill(false);
        }
    }
}