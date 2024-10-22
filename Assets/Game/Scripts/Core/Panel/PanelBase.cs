using System;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace Game.Scripts.Core.Panel
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    public abstract class PanelBase : MonoBehaviour
    {
        [SerializeField] private PanelAnimations _panelAnimations = new();
        public Canvas Canvas { get; private set; }
        public CanvasGroup CanvasGroup { get; private set; }
        public RectTransform RectTransform { get; private set; }

        private void Awake()
        {
            Canvas = GetComponent<Canvas>();
            CanvasGroup = GetComponent<CanvasGroup>();
            RectTransform = GetComponent<RectTransform>();
        }

        public async UniTask Show(bool isImmediate)
        {
            OnOpening();
            gameObject.SetActive(true);

            var openingAnimation = _panelAnimations.openingAnimation;
            if (openingAnimation == null)
            {
                OnOpened();
                return;
            }

            Tween.CompleteAll(transform);
            Tween.CompleteAll(CanvasGroup);

            if (isImmediate)
            {
                transform.localScale = openingAnimation.scaleSettings.endValue;
                CanvasGroup.alpha = openingAnimation.alphaSettings.endValue;
                OnOpened();
                return;
            }

            transform.localScale = openingAnimation.scaleSettings.startValue;
            CanvasGroup.alpha = openingAnimation.alphaSettings.startValue;

            var sequence = Sequence.Create()
                .Group(Tween.Alpha(CanvasGroup, openingAnimation.alphaSettings))
                .Group(Tween.Scale(transform, openingAnimation.scaleSettings));

            await sequence.ToUniTask();
            OnOpened();
        }

        public async UniTask Hide(bool isImmediate)
        {
            OnClosing();

            var closingAnimation = _panelAnimations.closingAnimation;
            if (closingAnimation == null)
            {
                gameObject.SetActive(false);
                OnClosed();
                return;
            }

            Tween.CompleteAll(transform);
            Tween.CompleteAll(CanvasGroup);

            if (isImmediate)
            {
                transform.localScale = closingAnimation.scaleSettings.endValue;
                CanvasGroup.alpha = closingAnimation.alphaSettings.endValue;
                gameObject.SetActive(false);
                OnClosed();
                return;
            }

            transform.localScale = closingAnimation.scaleSettings.startValue;
            CanvasGroup.alpha = closingAnimation.alphaSettings.startValue;

            var sequence = Sequence.Create()
                .Group(Tween.Alpha(CanvasGroup, closingAnimation.alphaSettings))
                .Group(Tween.Scale(transform, closingAnimation.scaleSettings));

            await sequence.ToUniTask();
            gameObject.SetActive(false);
            OnClosed();
        }

        protected virtual void OnOpening()
        {
        }

        protected virtual void OnOpened()
        {
        }

        protected virtual void OnClosing()
        {
        }

        protected virtual void OnClosed()
        {
        }
    }

    [Serializable]
    public class PanelAnimations
    {
        public PanelAnimationSettings openingAnimation;
        public PanelAnimationSettings closingAnimation;
    }
}