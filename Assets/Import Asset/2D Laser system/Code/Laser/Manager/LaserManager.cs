using System;
using UnityEngine;

namespace LaserSystem2D
{
    [DefaultExecutionOrder(-10)]
    public class LaserManager : MonoBehaviour
    {
        public readonly ReflectingColliders ReflectingColliders = new ReflectingColliders();
        public readonly HitEffectsPool HitPool = new HitEffectsPool();
        public readonly LaserPool LaserPool = new LaserPool();
        private static LaserManager _instance;
        
        public static LaserManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new Exception("Laser manager wasn't initiated");
                }

                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this;
        }

        private void Update()
        {
            LaserPool.Update();
            HitPool.Update();
        }
    }
}