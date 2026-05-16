using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    //public List<Card> Cards { get; protected set; } = new List<Card>();
    public List<GameObject> Cards { get; protected set; } = new List<GameObject>();
    [SerializeField] private GameObject CardPrefab;
    public const float CARD_OFFSET = 0.05f;
    private void Awake()
    {
        //for(int i = 0; i < Cards.Count; i++)
        for (int i = 0; i < 5; i++)
        {
            Cards.Add(Instantiate(CardPrefab));
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < Cards.Count; i++)
        {
            Cards[i].transform.position = transform.position + new Vector3(i, 0, 0);
        }
    }
}
