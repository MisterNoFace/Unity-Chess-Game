/*using Scripts.Grid;
using Scripts.Team;
using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor.UI;
using UnityEngine;

namespace ChessGame
{
    public enum ChessTileModifiers
    {
        None,
        Assembler,
        Teleport,
        Primer,
        Transformer,
        Terminator,
        BitShift,
        Spawner,
    }

    public class ChessTile
    {
        public ChessBoard Board { get; private set; }
        public Vector2Int Pos { get; private set; }
        public ChessPiece Piece { get; private set; }

        //public List<ChessTileModifiers> Modifiers = new();

        public ChessTile(ChessBoard board, Vector2Int position)
        {
            if (board == null)
                return;
            
            Board = board;
            Pos = position;

            Board.BoardTiles.SetTile(Pos, this);
        }

        public void ClearPiece()
        {
            Piece = null;
        }

        public void SetPiece(ChessPiece piece)
        {
            if (piece == null)
                return;

            if (Piece != null)
                Piece.Capture();

            ChessTile previous = piece.Tile;
            if (previous != null)
                previous.ClearPiece();
            
            Piece = piece;

            //Piece.transform.SetParent(PiecePivot);
            //Piece.transform.position = PiecePivot.position;
            //Piece.MoveTo(this);
        }

        virtual public void ApplyEffect(ChessBoard board) { }
    }
}*/