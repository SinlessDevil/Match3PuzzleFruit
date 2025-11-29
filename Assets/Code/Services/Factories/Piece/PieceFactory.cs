using Code.Logic.Match3;
using Code.Services.Levels;
using Code.StaticData.Levels.BoardConfigs;
using UnityEngine;
using Zenject;
using Quaternion = UnityEngine.Quaternion;

namespace Code.Services.Factories.Pieces
{
    public class PieceFactory : Factory, IPieceFactory
    {
        private readonly ILevelService _levelService;

        public PieceFactory(IInstantiator instantiator, ILevelService levelService) : base(instantiator)
        {
            _levelService = levelService;
        }

        public Piece CreatePieceByCurrentLevel(PieceType pieceType, Vector3 position, Quaternion rotation, Transform root)
        {
            GameObject gameObject = Instantiate(BoardConfig.PieceDictionary[pieceType].gameObject, root);
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
            Piece piece = gameObject.GetComponent<Piece>();
            return piece;
        }
        
        private BoardConfig BoardConfig => _levelService.GetCurrentLevelStaticData().boardConfig;
    }
}