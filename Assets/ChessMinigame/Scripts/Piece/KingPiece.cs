using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChessGame
{
    public class KingPiece : ChessPiece
    {
        static public readonly List<Vector2Int> KingMoves = new()
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

        public KingPiece(ChessBoard board, ChessTeam pieceTeam) : base(board, pieceTeam)
        {
            PieceName = "King";
            PieceType = ChessPieceType.King;
        }

        public override HashSet<Vector2Int> GetPseudoMoves()
        {
            HashSet<Vector2Int> moves = GetMovesInPositions(KingMoves);
            //castle
            /*if (!HasMovedOnce)
            {
                Vector2Int move = Pos + Vector2Int.left;
                while (Board.Exists(move))
                {
                    ChessPiece piece = Board.GetPiece(move);
                    if (piece == null)
                        continue;
                    if (piece is not RookPiece)
                        break;
                    else if (!piece.HasMovedOnce)
                    {
                        moves.Add(Pos + Vector2Int.left * 2);
                        break;
                    }
                    move += Vector2Int.left;
                }

                Vector2Int move = Pos + Vector2Int.left;
                while (Board.Exists(move))
                {
                    ChessPiece piece = Board.GetPiece(move);
                    if (piece == null)
                        continue;
                    if (piece is not RookPiece)
                        break;
                    else if (!piece.HasMovedOnce)
                    {
                        moves.Add(Pos + Vector2Int.left * 2);
                        break;
                    }
                    move += Vector2Int.left;
                }
            }*/
            return moves;
        }

        public override void UpdatePosition(Vector2Int newPosition)
        {
            base.UpdatePosition(newPosition);
        }




    }
}