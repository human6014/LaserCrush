// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("nmHex7W+c2Q2h7PxB49fOQBijc//fmSja0DDl310KQf1O/gk10rAfVam9/17bbhgz/r9kYDrFKF1Okc4wVikil3mdx6e91pvaEn3b+2RUlonuHa+Q6nPWyL30JXmF/GSTQ/m9ROii2IL5h4mODK84bmYEA7TudQiM5q4VJKWgV2CWrnb0VfHdPtdLa8Q1RPI5DFyiTAFhaH8wk8TIBTXzA7W02NeBKfB4aNCm8vFH6xdl+2gGpmXmKgamZKaGpmZmBS3PMDyIKbXb6g6LS+BCGFdbepYVsztfTjCdRiX0+0OdBu/qh4IXxVgF15nGa8r+FQMlJAXlfBvB1+3NmPCG02llkKoGpm6qJWekbIe0B5vlZmZmZ2Ym4YRFcGpgRDTqZqbmZiZ");
        private static int[] order = new int[] { 13,8,4,6,12,5,6,7,10,10,12,11,12,13,14 };
        private static int key = 152;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
