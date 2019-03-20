using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EffectsPrefabInstaller : MonoInstaller<EffectsPrefabInstaller>
{
 
    public PrefabEffectsSettings _effectsSettings;
    

    public override void InstallBindings()
    {
        Container.BindInstance(_effectsSettings);

        Container.BindFactory<TraceSettings.EffectType, int, Transform, PlayerEffect, PlayerEffect.Factory>().FromMethod(CreatePlayerEffect);
        Container.BindFactory<TraceSettings.EffectType, TraceSettings.ColorTypeList.ColorType, Transform, ObstacleEffect, ObstacleEffect.Factory>().FromMethod(CreateObstacleEffect);
        Container.BindFactory<Transform, PickUpEffect, PickUpEffect.Factory>().FromMethod(CreatePickUpEffect);


    }
    //PLAYER EFFECTS
    PlayerEffect CreatePlayerEffect(DiContainer _, TraceSettings.EffectType effectType, int i, Transform parent)
    {
        
        PlayerEffect instPrefab = null;
        if (effectType == TraceSettings.EffectType.MainEffect)
        {
            instPrefab = GetPlayerEffectPrefab(_effectsSettings.playerMainParticleEffects, i, parent);
            
        }
        else if (effectType == TraceSettings.EffectType.DieEffect)
        {
            instPrefab = GetPlayerEffectPrefab(_effectsSettings.playerDieEffects, i, parent);      
        }
        else if (effectType == TraceSettings.EffectType.AdditionalEffect)
        {
            instPrefab = GetPlayerEffectPrefab(_effectsSettings.playerShield, i, parent);
        }
        else if (effectType == TraceSettings.EffectType.OuterMainEffect)
        {
            instPrefab = GetPlayerEffectPrefab(_effectsSettings.playerOuterParticleEffects, i, parent);
        }

        return instPrefab;
       
    }
    PlayerEffect GetPlayerEffectPrefab(List<PrefabEffectsSettings.ParticleEffectsType> particleEffects, int num, Transform prefParent)
    {
       
        var prefab = particleEffects[num].effectPrefab;
        if (prefab != null)
        {
            Quaternion startRot = prefab.transform.localRotation;
            Vector3 startPos = prefab.transform.localPosition;
            PlayerEffect thisInstPrefab = Container.InstantiatePrefabForComponent<PlayerEffect>(prefab);

            if (prefParent != null)
                thisInstPrefab.transform.SetParent(prefParent);

            thisInstPrefab.transform.localRotation = startRot;
            thisInstPrefab.transform.localPosition = startPos;

            return thisInstPrefab;
        }
        else return null;
    }

    //OBSTACLE EFFECTS
    ObstacleEffect CreateObstacleEffect(DiContainer _, TraceSettings.EffectType effectType, TraceSettings.ColorTypeList.ColorType colorType, Transform parent)
    {
        GameObject prefab = null;
        ObstacleEffect instPrefab = null;


        if (effectType == TraceSettings.EffectType.DieEffect)
        {
            Quaternion startRot = Quaternion.identity;
            Vector3 startPos = Vector3.zero;
            foreach (var obstacleEffect in _effectsSettings.obstacleBreakEffects)
            {
                if (obstacleEffect.colorType == colorType && obstacleEffect.effectPrefab != null)
                {
                    prefab = obstacleEffect.effectPrefab;

                    startRot = prefab.transform.localRotation;
                    startPos = prefab.transform.localPosition;

                    prefab.SetActive(false);
                }
            }

            if (prefab != null)
            {
                instPrefab = Container.InstantiatePrefabForComponent<ObstacleEffect>(prefab);
                if (parent != null)
                    instPrefab.transform.SetParent(parent);

                //instPrefab.transform.localRotation = startRot;
                instPrefab.transform.localPosition = Vector3.zero;
            }
        }
        else if (effectType == TraceSettings.EffectType.WinEffect)
        {
            Quaternion startRot = Quaternion.identity;
            Vector3 startPos = Vector3.zero;
            foreach (var obstacleEffect in _effectsSettings.obstacleNotPassingEffects)
            {
                if (obstacleEffect.colorType == colorType && obstacleEffect.effectPrefab != null)
                {
                    prefab = obstacleEffect.effectPrefab;
                    startRot = prefab.transform.localRotation;
                    startPos = prefab.transform.localPosition;
                    prefab.SetActive(false);
                }
            }
            if (prefab != null)
            {
                instPrefab = Container.InstantiatePrefabForComponent<ObstacleEffect>(prefab);
                if (parent != null)
                    instPrefab.transform.SetParent(parent);
                // instPrefab.transform.localRotation = startRot;
                instPrefab.transform.localPosition = Vector3.zero;

            }
        }
        return instPrefab;
    }
    
    //PICKUP EFFECTS
    PickUpEffect CreatePickUpEffect(DiContainer _, Transform parent)
    {
        var prefab = _effectsSettings.PickUpDieEffect.effectPrefab;
        PickUpEffect instPrefab = Container.InstantiatePrefabForComponent<PickUpEffect>(prefab);

        if (parent != null)
            instPrefab.transform.SetParent(parent);
        instPrefab.transform.localPosition = Vector3.zero;
        //instPrefab.transform.position = position;
        instPrefab.transform.rotation = parent.transform.rotation;
        instPrefab.gameObject.SetActive(false);
        return instPrefab;
    }
}
