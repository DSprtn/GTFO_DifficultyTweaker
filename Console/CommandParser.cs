using GTFO_Difficulty_Tweaker;
using GTFO_Difficulty_Tweaker.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_DIfficulty_Tweaker.Console
{
    public static class CommandParser
    {
        public static List<Command> commands;

        public static void Init()
        {
            commands = new List<Command>();

            commands.Add(new HelpCommand("HELP", "Display all available commands"));
            commands.Add(new PropertySetCommand("ENEMY_MULT", "Enemy population multiplier", SpawnTweakSettings.SetPopulationMult, 0.1f, 10f));
            commands.Add(new SpawnModeCommand("ENEMY_MODE", $"Change the spawning mode of enemies."));
        }


        public static bool HandleMessage(string msg)
        {
            foreach (Command c in commands)
            {
                if (msg.StartsWith(c.commandName))
                {
                    string args = msg.Replace(c.commandName, "");
                    args = args.Trim();
                    c.Execute(args);
                    return true;
                }
            }
            return false;
        }

    }
}
