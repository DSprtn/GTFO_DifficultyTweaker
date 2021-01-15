namespace GTFO_Difficulty_Tweaker.Console
{
    public abstract class Command
    {
        public string commandName;
        public string commandDesc;
        public string payLoad;

        public Command(string commandName, string commandDesc)
        {
            this.commandName = commandName;
            this.commandDesc = commandDesc;
        }

        public virtual void Execute(string payLoad)
        {
            
        }

        public virtual string GetDescriptionString()
        {
            return "";
        }
    }
    
}
