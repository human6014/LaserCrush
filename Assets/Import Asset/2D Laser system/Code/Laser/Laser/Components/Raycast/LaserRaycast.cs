using System.Collections.Generic;
using UnityEngine;

namespace LaserSystem2D
{
    public class LaserRaycast
    {
        private readonly List<LaserHit> _previousHits = new List<LaserHit>();
        private readonly List<LaserHit> _hits = new List<LaserHit>();
        private float _nonHitLaserLength;
        private readonly Transform _transform;
        private bool _queriesStartInColliders;
        private int _depth;

        public LaserRaycast(Transform transform)
        {
            _transform = transform;
        }
    
        public LaserRaycastResult Shoot(LayerMask mask, int maxPoints, float nonHitDistance)
        {
            _depth = 0;
            _nonHitLaserLength = nonHitDistance;
        
            UpdatePreviousHits();
            ShootRecursive(mask, maxPoints);

            return new LaserRaycastResult(_hits, _depth, _previousHits);
        }

        private void UpdatePreviousHits()
        {
            for (int i = 0; i < _hits.Count; ++i)
            {
                if (_previousHits.Count <= i)
                {
                    _previousHits.Add(new LaserHit());
                }
            
                _previousHits[i].Update(_hits[i]);
            }
        }

        private void ShootRecursive(LayerMask mask, int maxPoints)
        {
            _queriesStartInColliders = Physics2D.queriesStartInColliders;
            Physics2D.queriesStartInColliders = false;
        
            ShootRecursive(_transform.position, _transform.right, mask, maxPoints);

            Physics2D.queriesStartInColliders = _queriesStartInColliders;
        }

        private void ShootRecursive(Vector2 startPoint, Vector2 direction, LayerMask mask, int maxPoints)
        {
            RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, _nonHitLaserLength, mask);
        
            if (hit.collider != null)
            {
                HandleHit(startPoint, direction, ref hit);
            
                if (LaserManager.Instance.ReflectingColliders.Contains(hit.collider) && _depth < maxPoints)
                {
                    ShootRecursive(hit.point, _hits[_depth - 1].ReflectedDirection, mask, maxPoints);
                }
            }
            else
            {
                hit.point = startPoint + direction * _nonHitLaserLength;
                HandleHit(startPoint, direction, ref hit);
            }
        }

        private void HandleHit(Vector2 startPoint, Vector2 direction, ref RaycastHit2D hit)
        {
            while (_hits.Count <= _depth)
            {
                _hits.Add(new LaserHit());
            }
        
            _hits[_depth].Update(ref hit, startPoint, direction);
            ++_depth;
        }
    }
}