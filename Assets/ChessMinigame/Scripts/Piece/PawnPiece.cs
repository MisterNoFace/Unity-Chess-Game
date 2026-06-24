using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.UI;

namespace ChessGame
{
    public class PawnPiece : ChessPiece
    {        
        public PawnPiece(ChessBoard board, ChessTeam pieceTeam) : base(board, pieceTeam)
        {
            PieceName = "Pawn";
            PieceType = ChessPieceType.Pawn;
            board.OnMove += AddEnPassant;
        }
        bool oneshot = false;
        bool canEnPass = false;
        PawnPiece enpassant = null;
        private void AddEnPassant(ChessAction action)
        {
            if (enpassant == null)
                return;

            if (canEnPass)
            {
                canEnPass = false;
                oneshot = false;
                return;
            }

            if (enpassant.Pos.y == Pos.y && oneshot)
                canEnPass = true;
            /*if (action is new ChessAction(
                            new ChessCapture(enpassant, enpassant.Pos),
                            new ChessMove(this, Pos, new Vector2Int(enpassant.Pos.x, Pos.y) + ForwardDirection)
                            )
                )
                return;


            foreach (var m in action.actions)
                if (m is ChessMove move && move.piece is PawnPiece pawn)
                {
                    if (move.destination == Pos + Vector2Int.right)
                        enpassant = pawn;
                    if (move.destination == Pos + Vector2Int.left)
                        enpassant = pawn;
                }*/
        }

        protected bool IsPromotionPosition(Vector2Int position)
        {
            if (ForwardDirection == TeamForwardDirections[ChessTeam.White])
                return position.y == Board.Size - 1;
            else if (ForwardDirection == TeamForwardDirections[ChessTeam.Black])
                return position.y == 0;
            return false;
        }


        protected override ChessAction GenerateAction(Vector2Int finalPosition, bool capture = true)
        {
            List<IChessOperation> ops = base.GenerateAction(finalPosition, capture).actions;
            if (IsPromotionPosition(finalPosition))
                ops.Add(new ChessPromotion(this, ChessPieceType.Queen, finalPosition));
            return new(ops);
        }

        public override List<ChessAction> PseudoMoves()
        {
            List<ChessAction> moves = new();

            //diagonal attacks
            foreach (Vector2Int x in ThreatenedPositions())
                if (IsEnemy(Board.GetPiece(x)))
                    moves.Add(GenerateAction(x));

            //normal step
            Vector2Int step = Pos + ForwardDirection;
            moves.Add(GenerateAction(step, false));


            //long step
            if (!HasMovedOnce)
            {
                step += ForwardDirection;
                moves.Add(GenerateAction(step, false));
            }

            //en passant
            if (canEnPass)
                moves.Add(new(
                            new ChessCapture(enpassant, enpassant.Pos),
                            new ChessMove(this, Pos, new Vector2Int(enpassant.Pos.x, Pos.y) + ForwardDirection)
                            ));

            return moves;
        }


        public override List<Vector2Int> ThreatenedPositions()
        {
            Vector2Int diagRight = ForwardDirection + Vector2Int.right;
            Vector2Int diagLeft = ForwardDirection + Vector2Int.left;
            return GetSingleCoordinates(new() { diagLeft, diagRight });
        }

        public override void UpdatePosition(ChessMove move)
        {
            base.UpdatePosition(move);
            Vector2Int at = Pos + ForwardDirection * 2 + Vector2Int.left;
            if (Board.GetPiece(at) is PawnPiece p &&
                !p.HasMovedOnce && IsEnemy(p))
            {
                enpassant = p;
                oneshot = true;
            }
            at = Pos + ForwardDirection * 2 + Vector2Int.right;
            if (Board.GetPiece(at) is PawnPiece p2 &&
                !p2.HasMovedOnce && IsEnemy(p2))
            {
                enpassant = p2;
                oneshot = true;
            }
        }
    }
}