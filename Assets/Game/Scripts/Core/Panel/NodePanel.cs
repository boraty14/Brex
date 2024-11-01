using System;
using Brui.Components;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace Game.Scripts.Core.Panel
{
    [RequireComponent(typeof(NodeCanvas))]
    public class NodePanel : MonoBehaviour
    {
        [SerializeField] private PanelAnimations _panelAnimations = new();
        private Tween _showTween;
        private Tween _hideTween;
        private NodeCanvas _nodeCanvas;

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
                _nodeCanvas.transform.localScale = openingAnimation.scaleSettings.endValue;
                OnOpened();
                return;
            }

            _nodeCanvas.transform.localScale = openingAnimation.scaleSettings.startValue;

            _showTween = Tween.Scale(_nodeCanvas.transform, openingAnimation.scaleSettings);
            await _showTween.ToUniTask();
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

            _hideTween.Complete();

            if (isImmediate)
            {
                _nodeCanvas.transform.localScale = closingAnimation.scaleSettings.endValue;
                gameObject.SetActive(false);
                OnClosed();
                return;
            }

            _nodeCanvas.transform.localScale = closingAnimation.scaleSettings.startValue;

            _hideTween = Tween.Scale(_nodeCanvas.transform, closingAnimation.scaleSettings);

            await _hideTween.ToUniTask();
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