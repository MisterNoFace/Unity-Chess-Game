using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    [Serializable]
    abstract public class ChessPiece    
    {
        //RECOGNITION PROPERTIES
        public string PieceName { get; protected set; } = "DEFAULT_CHESS_PIECE_NAME";
        public ChessPieceType PieceType { get; protected set; } = ChessPieceType.None;
        
        //PIECE DATA
        public readonly ChessBoard Board;
        public readonly ChessTeam Team;
        //public Vector2Int Pos { get; private set; }
        public Vector2Int Pos => Board.GetPositionOf(this);
        private Vector2Int VirtualPos;
        public bool HasMovedOnce { get; private set; } = false;

        public readonly Vector2Int ForwardDirection;
        public Dictionary<ChessTeam, Vector2Int> TeamForwardDirections = new()
        {
            {ChessTeam.White, Vector2Int.up},
            {ChessTeam.Black, Vector2Int.down},
        };


        #region Piece Events
        /*public delegate void ChessMove(ChessPiece movedPiece, Vector2Int previousPosition, Vector2Int currentPosition);
        public delegate void ChessPosition(ChessPiece piece, Vector2Int piecePosition);
        public event ChessMove OnPositionChanged;
        public event ChessPosition OnCapture;
        public event ChessPosition OnPromotion;*/
        public delegate void ChessActionDelegate(ChessAction action);
        public event ChessActionDelegate OnPieceAction;
        #endregion

        public ChessPiece(ChessBoard board, /*Vector2Int startingPosition,*/ ChessTeam pieceTeam)
        {
            Board = board;
            //Pos = startingPosition;
            Team = pieceTeam;
            ForwardDirection = TeamForwardDirections[pieceTeam];
        }

        public override string ToString()
        {
            switch (Team)
            {
                case ChessTeam.White:
                    return "White " + PieceName;
                case ChessTeam.Black:
                    return "Black " + PieceName;
                default:
                    return PieceName;
            }
        }

        public bool IsEnemy(ChessPiece piece)
        {
            if (piece == null)
                return false;
            return piece.Team != this.Team;
        }

        virtual public void Update(ChessAction action)
        {
            HasMovedOnce = true;
            
        }

        /*virtual public void UpdatePosition(Vector2Int newPosition)
        {
            Vector2Int previousPos = Pos;
            HasMovedOnce = true;
            Pos = newPosition;
            OnPositionChanged?.Invoke(this, previousPos, Pos);
        }

        public void SetVirtualPosition(Vector2Int position)
        {
            VirtualPos = Pos;
            Pos = position;
        }

        public void UnsetVirtualPosition()
        {
            Pos = VirtualPos;
            VirtualPos = default;
        }

        virtual public void Capture()
        {
            OnCapture?.Invoke(this, Pos);
            Pos = -Vector2Int.one;
        }

        //still considerting this shit
        virtual public void Promote(ChessPiece promotion)
        {
            OnPromotion?.Invoke(promotion, this.Pos);
            Pos = -Vector2Int.one;
        }*/
        

        #region Piece Moveset Methods
        abstract public HashSet<Vector2Int> GetPseudoMoves();
        public bool IsMoveInMoveset(Vector2Int move) => GetPseudoMoves().Contains(move);
        public bool IsMoveLegal(Vector2Int move) => Board.GetLegalMovesOf(this).Contains(move);
        virtual public HashSet<Vector2Int> GetThreatenedPositions() => GetPseudoMoves();
        

        /// <summary>
        /// Returns the moves of given directions,
        /// it can be used for pieces such as Queen, Rook, Bishop 
        /// which can travel an unlimited amount of tiles 
        /// </summary>
        /// <param name="directions">the list of directions</param>
        /// <param name="currentTilePos">the position to start checking the moves</param>
        /// <param name="board">the board object that contains all the game logic</param>
        virtual protected HashSet<Vector2Int> GetMovesInDirections(List<Vector2Int> directions)
        {
            HashSet<Vector2Int> movesInDirections = new();
            movesInDirections.Clear();
            Vector2Int move;
            foreach (Vector2Int dir in directions)
            {
                move = Pos + dir;
                while (Board.Exists(move))
                {
                    ChessPiece piece = Board.GetPiece(move);
                    if (piece == null)
                        movesInDirections.Add(move);
                    else
                    {
                        if (IsEnemy(piece))
                            movesInDirections.Add(move);
                        break;
                    }
                    move += dir;
                }
            }
            return movesInDirections;
        }

        /// <summary>
        /// Returns if the piece can move in the given coordinates,
        /// it can be used for pieces such as King, Knight 
        /// which have fixed tiles they can travel
        /// </summary>
        /// <param name="positions">the list of fixed positions</param>
        /// <param name="currentTilePos">the position to start checking the moves</param>
        /// <param name="board">the board object that contains all the game logic</param>
        virtual protected HashSet<Vector2Int> GetMovesInPositions(List<Vector2Int> positions)
        {
            HashSet<Vector2Int> availablePositions = new();
            availablePositions.Clear();
            Vector2Int move;
            foreach (Vector2Int pos in positions)
            {
                move = Pos + pos;
                if (Board.Exists(move))
                {
                    ChessPiece piece = Board.GetPiece(move);
                    if (piece == null || IsEnemy(piece))
                        availablePositions.Add(move);
                }
            }
            return availablePositions;
        }

        #endregion  

        
    }
    
}

