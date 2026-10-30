using UnityEngine;

namespace Helper
{
    public static class DebugMessage
    {
        public static void Log(bool debugMode, string message, Object context = null)
        {
            if (debugMode) Debug.Log(message, context);
        }
        public static void LogError(bool debugMode, string message, Object context = null)
        {
            if (debugMode) Debug.LogError(message, context);
        }
        public static void LogWarning(bool debugMode, string message, Object context = null)
        {
            if (debugMode) Debug.LogWarning(message, context);
        }
        public static void LogException(bool debugMode, System.Exception exception, Object context = null)
        {
            if (debugMode) Debug.LogException(exception, context);
        }
    }
}