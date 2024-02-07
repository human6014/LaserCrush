using System;
using UnityEngine;

namespace LaserSystem2D
{
    [Serializable]
    public class LaserData
    {
        [SerializeField] private LayerMask _raycastMask = 1 << 0;
        [SerializeField] private LayerMask _collisionMask = 1 << 0;
        [SerializeField] [Min(0)] private float _nonHitDistance = 20;
        [SerializeField] [Min(0)] private float _shootingSpeed = 15;
        [SerializeField] [Min(0)] private float _dissolveTime = 1;
        [SerializeField] [Range(0, 2)] private float _collisionWidth = 0.15f;
        [SerializeField] [Range(0, 0.5f)] private float _collisionPenetration = 0.03f;
        [SerializeField] private int _sortingOrder;
        [SerializeField] private FullRectLine _line;
        [SerializeField] private ParticleSystem _hitEffectPrefab;
        [SerializeField] private AudioSource _laserAudioSource;
        [SerializeField] private AudioSource _hitAudioSource;

        public LayerMask RaycastMask => _raycastMask;
        public LayerMask CollisionMask => _collisionMask;
        public float ShootingSpeed => _shootingSpeed;
        public float DissolveTime => _dissolveTime;
        public float CollisionWidth => _collisionWidth;
        public float NonHitDistance => _nonHitDistance;
        public float CollisionPenetration => _collisionPenetration;
        public int SortingOrder => _sortingOrder;
        public FullRectLine Line => _line;
        public ParticleSystem HitEffectPrefab => _hitEffectPrefab;
        public AudioSource LaserAudioSource => _laserAudioSource;
        public AudioSource HitAudioSource => _hitAudioSource;
    }
}