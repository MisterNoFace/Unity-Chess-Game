/*using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChessGame
{
    abstract public class ChessLogic
    {
        abstract public List<Vector2Int> GetLegalMoves(Vector2Int currentTilePos, HashSet<Vector2Int> availableTiles);
        protected List<Vector2Int> GetMovesInDirections(List<Vector2Int> directions, Vector2Int currentTilePos, HashSet<Vector2Int> availableTiles)
        {
            List<Vector2Int> movesInDirections = new();
            Vector2Int move;
            foreach (Vector2Int dir in directions)
            {
                move = currentTilePos + dir;
                while (availableTiles.Contains(move))
                {
                    movesInDirections.Add(move);
                    move += dir;
                }
            }

            return movesInDirections;
        }

        protected List<Vector2Int> GetAvailablePositions(List<Vector2Int> positions, Vector2Int currentTilePos, HashSet<Vector2Int> availableTiles)
        {
            List<Vector2Int> availablePositions = new();
            Vector2Int move;
            foreach (Vector2Int pos in positions)
            {
                move = currentTilePos + pos;
                if (availableTiles.Contains(move))
                {
                    availablePositions.Add(move);
                }
            }

            return availablePositions;
        }
    }

    public class PawnLogic : ChessLogic
    {
        public bool FirstMove { get; private set; } = true;
        public Vector2Int ForwardDirection = Vector2Int.up;

        public override List<Vector2Int> GetLegalMoves(Vector2Int currentTilePos, HashSet<Vector2Int> availableTiles)
        {
            List<Vector2Int> legalMoves = new();

            Vector2Int fwdTile = currentTilePos + ForwardDirection;
            if (availableTiles.Contains(fwdTile))
                legalMoves.Add(fwdTile);
            
            if (FirstMove)
            {
                fwdTile += ForwardDirection;
                if (availableTiles.Contains(fwdTile))
                {
                    legalMoves.Add(fwdTile);
                    FirstMove = false;
                }
                    
            }

            return legalMoves;
        }
        //todo
        public bool HasReachedTheEnd()
        {
            return false;
        }
    }

    public class RookLogic : ChessLogic
    {
        public List<Vector2Int> RookDirections = new()
        {
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up,
        };
        public override List<Vector2Int> GetLegalMoves(Vector2Int currentTilePos, HashSet<Vector2Int> availableTiles)
        {
            return GetMovesInDirections(RookDirections, currentTilePos, availableTiles);
        }
    }

    public class BishopLogic : ChessLogic
    {
        public List<Vector2Int> BishopDirections = new()
        {
            Vector2Int.one,
            -Vector2Int.one,
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
        };
        public override List<Vector2Int> GetLegalMoves(Vector2Int currentTilePos, HashSet<Vector2Int> availableTiles)
        {
            return GetMovesInDirections(BishopDirections, currentTilePos, availableTiles);
        }
    }

    public class QueenLogic : ChessLogic
    {
        public List<Vector2Int> QueenDirections = new()
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

        public override List<Vector2Int> GetLegalMoves(Vector2Int currentTilePos, HashSet<Vector2Int> availableTiles)
        {
            return GetMovesInDirections(QueenDirections, currentTilePos, availableTiles);
        }
    }

    public class KingLogic : ChessLogic
    {
        public readonly List<Vector2Int> KingMoves = new()
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

        public override List<Vector2Int> GetLegalMoves(Vector2Int currentTilePos, HashSet<Vector2Int> availableTiles)
        {
            return GetAvailablePositions(KingMoves, currentTilePos, availableTiles);
        }
    }

    public class KnightLogic : ChessLogic
    {
        public List<Vector2Int> KnightMoves = new()
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
        public override List<Vector2Int> GetLegalMoves(Vector2Int currentTilePos, HashSet<Vector2Int> availableTiles)
        {
            return GetAvailablePositions(KnightMoves, currentTilePos, availableTiles);
        }
    }

}*/