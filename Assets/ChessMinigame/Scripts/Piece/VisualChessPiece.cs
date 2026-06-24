using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static ChessGame.ChessBoard;

namespace ChessGame
{

    public class VisualChessPiece : MonoBehaviour
    {
        [Header("Piece Properties")]
        public ChessPiece Logic;
        public ChessTeam Team => Logic.Team;
        protected VisualChessTile VisualTile;
        public Vector2Int Pos => VisualTile.Pos;

        protected ChessBoardInstantiator BoardInstantiator;
        private CapsuleCollider SelectionCollider;

        [Header("Piece Models")]
        [SerializeField] private GameObject PawnPrefab;
        [SerializeField] private GameObject RookPrefab;
        [SerializeField] private GameObject KnightPrefab;
        [SerializeField] private GameObject BishopPrefab;
        [SerializeField] private GameObject QueenPrefab;
        [SerializeField] private GameObject KingPrefab;

        /*[Header("Special Pieces Prefabs")]
        [SerializeField] private GameObject SentinelPrefab;
        [SerializeField] private GameObject MonolithPrefab;
        [SerializeField] private GameObject BeastPrefab;
        [SerializeField] private GameObject PunkPrefab;*/

        [Header("Materials")]
        private GameObject PieceModel;
        [SerializeField] private Material WhiteMaterial;
        [SerializeField] private Material BlackMaterial;

        [Header("Ghost Properties")]
        public GameObject PieceGhost;
        [Range(0f, 1f)] [SerializeField] private float GhostTransparency = 0.4f;

        [Header("Piece Animation Properties")]
        private const float LERP_THRESHOLD = 0.05f;
        private Transform StandingPivot;
        [SerializeField] private float MovementSpeed = 7f;
        [SerializeField] private float RotatingSpeed = 4f;
        /*[Space]
        [SerializeField] private Animator MeshAnimator;
        [SerializeField] protected AnimationClip IdleAnimation;
        [SerializeField] protected AnimationClip SelectionAnimation;
        [SerializeField] protected AnimationClip MovementAnimation;
        [SerializeField] protected AnimationClip CaptureAnimation;
        [SerializeField] protected AnimationClip DeathAnimation;*/

        
        public delegate void VisualPieceMovement(VisualChessTile newTile, VisualChessPiece movedPiece);
        public event VisualPieceMovement OnTileChanged;
        //public event Action<VisualChessPiece> OnHover;
        //public event Action<VisualChessPiece> OnSelection;

        private void Awake()
        {
            SelectionCollider = gameObject.AddComponent<CapsuleCollider>();
            //SelectionCollider.convex = true;
            SelectionCollider.isTrigger = true;
        }
        public void Init(ChessBoardInstantiator boardInstance, VisualChessTile startingTile, ChessPiece associatedPiece)
        {
            BoardInstantiator = boardInstance;
            VisualTile = startingTile;
            AssignPiece(associatedPiece);

            StandingPivot = startingTile.PiecePivot;
            transform.right = new(Logic.ForwardDirection.x, 0, Logic.ForwardDirection.y);
            name = Logic.ToString();
        }

        public void AssignPiece(ChessPiece piece)
        {
            if (piece == null)
                return;
            if (Logic != null)
            {
                Logic.OnPositionChanged -= OnPieceMovement;
                Logic.OnCapture -= OnPieceCapture;
                Logic.OnPromotion -= OnPiecePromotion;
            }

            Logic = piece;
            Logic.OnPositionChanged += OnPieceMovement;
            Logic.OnCapture += OnPieceCapture;
            Logic.OnPromotion += OnPiecePromotion;


            AssignPieceMesh(piece);
        }

        private void OnPieceMovement(ChessMove movement)
        {
            Move(BoardInstantiator.Tiles[movement.destination]);
        }

        private void OnPieceCapture(ChessCapture capture)
        {
            //Destroy(gameObject);
            StartCoroutine(CaptureCoroutine());
        }

        private void OnPiecePromotion(ChessPromotion promotion)
        {
            AssignPiece(Logic.Board.GetPiece(promotion.position));
        }

