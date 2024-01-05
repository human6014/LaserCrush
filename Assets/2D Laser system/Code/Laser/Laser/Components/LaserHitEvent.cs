using System;

namespace LaserSystem2D
{
    public class LaserHitEvent
    {
        private readonly LaserActiveHits _activeHits;

        public LaserHitEvent(LaserActiveHits activeHits)
        {
            _activeHits = activeHits;
        }

        public void AddListener(Action<LaserHit> handler)
        {
            _activeHits.Hit += handler;
        }

        public void RemoveListener(Action<LaserHit> handler)
        {
            _activeHits.Hit -= handler;
        }
    }
}