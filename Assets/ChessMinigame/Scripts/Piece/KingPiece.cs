using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using static UnityEngine.Audio.ProcessorInstance;

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



        public override List<ChessAction> PseudoMoves()
        {
            List<ChessAction> moves = new();

            
            if (!HasMovedOnce)
            {
                int y = Pos.y;
                int rookX = 0;
                int kingX = Pos.x;
                ChessTeam opposite = ChessTeam.White;
                if (Team == ChessTeam.White)
                    opposite = ChessTeam.Black;

                ChessPiece rook = Board.GetPiece(new(rookX, y));

                if (rook is RookPiece && !rook.HasMovedOnce)
                {
                    for (int x = rookX + 1; x < kingX - 1; x++)
                    {
                        if (Board.GetPiece(new(x, y)) != null)
                            goto NextRook;
                    }

                    Vector2Int kingPos = new(kingX - 2, y);
                    Vector2Int rookPos = new(kingX - 1, y);

                    if (Board.IsSquareUnderAttack(kingPos, opposite) || 
                        Board.IsSquareUnderAttack(rookPos, opposite) ||
                        Board.IsKingUnderAttack(Team))
                        goto NextRook;

                    ChessAction leftCastle = new(
                        new ChessMove(this, Pos, kingPos),
                        new ChessMove(rook, rook.Pos, rookPos)
                        );
                    moves.Add(leftCastle);
                }



                NextRook:
                rookX = Board.Size - 1;

                rook = Board.GetPiece(new(rookX, y));

                if (rook is RookPiece && !rook.HasMovedOnce)
                {
                    for (int x = kingX + 1; x < rookX - 1; x++)
                    {
                        if (Board.GetPiece(new(x, y)) != null)
                            goto Finale;
                    }

                    Vector2Int kingPos = new(kingX + 2, y);
                    Vector2Int rookPos = new(kingX + 1, y);

                    if (Board.IsSquareUnderAttack(kingPos, opposite) ||
                        Board.IsSquareUnderAttack(rookPos, opposite) ||
                        Board.IsKingUnderAttack(Team))
                        goto Finale;

                    ChessAction rightCastle = new(
                        new ChessMove(this, Pos, kingPos),
                        new ChessMove(rook, rook.Pos, rookPos)
                        );
                    moves.Add(rightCastle);
                }

            }

            Finale:
            moves.AddRange(GetSlidingMoves(KingMoves));
            return moves;
        }

        public override List<Vector2Int> ThreatenedPositions()
        {
            return GetSingleCoordinates(KingMoves);
        }
    }
}