using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Manager
{
    public static class LayerManager
    {
        public static readonly int s_LaserHitableLayer = 1 << LayerMask.NameToLayer("Reflectable") | 1 << LayerMask.NameToLayer("Absorbable");
        public static readonly int s_TouchableLayer = 1 << LayerMask.NameToLayer("Touchable");
    }
}
