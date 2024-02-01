using UnityEngine;

namespace LaserCrush.Manager
{
    public static class RayManager
    {
        public static readonly int s_AllObjectLayer =
            1 << LayerMask.NameToLayer("Reflectable") |
            1 << LayerMask.NameToLayer("Absorbable") |
            1 << LayerMask.NameToLayer("Continuable") |
            1 << LayerMask.NameToLayer("TouchableArea");

        public static readonly int s_LaserHitableLayer =
            1 << LayerMask.NameToLayer("Reflectable") |
            1 << LayerMask.NameToLayer("Absorbable") |
            1 << LayerMask.NameToLayer("Continuable");

        public static readonly int s_TouchableAreaLayer = 1 << LayerMask.NameToLayer("TouchableArea");
        public static readonly int s_InstalledItemLayer = 1 << LayerMask.NameToLayer("Continuable");

        public static Camera MainCamera { get; set; }

        #region PC
        public static bool RaycastToClickable(out RaycastHit2D hit, int layer)
        {
            hit = Physics2D.Raycast(MainCamera.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, Mathf.Infinity, layer);
            return hit.collider != null;
        }

        public static Vector3 MousePointToWorldPoint()
            => MainCamera.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
        #endregion

        #region Mobile
        public static bool RaycastToTouchable(out RaycastHit2D hit, int layer, Touch touch)
        {
            hit = Physics2D.Raycast(MainCamera.ScreenToWorldPoint(touch.position), Vector3.forward, Mathf.Infinity, layer);
            return hit.collider != null;
        }

        public static Vector3 TouchPointToWorldPoint(Touch touch)
            => MainCamera.ScreenToWorldPoint(touch.position) + new Vector3(0, 0, 10);
        #endregion
    }
}
