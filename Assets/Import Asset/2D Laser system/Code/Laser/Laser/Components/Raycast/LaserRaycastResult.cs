using System.Collections.Generic;

namespace LaserSystem2D
{
    public readonly struct LaserRaycastResult 
    {
        public readonly List<LaserHit> PreviousHits;
        public readonly List<LaserHit> Hits;
        public readonly int Count;

        public LaserRaycastResult(List<LaserHit> hits, int count, List<LaserHit> previousHits)
        {
            Hits = hits;
            Count = count;
            PreviousHits = previousHits;
        }
    }
}