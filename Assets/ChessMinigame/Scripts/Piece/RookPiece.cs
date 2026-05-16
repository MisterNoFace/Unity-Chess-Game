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

        public override HashSet<Vector2Int> GetPseudoMoves()
        {
            return GetMovesInDirections(RookDirections);
        }

    }
}