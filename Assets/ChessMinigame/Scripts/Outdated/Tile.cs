/*using Scripts.Input;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public enum TileState
{
    None,
    Hovered,
    Selected,
    Occupied,
    Disabled
}
public class Tile : MonoBehaviour
{
    [Header("Mesh Settings")]
    //[SerializeField] public Skin TileSkin { get; protected set; }
    [SerializeField] private Mesh m_Mesh;
    [SerializeField] private Material WhiteTileMaterial;
    [SerializeField] private Material BlackTileMaterial;
    [SerializeField] private Material HoverTileMaterial;
    [SerializeField] private Material DisabledTileMaterial;
    protected Material MainTileMaterial;

    protected Renderer Renderer;
    protected const float OFFSET = 0.15f;
    public Vector3 SIZE { get; protected set; } = new Vector3(1f, 0.25f, 1f);

    public Vector2Int Coord { get; protected set; }
    public ChessPiece Piece { get; protected set; }
    MouseRaycaster mr;
    void Awake()
    {
        Renderer = gameObject.GetOrAddComponent<Renderer>();
        MeshFilter f = gameObject.GetOrAddComponent<MeshFilter>();
        f.mesh = m_Mesh;
        SIZE = m_Mesh.bounds.size;
        BoxCollider c = gameObject.GetOrAddComponent<BoxCollider>();   
        c.isTrigger = true;
        c.size = SIZE;
        mr = gameObject.AddComponent<MouseRaycaster>();
    }

    void Update()
    {
        if (mr.FirstHit == gameObject)
            ChangeMaterial(DisabledTileMaterial);

        if (Piece == null || Coord == null)
        {
            return;
        }
        

        if (Piece.CurrentTile != this)
        {
            Piece = null;
        }
    }

    public void SetCoord(Vector2Int coord)
    {
        Coord = coord;
        name = $"{coord.x}, {coord.y}";

        if ((Coord.x + Coord.y) % 2 == 0)
        {
            MainTileMaterial = WhiteTileMaterial;
        }
        else
        {
            MainTileMaterial = BlackTileMaterial;
        }

        ChangeMaterial(MainTileMaterial);
        transform.position = new Vector3(SIZE.x * coord.x, 0, SIZE.z * coord.y);
    }

    public Vector2Int GetLocalCoord(Tile tile)
    {
        if (tile == null)
        {
            return Coord;
        }
        return Coord - tile.Coord;
    }

    protected void ChangeMaterial(Material material)
    {
        if (material != null)
        {
            Renderer.material = material;
        }
    }
    public void Hover()
    {
        ChangeMaterial(HoverTileMaterial);
    }

    public void Select()
    {
        Hover();
        transform.position = new Vector3(SIZE.x * Coord.x, OFFSET, SIZE.x * Coord.y);
    }

    public void UnHover()
    {
        transform.position = new Vector3(SIZE.x * Coord.x, 0, SIZE.x * Coord.y); ;
        ChangeMaterial(MainTileMaterial);
    }

    public void AssignPiece(ChessPiece piece)
    {
        Piece = piece;
        if (piece != null)
        {
            Piece.gameObject.transform.SetParent(transform);
        }
    }
    public bool IsFree()
    {
        return Piece == null;
    }

}*/
