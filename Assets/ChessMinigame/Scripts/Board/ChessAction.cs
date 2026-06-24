using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    public interface IChessOperation
    {
        public ChessPiece piece { get; }
        public Vector2Int position { get; }
    }

    public readonly struct ChessMove : IChessOperation
    {
        public readonly ChessPiece piece { get; }
        public readonly Vector2Int position { get; }
        public readonly Vector2Int destination;

        public ChessMove(ChessPiece piece, Vector2Int from, Vector2Int to)
        {
            this.piece = piece;
            this.position = from;
            this.destination = to;
        }
    }

    public readonly struct ChessPromotion : IChessOperation
    {
        public readonly ChessPiece piece { get; }
        public readonly Vector2Int position { get; }
        public readonly ChessPieceType promotionType;

        public ChessPromotion(ChessPiece piece, ChessPieceType promotionType, Vector2Int position)
        {
            this.piece = piece;
            this.promotionType = promotionType;
            this.position = position;
        }
    }

    public readonly struct ChessCapture : IChessOperation
    {
        public readonly ChessPiece piece { get; }
        public readonly Vector2Int position { get; }

        public ChessCapture(ChessPiece piece, Vector2Int position)
        {
            this.piece = piece;
            this.position = position;
        }
    }

    /// <summary>
    /// Represents any move a piece can make as a list of IChessOperation.
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

        public ChessAction(List<IChessOperation> operations)
        {
            actions = operations;
        }
    }
}