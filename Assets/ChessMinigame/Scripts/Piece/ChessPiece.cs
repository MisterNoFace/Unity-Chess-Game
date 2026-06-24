using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

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
        public bool HasMovedOnce { get; private set; } = false;
        public bool Captured { get; private set; } = false;

        public Vector2Int ForwardDirection => TeamForwardDirections[Team];
        public Dictionary<ChessTeam, Vector2Int> TeamForwardDirections = new()
        {
            {ChessTeam.White, Vector2Int.up},
            {ChessTeam.Black, Vector2Int.down},
        };


        #region Piece Events
        public event ChessBoard.PieceMovement OnPositionChanged;
        public event ChessBoard.PieceCapture OnCapture;
        public event ChessBoard.PiecePromotion OnPromotion;

        #endregion

        public ChessPiece(ChessBoard board, ChessTeam pieceTeam)
        {
            Board = board;
            Team = pieceTeam;
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

        public bool IsAlly(ChessPiece piece)
        {
            if (piece == null)
                return false;
            return piece.Team == this.Team;
        }

        virtual public void UpdatePosition(ChessMove move)
        {
            if (move.piece != this)
                return;
            HasMovedOnce = true;
            OnPositionChanged?.Invoke(move);
        }

        virtual public void UpdateCapture(ChessCapture capture)
        {
            if (capture.piece != this)
                return;
            Captured = true;
            OnCapture?.Invoke(capture);
        }

        virtual public void UpdatePromotion(ChessPromotion promotion)
        {
            if (promotion.piece != this)
                return;
            OnPromotion.Invoke(promotion);
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
        abstract public List<ChessAction> PseudoMoves();
        public List<ChessAction> LegalMoves() => Board.GetLegalMovesOf(this);
        
        public Dictionary<Vector2Int, ChessAction> MovesList()
        {
            Dictionary<Vector2Int, ChessAction> positions = new();
            foreach (ChessAction action in LegalMoves())
                foreach (IChessOperation operation in action.actions)
                    if (operation is ChessMove move)
                    {
                        positions[move.destination] = action;
                        break;
                    }

            return positions;
        }

        abstract public List<Vector2Int> ThreatenedPositions();
        /*{
            List<Vector2Int> threats = new();
            foreach (ChessAction action in PseudoMoves())
                foreach (IChessOperation operation in action.actions)
                    if (operation is ChessCapture capture)
                        threats.Add(capture.position);

            return threats;
        }*/
        
        virtual protected ChessAction GenerateAction(Vector2Int finalPosition, bool capture = true)
        {
            List<IChessOperation> ops = new();
            if (!Board.Exists(finalPosition))
                return new();

            if (capture)
            {
                ops.Add(new ChessCapture(Board.GetPiece(finalPosition), finalPosition));
                ops.Add(new ChessMove(this, Pos, finalPosition));
            }

            else if (Board.GetPiece(finalPosition) == null)
                ops.Add(new ChessMove(this, Pos, finalPosition));

            return new(ops);
        }

        protected List<ChessAction> GetSlidingMoves(List<Vector2Int> directions, int repetition = 1, bool canCapture = true)
        {
            List<ChessAction> slidingActions = new();
            ChessAction action;
            Vector2Int target;

            foreach (Vector2Int dir in directions)
            {
                target = Pos + dir;
                for (int i = 0; i < repetition && Board.Exists(target); i++)
                {
                    ChessPiece captured = Board.GetPiece(target);

                    if (IsAlly(captured))
                        break;

                    action = GenerateAction(target, canCapture);
                    
                    slidingActions.Add(action);
                    target += dir;

                    if (IsEnemy(captured))
                        break;
                }
            }

            return slidingActions;
        }

        protected List<ChessAction> GetJumpMoves(List<Vector2Int> jumps, int repetition = 1, bool canCapture = true)
        {
            List<ChessAction> jumpActions = new();
            ChessAction action;
            Vector2Int target;

            int rep = 0;
            do
            {
                rep++;
                foreach (Vector2Int jump in jumps)
                {
                    target = Pos + jump * rep;
                    if (Board.Exists(target))
                    {
                        ChessPiece captured = Board.GetPiece(target);
                        if (IsAlly(captured))
                            continue;

                        action = GenerateAction(target, canCapture);

                        jumpActions.Add(action);
                    }
                }
            }
            while (rep < repetition);

            return jumpActions;
        }
        #endregion
        

        virtual protected List<Vector2Int> GetSlidingCoordinates(List<Vector2Int> directions, int repetition = 1)
        {
            List<Vector2Int> movesInDirections = new();
            movesInDirections.Clear();
            Vector2Int move;
            foreach (Vector2Int dir in directions)
            {
                move = Pos + dir;
                for (int i = 0; i < repetition && Board.Exists(move); i++)
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

        virtual protected List<Vector2Int> GetSingleCoordinates(List<Vector2Int> positions)
        {
            List<Vector2Int> availablePositions = new();
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


    }

}