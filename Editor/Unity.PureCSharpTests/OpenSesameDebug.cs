using System;
using System.Diagnostics;

namespace Coffee.OpenSesameCompilers
{
    internal class Debug
    {
        [Conditional("OPEN_SESAME_LOG")]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        [Conditional("OPEN_SESAME_LOG")]
        public static void LogFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(format, args);
        }

        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(format, args);
        }

        public static void LogException(Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
    }
}
