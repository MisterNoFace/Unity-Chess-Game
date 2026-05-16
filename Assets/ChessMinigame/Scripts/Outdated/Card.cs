using Unity.VisualScripting;
using UnityEngine;

public class CardData
{

}
public class Card : MonoBehaviour
{
    [Header("Card Properties")]
    [SerializeField] protected CardData Data;
    [SerializeField] GameObject CardMesh;
    void Awake()
    {
        /*MeshFilter f = gameObject.GetOrAddComponent<MeshFilter>();
        f.mesh = new Mesh();
        f.mesh.vertices = new Vector3[4]
        {
            new Vector3(-CARD_SIZE.x, 0, -CARD_SIZE.y) / 2,
            new Vector3(CARD_SIZE.x, 0, -CARD_SIZE.y) / 2,
            new Vector3(-CARD_SIZE.x, 0, CARD_SIZE.y) / 2,
            new Vector3(CARD_SIZE.x, 0, CARD_SIZE.y) / 2,
        };
        f.mesh.triangles = new int[6] {0, 1, 2, 1, 3, 2};
        f.mesh.RecalculateBounds();
        f.mesh.RecalculateNormals();
        Renderer mr = gameObject.GetOrAddComponent<Renderer>();*/
        Mesh m = CardMesh.GetComponent<MeshFilter>().sharedMesh;
        
        BoxCollider2D c = gameObject.GetOrAddComponent<BoxCollider2D>();
        c.size = new Vector2(m.bounds.size.x, m.bounds.size.y);
        c.offset = Vector2.zero;
        c.enabled = true;

        Instantiate(CardMesh, transform.position, Quaternion.identity);
    }

    void Update()
    {
        
    }
}
