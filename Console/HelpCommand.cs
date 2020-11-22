using GTFO_Difficulty_Tweaker.Console;
using GTFO_DIfficulty_Tweaker.Console;
using GTFO_DIfficulty_Tweaker.Util;
using MelonLoader;
using static GTFO_Difficulty_Tweaker.SpawnTweakSettings;

namespace GTFO_Difficulty_Tweaker.Console
{

        public class HelpCommand : Command
        {
            public HelpCommand(string commandName, string commandDesc) : base(commandName, commandDesc)
            {
            }

            public override void Execute(string payLoad)
            {
                base.Execute(payLoad);
                foreach (Command c in CommandParser.commands)
                {
                    LoggerWrapper.Log("\n" + c.commandName + " --- " + c.GetDescriptionString());
                }
            }
        }
    
}
