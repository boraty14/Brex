using System;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace Game.Scripts.Core.Panel
{
    public abstract class PanelBase : MonoBehaviour
    {
        [SerializeField] private PanelAnimations _panelAnimations = new();
        private Tween _showTween;
        private Tween _hideTween;
        
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

            _showTween.Complete();

            if (isImmediate)
            {
                transform.localScale = openingAnimation.scaleSettings.endValue;
                OnOpened();
                return;
            }

            transform.localScale = openingAnimation.scaleSettings.startValue;

            var sequence = Sequence.Create()
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

            if (isImmediate)
            {
                transform.localScale = closingAnimation.scaleSettings.endValue;
                gameObject.SetActive(false);
                OnClosed();
                return;
            }

            transform.localScale = closingAnimation.scaleSettings.startValue;

            var sequence = Sequence.Create()
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