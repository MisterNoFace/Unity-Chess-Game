/*using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "Skin", menuName = "Skins/Skin")]
public class Skin: ScriptableObject
{
    [Header("Skin Properties")]
    [SerializeField] public GameObject SkinMesh;
    [SerializeField] public string SkinName = "NAME";
    [SerializeField] public string SkinDescription = "DESCRIPTION";
    [SerializeField] public Rarity SkinRarity = Rarity.Common;
    [SerializeField] public SkinType Type;
    [SerializeField] public bool IsUnlocked;

    private void OnEnable()
    {
        if (PlayerPrefs.GetInt(SkinName, -1) == -1)
        {
            PlayerPrefs.SetInt(SkinName, IsUnlocked ? 1 : 0);
        }
        else
        {
            IsUnlocked = PlayerPrefs.GetInt(SkinName) == 1 ? true : false;
        }
    }
    public Skin()
    {
        Instantiate(SkinMesh);
    }
    public Vector3 GetSize()
    {
        return SkinMesh.GetComponent<MeshFilter>().mesh.bounds.size;
    }
    public Vector3 GetCenter()
    {
        return SkinMesh.GetComponent<MeshFilter>().mesh.bounds.center;
    }
    public void Lock()
    {
        PlayerPrefs.SetInt(SkinName, 0);
        IsUnlocked = false;
    }
    public void Unlock()
    {
        PlayerPrefs.SetInt(SkinName, 1);
        IsUnlocked = true;
    }
}*/