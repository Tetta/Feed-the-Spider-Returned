#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("cPP98sJw8/jwcPPz8mfyDfSMdnYmWFuSpJ2EwmVcHAFGQ8NBT+r5M5FHcOKwtNMSW7IKPO77EH31dv+CwnDz0ML/9PvYdLp0Bf/z8/P38vFR0nk7SjFbakTborwq9bbw31p7+OObwcrBAWc2IM56C8Tsjtv1IOznkczodp6Z15rNTRojGawqx0eoZ4bR4+eg3LAYe0k7F4V88hDIFH68T0MrJO0uBjk2QNHbV2my5u0HDa6NgAm1o9LfZYY3W2FiWlaG6yD344c3/s0m/qAGo9ElDU1n6PeT8GoiV9cLS4YSLXIARfJ3gb/aAyfblbN3Rl4rqo5jAISfrBeDdwGkTCkmr3czmjUxK/0CAQN8KXGwxJe62omEdRx1b35x8EC/b/Dx8/Lz");
        private static int[] order = new int[] { 1,11,6,11,5,13,9,7,13,12,10,12,12,13,14 };
        private static int key = 242;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
