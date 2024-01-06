using System.Collections.Generic;
using UnityEngine;

namespace LaserSystem2D
{
    public class LaserCollision
    {
        private readonly RaycastHit2D[] _hits = new RaycastHit2D[15];
        private readonly HashSet<Collider2D> _result = new HashSet<Collider2D>();
        private readonly Vector3[] _quad = new Vector3[4];
        private readonly Transform _transform;
        private readonly LaserLength _length;
        private readonly FullRectLine _line;
        private readonly float _boxHeight = 0.01f;

        public LaserCollision(Transform transform, LaserLength length, FullRectLine line)
        {
            _transform = transform;
            _length = length;
            _line = line;
        }

        private Vector3[] Vertices => _line.GeneratedMesh.vertices;
        private float Length => _length.Current;

        public HashSet<Collider2D> BoxCast(float width, float penetration, LayerMask mask, LaserRaycastResult raycastResult)
        {
            _result.Clear();
            float distance = 0;

            for (int i = Vertices.Length - 1, path = 0; i >= 0 && Length > distance; i -= 4, ++path)
            {
                distance += raycastResult.Hits[path].Distance;
                Vector3 direction = Vertices[i - 2] - Vertices[i - 0];
                direction += direction.normalized * penetration;
            
                if (distance > Length)
                {
                    float normalizedLength = (raycastResult.Hits[path].Distance - (distance - Length)) / raycastResult.Hits[path].Distance;
                    direction *= normalizedLength;
                }
            
                UpdateBoxCastQuad(Vertices, i, direction, width);
                BoxCast(direction, mask);
                Draw();
            }

            return _result;
        }

        private void UpdateBoxCastQuad(IReadOnlyList<Vector3> vertices, int i, Vector3 direction, float collisionWidth)
        {
            Vector3 bottomCentre = (vertices[i - 1] + vertices[i - 0]) / 2;
            Vector3 normal = new Vector3(-direction.y, direction.x, 0).normalized;
            _quad[0] = bottomCentre + normal * collisionWidth; 
            _quad[1] = bottomCentre - normal * collisionWidth;
            _quad[2] = _quad[1] + direction;
            _quad[3] = _quad[0] + direction;

            for (int j = 0; j < _quad.Length; ++j)
            {
                _quad[j] = (Vector3)(_transform.localToWorldMatrix * _quad[j]) + _transform.position;
            }
        }

        private void BoxCast(Vector2 direction, LayerMask mask)
        {
            Vector3 centre = (_quad[0] + _quad[1]) / 2;
            Vector3 size = new Vector2((_quad[1] - _quad[0]).magnitude, _boxHeight);
        
            direction = _transform.rotation * direction;
            float angle = Mathf.Atan(direction.y / direction.x) * Mathf.Rad2Deg + 90;

            int hitsCount = Physics2D.BoxCastNonAlloc(centre, size, angle, direction, _hits, direction.magnitude, mask);

            for (int i = 0; i < hitsCount; ++i)
            {
                _result.Add(_hits[i].collider);
            }
        }

        private void Draw()
        {
            Debug.DrawLine(_quad[0], _quad[1], Color.yellow);
            Debug.DrawLine(_quad[1], _quad[2], Color.yellow);
            Debug.DrawLine(_quad[2], _quad[3], Color.yellow);
            Debug.DrawLine(_quad[3], _quad[0], Color.yellow);
        }
    }
}