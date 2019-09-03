using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//标签 
//[CreateAssetMenu(menuName ="CreatManagerContainer")]
public class ManagerVars : ScriptableObject
{
    public static ManagerVars GetManagerVars()
    {
        return Resources.Load<ManagerVars>("ManagerVarsContainer");
    }
    public List<Sprite> bgThemeSpriteList = new List<Sprite>();
    public List<Sprite> platformThemeSpriteList = new List<Sprite>();
    public List<Sprite> skinSpriteList = new List<Sprite>();
    public List<Sprite> CharacterSpriteList = new List<Sprite>();

    /// <summary>
    /// 皮肤名称
    /// </summary>
    public List<string> SkinNameList = new List<string>();
    /// <summary>
    /// 皮肤价格
    /// </summary>
    public List<int> skinPriceList = new List<int>();

    public GameObject charcterPre;
    public GameObject diamondPre;
    public GameObject normalPlatformPre;
    public GameObject skinChooseItem;

    public List<GameObject> commonPlatformGroup= new List<GameObject>();
    public List<GameObject> grassPlatformGroup = new List<GameObject>();
    public List<GameObject> winterPlatformGroup = new List<GameObject>();
    public GameObject spikePlatformLeft;
    public GameObject spikePlatformRight;
    public GameObject deathEffect;
    

    public float nextXPos = 0.554f, nextYPos = 0.645f;

    public AudioClip jumpClip,fallClip,hitClip,diamondClip,buttonClip;
    public Sprite musicOn, musicOff;

}
