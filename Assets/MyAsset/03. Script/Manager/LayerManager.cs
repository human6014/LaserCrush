using UnityEngine;

namespace LaserCrush.Manager
{
    public static class LayerManager
    {
        public static readonly int s_LaserHitableLayer = 1 << LayerMask.NameToLayer("Reflectable") | 1 << LayerMask.NameToLayer("Absorbable") | 1 << LayerMask.NameToLayer("Continuable");
        public static readonly int s_TouchableAreaLayer = 1 << LayerMask.NameToLayer("TouchableArea");
    }
}
