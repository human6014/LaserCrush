using System;
using UnityEngine;

namespace LaserSystem2D
{
    [Serializable]
    public class SpriteFlip
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public void Update(Vector2 input)
        {
            if (input.x != 0)
            {
                _spriteRenderer.flipX = input.x < 0;
            }
        }
    }
}