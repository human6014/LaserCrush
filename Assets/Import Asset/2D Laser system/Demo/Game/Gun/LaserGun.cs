using UnityEngine;

namespace LaserSystem2D
{
    public class LaserGun : MonoBehaviour
    {
        [SerializeField] private Transform _turret;
        [SerializeField] private Transform _fastening;
        [SerializeField] private Transform _turretAnchor;
        [SerializeField] private Transform _laserPoint;
        [SerializeField] private Laser _laser;
        private GunFasteningRotation _fasteningRotation;
        private TransformMapper _mapper;

        private void Start()
        {
            _mapper = new TransformMapper(_turret, _turretAnchor);
            _fasteningRotation = new GunFasteningRotation(_fastening, transform);
        }

        private void Update()
        {
            _fasteningRotation.Update(2);
            _mapper.MapPosition();
            ManageLaser();
        }

        private void ManageLaser()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _laser = LaserManager.Instance.LaserPool.GetLaser(_laser);
                _laser.Enable(_laserPoint);
            }
        
            if (Input.GetMouseButtonUp(0))
            {
                _laser.Disable();
            }  
        }
    }
}