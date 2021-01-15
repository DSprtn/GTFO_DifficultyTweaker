using GameData;

namespace GTFO_Difficulty_Tweaker.Data
{
    public struct pAvailableEnemyTypes
    {
        public uint enemyID;
        public float enemyCost;
        public eEnemyRoleDifficulty difficulty;
        public eEnemyRole role;
        public float weight;

        public pAvailableEnemyTypes(uint id, float cost, eEnemyRoleDifficulty diff, eEnemyRole role, float weight)
        {
            enemyID = id;
            enemyCost = cost;
            difficulty = diff;
            this.role = role;
            this.weight = weight;
        }

        public override string ToString()
        {
            return $"PopData: EnemyID:{enemyID} Difficulty: {difficulty} Role: {role}";
        }
    }
}
