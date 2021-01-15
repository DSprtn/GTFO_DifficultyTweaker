
using BepInEx.Logging;
using GTFO_Difficulty_Tweaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_DIfficulty_Tweaker.Util
{
    public static class LoggerWrapper
    {
        public static ManualLogSource logSource;
        internal static List<PUI_GameEventLog> gameLogs;

        public static void Log(string message, LogLevel level = LogLevel.Debug, bool splitByLineInGameLog = false)
        {
            logSource.Log(level, message);
            if (level == LogLevel.Message && gameLogs != null)
            {
                TryLogToChat(message, Color.cyan, splitByLineInGameLog);
            }
        }

        public static void TryLogToChat(string message, Color c, bool split)
        {

            foreach(PUI_GameEventLog gameLog in gameLogs)
            {
                TryLog(message, c, gameLog);
            }
        }

        private static void TryLog(string message, Color c, PUI_GameEventLog gameLog)
        {
            if (gameLog == null || !gameLog.gameObject.activeSelf)
            {
                return;
            }
            foreach (string item in message.Split(Environment.NewLine.ToCharArray()))
            {
                if (item.Trim().Length < 1)
                {
                    continue;
                }
                gameLog.AddLogItem("<color=#" + ColorExt.ToHex(c) + "> > " + "DiffTweaker" + "</color>: " + item, eGameEventChatLogType.IncomingChat);
            }
        }

        public static void AddGameLogReference(PUI_GameEventLog log)
        {
            if(gameLogs == null)
            {
                gameLogs = new List<PUI_GameEventLog>();
            }
            gameLogs.Add(log);
        }

    }
}
