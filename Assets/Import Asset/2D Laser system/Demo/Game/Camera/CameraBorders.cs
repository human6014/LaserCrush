
namespace LaserSystem2D
{
    public class CameraBorders
    {
        public readonly float Top;
        public readonly float Down;
        public readonly float Left;
        public readonly float Right;

        public CameraBorders(float topBorder, float downBorder, float leftBorder, float rightBorder)
        {
            Top = topBorder;
            Down = downBorder;
            Left = leftBorder;
            Right = rightBorder;
        }
    }
}