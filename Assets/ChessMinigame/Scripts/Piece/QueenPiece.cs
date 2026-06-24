using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    public class QueenPiece : ChessPiece
    {
        static public readonly List<Vector2Int> QueenDirections = new()
        {
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up,

            Vector2Int.one,
            -Vector2Int.one,
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
        };

        public QueenPiece(ChessBoard board, ChessTeam pieceTeam) : base(board, pieceTeam)
        {
            PieceName = "Queen";
            PieceType = ChessPieceType.Queen;
        }

        public override List<ChessAction> PseudoMoves()
        {
            return GetSlidingMoves(QueenDirections, 8);
        }

        public override List<Vector2Int> ThreatenedPositions()
        {
            return GetSlidingCoordinates(QueenDirections, 8);
        }
    }
}