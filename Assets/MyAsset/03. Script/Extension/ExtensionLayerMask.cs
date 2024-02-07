using UnityEngine;

namespace LaserCrush.Extension
{
    public static class ExtensionLayerMask
    {
        public static int GetLayerNumber(this LayerMask layerMask)
        {
            if (layerMask.value == 0)
            {
                Debug.LogWarning("Layer non");
                return 0;
            }
            return Mathf.RoundToInt(Mathf.Log(layerMask.value, 2));
        }
    }
}
