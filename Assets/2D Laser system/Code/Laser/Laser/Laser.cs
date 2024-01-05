using System;
using UnityEngine;

namespace LaserSystem2D
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class Laser : PoolableMonoBehaviour
    {
        [SerializeField] private LaserData _data;
        private LaserUpdateCycle _updateCycle;
        private LaserComponents _components;
        private bool _enabled;
    
        public Guid Id { get; private set; } = Guid.NewGuid();
        public override bool IsActive { get { TryInitialize(); return _components.Dissolve.IsAnimating || _enabled; } }
        public LaserHitEvent HitEvent { get { TryInitialize(); return _components.HitEvent; } }
        public LaserData Data => _data;
    
        public void BranchLaser(Laser parent)
        {
            TryInitialize();
            Id = parent.Id;
            _components.ActiveHits.Hit = parent._components.ActiveHits.Hit;
        }
    
        private void Start()
        {
            TryInitialize();
        }
    
        private void TryInitialize()
        {
            if (_components == null)
            {
                _data.Line.Initialize(this);
                _components = new LaserComponents(_data, this);
                _updateCycle = new LaserUpdateCycle(_components, _data, this);
            }
        }
    
        private void OnDisable()
        {
            Disable();
        }
    
        public void Enable()
        {
            Enable(transform);
        }
    
        public void Enable(Transform laserPoint)
        {
            TryInitialize();
    
            if (_enabled == false)
            {
                _enabled = true;
                _updateCycle.Enable(laserPoint);
            }
        }
    
        public void Disable()
        {
            TryInitialize();
            
            if (_enabled)
            {
                _enabled = false;
                _updateCycle.Disable();
            }
        }
    
        private void Update()
        {
            if (_enabled)
            {
                _updateCycle.Update();
            }
    
            _updateCycle.UpdateView();
        }
    }
}