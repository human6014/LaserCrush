using UnityEngine;

namespace LaserSystem2D
{
    public class LaserLength
    {
        public float Fill { get; private set; }
        public float Current { get; private set; }

        public void SetToZero()
        {
            Fill = 0;
            Current = 0;
        }

        public void Update(float speed, float length, LaserRaycastResult raycastResult)
        {
            AlignLineLength(raycastResult);
            AddLength(speed, length);
        }

        private void AddLength(float speed, float length)
        {
            Current = Mathf.Clamp(Current + speed * Time.deltaTime, 0, length);
            Fill = Current / length;    
        }

        private void AlignLineLength(LaserRaycastResult raycastResult)
        {
            float previousDistance = 0;
            float distance = 0;
        
            for (int i = 0; i < raycastResult.Count; ++i)
            {
                AddHitDistance(ref raycastResult, i, ref previousDistance, ref distance);

                if (IsNewHit(ref raycastResult, i))
                {
                    AlignLineLength(previousDistance, distance);
                }
            }
        }

        private bool IsNewHit(ref LaserRaycastResult raycastResult, int hitIndex)
        {
            return raycastResult.PreviousHits[hitIndex].HitObject != raycastResult.Hits[hitIndex].HitObject ||
                   raycastResult.PreviousHits[hitIndex].Normal != raycastResult.Hits[hitIndex].Normal;
        }

        private void AlignLineLength(float previousDistance, float currentDistance)
        {
            if (Current >= previousDistance)
            {
                Current = previousDistance;
            }

            if (Current >= currentDistance)
            {
                Current = currentDistance;
            }
        }

        private void AddHitDistance(ref LaserRaycastResult raycastResult, int hitIndex, ref float previousDistance, ref float distance)
        {
            if (raycastResult.PreviousHits.Count <= hitIndex)
            {
                raycastResult.PreviousHits.Add(new LaserHit());
                raycastResult.PreviousHits[hitIndex].Update(raycastResult.Hits[hitIndex]);
            }
            
            previousDistance += raycastResult.PreviousHits[hitIndex].Distance;
            distance += raycastResult.Hits[hitIndex].Distance;
        }
    }
}