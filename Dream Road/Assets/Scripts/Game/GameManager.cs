using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.IO;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameData data;
    private ManagerVars vars;
    /// <summary>
    /// 游戏是否开始
    /// </summary>
    public bool IsGameStarted { get; set; }
    /// <summary>
    /// 游戏是否结束
    /// </summary>
    public bool IsGameOver { get; set; }
    /// <summary>
    /// 游戏是否暂停
    /// </summary>
    public bool IsPause { get; set; }
    /// <summary>
    /// 玩家是否开始移动
    /// </summary>
    public bool PlayerIsMove { get; set; }
    /// <summary>
    /// 游戏成绩
    /// </summary>
    private int gameScore;
    private int gameDiamond;


    // 是否是第一次开始游戏
    private bool isFirstGame;
    // 音效是否开启
    private bool isMusicOn;
    // 排行榜
    private int[] bestScoreArr;
    // 选中的皮肤
    private int selectSkin;
    // 解锁的皮肤
    private bool[] skinUnlocked;
    // 总的钻石数量
    private int diamondCount;



    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        Instance = this;
        EventCenter.AddListener(EventDefine.AddScore, AddGameScore);
        //EventCenter.AddListener(EventDefine.PlayerMove, PlayerMove);
        EventCenter.AddListener(EventDefine.AddDiamond, AddGameDiamond);

        //当用户点击再来一次可以将IsAgaginGame设为true，重新加载，自动开始游戏
        if (GameData.IsAgaginGame)
        {
            IsGameStarted = true;
        }
        InitGameData();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.AddScore, AddGameScore);
        //EventCenter.RemoveListener(EventDefine.PlayerMove, PlayerMove);
        EventCenter.RemoveListener(EventDefine.AddDiamond, AddGameDiamond);
    }

    /// <summary>
    /// 玩家移动会调用到此方法
    /// </summary>
    //private void PlayerMove()
    //{
    //    PlayerIsMove = true;
    //}

    /// <summary>
    /// 保存成绩
    /// </summary>
    /// <param name="score"></param>
    public void SaveScore(int score)
    {
        //将数组转换为list
        List<int> list = bestScoreArr.ToList();
        //从大到小排序list
        list.Sort((x, y) => (-x.CompareTo(y)));
        //重新赋值给最高分数组
        bestScoreArr = list.ToArray();

        
        int index = -1;
        //将数一个一个进行比较，传进来的数能排在哪个位置
        for (int i = 0; i < bestScoreArr.Length; i++)
        {
            if (score > bestScoreArr[i])
            {
                index = i;
            }
        }
        //如果数比排行榜上的3个数都小则直接return
        if (index == -1) return;

        //从后往前将数组中的数进行替换
        for (int i = bestScoreArr.Length-1; i > index; i--)
        {
            bestScoreArr[i] = bestScoreArr[i - 1];
        }
        //将index确定的数进行替换
        bestScoreArr[index] = score;

        Save();

    }
    /// <summary>
    /// 获取最高分
    /// </summary>
    /// <returns></returns>
    public int GetBestScore()
    {
        //通过linq里数组的max方法获取最大值
        return bestScoreArr.Max();
    }
    /// <summary>
    /// 获取最高分数组
    /// </summary>
    /// <returns></returns>
    public int[] GetScoreArr()
    {
        //将数组转换为list
        List<int> list = bestScoreArr.ToList();
        //从大到小排序list
        list.Sort((x, y) => (-x.CompareTo(y)));
        //重新赋值给最高分数组
        bestScoreArr = list.ToArray();

        return bestScoreArr;
    }
    /// <summary>
    /// 加分
    /// </summary>
    private void AddGameScore()
    {
        if (IsGameStarted == false || IsGameOver || IsPause) return;

        gameScore++;
        EventCenter.Broadcast(EventDefine.UpdataScoreText, gameScore);
    }
    /// <summary>
    /// 更新游戏中钻石数量
    /// </summary>
    private void AddGameDiamond()
    {
        gameDiamond++;
        EventCenter.Broadcast(EventDefine.UpdateDiamondText, gameDiamond);
    }
    /// <summary>
    /// 获取游戏成绩的方法
    /// </summary>
    /// <returns></returns>
    public int GetGameScore()
    {
        return gameScore;
    }
    /// <summary>
    /// 结束界面获得钻石的方法
    /// </summary>
    /// <returns></returns>
    public int GetGameDiamond()
    {
        return gameDiamond;
    }
    /// <summary>
    /// 设置当前皮肤解锁
    /// </summary>
    /// <param name="index">当前皮肤</param>
    public void SetSkinUnlocked(int index)
    {
        skinUnlocked[index] = true;
        Save();
    }
    /// <summary>
    /// 设置选中皮肤
    /// </summary>
    /// <param name="index">当前皮肤</param>
    public void SetSelectedSkin(int index)
    {
        selectSkin = index;
        Save();
    }
    /// <summary>
    /// 设置音效是否开启
    /// </summary>
    public void SetIsMusicOn(bool value)
    {
        isMusicOn = value;
        Save();
    }
    /// <summary>
    /// 获取音效是否开启
    /// </summary>
    /// <returns></returns>
    public bool GetIsMusicOn()
    {
        return isMusicOn;
    }
    /// <summary>
    /// 获得当前皮肤是否解锁
    /// </summary>
    /// <param name="index">当前皮肤</param>
    /// <returns></returns>
    public bool GetSkinUnlocked(int index)
    {
        return skinUnlocked[index];
    }
    /// <summary>
    /// 获得当前选择的皮肤
    /// </summary>
    /// <param name="index"></param>
    public int GetCurrentSkin()
    {
        return selectSkin;
    }
    /// <summary>
    /// 获取玩家所有钻石数量
    /// </summary>
    public int GetAllDiamond()
    {
        return diamondCount;
    }
    /// <summary>
    /// 更新玩家所有钻石数量
    /// </summary>
    public void UpdateAllDiamond(int value)
    {
        //value设置为正值或者负值，买东西为负值，获得钻石为正值
        diamondCount += value;
        Save();
    }
    /// <summary>
    /// 初始化游戏数据
    /// </summary>
    private void InitGameData()
    {
        Read();
        //如果没有文件则会给data一个null 
        if (data != null)
        {
            isFirstGame = data.GetIsFirstGame();
        }
        else
        {
            isFirstGame = true;
        }
        //如果第一次开始游戏
        //需要给所有数据一个默认值
        if (isFirstGame)
        {
            isFirstGame = false;
            isMusicOn = true;
            bestScoreArr = new int[3];
            selectSkin = 0;
            skinUnlocked = new bool[vars.skinSpriteList.Count];
            skinUnlocked[0] = true;
            diamondCount = 10;

            data = new GameData();
            Save();
        }
        else
        {
            //如果不是第一次进行游戏，直接从data中提取
            isMusicOn = data.GetIsMusicOn();
            bestScoreArr = data.GetBestScoreArr();
            selectSkin = data.GetSelectSkin();
            skinUnlocked = data.GetSkinUnlocked();
            diamondCount = data.GetDiamondCount();
        }
    }
    /// <summary>
    /// 用于存储数据
    /// </summary>
    private void Save()
    {
        try
        {
            //用于序列化的类
            BinaryFormatter bf = new BinaryFormatter();
            //使用using可以自动将文件流释放
            //不适用using则需要通过fs.close()进行关闭
            using (FileStream fs = File.Create(Application.persistentDataPath + "/GameData.data"))
            {
                data.SetBestScoreArr(bestScoreArr);
                data.SetDiamondCount(diamondCount);
                data.SetIsFirstGame(isFirstGame);
                data.SetIsMusicOn(isMusicOn);
                data.SetSelectSkin(selectSkin);
                data.SetSkinUnlocked(skinUnlocked);
                bf.Serialize(fs, data);
            }
        }
        catch (System.Exception e)
        {

            Debug.Log(e.Message);
        }
    }
    /// <summary>
    /// 读取数据
    /// </summary>
    private void Read()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = File.Open(Application.persistentDataPath + "/GameData.data", FileMode.Open))
            {
                //反序列化得到Object，强转成GameData赋值给data即可
                data = (GameData)bf.Deserialize(fs);
            }

        }
        catch (System.Exception e)
        {

            Debug.Log(e.Message);
        }
    }
    /// <summary>
    /// 重置数据调用
    /// </summary>
    public void ResetData()
    {
        isFirstGame = false;
        isMusicOn = true;
        bestScoreArr = new int[3];
        selectSkin = 0;
        skinUnlocked = new bool[vars.skinSpriteList.Count];
        skinUnlocked[0] = true;
        diamondCount = 10;

        Save();
    }
}
