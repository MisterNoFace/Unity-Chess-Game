/*using UnityEngine;
using UnityEditor;
using System.IO;

public class SkinAssetCreator
{
    [MenuItem("Assets/Create/Skins/Create Skin From Selected Mesh")]
    
    public static void CreateSkinFromMesh()
    {
        foreach (GameObject g in Selection.gameObjects)
        {
            if (g != null && g.GetComponent<MeshFilter>() != null)
            {
                Skin newSkin = ScriptableObject.CreateInstance<Skin>();
                newSkin.name = g.name + "_SKIN";
                newSkin.SkinMesh = g;
                newSkin.SkinName = g.name;
                newSkin.SkinDescription = "DEFAULT_DESCRIPTION_" + g.name;
                newSkin.IsUnlocked = false;

                string path = "Assets/Skins/" + newSkin.name + ".asset";
                if (!Directory.Exists("Assets/Skins"))
                {
                    Directory.CreateDirectory("Assets/Skins");
                }
                AssetDatabase.CreateAsset(newSkin, path);
                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.LogWarning("Select a 3DMesh to create a skin");
            }
        }

    }
}
*/