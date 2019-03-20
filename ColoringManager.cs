using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MaterialSettings
{
    public string _mainColor;
    public string _hdrColor;
    public string _specColor;
    public float _intensity;
}

[System.Serializable]
public struct ModelColorSettings
{
    public TraceSettings.ColorTypeList.ColorType _modelColor;
    public List<MaterialSettings> _materialsSettings;
}

public class ColoringManager : MonoBehaviour
{
    [SerializeField] List<Material> _materialsToSet;
    [SerializeField] List<ModelColorSettings> _modelColorSettings;

    public List<Material> GetModelMaterials()
    {
        return _materialsToSet;
    }

    public  ModelColorSettings GetMaterialSettings(TraceSettings.ColorTypeList.ColorType colorType)
    {

        ModelColorSettings currentColorSettings = new ModelColorSettings();
        foreach (var modelColorSetting in _modelColorSettings)
        {
            if (modelColorSetting._modelColor == colorType)
            {
                currentColorSettings = modelColorSetting;
            }
        }

       
        return currentColorSettings;
    }
}
