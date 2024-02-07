using UnityEngine;

namespace LaserSystem2D
{
    public class CameraClamping 
    {
        private readonly float _cameraZPosition = -10;
        private readonly CameraBorders _borders;

        public CameraClamping(LevelBorders levelBorders, Camera camera)
        {
            _borders = new CameraBordersFactory(levelBorders, camera).Create();
        }

        public Vector3 Clamp(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, _borders.Left, _borders.Right);
            position.y = Mathf.Clamp(position.y, _borders.Down, _borders.Top);
            position.z = _cameraZPosition;
        
            TryCentreCameraAxis(_borders.Right, _borders.Left, ref position.x);
            TryCentreCameraAxis(_borders.Top, _borders.Down, ref position.y);

            return position;
        }

        private void TryCentreCameraAxis(float upBorder, float downBorder, ref float cameraAxisPosition)
        {
            if (upBorder - downBorder <= 0)
            {
                cameraAxisPosition = (upBorder + downBorder) / 2f;
            }
        }
    }
}