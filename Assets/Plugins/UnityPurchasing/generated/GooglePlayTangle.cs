#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("PB6VMyiVDfMBgiqS8vbB70ohXU6W9XL7ZJ9Sn0jY9QctaGyvjYhNfFg6kGznZfBBZqTLMZEIEtsagU6QWTRzE2JFRsjSqX8XGWJN8SOIKnhRaDv8oGa13KKGGpuNGHo/wqPvlHMNEfLoZ1YPo3BvYnjWJf6sFUlJoP0d1QB3pRmpZSpb46t6T3xx1a5o6+Xq2mjr4Oho6+vqfs59J38qjC6HojyOhJQqE1nKoEFH1YqDLtljderDekZimih6tOTpLXxM6vjYKxvaaOvI2ufs48Bsomwd5+vr6+/q6Wnf8gA6qphSfQAIKJ8HjANnWuGE9h9i8XyH32DQQC99u7BP4tf5aswRpvfsKg0lDQ8fhleDHoLccAH3an1H4Gae+UYeVejp6+rr");
        private static int[] order = new int[] { 12,5,2,6,7,10,9,10,9,9,12,11,12,13,14 };
        private static int key = 234;

        public static byte[] Data() {
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
