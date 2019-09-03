using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ShopPanel : MonoBehaviour
{
    private ManagerVars vars;
    private Transform parent;
    private Text txt_Name;
    private Text txt_DiamondCount;
    private Button btn_Back;
    private Button btn_Select;
    private Button btn_Buy;
    private int selectIndex;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.ShowShopPanel, Show);
        parent = transform.Find("ScrollRect/parent");

        txt_Name = transform.Find("txt_Name").GetComponent<Text>();
        txt_DiamondCount = transform.Find("Diamond/txt_DiamondCount").GetComponent<Text>();

        btn_Back = transform.Find("btn_Back").GetComponent<Button>();
        btn_Back.onClick.AddListener(OnBackButtonClick);
        btn_Select = transform.Find("btn_Select").GetComponent<Button>();
        btn_Select.onClick.AddListener(OnSelectButtonClick);
        btn_Buy = transform.Find("btn_Buy").GetComponent<Button>();
        btn_Buy.onClick.AddListener(OnBuyButtonClick);

        vars = ManagerVars.GetManagerVars();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowShopPanel, Show);
    }
    private void Start()
    {
        Init();
        gameObject.SetActive(false);
    }

    private void Init()
    {
        //设定滑动框的长度，根据人物皮肤来定
        parent.GetComponent<RectTransform>().sizeDelta = new Vector2((vars.skinSpriteList.Count + 2) * 160, 302);
        for (int i = 0; i < vars.skinSpriteList.Count; i++)
        {
            //实例化所有皮肤
            GameObject go = Instantiate(vars.skinChooseItem, parent);
            //未解锁
            if (GameManager.Instance.GetSkinUnlocked(i)==false)
            {
                go.GetComponentInChildren<Image>().color = Color.gray;
            }
            else
            {
                go.GetComponentInChildren<Image>().color = Color.white;
            }
            //改变皮肤的sprite
            go.GetComponentInChildren<Image>().sprite = vars.skinSpriteList[i];
            //改变皮肤位置
            go.transform.localPosition = new Vector3((i + 1) * 160, 0, 0);
        }
        //游戏开始，打开页面，直接定位到上次选中的皮肤
        parent.transform.localPosition =
            new Vector3(GameManager.Instance.GetCurrentSkin() * -160, 0);
    }
    private void Update()
    {
        //确定当前选择的是哪个皮肤
        selectIndex = (int)Mathf.Round(parent.transform.localPosition.x / -160.0f);
        //实现选中人物在中心
        if (Input.GetMouseButtonUp(0))
        {
            parent.transform.DOLocalMoveX(selectIndex * -160,0.2f);
            //parent.transform.localPosition = new Vector3(curentIndex * -160, 0, 0);
        }
        SetItemSize(selectIndex);
        RefreshUI(selectIndex);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 返回按钮点击
    /// </summary>
    private void OnBackButtonClick()
    {

        EventCenter.Broadcast(EventDefine.playClickAudio);
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        gameObject.SetActive(false);

    }
    /// <summary>
    /// 购买按钮点击
    /// </summary>
    private void OnBuyButtonClick()
    {
        EventCenter.Broadcast(EventDefine.playClickAudio);
        //获取当前人物价格
        int price = int.Parse(btn_Buy.GetComponentInChildren<Text>().text);
        //如果钻石不够
        if(price > GameManager.Instance.GetAllDiamond())
        {
            EventCenter.Broadcast(EventDefine.Hint, "钻石不足");
            return;
        }
        //更新全部钻石
        GameManager.Instance.UpdateAllDiamond(-price);
        //解锁皮肤
        GameManager.Instance.SetSkinUnlocked(selectIndex);
        //改变角色在商店内的颜色
        parent.GetChild(selectIndex).GetChild(0).GetComponent<Image>().color = Color.white;
    }
    /// <summary>
    /// 选择按钮点击
    /// </summary>
    private void OnSelectButtonClick()
    {
        EventCenter.Broadcast(EventDefine.playClickAudio);
        //广播游戏里的皮肤更换
        EventCenter.Broadcast(EventDefine.ChangeSkin,selectIndex);
        //设置数据中当前选中皮肤
        GameManager.Instance.SetSelectedSkin(selectIndex);
        //将商店隐藏
        gameObject.SetActive(false);
        //打开主界面
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
    }
    /// <summary>
    /// 设定皮肤尺寸
    /// </summary>
    /// <param name="selectIndex">当前选择的皮肤index</param>
    private void SetItemSize(int selectIndex)
    {
        //遍历parent所有子物体
        for (int i = 0; i < parent.childCount; i++)
        {
            //选中的变大
            if (selectIndex == i)
            {
                parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(160,160);
            }
            //没选中的变小
            else
            {
                parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
            }
        }
    }
    /// <summary>
    /// 刷新UI
    /// </summary>
    /// <param name="selectIndex">当前选择的皮肤index</param>
    private void RefreshUI(int selectIndex)
    {
        //刷新人物名称
        txt_Name.text = vars.SkinNameList[selectIndex];
        //刷新所有钻石数量
        txt_DiamondCount.text = GameManager.Instance.GetAllDiamond().ToString();
        //刷新人物按钮
        //未解锁
        if (GameManager.Instance.GetSkinUnlocked(selectIndex) == false)
        {
            btn_Select.gameObject.SetActive(false);
            btn_Buy.gameObject.SetActive(true);
            //获取皮肤价格
            btn_Buy.GetComponentInChildren<Text>().text = vars.skinPriceList[selectIndex].ToString();
        }//解锁
        else
        {
            btn_Select.gameObject.SetActive(true);
            btn_Buy.gameObject.SetActive(false);
        }
    }
}
