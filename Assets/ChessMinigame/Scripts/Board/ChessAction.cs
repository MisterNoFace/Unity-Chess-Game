using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    public interface IChessOperation
    {
        public ChessPiece piece { get; }
        public void Apply(ChessBoard board);
    }

    public readonly struct ChessMove : IChessOperation
    {
        public readonly ChessPiece piece { get; }
        public readonly Vector2Int from;
        public readonly Vector2Int to;

        public ChessMove(ChessPiece movedPiece, Vector2Int from, Vector2Int to)
        {
            this.piece = movedPiece;
            this.from = from;
            this.to = to;
        }

        public void Apply(ChessBoard board)
        {
            
        }
    }

    public readonly struct ChessPromotion : IChessOperation
    {
        public readonly ChessPiece piece { get; }
        public readonly ChessPieceType promotionType;
        public readonly Vector2Int position;

        public ChessPromotion(ChessPiece promotedPiece, ChessPieceType promotionType, Vector2Int position)
        {
            this.piece = promotedPiece;
            this.promotionType = promotionType;
            this.position = position;
        }

        public void Apply(ChessBoard board)
        {
            throw new System.NotImplementedException();
        }
    }

    public readonly struct ChessCapture : IChessOperation
    {
        public readonly ChessPiece piece { get; }
        public readonly Vector2Int position;

        public ChessCapture(ChessPiece capturedPiece, Vector2Int position)
        {
            this.piece = capturedPiece;
            this.position = position;
        }


        public void Apply(ChessBoard board)
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// represents any move a piece makes as a list of operations
    /// </summary>
    public readonly struct ChessAction
    {
        public readonly List<IChessOperation> actions;
        
        public ChessAction(params IChessOperation[] operations)
        {
            actions = new();
            foreach (IChessOperation operation in operations)
                actions.Add(operation);
        }
        public readonly void Add(IChessOperation operation) => actions.Add(operation);
    }
}