using System;
using System.Collections.Generic;
using Code.Logic.Match3;
using Code.Services.Factories.Pieces;
using Code.Services.LevelConductors.Locator;
using Code.Services.Levels;
using Code.StaticData.Levels.BoardConfigs;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Code.Logic.Controllers
{
    public class MatchBoardController : IMatchBoardController
    {
        private GamePiece[,] _pieces;
        
        private GamePiece _pressedPiece;
        private GamePiece _enteredPiece;

        private bool _gameOver;
        private bool _inverse;

        private Transform _root;
        
        private readonly ILevelServiceLocator _levelServiceLocator;
        private readonly ILevelService _levelService;
        private readonly IPieceFactory _pieceFactory;
        private readonly ICameraAdapterService _cameraAdapterService;

        public MatchBoardController(
            ILevelServiceLocator levelServiceLocator, 
            ILevelService levelService,
            IPieceFactory pieceFactory,
            ICameraAdapterService cameraAdapterService)
        {
            _levelServiceLocator = levelServiceLocator;
            _levelService = levelService;
            _pieceFactory = pieceFactory;
            _cameraAdapterService = cameraAdapterService;
        }
        
        public void StartLevel()
        {
            // Find and setup camera adapter
            SetupCameraAdapter();
            
            // instantiate backgrounds
            for (int x = 0; x < BoardConfig.XDim; x++)
            {
                for (int y = 0; y < BoardConfig.YDim; y++)
                {
                    Piece background = _pieceFactory.CreatePieceByCurrentLevel(PieceType.Background, 
                        GetWorldPosition(x, y), Quaternion.identity, _root);
                }
            }

            // instantiating pieces
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

            FillAsync().Forget();
        }

        public void Dispose()
        {
            _pieces = null;
            _pressedPiece = null;
            _enteredPiece = null;
            _root = null;
            
            _gameOver = false;
            IsFilling = false;
            _inverse = false;
        }

        public void SetRootTransform(Transform rootTransform)
        {
            _root = rootTransform;
        }
        
        public bool IsFilling { get; private set; }
        
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
            if (IsAdjacent (_pressedPiece, _enteredPiece))
            {
                SwapPieces(_pressedPiece, _enteredPiece);
            }
        }
        
        public void ClearRow(int row)
        {
            for (int x = 0; x < BoardConfig.XDim; x++)
            {
                ClearPiece(x, row);
            }
        }

        public void ClearColumn(int column)
        {
            for (int y = 0; y < BoardConfig.YDim; y++)
            {
                ClearPiece(column, y);
            }
        }

        public void ClearColor(ColorType color)
        {
            for (int x = 0; x < BoardConfig.XDim; x++)
            {
                for (int y = 0; y < BoardConfig.YDim; y++)
                {
                    if ((_pieces[x, y].IsColored() && _pieces[x, y].ColorComponent.Color == color)
                        || (color == ColorType.Any))
                    {
                        ClearPiece(x, y);
                    }
                }
            }
        }

        public void GameOver() => _gameOver = true;

        public List<GamePiece> GetPiecesOfType(PieceType type)
        {
            var piecesOfType = new List<GamePiece>();

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

        private async UniTaskVoid FillAsync()
        {        
            bool needsRefill = true;
            IsFilling = true;

            while (needsRefill)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(BoardConfig.FillTime));
                while (FillStep())
                {
                    _inverse = !_inverse;
                    await UniTask.Delay(TimeSpan.FromSeconds(BoardConfig.FillTime));
                }

                needsRefill = ClearAllValidMatches();
            }

            IsFilling = false;
        }

        /// <summary>
        /// One pass through all grid cells, moving them down one grid, if possible.
        /// </summary>
        /// <returns> returns true if at least one _piece is moved down</returns>
        private bool FillStep()
        {
            bool movedPiece = false;
            // y = 0 is at the top, we ignore the last row, since it can't be moved down.
            for (int y = BoardConfig.YDim - 2; y >= 0; y--)
            {
                for (int loopX = 0; loopX < BoardConfig.XDim; loopX++)
                {
                    int x = loopX;
                    if (_inverse) { x = BoardConfig.XDim - 1 - loopX; }
                    GamePiece piece = _pieces[x, y];

                    if (!piece.IsMovable()) continue;
                
                    GamePiece pieceBelow = _pieces[x, y + 1];

                    if (pieceBelow.Type == PieceType.Empty)
                    {
                        pieceBelow.Dispose();
                        piece.MovableComponent.Move(x, y + 1, BoardConfig.FillTime);
                        _pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.Empty);
                        movedPiece = true;
                    }
                    else
                    {
                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if (diag == 0) continue;
                        
                            int diagX = x + diag;

                            if (_inverse)
                            {
                                diagX = x - diag;
                            }

                            if (diagX < 0 || diagX >= BoardConfig.XDim) continue;
                        
                            GamePiece diagonalPiece = _pieces[diagX, y + 1];

                            if (diagonalPiece.Type != PieceType.Empty) continue;
                        
                            bool hasPieceAbove = true;

                            for (int aboveY = y; aboveY >= 0; aboveY--)
                            {
                                GamePiece pieceAbove = _pieces[diagX, aboveY];

                                if (pieceAbove.IsMovable())
                                {
                                    break;
                                }
                                else if (/*!pieceAbove.IsMovable() && */pieceAbove.Type != PieceType.Empty)
                                {
                                    hasPieceAbove = false;
                                    break;
                                }
                            }

                            if (hasPieceAbove) continue;
                        
                            diagonalPiece.Dispose();
                            piece.MovableComponent.Move(diagX, y + 1, BoardConfig.FillTime);
                            _pieces[diagX, y + 1] = piece;
                            SpawnNewPiece(x, y, PieceType.Empty);
                            movedPiece = true;
                            break;
                        }
                    }
                }
            }

            // the highest row (0) is a special case, we must fill it with new pieces if empty
            for (int x = 0; x < BoardConfig.XDim; x++)
            {
                GamePiece pieceBelow = _pieces[x, 0];

                if (pieceBelow.Type != PieceType.Empty) continue;
            
                pieceBelow.Dispose();
                Piece newPiece =  _pieceFactory.CreatePieceByCurrentLevel(PieceType.Normal, 
                    GetWorldPosition(x, -1), Quaternion.identity, _root);

                _pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                _pieces[x, 0].Init(x, -1, this, PieceType.Normal);
                _pieces[x, 0].MovableComponent.Move(x, 0, BoardConfig.FillTime);
                _pieces[x, 0].ColorComponent.SetColor((ColorType)Random.Range(0, _pieces[x, 0].ColorComponent.NumColors));
                movedPiece = true;
            }

            return movedPiece;
        }

        private GamePiece SpawnNewPiece(int x, int y, PieceType type)
        {
            Piece newPiece =  _pieceFactory.CreatePieceByCurrentLevel(type, GetWorldPosition(x, y), 
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
            if (_gameOver) { return; }

            if (!piece1.IsMovable() || !piece2.IsMovable()) return;
        
            _pieces[piece1.X, piece1.Y] = piece2;
            _pieces[piece2.X, piece2.Y] = piece1;

            if (GetMatch(piece1, piece2.X, piece2.Y) != null || 
                GetMatch(piece2, piece1.X, piece1.Y) != null ||
                piece1.Type == PieceType.Rainbow ||
                piece2.Type == PieceType.Rainbow)
            {
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MovableComponent.Move(piece2.X, piece2.Y, BoardConfig.FillTime);
                piece2.MovableComponent.Move(piece1X, piece1Y, BoardConfig.FillTime);

                if (piece1.Type == PieceType.Rainbow && piece1.IsClearable() && piece2.IsColored())
                {
                    ClearColorPiece clearColor = piece1.GetComponent<ClearColorPiece>();

                    if (clearColor)
                    {
                        clearColor.Color = piece2.ColorComponent.Color;
                    }

                    ClearPiece(piece1.X, piece1.Y);
                }

                if (piece2.Type == PieceType.Rainbow && piece2.IsClearable() && piece1.IsColored())
                {
                    ClearColorPiece clearColor = piece2.GetComponent<ClearColorPiece>();

                    if (clearColor)
                    {
                        clearColor.Color = piece1.ColorComponent.Color;
                    }

                    ClearPiece(piece2.X, piece2.Y);
                }

                ClearAllValidMatches();

                // special pieces get cleared, event if they are not matched
                if (piece1.Type == PieceType.RowClear || piece1.Type == PieceType.ColumnClear)
                {
                    ClearPiece(piece1.X, piece1.Y);
                }

                if (piece2.Type == PieceType.RowClear || piece2.Type == PieceType.ColumnClear)
                {
                    ClearPiece(piece2.X, piece2.Y);
                }

                _pressedPiece = null;
                _enteredPiece = null;

                FillAsync().Forget();

                _levelServiceLocator.GetForCurrentLevel().OnMove();
            }
            else
            {
                _pieces[piece1.X, piece1.Y] = piece1;
                _pieces[piece2.X, piece2.Y] = piece2;
            }
        }

        private bool ClearAllValidMatches()
        {
            bool needsRefill = false;

            for (int y = 0; y < BoardConfig.YDim; y++)
            {
                for (int x = 0; x < BoardConfig.XDim; x++)
                {
                    if (!_pieces[x, y].IsClearable()) continue;
                
                    List<GamePiece> match = GetMatch(_pieces[x, y], x, y);

                    if (match == null) continue;
                
                    PieceType specialPieceType = PieceType.Count;
                    GamePiece randomPiece = match[Random.Range(0, match.Count)];
                    int specialPieceX = randomPiece.X;
                    int specialPieceY = randomPiece.Y;

                    // Spawning special pieces
                    if (match.Count == 4)
                    {
                        if (_pressedPiece == null || _enteredPiece == null)
                        {
                            specialPieceType = (PieceType) Random.Range((int) PieceType.RowClear, (int) PieceType.ColumnClear);
                        }
                        else if (_pressedPiece.Y == _enteredPiece.Y)
                        {
                            specialPieceType = PieceType.RowClear;
                        }
                        else
                        {
                            specialPieceType = PieceType.ColumnClear;
                        }
                    } // Spawning a rainbow _piece
                    else if (match.Count >= 5)
                    {
                        specialPieceType = PieceType.Rainbow;
                    }

                    foreach (var gamePiece in match)
                    {
                        if (!ClearPiece(gamePiece.X, gamePiece.Y)) continue;
                    
                        needsRefill = true;

                        if (gamePiece != _pressedPiece && gamePiece != _enteredPiece) continue;
                    
                        specialPieceX = gamePiece.X;
                        specialPieceY = gamePiece.Y;
                    }

                    // Setting their colors
                    if (specialPieceType == PieceType.Count) continue;
                    
                    _pieces[specialPieceX, specialPieceY].Dispose();
                    
                    GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPieceType);

                    if ((specialPieceType == PieceType.RowClear || specialPieceType == PieceType.ColumnClear) 
                        && newPiece.IsColored() && match[0].IsColored())
                    {
                        newPiece.ColorComponent.SetColor(match[0].ColorComponent.Color);
                    }
                    else if (specialPieceType == PieceType.Rainbow && newPiece.IsColored())
                    {
                        newPiece.ColorComponent.SetColor(ColorType.Any);
                    }
                }
            }

            return needsRefill;
        }

        private List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
        {
            if (!piece.IsColored()) return null;
            var color = piece.ColorComponent.Color;
            var horizontalPieces = new List<GamePiece>();
            var verticalPieces = new List<GamePiece>();
            var matchingPieces = new List<GamePiece>();

            // First check horizontal
            horizontalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < BoardConfig.XDim; xOffset++)
                {
                    int x;

                    if (dir == 0)
                    { // Left
                        x = newX - xOffset;
                    }
                    else
                    { // right
                        x = newX + xOffset;                        
                    }

                    // out-of-bounds
                    if (x < 0 || x >= BoardConfig.XDim) { break; }

                    // _piece is the same color?
                    if (_pieces[x, newY].IsColored() && _pieces[x, newY].ColorComponent.Color == color)
                    {
                        horizontalPieces.Add(_pieces[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (horizontalPieces.Count >= 3)
            {
                matchingPieces.AddRange(horizontalPieces);
            }

            // Traverse vertically if we found a match (for L and T shape)
            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++ )
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < BoardConfig.YDim; yOffset++)                        
                        {
                            int y;
                            
                            if (dir == 0)
                            { // Up
                                y = newY - yOffset;
                            }
                            else
                            { // Down
                                y = newY + yOffset;
                            }

                            if (y < 0 || y >= BoardConfig.YDim)
                            {
                                break;
                            }

                            if (_pieces[horizontalPieces[i].X, y].IsColored() && _pieces[horizontalPieces[i].X, y].ColorComponent.Color == color)
                            {
                                verticalPieces.Add(_pieces[horizontalPieces[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (verticalPieces.Count < 2)
                    {
                        verticalPieces.Clear();
                    }
                    else
                    {
                        matchingPieces.AddRange(verticalPieces);
                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }


            // Didn't find anything going horizontally first,
            // so now check vertically
            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < BoardConfig.XDim; yOffset++)
                {
                    int y;

                    if (dir == 0)
                    { // Up
                        y = newY - yOffset;
                    }
                    else
                    { // Down
                        y = newY + yOffset;                        
                    }

                    // out-of-bounds
                    if (y < 0 || y >= BoardConfig.YDim) { break; }

                    // _piece is the same color?
                    if (_pieces[newX, y].IsColored() && _pieces[newX, y].ColorComponent.Color == color)
                    {
                        verticalPieces.Add(_pieces[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (verticalPieces.Count >= 3)
            {
                matchingPieces.AddRange(verticalPieces);
            }

            // Traverse horizontally if we found a match (for L and T shape)
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < BoardConfig.YDim; xOffset++)
                        {
                            int x;

                            if (dir == 0)
                            { // Left
                                x = newX - xOffset;
                            }
                            else
                            { // Right
                                x = newX + xOffset;
                            }

                            if (x < 0 || x >= BoardConfig.XDim)
                            {
                                break;
                            }

                            if (_pieces[x, verticalPieces[i].Y].IsColored() && _pieces[x, verticalPieces[i].Y].ColorComponent.Color == color)
                            {
                                horizontalPieces.Add(_pieces[x, verticalPieces[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (horizontalPieces.Count < 2)
                    {
                        horizontalPieces.Clear();
                    }
                    else
                    {
                        matchingPieces.AddRange(horizontalPieces);
                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }

            return null;
        }

        private bool ClearPiece(int x, int y)
        {
            if (!_pieces[x, y].IsClearable() || _pieces[x, y].ClearableComponent.IsBeingCleared) return false;
        
            _pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.Empty);

            ClearObstacles(x, y);

            return true;

        }

        private void ClearObstacles(int x, int y)
        {
            for (int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++)
            {
                if (adjacentX == x || adjacentX < 0 || adjacentX >= BoardConfig.XDim) continue;

                if (_pieces[adjacentX, y].Type != PieceType.Bubble || !_pieces[adjacentX, y].IsClearable()) continue;
            
                _pieces[adjacentX, y].ClearableComponent.Clear();
                SpawnNewPiece(adjacentX, y, PieceType.Empty);
            }

            for (int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
            {
                if (adjacentY == y || adjacentY < 0 || adjacentY >= BoardConfig.YDim) continue;

                if (_pieces[x, adjacentY].Type != PieceType.Bubble || !_pieces[x, adjacentY].IsClearable()) continue;
            
                _pieces[x, adjacentY].ClearableComponent.Clear();
                SpawnNewPiece(x, adjacentY, PieceType.Empty);
            }
        }
        
        private void SetupCameraAdapter()
        {
            _cameraAdapterService.CenterCameraOnBoard();
            _cameraAdapterService.AdaptCameraToBoard(BoardConfig.XDim, BoardConfig.YDim);
        }
        
        private BoardConfig BoardConfig => _levelService.GetCurrentLevelStaticData().boardConfig;
    }
}
