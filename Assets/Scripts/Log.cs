using System;

namespace ZMDFQ
{
    public static class Log
    {
        public static void Trace(string msg)
        {
            UnityEngine.Debug.Log(msg);
            onLog?.Invoke(nameof(Trace), msg);
        }
        public static void Game(string msg)
        {
            UnityEngine.Debug.Log(msg);
            onLog?.Invoke(nameof(Game), msg);
        }
        public static void Debug(string msg)
        {
            UnityEngine.Debug.Log(msg);
            onLog?.Invoke(nameof(Debug), msg);
        }

        public static void Info(string msg)
        {
            UnityEngine.Debug.Log(msg);
            onLog?.Invoke(nameof(Info), msg);
        }

        public static void Warning(string msg)
        {
            UnityEngine.Debug.LogWarning(msg);
            onLog?.Invoke(nameof(Warning), msg);
        }

        public static void Error(string msg)
        {
            UnityEngine.Debug.LogError(msg);
            onLog?.Invoke(nameof(Error), msg);
        }

        public static void Error(Exception e)
        {
            UnityEngine.Debug.LogException(e);
            onLog?.Invoke(nameof(Error), e.ToString());
        }
        public static void Fatal(string msg)
        {
            UnityEngine.Debug.LogAssertion(msg);
            onLog?.Invoke(nameof(Fatal), msg);
        }
        public delegate void OnLogDelegate(string channel, string msg);
        public static event OnLogDelegate onLog;
        public static void Trace(string message, params object[] args)
        {
            UnityEngine.Debug.LogFormat(message, args);
        }

        public static void Warning(string message, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(message, args);
        }

        public static void Info(string message, params object[] args)
        {
            UnityEngine.Debug.LogFormat(message, args);
        }

        public static void Debug(string message, params object[] args)
        {
            UnityEngine.Debug.LogFormat(message, args);
        }

        public static void Error(string message, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(message, args);
        }

        public static void Fatal(string message, params object[] args)
        {
            UnityEngine.Debug.LogAssertionFormat(message, args);
        }
    }
}