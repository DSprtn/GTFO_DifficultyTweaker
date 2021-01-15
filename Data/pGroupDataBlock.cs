using GameData;
using GTFO_DIfficulty_Tweaker.Util;
using System;
using System.Collections.Generic;

namespace GTFO_DIfficulty_Tweaker.Data
{
    public struct pGroupDataBlock
    {
        public List<Tuple<eEnemyRole,eEnemyRoleDistribution>> roleCache;

        public eEnemyGroupType groupType;

        public eEnemyRoleDifficulty difficulty;

        public eSpawnPlacementType placement;

        public float maxScore;

        public float RelativeWeight;

        public pGroupDataBlock(EnemyGroupDataBlock copyBlock)
        {
            string debugStr = $"Original - Difficulty: {copyBlock.Difficulty}";
            roleCache = new List<Tuple<eEnemyRole, eEnemyRoleDistribution>>();
            foreach(EnemyGroupCompositionData data in copyBlock.Roles)
            {
                debugStr += $"\n Role: {data.Role} Distr: {data.Distribution}";
                roleCache.Add(new Tuple<eEnemyRole, eEnemyRoleDistribution> (data.Role, data.Distribution));
            }
            difficulty = copyBlock.Difficulty;
            placement = copyBlock.SpawnPlacementType;
            maxScore = copyBlock.MaxScore;
            RelativeWeight = copyBlock.RelativeWeight;
            groupType = copyBlock.Type;
            
            LoggerWrapper.Log(debugStr);
            LoggerWrapper.Log(ToString());
        }

        public override string ToString()
        {

            string baseStr = $"GroupType {groupType} Difficulty: {difficulty} Placement {placement}";
            foreach(Tuple<eEnemyRole, eEnemyRoleDistribution> t in roleCache)
            {
                baseStr += $"\nRole: {t.Item1} Distribution: {t.Item2}";
            }
            return baseStr;
        }

    }
}