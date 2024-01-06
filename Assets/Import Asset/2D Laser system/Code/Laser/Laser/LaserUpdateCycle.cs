using System.Collections.Generic;
using UnityEngine;

namespace LaserSystem2D
{
    public class LaserUpdateCycle
    {
        private readonly LaserComponents _components;
        private readonly MonoBehaviour _context;
        private readonly LaserData _data;

        public LaserUpdateCycle(LaserComponents components, LaserData data, MonoBehaviour context)
        {
            _components = components;
            _context = context;
            _data = data;
        }

        public void Enable(Transform laserPoint)
        {
            _components.TransformMapper.SetSource(laserPoint);
            _components.Length.SetToZero();    
            _components.Dissolve.SetToZero();
            _components.Audio.Start();
            _context.StopAllCoroutines();
        }

        public void Disable()
        {
            _components.HitEffect.Disable();
            _components.Audio.Stop();
            _components.Interaction.ExitAll();

            if (_context.isActiveAndEnabled)
            {
                _context.StartCoroutine(_components.Dissolve.Disappear(_data.DissolveTime));
            }
            else
            {
                _components.Dissolve.SetToZero();
                _components.Length.SetToZero();
            }
        }

        public void Update()
        {
            _components.TransformMapper.MapPositionAndRotation();
            LaserRaycastResult raycastResult = _components.Raycast.Shoot(_data.RaycastMask, _data.Line.MaxPoints, _data.NonHitDistance);
            _data.Line.Regenerate(raycastResult, _context.transform.position);
            _components.Length.Update(_data.ShootingSpeed, _data.Line.Length, raycastResult);
            _components.ActiveHits.Update(_components.Length.Current, raycastResult);
            _components.HitEffect.Update(raycastResult, _components.ActiveHits.Value);
            HashSet<Collider2D> hits = _components.Collision.BoxCast(_data.CollisionWidth, _data.CollisionPenetration, _data.CollisionMask, raycastResult);
            _components.Interaction.Update(hits);
        }

        public void UpdateView()
        {
            _components.View.Update(_data.SortingOrder);
        }
    }
}