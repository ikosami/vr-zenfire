using System.ComponentModel;
using UnityEngine.Internal;
using UnityEngine;

namespace NS
{
    public class ED
    {
        /// <summary>
        /// Logs a message to the Unity Console 
        /// only when DEVELOPMENT_BUILD or UNITY_EDITOR is defined.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [
            System.Diagnostics.Conditional("DEVELOPMENT_BUILD"),
            System.Diagnostics.Conditional("UNITY_EDITOR"),
        ]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        /// <summary>
        /// Logs a message to the Unity Console 
        /// only when DEVELOPMENT_BUILD or UNITY_EDITOR is defined.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [
            System.Diagnostics.Conditional("DEVELOPMENT_BUILD"),
            System.Diagnostics.Conditional("UNITY_EDITOR"),
        ]
        public static void Log(object message, Object context)
        {
            Debug.Log(message, context);
        }

        /// <summary>
        /// Logs a message to the Unity Console 
        /// only when DEVELOPMENT_BUILD or UNITY_EDITOR is defined.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        [
            System.Diagnostics.Conditional("DEVELOPMENT_BUILD"),
            System.Diagnostics.Conditional("UNITY_EDITOR"),
        ]
        public static void LogException(System.Exception exception)
        {
            Debug.LogException(exception);
        }

        /// <summary>
        /// Logs a message to the Unity Console 
        /// only when DEVELOPMENT_BUILD or UNITY_EDITOR is defined.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [
            System.Diagnostics.Conditional("DEVELOPMENT_BUILD"),
            System.Diagnostics.Conditional("UNITY_EDITOR"),
        ]
        public static void LogException(System.Exception exception, Object context)
        {
            Debug.LogException(exception, context);
        }

        /// <summary>
        /// Logs a message to the Unity Console 
        /// only when DEVELOPMENT_BUILD or UNITY_EDITOR is defined.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        [
            System.Diagnostics.Conditional("DEVELOPMENT_BUILD"),
            System.Diagnostics.Conditional("UNITY_EDITOR"),
        ]
        public static void LogError(object message)
        {
            Debug.LogError("[Error] " + message);
        }

        /// <summary>
        /// Logs a message to the Unity Console 
        /// only when DEVELOPMENT_BUILD or UNITY_EDITOR is defined.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [
            System.Diagnostics.Conditional("DEVELOPMENT_BUILD"),
            System.Diagnostics.Conditional("UNITY_EDITOR"),
        ]
        public static void LogError(object message, Object context)
        {
            Debug.LogError(message, context);
        }

        public static string GetHierarchyPath(Transform transform)
        {
            if (transform.parent == null)
                return transform.name;
            return GetHierarchyPath(transform.parent) + "/" + transform.name;
        }

    }

    public enum LogTextColor
    {
        white,
        red,
        green,
        yellow,
        orange,
        blue,
    }
}
