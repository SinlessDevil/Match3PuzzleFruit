using System;
using System.Collections.Generic;
using Code.Services.LevelInfo;
using Code.Services.Levels;
using Code.Services.StaticData;
using Code.StaticData;
using Code.StaticData.Levels;

namespace Code.Logic.Level.PM
{
    public class LevelInfoPm : ILevelInfoPM
    {
        private readonly ILevelInfoService _levelInfoService;
        private readonly ILevelService _levelService;
        private readonly IStaticDataService _staticDataService;
        
        public LevelInfoPm(
            ILevelInfoService levelInfoService,
            ILevelService levelService, 
            IStaticDataService staticDataService)
        {
            _levelInfoService = levelInfoService;
            _levelService = levelService;
            _staticDataService = staticDataService;
        }

        public int GetScore()
        {
            return _levelInfoService.CurrentScore;
        }

        public string GetTargetText()
        {
            return _levelInfoService.TargetText;
        }

        public string GetRemainingText()
        {
            return _levelInfoService.RemainingText;
        }
        
        public string GetLevelName()
        {
            return _levelInfoService.LevelName;
        }
        
        public int GetLevelNumber()
        {
            return _levelService.GetCurrentLevel();
        }
        
        public int GetStars()
        {
            return _levelInfoService.CurrentStars;
        }
        
        public LevelTypeId GetCurrentLevelTypeId()
        {
            return _levelInfoService.CurrentLevelTypeId;
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