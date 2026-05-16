using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

namespace ChessGame
{
    public class ChessBoard
    {
        public readonly int Size;
        private readonly ChessPiece[,] Grid;
        public ChessTeam CurrentTurnTeam { get; private set; } = ChessTeam.White;
        private bool IsCheck = false;
        private bool IsCheckmate = false;

        #region Pieces Memory
        public KingPiece WhiteKing { get; protected set; }
        private readonly List<ChessPiece> WhitePieces = new();
        public KingPiece BlackKing { get; protected set; }
        private readonly List<ChessPiece> BlackPieces = new();
        #endregion

        #region Piece Factory
        //private delegate ChessPiece PieceInstantiator(ChessBoard board, Vector2Int startingPosition, ChessTeam pieceTeam);
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
        public delegate void TurnDelegate(ChessTeam currentTeam);
        public delegate void PieceAddDelegate(ChessPieceType pieceType, ChessPiece addedPiece, Vector2Int startingPosition);
        public delegate void CheckDelegate(ChessTeam checkedTeam);

        public event TurnDelegate OnTurnChanged;
        public event PieceAddDelegate OnPieceAdded;
        public event PieceAddDelegate OnPieceReplaced;
        public event ChessPiece.ChessMove OnPieceMoved;
        public event ChessPiece.ChessPosition OnPieceRemoved;
        public event CheckDelegate OnCheck;
        public event CheckDelegate OnStaleMate;
        public event CheckDelegate OnCheckmate;
        #endregion

        public ChessBoard(int boardSize, Vector2Int whiteKingPosition = default, Vector2Int blackKingPosition = default)
        {
            Size = boardSize;
            Grid = new ChessPiece[Size, Size];

            if (whiteKingPosition == default)
                whiteKingPosition = new(4, 0);
            if (blackKingPosition == default)
                blackKingPosition = new(4, 7);

            WhiteKing = (KingPiece) AddPiece(ChessPieceType.King, whiteKingPosition, ChessTeam.White);
            BlackKing = (KingPiece) AddPiece(ChessPieceType.King, blackKingPosition, ChessTeam.Black);
        }


        virtual public void GenerateBoard()
        {
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
            //AddPiece(ChessPieceType.King, new Vector2Int(4, 0), ChessTeam.White);

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
            //AddPiece(ChessPieceType.King, new Vector2Int(4, 7), ChessTeam.Black);
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
            (uint) position.x < Size && (uint) position.y < Size;
        
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
                        WhiteKing = (KingPiece) piece;
                    else
                        return null;
                }
                if (pieceTeam == ChessTeam.Black)
                {
                    if (BlackKing == null)
                        BlackKing = (KingPiece) piece;
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

        public ChessPiece ReplacePiece(ChessPiece replaced, ChessPieceType newType)
        {
            //avoid replacing a piece with one of the same type
            //avoid placing/replacing a piece of type king
            if (replaced == null ||replaced.Board != this || replaced.PieceType == newType || 
                replaced.PieceType == ChessPieceType.King || newType == ChessPieceType.King)
                return null;

            ChessPiece newPiece = PieceFactory[newType](this, replaced.Team);
            
            //remove the old piece
            WhitePieces.Remove(replaced);
            BlackPieces.Remove(replaced);
            Set(replaced.Pos, null);

            //replacement
            if (newPiece.Team == ChessTeam.White)
                WhitePieces.Add(newPiece);
            else
                BlackPieces.Add(newPiece);
            Set(newPiece.Pos, newPiece);
            OnPieceReplaced?.Invoke(newType, newPiece, newPiece.Pos);
            return newPiece;
        }

        public bool Capture(ChessPiece piece)
        {
            if (!(WhitePieces.Remove(piece) || BlackPieces.Remove(piece)))
                return false;
            Set(piece.Pos, null);
            OnPieceRemoved?.Invoke(piece, piece.Pos);
            return true;
        }

        public bool CapturePieceAt(Vector2Int position) => Capture(Get(position));

        private void UpdateTurn()
        {
            if (CurrentTurnTeam == ChessTeam.White)
                CurrentTurnTeam = ChessTeam.Black;
            else
                CurrentTurnTeam = ChessTeam.White;

            OnTurnChanged?.Invoke(CurrentTurnTeam);
        }


        private bool MovePiece(Vector2Int from, Vector2Int to)
        {
            ChessPiece piece = Get(from);
            if (piece == null || !Exists(from) || !Exists(to) || from == to)
                return false;
            Set(to, piece);
            //piece.UpdatePosition(to);
            Set(from, null);
            return true;
        }

        //the goat
        public bool MakeMove(Vector2Int oldPosition, Vector2Int newPosition)
        {
            IsCheck = false;

            ChessPiece piece = Get(oldPosition);
            ChessPiece captured = Get(newPosition);
            
            //move the piece to a new position
            if (!MovePiece(oldPosition, newPosition))
                //if the move didn't happen
                return false;
            
            //capture and delete any piece in the new position
            Capture(captured);


            
            UpdateTurn();
            if (IsKingUnderAttack(CurrentTurnTeam))
            {
                IsCheck = true;
                OnCheck?.Invoke(CurrentTurnTeam);
            }
            CalculateCheckmate();

            OnPieceMoved?.Invoke(piece, oldPosition, newPosition);
            return true;
        }

        
        public bool IsSquareUnderAttack(Vector2Int position, ChessTeam attackerTeam)
        {
            switch (attackerTeam)
            {
                case ChessTeam.White:
                    foreach (ChessPiece p in WhitePieces)
                        if (p.GetThreatenedPositions().Contains(position))
                            return true;
                    break;

                case ChessTeam.Black:
                    foreach (ChessPiece p in BlackPieces)
                        if (p.GetThreatenedPositions().Contains(position))
                            return true;
                    break;

                default:
                    return false;
            }
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


        private bool TryVirtualMove(ChessPiece piece, Vector2Int virtualPosition)
        {
            Vector2Int originalPosition = piece.Pos;
            ChessPiece captured = Get(virtualPosition);

            //remove any captured piece
            if (captured != null)
            {
                WhitePieces.Remove(captured);
                BlackPieces.Remove(captured);
            }

            //do virtual move
            Set(originalPosition, null);
            Set(virtualPosition, piece);
            //piece.SetVirtualPosition(virtualPosition);

            //find a check
            bool check = IsKingUnderAttack(piece.Team);

            //undo
            Set(originalPosition, piece);
            Set(virtualPosition, captured);
            //piece.UnsetVirtualPosition();

            //reinsert the captured piece
            if (captured != null)
            {
                if (captured.Team == ChessTeam.White)
                    WhitePieces.Add(captured);
                else
                    BlackPieces.Add(captured);
            }

            return check;
        }

        public HashSet<Vector2Int> GetLegalMovesOf(ChessPiece piece)
        {
            HashSet<Vector2Int> legals = new();
            if (piece.Board != this)
                return null;
            foreach (Vector2Int move in piece.GetPseudoMoves())
            {
                if (!TryVirtualMove(piece, move))
                    legals.Add(move);
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

            IsCheckmate = true;
            return IsCheckmate;
        }

    }
}
