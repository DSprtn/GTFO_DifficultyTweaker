using GameData;
using GTFO_DIfficulty_Tweaker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_DIfficulty_Tweaker.Data
{   
    /// <summary>
    /// Not working as of yet, might be useful in the future
    /// </summary>
    public static class EnemyGroupData
    {
        public static Dictionary<uint, pGroupDataBlock> originalData;

        public static void TryAddDataBlock(EnemyGroupDataBlock block)
        {
            if(originalData == null)
            {
                originalData = new Dictionary<uint, pGroupDataBlock>();
            }
            if(originalData.ContainsKey(block.persistentID))
            {
                return;
            }
            originalData.Add(block.persistentID, new pGroupDataBlock(block));
        }

        public static void TryResetBlockData(EnemyGroupDataBlock block) 
        { 
            if(originalData == null)
            {
                return;
            }
            if(originalData.ContainsKey(block.persistentID))
            {
                pGroupDataBlock data = originalData[block.persistentID];
                block.Difficulty = data.difficulty;
                block.MaxScore = data.maxScore;
                block.RelativeWeight = data.RelativeWeight;
                block.SpawnPlacementType = data.placement;

                for (int i = 0; i < block.Roles.Count; i++)
                {
                    block.Roles[i].Role = data.roleCache[i].Item1;
                    block.Roles[i].Distribution = data.roleCache[i].Item2;
                }
            } else
            {
                LoggerWrapper.Log($"COULD NOT RECOVER BLOCK {block.persistentID}", BepInEx.Logging.LogLevel.Fatal);
            }
        }

        internal static void TryResetAll()
        {
            if (originalData == null)
            {
                return;
            }
            foreach (uint id in originalData.Keys)
            {
                TryResetBlockData(EnemyGroupDataBlock.GetBlock(id));
            }
        }
    }
}
