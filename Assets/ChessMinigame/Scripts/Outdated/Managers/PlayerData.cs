/*using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerData: MonoBehaviour
{
    public const string Tag = "GameController";
    public Dictionary<string, int> Data = new Dictionary<string, int>();
    public Dictionary<string, int> UnlockedSkins = new Dictionary<string, int>();
    [SerializeField] public Skin PawnSkin;
    [SerializeField] public Skin RookSkin;
    [SerializeField] public Skin KnightSkin;
    [SerializeField] public Skin BishopSkin;
    [SerializeField] public Skin QueenSkin;
    [SerializeField] public Skin KingSkin;

    private static PlayerData instance;


    void Awake()
    {
        gameObject.tag = Tag;
        gameObject.name = Tag;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetElo()
    {
        return Data["Elo"];
    }
    public void SetElo(int quantity)
    {
        if (Data["Elo"] + quantity > 0)
        {
            Data["Elo"] += quantity;
        }
        else
        {
            Data["Elo"] = 0;
        }
    }

    public void UpdatePlayerPrefs()
    {
        foreach (KeyValuePair<string, int> i in Data)
        {
            PlayerPrefs.SetInt(i.Key, i.Value);
        }
        PlayerPrefs.Save();
    }


}*/