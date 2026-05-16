using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ChessGame
{
    public class VisualChessTile : MonoBehaviour
    {
        public Vector2Int Pos { get; private set; }
        public Transform PiecePivot;
        
        [Header("Tile Visual Properties")]
        [SerializeField] private GameObject TileModel;
        [SerializeField] private Material WhiteMaterial;
        [SerializeField] private Material BlackMaterial;
        [Header("Animation Properties")]
        //[SerializeField] private Material HoverTileMat;
        [SerializeField] private float AnimationMovementSpeed = 3f;
        [SerializeField] private Material SelectedTileMat;

        public void Init(ChessBoardInstantiator boardInstance, Vector2Int tilePosition)
        {
            Pos = tilePosition;
            boardInstance.OnPieceSelected += OnPieceSelected;
            boardInstance.OnPieceReleased += OnPieceReleased;
            transform.SetParent(boardInstance.transform);
            name = Pos.ToString();

            //white tile
            if ((Pos.x + Pos.y) % 2 == 0)
                TileModel.GetComponent<Renderer>().material = WhiteMaterial;
            //black tile
            else
                TileModel.GetComponent<Renderer>().material = BlackMaterial;
        }

        
        float angle = 0f;
        bool animation = false;


        private void Update()
        {
            if (animation)
            {
                //TileModel.transform.position = transform.position + Vector3.up * Mathf.Sin(angle);
                TileModel.transform.localScale = Vector3.one * Mathf.Abs(Mathf.Sin(angle + 0.01f));
                angle += Time.deltaTime * AnimationMovementSpeed;
            }
        }

        private void OnPieceSelected(VisualChessPiece selectedPiece)
        {
            if (selectedPiece == null)
                return;

            if (selectedPiece.Logic.IsMoveLegal(Pos))
                animation = true;
    
        }

        private void OnPieceReleased(VisualChessPiece selectedPiece)
        {
            if (!animation)
                return;
            animation = false;
            angle = 0f;
            TileModel.transform.position = transform.position;
            TileModel.transform.localScale = Vector3.one;
        }




        /*private void DisplayPiecePotentialActions(ChessPieceLogic obj)
        {
            List<Vector2Int> moves = obj.GetPseudoMoves();
            if (moves.Contains(Pos))
                TileVisualRenderer.material = SelectedTileMat;
            else
                TileVisualRenderer.material = MainMaterial;
        }

        private void Hover(ChessTile obj)
        {
            if (obj == this)
                TileVisualRenderer.material = HoverTileMat;
        }*/



    }
}