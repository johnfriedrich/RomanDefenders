using UnityEngine;

namespace Utils {
    public static class Log {

        public static void Verbose(string template, params object[] args) {
            var message = string.Format(template, args);
            VerbosePrint(message);
        }

        private static void VerbosePrint(object message) {
            Debug.Log(message);
        }

    }
}
