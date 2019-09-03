using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    public Text txt_Score, txt_bestScore, txt_AddDiamondCount;
    public Button btn_Rank, btn_Home, btn_Restart;
    public Image img_New;
    private void Awake()
    {
        btn_Restart.onClick.AddListener(OnRestartButtonClick);
        btn_Rank.onClick.AddListener(OnRankButtonClick);
        btn_Home.onClick.AddListener(OnHomeButtonClick);
        gameObject.SetActive(false);
        EventCenter.AddListener(EventDefine.ShowGameOver, Show);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowGameOver, Show);
    }
    /// <summary>
    /// 显示结束页面
    /// </summary>
    private void Show()
    {
        //最高分显示
        if (GameManager.Instance.GetGameScore() > GameManager.Instance.GetBestScore())
        {
            img_New.gameObject.SetActive(true);
            txt_bestScore.text = "最高分  " + GameManager.Instance.GetGameScore();
        }
        else
        {
            img_New.gameObject.SetActive(false);
            txt_bestScore.text = "最高分  " + GameManager.Instance.GetBestScore();
        }
        //将当前成绩存起来
        GameManager.Instance.SaveScore(GameManager.Instance.GetGameScore());
        //显示成绩
        txt_Score.text = GameManager.Instance.GetGameScore().ToString();
        //显示增加钻石数量
        txt_AddDiamondCount.text ="+"+ GameManager.Instance.GetGameDiamond().ToString();
        //更新总钻石数量
        GameManager.Instance.UpdateAllDiamond(GameManager.Instance.GetGameDiamond());
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 再来一局按钮点击
    /// </summary>
    private void OnRestartButtonClick()
    {
        EventCenter.Broadcast(EventDefine.playClickAudio);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameData.IsAgaginGame = true;
    }
    /// <summary>
    /// 排行榜点击
    /// </summary>
    private void OnRankButtonClick()
    {
        EventCenter.Broadcast(EventDefine.playClickAudio);
        EventCenter.Broadcast(EventDefine.ShowRankPanel);
    }
    /// <summary>
    /// 回首页点击
    /// </summary>
    private void OnHomeButtonClick()
    {
        EventCenter.Broadcast(EventDefine.playClickAudio);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameData.IsAgaginGame = false;
    }

}
