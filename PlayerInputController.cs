using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerInputController : MonoBehaviour
{
    PlayerStateManager _playerStateManager;
    GameManager _gameManager;
   
    [Inject]
    public void Construct(PlayerStateManager playerStateManager, GameManager gameManager)
    {
        _playerStateManager = playerStateManager;
        _gameManager = gameManager;


    }
    [Inject]
    TraceSettings _traceSettings;

    PlayerEffectsPooler _playerEffectsPooler;
    ColoringManager _coloringManager;
    List<GameObject> _currentPlayerEffects;
    List<GameObject> _currentPlayerOuterEffects;

    GameObject _defaultEffect;
    GameObject _tappedEffect;
    GameObject _holdedEffect;

    GameObject _defaultOuterEffect;
    GameObject _tappedOuterEffect;
    GameObject _holdedOuterEffect;

    GameObject _lastEffect;

    GameObject _lastOuterEffect;
    bool _isOuterEffectOn = false;

    TraceSettings.ColorTypeList.ColorType _defaultColor;
    TraceSettings.ColorTypeList.ColorType _tappedColor;
    TraceSettings.ColorTypeList.ColorType _holdedColor;
    TraceSettings.ColorTypeList.ColorType _lastColor;

    [SerializeField] float _holdTime = 0.3f;
    [SerializeField] float _outerEffectsFrames = 15;
    bool _wasHolded = false;
    float _touchTime = 0;
    
    
    void Start()
    {
        _playerStateManager.currentObstacleType = TraceSettings.ObstacleType.None;
        _coloringManager = GetComponent<ColoringManager>();
        SetPlayerEffects();
        SetNewMaterialColor(_defaultColor);
        _lastEffect = _defaultEffect;
        _lastOuterEffect = _defaultOuterEffect;
        _lastColor = _defaultColor;       

    }

    private void SetPlayerEffects()
    {
        _playerEffectsPooler = GetComponent<PlayerEffectsPooler>();
        _currentPlayerEffects = new List<GameObject>(_playerEffectsPooler.GetCurrentMainEffectsList());
        _currentPlayerOuterEffects = new List<GameObject>(_playerEffectsPooler.GetOuterMainEffectsList());


        foreach (var currentPlayerEffect in _currentPlayerEffects)
        {
            var currentEffectManager = currentPlayerEffect.GetComponent<PlayerEffectManager>();
            var currentEffectColorType = currentEffectManager.GetPlayerEffectColorType();

            if (currentEffectColorType == _traceSettings.traceSettings[0].colorType)
            {
                _defaultEffect = currentPlayerEffect;
                _defaultColor = currentEffectColorType;
               
            }
            else if (currentEffectColorType == _traceSettings.traceSettings[1].colorType)
            {
                _tappedEffect = currentPlayerEffect;
                _tappedColor = currentEffectColorType;
              
            }
            else if (currentEffectColorType == _traceSettings.traceSettings[2].colorType)
            {
                _holdedEffect = currentPlayerEffect;
                _holdedColor = currentEffectColorType;
                
            }
        
        }

        foreach (var currentPlayerOuterEffect in _currentPlayerOuterEffects)
        {
            var currentEffectManager = currentPlayerOuterEffect.GetComponent<PlayerEffectManager>();
            var currentEffectColorType = currentEffectManager.GetPlayerEffectColorType();

            if (currentEffectColorType == _traceSettings.traceSettings[0].colorType)
            {
                _defaultOuterEffect = currentPlayerOuterEffect;
            }
            else if (currentEffectColorType == _traceSettings.traceSettings[1].colorType)
            {
                _tappedOuterEffect = currentPlayerOuterEffect;
            }
            else if (currentEffectColorType == _traceSettings.traceSettings[2].colorType)
            {
                _holdedOuterEffect = currentPlayerOuterEffect;
            }

        }
        _playerStateManager.currentPlayerColor = _defaultColor;//write current color to state Manager
       
    }

    void Update()
    {
        if (_gameManager.GetCurrentGameState() != GameManager.GameState.Menu)
        {
            if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
            {
                _touchTime += Time.deltaTime;

                if (_touchTime >= _holdTime)
                {
                    HoldState();
                }
            }
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
            {
                if (_wasHolded)
                {
                    HoldEndState();
                }
                else
                {
                    if (_lastColor == _defaultColor)
                    {
                        FromDefaultState();
                    }
                    else if (_lastColor == _tappedColor)
                    {
                        FromTappedState();
                    }
                }
            }
        }
    }
     
    void HoldState()
    {
       
        _wasHolded = true;
        if (_holdedEffect && _lastEffect)
        {
            SetNewMaterialColor(_holdedColor);
            _holdedEffect.SetActive(true);

            if (!_isOuterEffectOn)
            {
                StartCoroutine(SwitchOnOuterEffect(_holdedOuterEffect));
                _isOuterEffectOn = true;
            }
            StartCoroutine(SwitchOffOuterEffect(_holdedOuterEffect));

            _lastEffect.SetActive(false);
            _playerStateManager.currentPlayerColor = _holdedColor;
        }
        CheckPerfectTap();
    }

    

    void HoldEndState()
    {
        _wasHolded = false;
        _touchTime = 0;
        if (_holdedEffect && _lastEffect)
        {
            SetNewMaterialColor(_lastColor);
            _holdedEffect.SetActive(false);
            _lastEffect.SetActive(true);

            StartCoroutine(SwitchOnOuterEffect(_lastOuterEffect));
            StartCoroutine(SwitchOffOuterEffect(_lastOuterEffect));

            _playerStateManager.currentPlayerColor = _lastColor;
        }
        _playerStateManager.isPerfectTapChecked = false;
       
      
    }

    void FromDefaultState()
    {
        _touchTime = 0;

        if (_defaultEffect && _tappedEffect && _lastEffect)
        {
            SetNewMaterialColor(_tappedColor);

            _defaultEffect.SetActive(false);
            _tappedEffect.SetActive(true);

            StartCoroutine(SwitchOnOuterEffect(_tappedOuterEffect));
            StartCoroutine(SwitchOffOuterEffect(_tappedOuterEffect));


            _lastEffect = _tappedEffect;
            _lastOuterEffect = _tappedOuterEffect;
            _lastColor = _tappedColor;
            _playerStateManager.currentPlayerColor = _tappedColor;
        }
        CheckPerfectTap();
    }

    void FromTappedState()
    {
        _touchTime = 0;

        if (_defaultEffect && _tappedEffect && _lastEffect)
        {
            SetNewMaterialColor(_defaultColor);
            _defaultEffect.SetActive(true);

            StartCoroutine(SwitchOnOuterEffect(_defaultOuterEffect));
            StartCoroutine(SwitchOffOuterEffect(_defaultOuterEffect));

            _tappedEffect.SetActive(false);
            _lastEffect = _defaultEffect;
            _lastOuterEffect = _defaultOuterEffect;
            _lastColor = _defaultColor;
            _playerStateManager.currentPlayerColor = _defaultColor;
        }
        CheckPerfectTap();
    }

    IEnumerator SwitchOnOuterEffect(GameObject effect)
    {
        if (effect != null)
        {
            int num = 0;
            while (num < _outerEffectsFrames)
            {
                effect.transform.position = this.gameObject.transform.position;
                effect.SetActive(true);
                num++;
                yield return new WaitForEndOfFrame();
            }
        }

    }
    IEnumerator SwitchOffOuterEffect(GameObject effect)
    {
        if (effect != null)
        {
            yield return new WaitForSeconds(1f);
            effect.SetActive(false);
            _isOuterEffectOn = false;
        }
    }

    private void CheckPerfectTap()
    {
        if (_playerStateManager.currentObstacleType == TraceSettings.ObstacleType.PerfectTap && !_playerStateManager.isPerfectTapChecked)
        {
            _playerStateManager.SetPerfectTap();
            _playerStateManager.isPerfectTapChecked = true;
        }
        if (_playerStateManager.currentObstacleType != TraceSettings.ObstacleType.PerfectTap && !_playerStateManager.isPerfectTapChecked)
        {
            _playerStateManager.ResetCombo();
            _playerStateManager.isPerfectTapChecked = true;
        }

    }

    private void SetNewMaterialColor(TraceSettings.ColorTypeList.ColorType colorType)
    {
        var materials = _coloringManager.GetModelMaterials();
        var currentMaterialSettings = _coloringManager.GetMaterialSettings(colorType);
        for (int i = 0; i < materials.Count; i++)
        {
            Color mainColor = Color.clear;
            Color emitColor = Color.clear;
            Color specColor = Color.clear;

            ColorUtility.TryParseHtmlString(currentMaterialSettings._materialsSettings[i]._mainColor, out mainColor);
            ColorUtility.TryParseHtmlString(currentMaterialSettings._materialsSettings[i]._hdrColor, out emitColor);
            ColorUtility.TryParseHtmlString(currentMaterialSettings._materialsSettings[i]._specColor, out specColor);

            materials[i].SetColor("_Color", mainColor);
            materials[i].SetColor("_EmissionColor", emitColor * currentMaterialSettings._materialsSettings[i]._intensity);
            materials[i].SetColor("_SpecColor", specColor);
        }
    }


}
