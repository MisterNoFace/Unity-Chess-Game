using System;
using System.Collections.Generic;
using System.Net;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ChessGame
{
    public class ChessBoardInstantiator : MonoBehaviour
    {
        //BOARDS
        private readonly int BOARD_SIZE = 8;
        private ChessBoard Board;
        public Dictionary<Vector2Int, VisualChessTile> Tiles = new();
        public Dictionary<Vector2Int, VisualChessPiece> Pieces = new();


        [Header("Piece Object")]
        public GameObject PiecePrefab;
        public ChessTeam WhiteTeam => ChessTeam.White;
        public ChessTeam BlackTeam => ChessTeam.Black;

        [Header("Cameras")]
        [SerializeField] Camera BoardCamera;
        [SerializeField] CinemachineCamera WhiteView;
        [SerializeField] CinemachineCamera BlackView;

        [Header("Selection Variables")]
        [SerializeField] protected GameObject DefaultTilePrefab;
        [SerializeField] private float PieceSelectionPlaneHeight = 1f;
        public Vector2 TileSpacing = new(1.02f, 1.02f);

        #region Phisical Board Events
        public delegate void VisualPieceDelegate(VisualChessPiece selectedPiece);
        public event VisualPieceDelegate OnPieceSelected;
        public event VisualPieceDelegate OnPieceReleased;
        #endregion


        //GAME VARIABLES
        protected VisualChessPiece SelectedPiece;
        protected VisualChessTile SelectedTile;

        private void Awake()
        {
            WhiteView.Priority = 100;
            GenerateTiles();

            Board = new(BOARD_SIZE);
            Board.OnPieceAdded += InstantiatePiece;
            Board.OnTurnChanged += UpdateTurn;
            Board.OnCheckmate += DisplayCheckmate;
            //Board.OnPieceMoved += Board_OnPieceMoved;
            //Board.OnPieceRemoved += Board_OnPieceRemoved;
            //Board.GenerateBoard();
            Board.AddPiece(ChessPieceType.Queen, new(5, 3), WhiteTeam);
            Board.AddPiece(ChessPieceType.Pawn, new(0, 1), WhiteTeam);
            Board.AddPiece(ChessPieceType.Pawn, new(2, 6), BlackTeam);
        }

        private void DisplayCheckmate(ChessTeam checkedTeam)
        {
            Debug.Log("CHECKMATEEDDDDD team " + checkedTeam);
        }

        private void UpdateTurn(ChessTeam turn)
        {
            foreach (var i in Pieces)
            {
                VisualChessPiece p = i.Value;
                if (p.Team != turn)
                    p.Disable();
                else
                    p.Enable();
            }

            if (turn == WhiteTeam)
            {
                WhiteView.Priority = 1;
                BlackView.Priority = 0;
            }
            else if (turn == BlackTeam)
            {
                WhiteView.Priority = 0;
                BlackView.Priority = 1;
            }
            else
                Debug.LogWarning("Switched to Unidentified team");
        }


        [ContextMenu("PRINT THE CHESSBOARD CURRENT STATE")]
        public void PrintBoard()
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                for (int y = 0; y < BOARD_SIZE; y++)
                {
                    Vector2Int tilePos = new(x, y);
                    if (Board.GetPiece(tilePos) != null)
                        Debug.Log(tilePos + ":" + Board.GetPiece(tilePos).PieceName);
                }
            }
        }


        private readonly RaycastHit[] hitColliders = new RaycastHit[8];
        protected bool GetHitTile(Ray ray, out VisualChessTile hitTile)
        {
            int n = Physics.RaycastNonAlloc(ray, hitColliders);
            hitTile = null;
            for (int i = 0; i < n && hitTile == null; i++)
            {
                if (hitColliders[i].collider.TryGetComponent<VisualChessTile>(out VisualChessTile tile))
                    hitTile = tile;
            }

            return hitTile != null;
        }

        private void Update()
        {
            Ray ray = BoardCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                //HANDLE PIECE SELECTION
                if (Physics.Raycast(ray, out RaycastHit info))
                {
                    info.collider.TryGetComponent<VisualChessPiece>(out var p);
                    SelectedPiece = p;
                    OnPieceSelected?.Invoke(SelectedPiece);
                }
            }

            if (SelectedPiece == null)
                return;

            if (Mouse.current.leftButton.isPressed)
            {
                //HANDLE PIECE DRAG
                if (GetHitTile(ray, out VisualChessTile tile) && SelectedPiece.Logic.IsMoveLegal(tile.Pos))
                {
                    SelectedPiece.MoveGhostAt(tile.PiecePivot.position);
                }
                else
                {
                    Plane p = new(Vector3.up, Vector3.up * PieceSelectionPlaneHeight);
                    p.Raycast(ray, out float dist);
                    SelectedPiece.MoveGhostAt(ray.GetPoint(dist));
                }
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                //HANDLE PIECE MOVEMENT
                if (GetHitTile(ray, out VisualChessTile tile))
                {
                    SelectedTile = tile;
                    /*Debug.Log($"Logic: {SelectedPiece.name}");
                    Debug.Log($"SelectedPiece: {SelectedPiece.Logic.PieceName}");
                    Debug.Log($"SelectedTile: {SelectedTile.Pos}");*/
                    if (SelectedPiece.Logic.IsMoveLegal(SelectedTile.Pos))
                        Board.MakeMove(SelectedPiece.Pos, SelectedTile.Pos);
                }
                OnPieceReleased?.Invoke(SelectedPiece);

                SelectedPiece.ClearGhost();
                SelectedPiece = null;
                SelectedTile = null;
            }
        }


        #region Tile Positioning
        Vector2 currentSpacing = Vector2.zero;
        private void FixedUpdate()
        {
            if (currentSpacing == TileSpacing)
                return;

            foreach (var item in Tiles)
            {
                if (item.Value == null)
                    continue;
                Vector3 pos = new(item.Key.x * TileSpacing.x, 0, item.Key.y * TileSpacing.y);
                item.Value.transform.position = transform.position + pos;
            }

            currentSpacing = TileSpacing;
        }
        #endregion

        #region Board Instantiation

        protected void InstantiatePiece(ChessPieceType pieceType, ChessPiece piece, Vector2Int startingPosition)
        {
            GameObject go = Instantiate(PiecePrefab);
            VisualChessPiece visualPiece = go.GetComponent<VisualChessPiece>();
            visualPiece.Init(this, Tiles[startingPosition], piece);
            Pieces.Add(startingPosition, visualPiece);
        }

        virtual protected void GenerateTiles()
        {
            Tiles = new();

            for (int x = 0; x < BOARD_SIZE; x++)
            {
                for (int y = 0; y < BOARD_SIZE; y++)
                {
                    Vector2Int tilePos = new(x, y);
                    VisualChessTile tile = Instantiate(DefaultTilePrefab).GetComponent<VisualChessTile>();
                    tile.Init(this, tilePos);
                    Tiles.Add(tilePos, tile);
                }
            }
        }

        /*virtual protected void GeneratePieces()
        {
            Pieces = new();
            //WHITE PIECES
            InstantiatePiece(ChessPieceType.Rook, new(0, 0), WhiteTeam);
            for (int i = 0; i < BOARD_SIZE; i++)
                InstantiatePiece(ChessPieceType.Pawn, new(i, 1), WhiteTeam);

            InstantiatePiece(ChessPieceType.Rook, new(7, 0), WhiteTeam);
            InstantiatePiece(ChessPieceType.Knight, new(1, 0), WhiteTeam);
            InstantiatePiece(ChessPieceType.Knight, new(6, 0), WhiteTeam);
            InstantiatePiece(ChessPieceType.Bishop, new(2, 0), WhiteTeam);
            InstantiatePiece(ChessPieceType.Bishop, new(5, 0), WhiteTeam);
            InstantiatePiece(ChessPieceType.Queen, new(3, 0), WhiteTeam);
            InstantiatePiece(ChessPieceType.King, new(4, 0), WhiteTeam);
            
            //BLACK PIECES
            for (int i = 0; i < BOARD_SIZE; i++)
                InstantiatePiece(ChessPieceType.Pawn, new(i, 6), BlackTeam);

            InstantiatePiece(ChessPieceType.Rook, new(0, 7), BlackTeam);
            InstantiatePiece(ChessPieceType.Rook, new(7, 7), BlackTeam);
            InstantiatePiece(ChessPieceType.Knight, new(1, 7), BlackTeam);
            InstantiatePiece(ChessPieceType.Knight, new(6, 7), BlackTeam);
            InstantiatePiece(ChessPieceType.Bishop, new(2, 7), BlackTeam);
            InstantiatePiece(ChessPieceType.Bishop, new(5, 7), BlackTeam);
            InstantiatePiece(ChessPieceType.Queen, new(3, 7), BlackTeam);
            InstantiatePiece(ChessPieceType.King, new(4, 7), BlackTeam);
        }*/
        #endregion
    }
}