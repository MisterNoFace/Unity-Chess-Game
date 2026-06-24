using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    public class ChessBoard
    {
        public readonly int Size;
        private readonly ChessPiece[,] Grid;
        public ChessTeam CurrentTurnTeam { get; private set; } = ChessTeam.White;
        private bool IsCheck = false;

        #region Pieces Memory
        public KingPiece WhiteKing { get; protected set; }
        private readonly List<ChessPiece> WhitePieces = new();
        public KingPiece BlackKing { get; protected set; }
        private readonly List<ChessPiece> BlackPieces = new();
        #endregion

        #region Piece Factory
        private delegate ChessPiece PieceInstantiator(ChessBoard board, ChessTeam pieceTeam);
        private readonly Dictionary<ChessPieceType, PieceInstantiator> PieceFactory = new()
        {
            {ChessPieceType.Pawn, (board, team) => new PawnPiece(board, team)},
            {ChessPieceType.Rook, (board, team) => new RookPiece(board, team)},
            {ChessPieceType.Knight, (board, team) => new KnightPiece(board, team)},
            {ChessPieceType.Bishop, (board, team) => new BishopPiece(board, team)},
            {ChessPieceType.Queen, (board, team) => new QueenPiece(board, team)},
            {ChessPieceType.King, (board, team) => new KingPiece(board, team)},
        };
        #endregion

        #region Board Events
        //ChessAction operations
        public delegate void PieceAddition(ChessPieceType pieceType, ChessPiece addedPiece, Vector2Int startingPosition);
        public delegate void PieceMovement(ChessMove movement);
        public delegate void PieceCapture(ChessCapture capture);
        public delegate void PiecePromotion(ChessPromotion promotion);

        public event PieceAddition OnPieceAdded;
        public event PieceMovement OnPieceMoved;
        public event PieceCapture OnPieceCaptured;
        public event PiecePromotion OnPieceReplaced;

        //Game events
        public delegate void ChessActionDelegate(ChessAction action);
        public delegate void TurnDelegate(ChessTeam currentTeam);
        public delegate void CheckDelegate(ChessTeam checkedTeam);

        public event ChessActionDelegate OnMove;
        public event TurnDelegate OnTurnChanged;
        public event CheckDelegate OnCheck;
        public event CheckDelegate OnStaleMate;
        public event CheckDelegate OnCheckmate;
        #endregion

        public ChessBoard(int boardSize)
        {
            Size = boardSize;
            Grid = new ChessPiece[Size, Size];
        }

        public void Init(Vector2Int whiteKingPosition = default, Vector2Int blackKingPosition = default)
        {
            if (whiteKingPosition == default)
                whiteKingPosition = new(4, 0);
            if (blackKingPosition == default)
                blackKingPosition = new(4, 7);
            if (WhiteKing == null)
                WhiteKing = (KingPiece)AddPiece(ChessPieceType.King, whiteKingPosition, ChessTeam.White);
            if (BlackKing == null)
                BlackKing = (KingPiece)AddPiece(ChessPieceType.King, blackKingPosition, ChessTeam.Black);
        }

        virtual public void GenerateBoard()
        {
            Init();

            //WHITE PIECES
            AddPiece(ChessPieceType.Rook, new Vector2Int(0, 0), ChessTeam.White);
            for (int i = 0; i < Size; i++)
                AddPiece(ChessPieceType.Pawn, new Vector2Int(i, 1), ChessTeam.White);

            AddPiece(ChessPieceType.Rook, new Vector2Int(7, 0), ChessTeam.White);
            AddPiece(ChessPieceType.Knight, new Vector2Int(1, 0), ChessTeam.White);
            AddPiece(ChessPieceType.Knight, new Vector2Int(6, 0), ChessTeam.White);
            AddPiece(ChessPieceType.Bishop, new Vector2Int(2, 0), ChessTeam.White);
            AddPiece(ChessPieceType.Bishop, new Vector2Int(5, 0), ChessTeam.White);
            AddPiece(ChessPieceType.Queen, new Vector2Int(3, 0), ChessTeam.White);

            //BLACK PIECES
            for (int i = 0; i < Size; i++)
                AddPiece(ChessPieceType.Pawn, new Vector2Int(i, 6), ChessTeam.Black);

            AddPiece(ChessPieceType.Rook, new Vector2Int(0, 7), ChessTeam.Black);
            AddPiece(ChessPieceType.Rook, new Vector2Int(7, 7), ChessTeam.Black);
            AddPiece(ChessPieceType.Knight, new Vector2Int(1, 7), ChessTeam.Black);
            AddPiece(ChessPieceType.Knight, new Vector2Int(6, 7), ChessTeam.Black);
            AddPiece(ChessPieceType.Bishop, new Vector2Int(2, 7), ChessTeam.Black);
            AddPiece(ChessPieceType.Bishop, new Vector2Int(5, 7), ChessTeam.Black);
            AddPiece(ChessPieceType.Queen, new Vector2Int(3, 7), ChessTeam.Black);       
        }
        public void ResetBoard()
        {
            CurrentTurnTeam = ChessTeam.White;
            WhitePieces.Clear();
            BlackPieces.Clear();
            WhiteKing = null;
            BlackKing = null;
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    Set(new(i, j), null);
        }

        public bool Exists(Vector2Int position) =>
            (uint)position.x < Size && (uint)position.y < Size;

        public Vector2Int GetPositionOf(ChessPiece piece)
        {
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    if (Grid[x, y] == piece)
                        return new(x, y);
            return -Vector2Int.one;
        }

        private ChessPiece Get(Vector2Int position)
        {
            if (!Exists(position))
                return null;
            return Grid[position.x, position.y];
        }
        public ChessPiece GetPiece(Vector2Int piecePosition) => Get(piecePosition);


        private void Set(Vector2Int position, ChessPiece piece)
        {
            if (Exists(position))
                Grid[position.x, position.y] = piece;
        }



        public ChessPiece AddPiece(ChessPieceType pieceType, Vector2Int startingPosition, ChessTeam pieceTeam)
        {
            ChessPiece piece = PieceFactory[pieceType](this, pieceTeam);

            //avoid having two kings in the same team
            if (pieceType == ChessPieceType.King)
            {
                if (pieceTeam == ChessTeam.White)
                {
                    if (WhiteKing == null)
                        WhiteKing = (KingPiece)piece;
                    else
                        return null;
                }
                if (pieceTeam == ChessTeam.Black)
                {
                    if (BlackKing == null)
                        BlackKing = (KingPiece)piece;
                    else
                        return null;
                }
            }

            if (piece.Team == ChessTeam.White)
                WhitePieces.Add(piece);
            else
                BlackPieces.Add(piece);

            Set(startingPosition, piece);
            OnPieceAdded?.Invoke(pieceType, piece, startingPosition);
            return piece;
        }

        private void UpdateTurn()
        {
            if (CurrentTurnTeam == ChessTeam.White)
                CurrentTurnTeam = ChessTeam.Black;
            else
                CurrentTurnTeam = ChessTeam.White;

            OnTurnChanged?.Invoke(CurrentTurnTeam);
        }


        #region Chess Operations
        public bool Capture(ChessCapture capture)
        {
            if (capture.piece == null)
                return false;

            WhitePieces.Remove(capture.piece);
            BlackPieces.Remove(capture.piece);

            Set(capture.position, null);
            capture.piece.UpdateCapture(capture);
            OnPieceCaptured?.Invoke(capture);
            return true;
        }

        public bool Move(ChessMove move)
        {
            Set(move.destination, move.piece);
            Set(move.position, null);
            move.piece.UpdatePosition(move);
            OnPieceMoved?.Invoke(move);
            return true;
        }

        public bool Promote(ChessPromotion promotion)
        {
            ChessPiece promoted = promotion.piece;
            if (promoted == null || 
                promotion.promotionType == ChessPieceType.King || 
                promotion.promotionType == promotion.piece.PieceType)
                return false;

            ChessPiece newPiece = PieceFactory[promotion.promotionType](this, promoted.Team);
            Set(promotion.position, newPiece);

            if (promoted.Team == ChessTeam.White)
            {
                WhitePieces.Remove(promoted);
                WhitePieces.Add(newPiece);
            }
            else
            {
                BlackPieces.Remove(promoted);
                BlackPieces.Add(newPiece);
            }

            promotion.piece.UpdatePromotion(promotion);
            OnPieceReplaced?.Invoke(promotion);
            return true;
        }

        public void MakeMove(ChessAction action)
        {
            IsCheck = false;

            foreach (IChessOperation op in action.actions)
            {
                if (op is ChessMove move)
                    Move(move);
                else if (op is ChessCapture capture)
                    Capture(capture);
                else if (op is ChessPromotion promotion)
                    Promote(promotion);
            }

            UpdateTurn();
            if (IsKingUnderAttack(CurrentTurnTeam))
            {
                IsCheck = true;
                OnCheck?.Invoke(CurrentTurnTeam);
            }
            CalculateCheckmate();
            OnMove?.Invoke(action);
        }
        #endregion


        #region Virtual Operations

        Stack<ChessAction> VirtualActions = new();
        private void VirtualCapture(ChessCapture capture)
        {
            WhitePieces.Remove(capture.piece);
            BlackPieces.Remove(capture.piece);
            Set(capture.position, null);
        }

        private void VirtualMove(ChessMove move)
        {
            Set(move.destination, move.piece);
            Set(move.position, null);
        }

        private void VirtualPromote(ChessPromotion promotion)
        {
            ChessPiece promoted = promotion.piece;
            
            ChessPiece newPiece = PieceFactory[promotion.promotionType](this, promoted.Team);
            Set(promotion.position, newPiece);
            
            if (promoted.Team == ChessTeam.White)
            {
                WhitePieces.Remove(promoted);
                WhitePieces.Add(newPiece);
            }
            else
            {
                BlackPieces.Remove(promoted);
                BlackPieces.Add(newPiece);
            }

        }

        private void VirtualAction(ChessAction action)
        {
            foreach (IChessOperation op in action.actions)
            {
                if (op is ChessMove move)
                    VirtualMove(move);
                else if (op is ChessCapture capture)
                    VirtualCapture(capture);
                else if (op is ChessPromotion promotion)
                    VirtualPromote(promotion);
            }
            VirtualActions.Push(action);
        }

        private void UndoVirtualCapture(ChessCapture capture)
        {
            if (capture.piece == null)
                return;
            if (capture.piece.Team == ChessTeam.White)
                WhitePieces.Add(capture.piece);
            else
                BlackPieces.Add(capture.piece);
            Set(capture.position, capture.piece);
        }

        private void UndoVirtualMove(ChessMove move)
        {
            Set(move.position, move.piece);
            if (Get(move.destination) == move.piece)
                Set(move.destination, null);
        }

        private void UndoVirtualPromote(ChessPromotion promotion)
        {
            ChessPiece promoted = Get(promotion.position);
            Set(promotion.position, promotion.piece);
            if (promoted.Team == ChessTeam.White)
            {
                WhitePieces.Remove(promoted);
                WhitePieces.Add(promotion.piece);
            }
            else
            {
                BlackPieces.Remove(promoted);
                BlackPieces.Add(promotion.piece);
            }

        }


        private void UndoVirtualAction()
        {
            if (!VirtualActions.TryPop(out ChessAction action))
                return;
            
            
            for (int i = action.actions.Count - 1; i >= 0; i--)
            {
                IChessOperation op = action.actions[i];
                if (op is ChessMove move)
                    UndoVirtualMove(move);
                else if (op is ChessCapture capture)
                    UndoVirtualCapture(capture);
                else if (op is ChessPromotion promotion)
                    UndoVirtualPromote(promotion);
            }
        }
        #endregion


        public bool IsSquareUnderAttack(Vector2Int position, ChessTeam attackerTeam)
        {
            List<ChessPiece> team = WhitePieces;
            if (attackerTeam != ChessTeam.White)
                team = BlackPieces;

            foreach (ChessPiece piece in team)
                if (piece.ThreatenedPositions().Contains(position))
                    return true;
            return false;
        }

        public bool IsKingUnderAttack(ChessTeam kingTeam)
        {
            if (kingTeam == ChessTeam.White)
                return IsSquareUnderAttack(WhiteKing.Pos, ChessTeam.Black);
            else if (kingTeam == ChessTeam.Black)
                return IsSquareUnderAttack(BlackKing.Pos, ChessTeam.White);
            else
                return false;
        }


        public List<ChessAction> GetLegalMovesOf(ChessPiece piece)
        {
            List<ChessAction> legals = new();
            if (piece.Board != this)
                return null;

            foreach (ChessAction action in piece.PseudoMoves())
            {
                VirtualAction(action);
                if (!IsKingUnderAttack(piece.Team))
                    legals.Add(action);
                UndoVirtualAction();
            }

            return legals;
        }

        private bool CalculateCheckmate()
        {
            List<ChessPiece> team = WhitePieces;
            if (CurrentTurnTeam != ChessTeam.White)
                team = BlackPieces;

            foreach (ChessPiece piece in team)
                if (GetLegalMovesOf(piece).Count != 0)
                    return false;

            if (IsCheck)
                OnCheckmate?.Invoke(CurrentTurnTeam);
            else
                OnStaleMate?.Invoke(CurrentTurnTeam);

            return true;
        }

    }
}
