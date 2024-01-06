using UnityEngine;
using System.Collections;

namespace LaserSystem2D
{
    public class FPSCounter : MonoBehaviour
    {
        private float _count;
    
        private IEnumerator Start()
        {
            GUI.depth = 2;
            while (true)
            {
                _count = 1f / Time.unscaledDeltaTime;
                yield return new WaitForSeconds(0.1f);
            }
        }
    
        private void OnGUI()
        {
            GUI.Label(new Rect(5, 40, 100, 25), "FPS: " + Mathf.Round(_count));
        }
    }
}