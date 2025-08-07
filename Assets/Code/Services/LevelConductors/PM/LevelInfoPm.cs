using System;
using System.Collections.Generic;
using Code.Services.Levels;
using Code.Services.StaticData;
using Code.StaticData;
using Code.StaticData.Levels;

namespace Code.Logic.Level.PM
{
    public class LevelInfoPm : ILevelInfoPM
    {
        private ILevelService _levelService;
        private IStaticDataService _staticDataService;
        
        public LevelInfoPm(
            ILevelService levelService, 
            IStaticDataService staticDataService)
        {
            _levelService = levelService;
            _staticDataService = staticDataService;
        }

        public int GetScore()
        {
            return 0;
        }

        public string GetTargetText()
        {
            return string.Empty;
        }

        public string GetRemainingText()
        {
            return string.Empty;
        }
        
        public string GetConstantLevelText(TextTypeId textTypeId, LevelTypeId levelTypeId)
        {
            Dictionary<LevelTypeId, string> dictionary = GetTextDictionary(textTypeId);

            if (dictionary.TryGetValue(levelTypeId, out var text))
                return text;

            throw new ArgumentException($"LevelTypeId {levelTypeId} not found in {textTypeId} dictionary");
        }

        private Dictionary<LevelTypeId, string> GetTextDictionary(TextTypeId textTypeId)
        {
            BalanceStaticData balance = GetBalanceStaticData();

            return textTypeId switch
            {
                TextTypeId.RemainingSubText => balance.RemainingSubTexts,
                TextTypeId.TargetSubText => balance.TargetSubTexts,
                _ => throw new ArgumentOutOfRangeException(nameof(textTypeId), textTypeId, null)
            };
        }
        
        private BalanceStaticData GetBalanceStaticData() => _staticDataService.Balance;

        private LevelStaticData GetCurrentLevelStaticData() => _levelService.GetCurrentLevelStaticData();
    }
}