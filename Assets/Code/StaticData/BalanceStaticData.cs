using System.Collections.Generic;
using Code.StaticData.Levels;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.StaticData
{
    [CreateAssetMenu(menuName = "StaticData/Balance", fileName = "Balance", order = 0)]
    public class BalanceStaticData : SerializedScriptableObject
    {
        public Dictionary<LevelTypeId, string> RemainingSubTexts = new Dictionary<LevelTypeId, string>
        {
            { LevelTypeId.Moves, "moves remaining" },
            { LevelTypeId.Obstacle, "moves remaining" },
            { LevelTypeId.Timer, "time remaining" },
        };
        
        public Dictionary<LevelTypeId, string> TargetSubTexts = new Dictionary<LevelTypeId, string>
        {
            { LevelTypeId.Moves, "target score" },
            { LevelTypeId.Obstacle, "bubbles remaining" },
            { LevelTypeId.Timer, "target score" },
        };
    }
}