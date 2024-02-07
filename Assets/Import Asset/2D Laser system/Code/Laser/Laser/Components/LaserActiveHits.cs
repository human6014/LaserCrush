using System;
using System.Collections.Generic;

namespace LaserSystem2D
{
    public class LaserActiveHits
    {
        private readonly float _hitCalibration = 0.01f;
        private float _hitDistanceSum;
        private float _currentLength;
        private int _currentHits;

        public Action<LaserHit> Hit;
        public int Value { get; private set; }

        public void Update(float currentLength, LaserRaycastResult raycastResult)
        {
            UpdateActiveHits(currentLength, ref raycastResult);
            UpdateHitEvent(raycastResult.Hits);
        }

        private void UpdateActiveHits(float currentLength, ref LaserRaycastResult raycastResult)
        {
            _hitDistanceSum = 0;
            _currentLength = currentLength; 
            Value = 0;

            for (int i = 0; i < raycastResult.Count && IsLaserReachedHitPoint(raycastResult.Hits[i].Distance); ++i)
            {
                _hitDistanceSum += raycastResult.Hits[i].Distance;
                ++Value;
            }
        }
    
        private void UpdateHitEvent(List<LaserHit> hits)
        {
            if (_currentHits != Value)
            {
                _currentHits = Value;
            
                if (Value != 0)
                {
                    Hit?.Invoke(hits[Value - 1]);
                }
            }
        }

        private bool IsLaserReachedHitPoint(float newHitDistance)
        {
            return _currentLength > _hitDistanceSum + newHitDistance - _hitCalibration;
        }
    }
}