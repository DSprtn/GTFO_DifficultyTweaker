using GTFO_DIfficulty_Tweaker.Util;
using MelonLoader;
using System;
using System.Collections.Generic;

namespace GTFO_Difficulty_Tweaker.Console
{
    public class SpawnModeCommand : Command
    {
        Dictionary<SpawnTweakerMode, string> spawnModes = new Dictionary<SpawnTweakerMode, string>();

        public SpawnModeCommand(string commandName, string commandDesc) : base(commandName, commandDesc)
        {
            foreach (SpawnTweakerMode m in Enum.GetValues(typeof(SpawnTweakerMode)))
            {
                spawnModes.Add(m, Enum.GetName(typeof(SpawnTweakerMode), m));
            }
        }

        public override void Execute(string payLoad)
        {
            SpawnTweakerMode mode = SpawnTweakSettings.mode;

            foreach (KeyValuePair<SpawnTweakerMode, string> item in spawnModes)
            {
                if (payLoad.StartsWith(item.Value))
                {
                    mode = item.Key;
                }
            }

            if (SpawnTweakSettings.mode != mode)
            {
                LoggerWrapper.Log("Switching spawning mode to " + mode, LogLevel.UserInfo);
                SpawnTweakSettings.mode = mode;
            }
        }

        public static string GetAllAvailableModes()
        {
            string result = "\n";
            foreach (SpawnTweakerMode m in Enum.GetValues(typeof(SpawnTweakerMode)))
            {
                result += Enum.GetName(typeof(SpawnTweakerMode), m) + "\n";
            }
            return result;
        }

        public override string GetDescriptionString()
        {
            return base.GetDescriptionString() + $" \nAvailable modes:\n{  SpawnModeCommand.GetAllAvailableModes()}";
        }
    }
}
