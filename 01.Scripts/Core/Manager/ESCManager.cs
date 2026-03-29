using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ESCManager : MonoBehaviour,ISavable
{
    [SerializeField] private BoolEventChannelSO _saveEventChannel;
    [SerializeField] private GameObject ESCMenu;
    [SerializeField] private Slider SFX, BGM;
    [SerializeField] private AudioMixer _audioMixer;
    
    
    private bool _isEscOpen = false;
    public void SFXSoundChange(float value)
    {
        Debug.Log(Mathf.Log(value));
        _audioMixer.SetFloat("SFXParam", Mathf.Log10(value) * 20);
    }
    public void BGMSoundChange(float value)
    {
        _audioMixer.SetFloat("BGMParam", Mathf.Log10(value) * 20);
    }
    public void MasterSoundChange(float value)
    {
        _audioMixer.SetFloat("MasterParam", Mathf.Log10(value) * 20);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            EscOnOff(_isEscOpen);
        }
    }//asdasdasdsaasd

    private void EscOnOff(bool value)
    {
        _isEscOpen = !value;
        ESCMenu.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 5), 2f).SetEase(Ease.OutQuart);
        // Time.timeScale = _isEscOpen ? 0f : 1f;
    }
    public void OffBtnClick()
    {
        EscOnOff(_isEscOpen);
        ESCMenu.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 1125), 2f).SetEase(Ease.OutQuart);
    }
    public void ExitBtnClick()
    {
        _saveEventChannel.RaiseEvent(false);
        EscOnOff(_isEscOpen);
        if (SceneManager.GetActiveScene().name == SceneName.LobbyScene)
            SceneManager.LoadScene("TitleScene");
        else
            SceneManager.LoadScene(SceneName.LobbyScene);
        Time.timeScale = 1;
    }

    #region SaveSys

    [field : SerializeField]public SaveIDSO IdData { get; private set; }
    
    [Serializable]
    public struct SoundScale
    {
        public float masterVol;
        public float bgmVol;
        public float sfxVol;
    }
    public string GetSaveData()
    {
        SoundScale dt = new SoundScale
        {
            bgmVol = BGM.value,
            sfxVol = SFX.value
        };
        return JsonUtility.ToJson(dt);
    }

    public void RestoreData(string data)
    {
        SoundScale loadData = JsonUtility.FromJson<SoundScale>(data);
        BGM.value = loadData.bgmVol;
        SFX.value = loadData.sfxVol;
    }

    #endregion
    
}

public class SceneName
{
    public static readonly string LobbyScene = "TitleScene";
}
