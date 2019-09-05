using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAudio : MonoBehaviour
{
    private AudioSource my_AudioSource;
    private ManagerVars vars;
    private void Awake()
    {
        my_AudioSource = GetComponent<AudioSource>();
        vars = ManagerVars.GetManagerVars();
        EventCenter.AddListener(EventDefine.playClickAudio, PlayAudio);
        EventCenter.AddListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.playClickAudio, PlayAudio);
        EventCenter.RemoveListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
    }
    private void PlayAudio()
    {
        my_AudioSource.PlayOneShot(vars.buttonClip);
    }
    /// <summary>
    /// 音效是否开启
    /// </summary>
    private void IsMusicOn(bool value)
    {
        my_AudioSource.mute = !value;
    }
}
