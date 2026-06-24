/*using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinCatalog", menuName = "Skins/Skin Catalog")]
public class SkinCatalog : ScriptableObject
{
    [Header("The Catalog")]
    [SerializeField] public List<Skin> Catalog = new List<Skin>();
    protected List<Skin> AllSkins = new List<Skin>();
    protected List<Skin> UnlockedSkins = new List<Skin>();
    public SkinCatalog()
    { 
        foreach (Skin s in Catalog)        
        {          
            AllSkins.Add(s);            
            if (s.IsUnlocked)
            {     
                UnlockedSkins.Add(s);        
            }     
        } 
    }
    public List<Skin> GetPieceSkins(SkinType type, bool OnlyUnlocked = true)
    {
        if (OnlyUnlocked)
        {
            return UnlockedSkins.FindAll(i => i.Type == type);
        }
        return AllSkins.FindAll(i => i.Type == type);
    }

}*/


