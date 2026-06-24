using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
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
        public List<VisualChessPiece> Pieces = new();


        [Header("Piece Object")]
        public GameObject PiecePrefab;

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
            GenerateTiles();
            Board = new(BOARD_SIZE);
            Board.OnPieceAdded += InstantiatePiece;
            //Board.OnPieceCaptured += OnPieceCaptured;
            //Board.OnPieceMoved += OnPieceMoved;
            //Board.OnPieceReplaced += OnPieceReplaced;
            Board.OnTurnChanged += UpdateTurn;
            Board.OnCheck += OnCheck;
            Board.OnStaleMate += DisplayStalemate;
            Board.OnCheckmate += DisplayCheckmate;
            Board.GenerateBoard();
            /*Board.Init();
            Board.AddPiece(ChessPieceType.Rook, new(0, 0), ChessTeam.White);
            Board.AddPiece(ChessPieceType.Rook, new(7, 0), ChessTeam.White);
            Board.AddPiece(ChessPieceType.Rook, new(0, 7), ChessTeam.Black);
            Board.AddPiece(ChessPieceType.Rook, new(7, 7), ChessTeam.Black);
            Board.AddPiece(ChessPieceType.Pawn, new(0, 2), ChessTeam.White);
            Board.AddPiece(ChessPieceType.Pawn, new(1, 6), ChessTeam.Black);
            Board.AddPiece(ChessPieceType.Pawn, new(2, 2), ChessTeam.White);
            Board.AddPiece(ChessPieceType.Pawn, new(3, 6), ChessTeam.Black);
            /*Board.AddPiece(ChessPieceType.Queen, new(5, 3), ChessTeam.White);
            Board.AddPiece(ChessPieceType.Pawn, new(0, 4), ChessTeam.White);
            Board.AddPiece(ChessPieceType.Rook, new(3, 0), ChessTeam.Black);
            Board.AddPiece(ChessPieceType.Pawn, new(2, 6), ChessTeam.Black);*/
        }


        protected void InstantiatePiece(ChessPieceType pieceType, ChessPiece piece, Vector2Int startingPosition)
        {
            GameObject go = Instantiate(PiecePrefab);
            VisualChessPiece visualPiece = go.GetComponent<VisualChessPiece>();
            visualPiece.Init(this, Tiles[startingPosition], piece);
            Pieces.Add(visualPiece);
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

        /*private void OnPieceCaptured(ChessCapture capture)
        {
            foreach (VisualChessPiece piece in Pieces)
                if (piece.Logic == capture.piece)
                {
                    piece.Capture();
                    Pieces.Remove(piece);
                    break;
                }
        }

        private void OnPieceMoved(ChessMove movement)
        {
            foreach (VisualChessPiece piece in Pieces)
                if (piece.Logic == movement.piece)
                {
                    piece.Move(Tiles[movement.destination]);
                    break;
                }
        }

        private void OnPieceReplaced(ChessPromotion promotion)
        {
            //Pieces[promotion.position].AssignPiece(Board.GetPiece(promotion.position));//this is wronge
        }*/

        private void OnCheck(ChessTeam checkedTeam)
        {
            Debug.Log("Hey there! you are in check mr " + checkedTeam);
        }

        private void DisplayStalemate(ChessTeam checkedTeam)
        {
            Debug.Log("STALEMATEDDDD team" + checkedTeam);
        }

        private void DisplayCheckmate(ChessTeam checkedTeam)
        {
            Debug.Log("CHECKMATEEDDDDD team " + checkedTeam);
        }

        private void UpdateTurn(ChessTeam turn)
        {
            foreach (VisualChessPiece p in Pieces)
            {
                if (p.Team != turn)
                    p.Disable();
                else
                    p.Enable();
            }

            if (turn == ChessTeam.White)
            {
                WhiteView.Priority = 1;
                BlackView.Priority = 0;
            }
            else
            {
                WhiteView.Priority = 0;
                BlackView.Priority = 1;
            }
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


        #region Update Game Logic

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
                if (GetHitTile(ray, out VisualChessTile tile) && SelectedPiece.Logic.MovesList().ContainsKey(tile.Pos))
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
                    if (SelectedPiece.Logic.MovesList().TryGetValue(SelectedTile.Pos, out ChessAction action))
                        Board.MakeMove(action);
                }
                OnPieceReleased?.Invoke(SelectedPiece);

                SelectedPiece.ClearGhost();
                SelectedPiece = null;
                SelectedTile = null;
            }
        }
        #endregion

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

    }
}