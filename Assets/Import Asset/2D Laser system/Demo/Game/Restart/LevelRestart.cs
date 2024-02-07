using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LaserSystem2D
{
    public class LevelRestart : MonoBehaviour
    {
        private IEnumerable<IRestart> _restarts;
    
        private void Start()
        {
            _restarts = FindObjectsOfType<MonoBehaviour>().OfType<IRestart>();
        }

        public void RestartForEach()
        {
            foreach (IRestart restartableObject in _restarts)
            {
                restartableObject.Restart();
            }
        }
    }
}