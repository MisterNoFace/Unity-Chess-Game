using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    public class RookPiece : ChessPiece
    {
        static public readonly List<Vector2Int> RookDirections = new()
        {
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up,
        };

        public RookPiece(ChessBoard board, ChessTeam pieceTeam) : base(board, pieceTeam)
        {
            PieceName = "Rook";
            PieceType = ChessPieceType.Rook;
        }

        public override List<ChessAction> PseudoMoves()
        {
            return GetSlidingMoves(RookDirections, 8);
        }

        public override List<Vector2Int> ThreatenedPositions()
        {
            return GetSlidingCoordinates(RookDirections, 8);
        }
    }
}