using UnityEngine;

namespace Helper.Debug
{
    public static class DebugMessage
    {
        public static void Log(bool debugMode, string message, Object context = null)
        {
            if (debugMode) UnityEngine.Debug.Log(message, context);
        }
        public static void LogError(bool debugMode, string message, Object context = null)
        {
            if (debugMode) UnityEngine.Debug.LogError(message, context);
        }
        public static void LogWarning(bool debugMode, string message, Object context = null)
        {
            if (debugMode) UnityEngine.Debug.LogWarning(message, context);
        }
        public static void LogException(bool debugMode, System.Exception exception, Object context = null)
        {
            if (debugMode) UnityEngine.Debug.LogException(exception, context);
        }
    }
}