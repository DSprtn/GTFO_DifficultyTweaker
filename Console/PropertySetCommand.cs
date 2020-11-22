using GTFO_Difficulty_Tweaker.Console;
using GTFO_DIfficulty_Tweaker.Util;
using MelonLoader;
using System;
using UnityEngine;

namespace GTFO_Difficulty_Tweaker.Console
{
    public class PropertySetCommand : Command
    {
        float minValue;
        float maxValue;
        Action<float> targetMethod;

        public PropertySetCommand(string commandName, string commandDesc, Action<float> valueSetMethod, float minvalue, float maxValue) : base(commandName, commandDesc)
        {
            this.minValue = minvalue;
            this.maxValue = maxValue;
            this.targetMethod = valueSetMethod;
        }
        public override void Execute(string payLoad)
        {
            float value = 1f;

            if (float.TryParse(payLoad, out value))
            {
                value = Mathf.Clamp(value, minValue, maxValue);
                targetMethod.Invoke(value);
                LoggerWrapper.Log($"Setting {commandDesc} to {value}");
            }
            else
            {
                LoggerWrapper.Log("Invalid value");
            }
        }

        public override string GetDescriptionString()
        {
            return commandDesc + $" (Min: {minValue.ToString()} Max: {maxValue.ToString()})";
        }
    }
}
