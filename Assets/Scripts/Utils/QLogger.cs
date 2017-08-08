using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class QLogger
    {
        private static bool printLogs = true;
        private static bool printWarnings = true;
        private static bool printErrors = true;
        private static bool printExceptions = true;

        public static void Log(string message)
        {
            if (!shouldPrint(printLogs))
            {
                return;
            }

            Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            if (!shouldPrint(printWarnings))
            {
                return;
            }

            Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            if (!shouldPrint(printErrors))
            {
                return;
            }

            Debug.LogError(message);
        }

        public static void LogException(Exception exception)
        {
            if (!shouldPrint(printExceptions))
            {
                return;
            }

            Debug.LogException(exception);
        }

        private static bool shouldPrint(bool printLevel)
        {
            return printLevel;
        }
    }
}
