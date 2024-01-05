using UnityEngine;

namespace LaserSystem2D
{
    public class TransformMapper
    {
        private readonly Transform _target;

        public TransformMapper(Transform target, Transform source = null)
        {
            _target = target;
            Source = source;
        }

        public Transform Source { get; private set; }

        public void SetSource(Transform source)
        {
            Source = source;
        }

        public void MapPositionAndRotation()
        {
            MapPosition();
            MapRotation();
        }

        public void MapPosition()
        {
            if (Source != null)
            {
                _target.position = Source.position;    
            }
        }

        public void MapRotation()
        {
            if (Source != null)
            {
                _target.rotation = Source.rotation;    
            }
        }
    }
}