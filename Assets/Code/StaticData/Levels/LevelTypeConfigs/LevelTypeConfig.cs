using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.StaticData.Levels.LevelTypeConfigs
{
    [Serializable]
    public abstract class LevelTypeConfig
    {
        [Space(10)] [Header("Core Settings")]
        public List<int> Scores = new List<int> { 1000, 2000, 3000 };
    }
}