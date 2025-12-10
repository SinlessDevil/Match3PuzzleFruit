using System.Collections.Generic;
using System.Threading;
using Code.Logic.Match3;
using Code.Services.Board;
using Code.Services.Factories.Pieces;
using Code.Services.LevelConductors.Locator;
using Code.Services.Levels;
using Code.StaticData.Levels.BoardConfigs;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Logic.Controllers
{
    public class MatchBoardController : IMatchBoardController
    {
        private GamePieceView[,] _pieces;
        private Cell[,] _cells;
        
        private Cell _pressedCell;
        private Cell _enteredCell;

        private GamePieceDragPreview _dragPreview;

        private bool _gameOver;
        private CancellationTokenSource _fillCancellationTokenSource;

        private Transform _root;
        
        private readonly ILevelServiceLocator _levelServiceLocator;
        private readonly ILevelService _levelService;
        private readonly IPieceFactory _pieceFactory;
        private readonly ICameraAdapterService _cameraAdapterService;
        private readonly IBoardFillService _boardFillService;
        private readonly IMatchFinderService _matchFinderService;
        private readonly IPieceSwapService _pieceSwapService;
        private readonly IBoardClearService _boardClearService;

        public MatchBoardController(
            ILevelServiceLocator levelServiceLocator, 
            ILevelService levelService,
            IPieceFactory pieceFactory,
            ICameraAdapterService cameraAdapterService,
            IBoardFillService boardFillService,
            IMatchFinderService matchFinderService,
            IPieceSwapService pieceSwapService,
            IBoardClearService boardClearService)
        {
            _levelServiceLocator = levelServiceLocator;
            _levelService = levelService;
            _pieceFactory = pieceFactory;
            _cameraAdapterService = cameraAdapterService;
            _boardFillService = boardFillService;
            _matchFinderService = matchFinderService;
            _pieceSwapService = pieceSwapService;
            _boardClearService = boardClearService;
            _dragPreview = new GamePieceDragPreview(this, 0.25f);
        }
        
        public void StartLevel()
        {
            SetupCameraAdapter();
            
            _cells = new Cell[BoardConfig.XDim, BoardConfig.YDim];
            _pieces = new GamePieceView[BoardConfig.XDim, BoardConfig.YDim];
            
            for (int x = 0; x < BoardConfig.XDim; x++)
            {
                for (int y = 0; y < BoardConfig.YDim; y++)
                {
                    Piece background = _pieceFactory.CreatePieceByCurrentLevel(
                        PieceType.Background, 
                        GetWorldPosition(x, y), 
                        Quaternion.identity, 
                        _root);

                    Cell cell = background.GetComponent<Cell>();
                    if (cell == null)
                    {
                        cell = background.gameObject.AddComponent<Cell>();
                    }
                    
                    cell.Initialize(x, y, this);
                    _cells[x, y] = cell;
                }
            }

            for (int i = 0; i < BoardConfig.InitialPieces.Length; i++)
            {
                if (BoardConfig.InitialPieces[i].X >= 0 && BoardConfig.InitialPieces[i].Y < BoardConfig.XDim
                                                    && BoardConfig.InitialPieces[i].Y >= 0 
                                                    && BoardConfig.InitialPieces[i].Y < BoardConfig.YDim)
                {
                    SpawnNewPiece(BoardConfig.InitialPieces[i].X, BoardConfig.InitialPieces[i].Y, BoardConfig.InitialPieces[i].Type);
                }
            }

            for (int x = 0; x < BoardConfig.XDim; x++)
            {
                for (int y = 0; y < BoardConfig.YDim; y++)
                {
                    if (_pieces[x, y] == null)
                    {
                        SpawnNewPiece(x, y, PieceType.Empty);
                    }                
                }
            }

            StartFillAsync();
        }

        public void Dispose()
        {
            _fillCancellationTokenSource?.Cancel();
            _fillCancellationTokenSource?.Dispose();
            _fillCancellationTokenSource = null;
            
            _dragPreview?.Dispose();
            _dragPreview = null;
            
            _pieces = null;
            _cells = null;
            _pressedCell = null;
            _enteredCell = null;
            _root = null;
            
            _gameOver = false;
        }

        public void SetRootTransform(Transform rootTransform)
        {
            _root = rootTransform;
        }
        
        public bool IsFilling => _boardFillService.IsFilling;
        
        public Vector2 GetWorldPosition(int x, int y)
        {
            float halfX = (BoardConfig.XDim - 1) / 2f;
            float halfY = (BoardConfig.YDim - 1) / 2f;

            float worldX = _root.position.x + (x - halfX) + BoardConfig.BoardOffset.x;
            float worldY = _root.position.y + (halfY - y) + BoardConfig.BoardOffset.y;

            return new Vector2(worldX, worldY);
        }
        
        public void PressCell(Cell cell)
        {
            if (cell != null && cell.HasPieceView)
            {
                _pressedCell = cell;
                _dragPreview?.StartPreview(cell);
            }
        }

        public void EnterCell(Cell cell)
        {
            if (cell != null && cell.HasPieceView)
            {
                _enteredCell = cell;
                _dragPreview?.UpdatePreview(cell);
            }
        }
        
        public void ReleaseCell()
        {
            if (_pressedCell?.CurrentPieceView?.Data != null && 
                _enteredCell?.CurrentPieceView?.Data != null && 
                IsAdjacent(_pressedCell.X, _pressedCell.Y, _enteredCell.X, _enteredCell.Y))
            {
                _dragPreview?.FinishPreview();
                SwapPieces(_pressedCell.CurrentPieceView, _enteredCell.CurrentPieceView);
            }
            else
            {
                _dragPreview?.CancelPreview();
            }
            
            _pressedCell = null;
            _enteredCell = null;
        }
        
        public Cell GetCell(int x, int y)
        {
            if (x < 0 || x >= BoardConfig.XDim || y < 0 || y >= BoardConfig.YDim)
                return null;
            
            return _cells[x, y];
        }
        
        public void ClearRow(int row)
        {
            BoardClearConfig config = CreateClearConfig();
            _boardClearService.ClearRow(_pieces, row, config);
        }

        public void ClearColumn(int column)
        {
            BoardClearConfig config = CreateClearConfig();
            _boardClearService.ClearColumn(_pieces, column, config);
        }

        public void ClearColor(ColorType color)
        {
            BoardClearConfig config = CreateClearConfig();
            _boardClearService.ClearColor(_pieces, color, config);
        }

        public void GameOver() => _gameOver = true;

        public List<GamePieceView> GetPiecesOfType(PieceType type)
        {
            List<GamePieceView> piecesOfType = new List<GamePieceView>();

            for (int x = 0; x < BoardConfig.XDim; x++)
            {
                for (int y = 0; y < BoardConfig.YDim; y++)
                {
                    if (_pieces[x, y]?.Data?.Type == type)
                    {
                        piecesOfType.Add(_pieces[x, y]);
                    }
                }
            }

            return piecesOfType;
        }

        private void StartFillAsync()
        {
            _fillCancellationTokenSource?.Cancel();
            _fillCancellationTokenSource?.Dispose();
            _fillCancellationTokenSource = new CancellationTokenSource();
            
            BoardFillConfig config = new BoardFillConfig
            {
                XDim = BoardConfig.XDim,
                YDim = BoardConfig.YDim,
                FillTime = BoardConfig.FillTime,
                Root = _root,
                PieceFactory = _pieceFactory,
                MatchBoardController = this,
                GetWorldPosition = GetWorldPosition
            };
            
            _boardFillService.FillAsync(_pieces, config, _fillCancellationTokenSource.Token)
                .ContinueWith(() => SyncCellsWithPieces())
                .Forget();
        }

        private GamePieceView SpawnNewPiece(int x, int y, PieceType type)
        {
            Piece newPiece = _pieceFactory.CreatePieceByCurrentLevel(type, GetWorldPosition(x, y), 
                Quaternion.identity, _root);
            GamePieceView pieceView = newPiece.GetComponent<GamePieceView>();
            
            GamePieceData data = new GamePieceData(x, y, type);
            data.Score = GetScoreForPieceType(type);
            data.SetMatchBoardController(this);
            pieceView.Initialize(data);
            
            _pieces[x, y] = pieceView;
            
            if (_cells[x, y] != null)
            {
                _cells[x, y].CurrentPieceView = pieceView;
            }

            return _pieces[x, y];
        }
        
        private int GetScoreForPieceType(PieceType type)
        {
            return type switch
            {
                PieceType.Normal => 10,
                PieceType.RowClear => 50,
                PieceType.ColumnClear => 50,
                PieceType.Rainbow => 100,
                _ => 0
            };
        }

        private static bool IsAdjacent(int x1, int y1, int x2, int y2) =>
            (x1 == x2 && Mathf.Abs(y1 - y2) == 1) ||
            (y1 == y2 && Mathf.Abs(x1 - x2) == 1);

        private void SwapPieces(GamePieceView piece1, GamePieceView piece2)
        {
            if (_gameOver || piece1?.Data == null || piece2?.Data == null) 
                return;

            BoardSwapConfig config = new BoardSwapConfig
            {
                XDim = BoardConfig.XDim,
                YDim = BoardConfig.YDim,
                FillTime = BoardConfig.FillTime,
                Root = _root,
                PieceFactory = _pieceFactory,
                OnMoveCallback = () => _levelServiceLocator.GetForCurrentLevel().OnMove(),
                GetWorldPosition = GetWorldPosition
            };

            if (_pieceSwapService.TrySwapPieces(_pieces, piece1, piece2, config))
            {
                SyncCellsWithPieces();
                _pressedCell = null;
                _enteredCell = null;
                StartFillAsync();
            }
        }
        
        private void SetupCameraAdapter()
        {
            _cameraAdapterService.CenterCameraOnBoard();
            _cameraAdapterService.AdaptCameraToBoard(BoardConfig.XDim, BoardConfig.YDim);
        }
        
        private BoardClearConfig CreateClearConfig()
        {
            return new BoardClearConfig
            {
                XDim = BoardConfig.XDim,
                YDim = BoardConfig.YDim,
                Root = _root,
                PieceFactory = _pieceFactory,
                MatchBoardController = this,
                GetWorldPosition = GetWorldPosition
            };
        }
        
        private void SyncCellsWithPieces()
        {
            if (_cells == null || _pieces == null)
                return;
            
            for (int x = 0; x < BoardConfig.XDim; x++)
            {
                for (int y = 0; y < BoardConfig.YDim; y++)
                {
                    if (_cells[x, y] != null)
                    {
                        _cells[x, y].CurrentPieceView = _pieces[x, y];
                    }
                }
            }
        }
        
        private BoardConfig BoardConfig => _levelService.GetCurrentLevelStaticData().boardConfig;
    }
}
