using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[System.Serializable]
public struct STRColorList
{
    public string STRmainColor;
    public string STRemissionColor;
}

public struct ColorList
{
    public Color mainColor;
    public Color emissionColor;
}

[CreateAssetMenu(fileName = "Material Modifier", menuName = "TraceSettings/Modify Model Material", order = 1)]
public class MaterailColorChanger : ScriptableObject
{
    [SerializeField] GameObject modelToModify;

    [Header ("Original Material")]
    [SerializeField] Material[] initialMaterial;
    
    [Header("New Material")]
    [SerializeField] Material[] newMaterials;

    [Header("New Colors")]

    [SerializeField] STRColorList[] newColorList; 

    [SerializeField] string newCommonColor = "";
    [SerializeField] string newMainColor = "";
    [SerializeField] string newEmissionColor = "";


    [Button]
    void CreateAndChangeMaterial()
    {
        var meshRenderers = modelToModify.GetComponentsInChildren<MeshRenderer>();
        bool[] isMaterialCopied = new bool[initialMaterial.Length];
        for (int num = 0; num < isMaterialCopied.Length; num++) { isMaterialCopied[num] = false; }
       
        foreach (var meshRenderer in meshRenderers)
        {
            for (int i = 0; i < initialMaterial.Length; i++)
            { Debug.Log(isMaterialCopied[i]);
                if (newMaterials[i] != null && initialMaterial[i] != null && meshRenderer.sharedMaterial == initialMaterial[i])
                {
                    if (!isMaterialCopied[i])
                    {
                        newMaterials[i].SetVector("_EmissionColor", initialMaterial[i].GetVector("_EmissionColor"));
                        newMaterials[i].CopyPropertiesFromMaterial(meshRenderer.sharedMaterial);
                        isMaterialCopied[i] = true;
                    }
                    meshRenderer.sharedMaterial = newMaterials[i];
                }
            }
        }
    }

   /* [Button]
    private void ChangeColor()
    {
        ColorList[] colorsToSet = new ColorList[newColorList.Length];
        for (int num = 0; num < colorsToSet.Length; num++) { colorsToSet[num].mainColor = Color.clear; colorsToSet[num].emissionColor = Color.clear;}

        for (int i = 0; i < colorsToSet.Length; i++)
        {
            if (newColorList[i].STRmainColor != "")
            { ColorUtility.TryParseHtmlString(newColorList[i].STRmainColor, out colorsToSet[i].mainColor); }
            else { colorsToSet[i].mainColor = newMaterials[i].GetVector("_Color"); }

            if (newColorList[i].STRemissionColor != "")
            { ColorUtility.TryParseHtmlString(newColorList[i].STRemissionColor, out colorsToSet[i].emissionColor); }
            else { colorsToSet[i].emissionColor = newMaterials[i].GetVector("_EmissionColor"); }
        }

        for (int i = 0; i < newMaterials.Length; i++)
        {
            if (newMaterials[i] != null)
            {
                newMaterials[i].color = colorsToSet[i].mainColor;
                newMaterials[i].SetVector("_EmissionColor", colorsToSet[i].emissionColor);
            }

        }
            
         if (newCommon != null)
          {
              newCommon.color = commonColorToSet;
              newCommon.SetColor("_EmissionColor", Color.white);
          }

          if (newMain != null)
          {
              newMain.color = mainColorToSet;
          }

          if (newEmission != null)
          {

              newEmission.color = mainColorToSet;
              newEmission.SetColor("_EmissionColor", emissionColorToSet);
          }
    }*/

} 
    