        private void AssignPieceMesh(ChessPieceType pieceType)
        {
            if (PieceModel != null)
                Destroy(PieceModel);
            
            GameObject appliedModel = pieceType switch
            {
                ChessPieceType.Pawn => PawnPrefab,
                ChessPieceType.Rook => RookPrefab,
                ChessPieceType.Knight => KnightPrefab,
                ChessPieceType.Bishop => BishopPrefab,
                ChessPieceType.Queen => QueenPrefab,
                ChessPieceType.King => KingPrefab,
                _ => PawnPrefab,
            };

            PieceModel = Instantiate(appliedModel, transform.position, transform.rotation, transform);
            Mesh m = PieceModel.GetComponent<MeshFilter>().mesh;
            SelectionCollider.radius = m.bounds.size.x / 2;
            SelectionCollider.height = m.bounds.size.y;
            SelectionCollider.center = m.bounds.center;

            Renderer pieceRenderer = PieceModel.GetComponent<Renderer>();
            if (Team == ChessTeam.White)
                pieceRenderer.material = WhiteMaterial;
            else
                pieceRenderer.material = BlackMaterial;

            Destroy(PieceGhost);
            PieceGhost = CreateGhost(); //the ghost is to be dragged to show the potential position
            PieceGhost.transform.SetParent(transform);
            ClearGhost();
        }

        private void AssignPieceMesh(ChessPiece piece) => AssignPieceMesh(piece.PieceType);

        /*private void OnCapture(ChessPiece piece, Vector2Int piecePosition)
        {
            StartCoroutine(CaptureCoroutine());
        }

        private void OnPositionChanged(ChessPiece movedPiece, Vector2Int previousPosition, Vector2Int currentPosition)
        {
            //StartCoroutine(MoveToTileCoroutine(BoardInstantiator.Tiles[currentPosition]));
            MoveToTile(BoardInstantiator.Tiles[currentPosition]);
        }*/


        private GameObject CreateGhost()
        {
            GameObject ghost = Instantiate(PieceModel);
            
            // scala leggermente più piccolo
            ghost.transform.localScale *= 0.8f;

            // materiale indipendente
            Renderer ghostRend = ghost.GetComponent<Renderer>();
            Material mat = new(ghostRend.material);
            ghostRend.material = mat;

            // trasparenza (Standard shader)
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = 3000;

            // colore ghost
            Color ghostCol = mat.GetColor("_Color");
            ghostCol.a = GhostTransparency;
            mat.SetColor("_Color", ghostCol);

            return ghost;
        }


        private void FixedUpdate()
        {
            if (StandingPivot == null)
                return;
            if (Vector3.Distance(StandingPivot.position, transform.position) > LERP_THRESHOLD)
                transform.position = Vector3.Lerp(transform.position, StandingPivot.position, MovementSpeed * Time.fixedDeltaTime);

            //transform.position = StandingPivot.position;
        }


        protected IEnumerator SetFacingDirection(Vector2Int facingDirection)
        {
            if (facingDirection == Vector2Int.zero)
                yield break;

            Vector3 PreferredFacingDirection = new Vector3(facingDirection.x, 0f, facingDirection.y).normalized;
            
            while (Vector3.Angle(PreferredFacingDirection, transform.right) > LERP_THRESHOLD)
            {
                transform.right = Vector3.RotateTowards(transform.right, PreferredFacingDirection, RotatingSpeed * Time.deltaTime, 0);
                yield return null;
            }
        }

        private IEnumerator MoveToTileCoroutine(VisualChessTile newTile)
        { 
            if (newTile == null)
            {
                Debug.LogWarning("Tile is null " + newTile);
                yield break;
            }
            Disable();
            StandingPivot = null;
            yield return StartCoroutine(SetFacingDirection(newTile.Pos - Pos));
            while (Vector3.Distance(newTile.PiecePivot.position, transform.position) > LERP_THRESHOLD)
            {
                transform.position = Vector3.Lerp(transform.position, newTile.PiecePivot.position, MovementSpeed * Time.deltaTime);
                yield return null;
            }
            
            VisualTile = newTile;
            StandingPivot = newTile.PiecePivot;
            Enable();
            OnTileChanged?.Invoke(VisualTile, this);
        }
        
        public void Move(VisualChessTile newTile)
        {
            if (newTile == null)
                Debug.LogWarning("Tile is null" + this);
            StartCoroutine(SetFacingDirection(newTile.Pos - Pos));
            VisualTile = newTile;
            StandingPivot = newTile.PiecePivot;
            OnTileChanged?.Invoke(VisualTile, this);
        }

        protected IEnumerator CaptureCoroutine()
        {
            //play a cool capture animation
            transform.localScale /= 2;
            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
            yield return null;
        }

        public void MoveGhostAt(Vector3 ghostPosition)
        {
            PieceGhost.SetActive(true);
            PieceGhost.transform.position = ghostPosition;
            Vector3 ghostRotation = (ghostPosition - transform.position).normalized;
            ghostRotation.y = 0f;
            PieceGhost.transform.right = ghostRotation;
        }

        public void ClearGhost()
        {
            PieceGhost.transform.position = transform.position;
            PieceGhost.SetActive(false);
        }

        public void Enable()
        {
            SelectionCollider.enabled = true;
        }
        public void Disable()
        {
            SelectionCollider.enabled = false;
        }
    }
}
