using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_DIfficulty_Tweaker.Util
{
    public enum LogLevel
    {
        UserInfo = 0,
        Info = 1,
        Debug = 2
    }


    public static class LoggerWrapper
    {
        public static LogLevel logLevel = LogLevel.Debug;

        public static void Log(string message, LogLevel level = LogLevel.UserInfo, ConsoleColor color = ConsoleColor.White)
        {
            if((int) level <= (int) logLevel)
            {
                if(color != ConsoleColor.White)
                {
                    MelonModLogger.Log(color, message);
                } else
                {
                    MelonModLogger.Log(message);
                }
            }
        }
    }
}
