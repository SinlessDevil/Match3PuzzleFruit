using System.Collections.Generic;
using Code.Logic.Controllers;
using DG.Tweening;
using UnityEngine;

namespace Code.Logic.Match3
{
    public class GamePieceDragPreview
    {
        private struct PiecePositionInfo
        {
            public GamePieceView View;
            public Vector3 OriginalPosition;
            public Vector3 CurrentPosition;

            public PiecePositionInfo(GamePieceView view, Vector3 originalPosition)
            {
                View = view;
                OriginalPosition = originalPosition;
                CurrentPosition = originalPosition;
            }
        }

        private readonly MatchBoardController _controller;
        private readonly float _animationDuration;

        private readonly Dictionary<GamePieceView, PiecePositionInfo> _piecesPositions;

        private GamePieceView _pressedView;
        private GamePieceView _currentTargetView;

        private Tween _pressedTween;
        private Tween _targetTween;

        private bool _isAnimating;

        public GamePieceDragPreview(MatchBoardController controller, float animationDuration = 0.25f)
        {
            _controller = controller;
            _animationDuration = animationDuration;
            _piecesPositions = new Dictionary<GamePieceView, PiecePositionInfo>();
        }

        public void StartPreview(Cell pressedCell)
        {
            if (pressedCell == null || pressedCell.CurrentPieceView == null || pressedCell.CurrentPieceView.Data == null)
            {
                return;
            }

            GamePieceView pressedView = pressedCell.CurrentPieceView;

            if (_pressedView != null && _pressedView != pressedView)
            {
                AnimateToOriginalPositions();
            }

            CancelTweens();

            _pressedView = pressedView;
            _currentTargetView = null;
            _piecesPositions.Clear();

            Vector2 originalPosition = _controller.GetWorldPosition(pressedCell.X, pressedCell.Y);
            _piecesPositions[_pressedView] = new PiecePositionInfo(_pressedView, originalPosition);
        }

        public void UpdatePreview(Cell targetCell)
        {
            if (_pressedView == null)
            {
                return;
            }

            if (targetCell == null || targetCell.CurrentPieceView == null)
            {
                ResetToOriginalPositions();
                _currentTargetView = null;
                return;
            }

            if (targetCell.CurrentPieceView == _pressedView)
            {
                ResetToOriginalPositions();
                _currentTargetView = null;
                return;
            }

            if (!IsAdjacent(targetCell))
            {
                ResetToOriginalPositions();
                _currentTargetView = null;
                return;
            }

            GamePieceView targetView = targetCell.CurrentPieceView;

            if (_currentTargetView == targetView)
            {
                return;
            }

            if (_currentTargetView != null && _currentTargetView != targetView)
            {
                ResetToOriginalPositionsSync(targetCell);
            }

            if (!_piecesPositions.ContainsKey(targetView))
            {
                Vector2 originalPosition = _controller.GetWorldPosition(targetCell.X, targetCell.Y);
                _piecesPositions[targetView] = new PiecePositionInfo(targetView, originalPosition);
            }

            _currentTargetView = targetView;
            AnimatePreview(targetCell);
        }

        public void CancelPreview()
        {
            AnimateToOriginalPositions();
            _pressedView = null;
            _currentTargetView = null;
            _piecesPositions.Clear();
        }

        public void FinishPreview()
        {
            CancelTweens();
            _pressedView = null;
            _currentTargetView = null;
            _piecesPositions.Clear();
        }

        public void Dispose()
        {
            CancelTweens();
            _piecesPositions.Clear();
        }

        private void AnimatePreview(Cell targetCell)
        {
            if (_pressedView == null || _currentTargetView == null)
            {
                return;
            }

            CancelTweens();
            _isAnimating = true;

            Vector2 pressedTargetPos = _controller.GetWorldPosition(targetCell.X, targetCell.Y);
            Vector2 targetTargetPos = _controller.GetWorldPosition(targetCell.X, targetCell.Y);

            PiecePositionInfo pressedInfo = _piecesPositions[_pressedView];
            PiecePositionInfo targetInfo = _piecesPositions[_currentTargetView];

            _pressedTween = _pressedView.transform.DOMove(pressedTargetPos, _animationDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    pressedInfo.CurrentPosition = pressedTargetPos;
                    _piecesPositions[_pressedView] = pressedInfo;
                    _isAnimating = false;
                });

