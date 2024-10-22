using PrimeTween;
using UnityEngine;

namespace Game.Scripts.Core.Panel
{
    [CreateAssetMenu(menuName = "Game/Settings/Panel Animation Settings", fileName = "PanelAnimationSettings")]
    public class PanelAnimationSettings : ScriptableObject
    {
        public TweenSettings<float> alphaSettings;
        public TweenSettings<Vector3> scaleSettings;
    }
}