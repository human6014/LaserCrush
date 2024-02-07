using UnityEngine;

namespace LaserSystem2D
{
    public class CameraBordersFactory
    {
        private readonly LevelBorders _levelBorders;
        private readonly Camera _camera;

        public CameraBordersFactory(LevelBorders levelBorders, Camera camera)
        {
            _levelBorders = levelBorders;
            _camera = camera;
        }

        public CameraBorders Create()
        {
            float cameraHalfHeight = _camera.orthographicSize;
            float cameraHalfWidth = cameraHalfHeight * _camera.aspect;

            return CreateCameraBorders(cameraHalfWidth, cameraHalfHeight);
        }

        private CameraBorders CreateCameraBorders(float cameraHalfWidth, float cameraHalfHeight)
        {
            const float tileOffset = 0.75f;
            float widthOffset = cameraHalfWidth - tileOffset;
            float heightOffset = cameraHalfHeight - tileOffset;

            float cameraTopBorder = _levelBorders.Top - heightOffset;
            float cameraDownBorder = _levelBorders.Bottom + heightOffset;
            float cameraLeftBorder = _levelBorders.Left + widthOffset;
            float cameraRightBorder = _levelBorders.Right - widthOffset;

            return new CameraBorders(cameraTopBorder, cameraDownBorder, cameraLeftBorder, cameraRightBorder);
        }
    }
}