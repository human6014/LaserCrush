using UnityEngine;

namespace LaserSystem2D
{
    public class GameParallaxLayer : MonoBehaviour
    {
        [SerializeField] [Range(0, 1)] private float _parallaxEffect;
        [SerializeField] private RectTransform _layer;
        [SerializeField] private float _speed = 60;
        private ParallaxLayerClamping _parallaxLayerClamping;
        private float _lastCameraPosition;
        private Transform _cameraTransform;

        private void Start()
        {
            foreach (ParallaxLayerStartPosition startPosition in GetComponentsInChildren<ParallaxLayerStartPosition>())
            {
                startPosition.Evaluate();
            }
        
            _parallaxLayerClamping = new ParallaxLayerClamping();
            _cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            float newCameraPosition = _cameraTransform.transform.position.x;

            _layer.anchoredPosition = GetParallaxLayerPosition(newCameraPosition);

            _lastCameraPosition = newCameraPosition;
        }

        private Vector2 GetParallaxLayerPosition(float newCameraPosition)
        {
            float cameraDiffPosition = newCameraPosition - _lastCameraPosition;
            float targetXParallaxPosition = cameraDiffPosition * _parallaxEffect * _speed;
            targetXParallaxPosition = _layer.anchoredPosition.x - targetXParallaxPosition;

            _parallaxLayerClamping.ClampParallaxLayerPosition(ref targetXParallaxPosition);

            return new Vector2(targetXParallaxPosition, _layer.anchoredPosition.y);
        }
    }
}