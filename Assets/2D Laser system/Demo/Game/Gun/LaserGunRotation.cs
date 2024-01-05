using UnityEngine;

namespace LaserSystem2D
{
    public class LaserGunRotation : MonoBehaviour
    {
        [SerializeField] private float _offset = -90;
        [SerializeField] private Transform _target;
        [SerializeField] private bool _useMouse = false;
    
        private void Update()
        {
            Vector2 worldMousePosition = _useMouse ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : _target.position;
            RotateTowards(worldMousePosition, _offset);
        }
    
        private void RotateTowards(Vector3 position, float offset)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, transform.position - position);
            targetRotation *= Quaternion.Euler(0, 0, offset);
            transform.rotation = targetRotation;
        }
    }
}