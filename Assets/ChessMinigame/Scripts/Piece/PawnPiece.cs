using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    public class PawnPiece : ChessPiece
    {        
        public PawnPiece(ChessBoard board, ChessTeam pieceTeam) : base(board, pieceTeam)
        {
            PieceName = "Pawn";
            PieceType = ChessPieceType.Pawn;
        }

        public override HashSet<Vector2Int> GetPseudoMoves()
        {
            HashSet<Vector2Int> moves = new();
            foreach (Vector2Int move in GetThreatenedPositions())
                if (IsEnemy(Board.GetPiece(move)))
                    moves.Add(move);

            Vector2Int step = Pos + ForwardDirection;
            if (Board.Exists(step) && Board.GetPiece(step) == null)
                moves.Add(step);
            else
                return moves;

            if (!HasMovedOnce)
            {
                step += ForwardDirection;
                if (Board.Exists(step) && Board.GetPiece(step) == null)
                    moves.Add(step);
            }
            
            return moves;
        }


        public override HashSet<Vector2Int> GetThreatenedPositions()
        {
            HashSet<Vector2Int> threatenedPositions = new();

            Vector2Int diagRight = Pos + ForwardDirection + Vector2Int.right;
            if (Board.Exists(diagRight))
                threatenedPositions.Add(diagRight);

            Vector2Int diagLeft = Pos + ForwardDirection + Vector2Int.left;
            if (Board.Exists(diagLeft))
                threatenedPositions.Add(diagLeft);

            return threatenedPositions;
        }

        public override void Update(Vector2Int newPosition)
        {
            base.UpdatePosition(newPosition);
            if (CheckPromotion())
                OnPawnPromotion?.Invoke(this, Pos);
        }

        public bool CheckPromotion()
        {
            if (ForwardDirection == TeamForwardDirections[ChessTeam.White])
                return Pos.y == Board.Size - 1;
            else if (ForwardDirection == TeamForwardDirections[ChessTeam.Black])
                return Pos.y == 0;
            return false;
        }

        public ChessPiece Promote(ChessPieceType promotionType)
        {
            if (promotionType == ChessPieceType.King || promotionType == ChessPieceType.Pawn)
                return null;
            
            return Board.ReplacePiece(this, promotionType);
        }
    }
}