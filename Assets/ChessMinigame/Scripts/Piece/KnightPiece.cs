using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    public class KnightPiece : ChessPiece
    {
        static public readonly List<Vector2Int> KnightMoves = new()
        {
            new Vector2Int(1, 2),
            new Vector2Int(-1, 2),
            new Vector2Int(1, -2),
            new Vector2Int(-1, -2),
            new Vector2Int(2, 1),
            new Vector2Int(-2, 1),
            new Vector2Int(2, -1),
            new Vector2Int(-2, -1),
        };

        public KnightPiece(ChessBoard board, ChessTeam pieceTeam) : base(board, pieceTeam)
        {
            PieceName = "Knight";
            PieceType = ChessPieceType.Knight;
        }

        public override HashSet<Vector2Int> GetPseudoMoves()
        {
            return GetMovesInPositions(KnightMoves);
        }


    }
}