// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("VdTOCcHqaT3X3oOtX5FSjn3gatekfHnJ9K4Na0sJ6DFhb7UG9z1HCmvyDiD3TN20NF3wxcLjXcVHO/jwjRLcFOkDZfGIXXo/TL1bOOelTF+yPXlHpN6xFQC0ovW/yr30zbMFgZkwEv44PCv3KPATcXv9bd5R94cFArAzEAI/NDsYtHq0xT8zMzM3MjF9xQKQh4Urosv3x0Dy/GZH15Jo3/wMXVfRxxLKZVBXOypBvgvfkO2SuQghyKFMtIySmBZLEzK6pHkTfohS/qY+Or0/WsWt9R2cyWix5w886LAzPTICsDM4MLAzMzK+HZZqWIoMNMt0bR8U2c6cLRlbrSX1k6rIJ2W6f7liTpvYI5qvLwtWaOW5ir59Ziy7v2sDK7p5AzAxMzIz");
        private static int[] order = new int[] { 8,10,6,12,11,12,8,8,11,12,12,12,13,13,14 };
        private static int key = 50;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