            _targetTween = _currentTargetView.transform.DOMove(targetTargetPos, _animationDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    targetInfo.CurrentPosition = targetTargetPos;
                    _piecesPositions[_currentTargetView] = targetInfo;
                });
        }

        private void ResetToOriginalPositions()
        {
            if (_pressedView == null)
            {
                return;
            }

            AnimateToOriginalPositions();
        }

        private void ResetToOriginalPositionsSync(Cell currentTargetCell)
        {
            CancelTweens();

            if (_pressedView != null && _piecesPositions.TryGetValue(_pressedView, out PiecePositionInfo pressedInfo))
            {
                Vector2 originalPosition = _controller.GetWorldPosition(currentTargetCell.X, currentTargetCell.Y);
                _pressedView.transform.position = originalPosition;
                pressedInfo.OriginalPosition = originalPosition;
                pressedInfo.CurrentPosition = originalPosition;
                _piecesPositions[_pressedView] = pressedInfo;
            }

            if (_currentTargetView != null && _piecesPositions.TryGetValue(_currentTargetView, out PiecePositionInfo targetInfo))
            {
                targetInfo.CurrentPosition = targetInfo.OriginalPosition;
                _currentTargetView.transform.position = targetInfo.OriginalPosition;
                _piecesPositions[_currentTargetView] = targetInfo;
            }
        }

        private void AnimateToOriginalPositions()
        {
            if (_pressedView == null)
            {
                return;
            }

            GamePieceView previousTarget = _currentTargetView;
            _currentTargetView = null;

            CancelTweens();
            _isAnimating = true;

            bool hasAnimations = false;

            if (_pressedView != null && _piecesPositions.TryGetValue(_pressedView, out PiecePositionInfo pressedInfo))
            {
                Vector3 originalPos = pressedInfo.OriginalPosition;
                Vector3 currentPos = _pressedView.transform.position;

                if (Vector3.Distance(currentPos, originalPos) > 0.01f)
                {
                    _pressedTween = _pressedView.transform.DOMove(originalPos, _animationDuration)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            pressedInfo.CurrentPosition = originalPos;
                            _piecesPositions[_pressedView] = pressedInfo;
                            _isAnimating = false;
                        });
                    hasAnimations = true;
                }
                else
                {
                    pressedInfo.CurrentPosition = originalPos;
                    _piecesPositions[_pressedView] = pressedInfo;
                }
            }

            if (previousTarget != null && _piecesPositions.TryGetValue(previousTarget, out PiecePositionInfo targetInfo))
            {
                Vector3 originalPos = targetInfo.OriginalPosition;
                Vector3 currentPos = previousTarget.transform.position;

                if (Vector3.Distance(currentPos, originalPos) > 0.01f)
                {
                    _targetTween = previousTarget.transform.DOMove(originalPos, _animationDuration)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            targetInfo.CurrentPosition = originalPos;
                            _piecesPositions[previousTarget] = targetInfo;
                            if (!hasAnimations)
                            {
                                _isAnimating = false;
                            }
                        });
                    hasAnimations = true;
                }
                else
                {
                    targetInfo.CurrentPosition = originalPos;
                    _piecesPositions[previousTarget] = targetInfo;
                }
            }

            if (!hasAnimations)
            {
                _isAnimating = false;
            }
        }

        private void CancelTweens()
        {
            _pressedTween?.Kill();
            _targetTween?.Kill();
            _pressedTween = null;
            _targetTween = null;
            _isAnimating = false;
        }

        private bool IsAdjacent(Cell targetCell)
        {
            if (targetCell == null || _pressedView == null || targetCell.CurrentPieceView == null)
            {
                return false;
            }

            GamePieceData pressedData = _pressedView.Data;
            GamePieceData targetData = targetCell.CurrentPieceView.Data;

            if (pressedData == null || targetData == null)
            {
                return false;
            }

            int dx = Mathf.Abs(pressedData.X - targetData.X);
            int dy = Mathf.Abs(pressedData.Y - targetData.Y);

            return dx + dy == 1;
        }
    }
}


