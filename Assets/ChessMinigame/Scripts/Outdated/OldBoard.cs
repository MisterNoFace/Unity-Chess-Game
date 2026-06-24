using UnityEngine;
using UnityEngine.InputSystem;

public class OldBoard : MonoBehaviour
{
    //logic var
    [Header("Graphics")]
    [SerializeField] private Material TileMaterial;
    [SerializeField] private Material HoverMaterial;

    private const float TILE_SIZE = 1f;
    private Vector2Int BOARD_SIZE = new Vector2Int(8, 8);
    public GameObject[,] board_tiles;
    private Vector2Int currentTile;
    private Camera cam;

    //game logic
    private void Awake()
    {
        board_tiles = GenerateTiles(TILE_SIZE, BOARD_SIZE);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!cam)
        {
            cam = Camera.main;
            return;
        }

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Tile", "Hover")))
        {
            Vector2Int tilePos = GetTileCoord(hit.transform.gameObject);

            if (currentTile != tilePos)
            {
                // Ripristina il tile precedente
                if (currentTile != -Vector2Int.one)
                {
                    var prev = board_tiles[currentTile.x, currentTile.y];
                    prev.layer = LayerMask.NameToLayer("Tile");
                    prev.GetComponent<Renderer>().material = TileMaterial;
                }

                // Aggiorna nuovo tile hover
                var tile = board_tiles[tilePos.x, tilePos.y];
                tile.layer = LayerMask.NameToLayer("Hover");
                tile.GetComponent<Renderer>().material = HoverMaterial;

                currentTile = tilePos;
            }
        }
        else
        {
            // Quando il mouse non è su alcun tile
            if (currentTile != -Vector2Int.one)
            {
                var prev = board_tiles[currentTile.x, currentTile.y];
                prev.layer = LayerMask.NameToLayer("Tile");
                prev.GetComponent<Renderer>().material = TileMaterial;
                currentTile = -Vector2Int.one;
            }
        }
    }


    //board gen
    private GameObject[,] GenerateTiles(float tile_size, Vector2Int ntiles)
    {
        GameObject[,] tiles = new GameObject[ntiles.x, ntiles.y];
        for (int x = 0; x < ntiles.x; x++)
        {
            for (int y = 0; y < ntiles.y; y++)
            {
                GameObject g = GenerateSingleTile(tile_size);
                g.transform.position = new Vector3(x * tile_size, 0, y * tile_size);
                tiles[x, y] = g;
            }
        }

        return tiles;
    }
    private GameObject GenerateSingleTile(float tile_size)
    {
        GameObject t = new GameObject();
        Mesh m = new Mesh();

        t.transform.parent = transform;
        t.layer = LayerMask.NameToLayer("Tile");
        t.AddComponent<MeshFilter>().mesh = m;
        t.AddComponent<Renderer>().material = TileMaterial;
        t.AddComponent<BoxCollider>().size = new Vector3(1, 0.2f, 1);

        Vector3[] vert = new Vector3[4];
        vert[0] = new Vector3(-tile_size / 2, 0, -tile_size / 2);
        vert[1] = new Vector3(-tile_size / 2, 0, tile_size / 2);
        vert[2] = new Vector3(tile_size / 2, 0, -tile_size / 2);
        vert[3] = new Vector3(tile_size / 2, 0, tile_size / 2);

        m.vertices = vert;
        m.triangles = new int[] { 0, 1, 2, 1, 3, 2 };

        m.RecalculateNormals();
        m.RecalculateBounds();

        return t;
    }
    private Vector2Int GetTileCoord(GameObject t)
    {
        for (int x = 0; x < BOARD_SIZE.x; x++)
        {
            for (int y = 0; y < BOARD_SIZE.y; y++)
            {
                if (board_tiles[x, y] == t)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return -Vector2Int.one;
    }
}

/*
protected void HandleHovering()
{
    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    RaycastHit hit;
    if (Physics.Raycast(ray, out hit, 100f, layerMask: LayerMask.GetMask(ChessPiece.Layer, ChessTile.Layer)))
    {
        ChessTile t = hit.collider.gameObject.GetComponent<ChessTile>();
        ChessPiece p = hit.collider.gameObject.GetComponent<ChessPiece>();
        if (t == null)
        {
            t = p.CurrentTile;
        }
        else
        {
            p = t.Piece;
        }

        if (t != HoveredTile)
        {
            if (HoveredTile != null)
            {
                HoveredTile.UnHover();
            }
            HoveredTile = t;
            HoveredTile.Hover();
        }
    }
    else
    {
        if (HoveredTile != null)
        {
            HoveredTile.UnHover();
            HoveredTile = null;
        }
    }
}

protected void HandleTileSelection()
{
    if (Mouse.current.leftButton.wasPressedThisFrame)
    {
        if (HoveredTile != null)
        {
            if (SelectedTile == null)
            {
                SelectedTile = HoveredTile;
                PossibleMoves = SelectedTile.Piece == null ? new List<ChessTile>() : SelectedTile.Piece.AvailableTiles;
                foreach (var p in PossibleMoves)
                {
                    p.Select();
                }
            }
            else
            {
                if (PossibleMoves.Contains(HoveredTile))
                {
                    HoveredTile.Piece.MoveToTile(HoveredTile);   
                }
                foreach (var p in PossibleMoves)
                {
                    p.UnHover();
                }
                SelectedTile = null;
            }
        }
    }
}*/