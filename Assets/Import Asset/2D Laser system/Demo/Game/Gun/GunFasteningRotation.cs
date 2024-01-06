using UnityEngine;

namespace LaserSystem2D
{
    public class GunFasteningRotation 
    {
        private readonly Transform _fastening;
        private readonly Transform _transform;
        private readonly float _rightAngle = 90;

        public GunFasteningRotation(Transform fastening, Transform transform)
        {
            _fastening = fastening;
            _transform = transform;
        }

        public void Update(float rotationSpeed)
        {
            Vector2 normal = Vector2.down;
            float zOffset = -90;

            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 characterInLaserSpace = _transform.InverseTransformPoint(position);
            float crossProduct = normal.x * (normal.y - characterInLaserSpace.y) - normal.y * (normal.x - characterInLaserSpace.x);

            float targetZRotation = (_rightAngle - Vector3.SignedAngle(normal, characterInLaserSpace, Vector3.up)) * Mathf.Sign(crossProduct) + zOffset;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetZRotation);

            _fastening.rotation = Quaternion.Lerp(_fastening.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}