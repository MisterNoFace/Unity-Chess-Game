using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    public class BishopPiece : ChessPiece
    {
        static public readonly List<Vector2Int> BishopDirections = new()
        {
            Vector2Int.one,
            -Vector2Int.one,
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
        };

        public BishopPiece(ChessBoard board, ChessTeam pieceTeam) : base(board, pieceTeam)
        {
            PieceName = "Bishop";
            PieceType = ChessPieceType.Bishop;
        }

        public override HashSet<Vector2Int> GetPseudoMoves()
        {
            return GetMovesInDirections(BishopDirections);
        }


    }
}