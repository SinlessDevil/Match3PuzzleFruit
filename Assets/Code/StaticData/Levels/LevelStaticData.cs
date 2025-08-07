using UnityEngine;
using Code.StaticData.Levels.LevelTypeConfigs;
using Sirenix.OdinInspector;

namespace Code.StaticData.Levels
{
    [CreateAssetMenu(fileName = "LevelStaticData", menuName = "StaticData/LevelConductor", order = 0)]
    public class LevelStaticData : SerializedScriptableObject
    {
        [Space(10)] [Header("LevelConductor Info")]
        public string LevelName;
        public int LevelId;
        public LevelTypeId LevelTypeId;
        public LevelTypeConfig LevelTypeConfigs;
    }
}