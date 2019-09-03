using System.Collections;
using System.Collections.Generic;


//加上这个标签才可以序列化和反序列化
[System.Serializable]
public class GameData 
{
    // 是否再来一次游戏
    public static bool IsAgaginGame = false;
    // 是否是第一次开始游戏
    private bool isFirstGame;
    // 音效是否开启
    private bool isMusicOn;
    // 排行榜
    private int[] bestScoreArr;
    // 选中的皮肤
    private int selectSkin;
    // 未解锁的皮肤
    private bool[] skinUnlocked;
    // 总的钻石数量
    private int diamondCount;

    public void SetIsFirstGame(bool isFirstGame)
    {
        this.isFirstGame = isFirstGame;
    }
    public void SetIsMusicOn(bool isMusicOn)
    {
        this.isMusicOn = isMusicOn;
    }
    public void SetBestScoreArr(int[] bestScoreArr)
    {
        this.bestScoreArr = bestScoreArr;
    }
    public void SetSelectSkin(int selectSkin)
    {
        this.selectSkin = selectSkin;
    }
    public void SetSkinUnlocked(bool[] skinUnlocked)
    {
        this.skinUnlocked = skinUnlocked;
    }
    public void SetDiamondCount(int diamondCount)
    {
        this.diamondCount = diamondCount;
    }


    public bool GetIsFirstGame()
    {
        return isFirstGame;
    }
    public bool GetIsMusicOn()
    {
        return isMusicOn;
    }
    public int[] GetBestScoreArr()
    {
        return bestScoreArr;
    }
    public int GetSelectSkin()
    {
        return selectSkin;
    }
    public bool[] GetSkinUnlocked()
    {
        return skinUnlocked;
    }
    public int GetDiamondCount()
    {
        return diamondCount;
    }
}
