using System;
using Code.Logic.Match3;

namespace Code.Services.LevelConductors
{
    public interface ILevelConductor
    {
        event Action<int> ChangedCurrentScoreEvent;
        event Action<string> ChangedRemainingEvent;
        event Action<string> ChangedTargetEvent;
        void OnMove();
        void OnPieceCleared(GamePieceView pieceView);
        void Dispose();
    }
}