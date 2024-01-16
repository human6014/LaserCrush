using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Data
{
    [CreateAssetMenu(fileName = "Scriptable Data", menuName = "Scriptable/Laser Data", order = int.MaxValue)]
    public class LaserData : ScriptableObject
    {
        [Tooltip("1 프레임당 데미지")]
        [SerializeField] private int m_Damage = 1;

        [Tooltip("레이저 삭제하는 속도")]
        [SerializeField] private float m_EraseVelocity = 150;

        [Tooltip("레이저 발사하는 속도")]
        [SerializeField] private float m_ShootingVelocity = 90;

        public int Damage { get => m_Damage; }
        public float EraseVelocity { get => m_EraseVelocity; }
        public float ShootingVelocity { get => m_ShootingVelocity; }
    }
}
