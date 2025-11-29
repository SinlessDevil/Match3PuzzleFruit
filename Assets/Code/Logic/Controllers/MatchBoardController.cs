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
        private GamePiece[,] _pieces;
        
        private GamePiece _pressedPiece;
        private GamePiece _enteredPiece;

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
        }
        
        public void StartLevel()
        {
            SetupCameraAdapter();
            
            for (int x = 0; x < BoardConfig.XDim; x++)
            {
                for (int y = 0; y < BoardConfig.YDim; y++)
                {
                    Piece background = _pieceFactory.CreatePieceByCurrentLevel(PieceType.Background, 
                        GetWorldPosition(x, y), Quaternion.identity, _root);
                }
            }

            _pieces = new GamePiece[BoardConfig.XDim, BoardConfig.YDim];

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
            
            _pieces = null;
            _pressedPiece = null;
            _enteredPiece = null;
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
        
        public void PressPiece(GamePiece piece) => _pressedPiece = piece;

        public void EnterPiece(GamePiece piece) => _enteredPiece = piece;
        
        public void ReleasePiece()
        {
            if (IsAdjacent(_pressedPiece, _enteredPiece))
            {
                SwapPieces(_pressedPiece, _enteredPiece);
            }
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

        public List<GamePiece> GetPiecesOfType(PieceType type)
        {
            List<GamePiece> piecesOfType = new List<GamePiece>();

            for (int x = 0; x < BoardConfig.XDim; x++)
            {
                for (int y = 0; y < BoardConfig.YDim; y++)
                {
                    if (_pieces[x, y].Type == type)
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
            
            _boardFillService.FillAsync(_pieces, config, _fillCancellationTokenSource.Token).Forget();
        }

        private GamePiece SpawnNewPiece(int x, int y, PieceType type)
        {
            Piece newPiece = _pieceFactory.CreatePieceByCurrentLevel(type, GetWorldPosition(x, y), 
                Quaternion.identity, _root);
            _pieces[x, y] = newPiece.GetComponent<GamePiece>();
            _pieces[x, y].Init(x, y, this, type);

            return _pieces[x, y];
        }

        private static bool IsAdjacent(GamePiece piece1, GamePiece piece2) =>
            (piece1.X == piece2.X && Mathf.Abs(piece1.Y - piece2.Y) == 1) ||
            (piece1.Y == piece2.Y && Mathf.Abs(piece1.X - piece2.X) == 1);

        private void SwapPieces(GamePiece piece1, GamePiece piece2)
        {
            if (_gameOver) 
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
                _pressedPiece = null;
                _enteredPiece = null;
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
        
        private BoardConfig BoardConfig => _levelService.GetCurrentLevelStaticData().boardConfig;
    }
}
