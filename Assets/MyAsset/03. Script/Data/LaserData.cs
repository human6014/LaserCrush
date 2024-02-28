using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Data
{
    [CreateAssetMenu(fileName = "Scriptable Data", menuName = "Scriptable/Laser Data", order = int.MaxValue)]
    public class LaserData : ScriptableObject
    {
        [Tooltip("레이저 삭제하는 속도")]
        [SerializeField] private float m_EraseVelocity = 500;

        [Tooltip("레이저 발사하는 속도")]
        [SerializeField] private float m_ShootingVelocity = 260;

        public float EraseVelocity { get => m_EraseVelocity; }
        public float ShootingVelocity { get => m_ShootingVelocity; }
    }
}
