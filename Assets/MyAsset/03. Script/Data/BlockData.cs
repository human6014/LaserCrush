using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Data
{
    [CreateAssetMenu(fileName = "Scriptable Data", menuName = "Scriptable/Block Data", order = int.MaxValue)]
    public class BlockData : ScriptableObject
    {
        [SerializeField] private Color m_NormalBlockColor;
        [SerializeField] private Color m_ReflectBlockColor;
        [SerializeField] private Vector2 m_InitScale;

        public Color NormalBlockColor { get => m_NormalBlockColor; }
        public Color ReflectBlockColor { get => m_ReflectBlockColor; }
        public Vector2 InitScale { get => m_InitScale; }
    }
}
