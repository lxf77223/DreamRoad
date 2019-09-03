using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgTheme : MonoBehaviour
{
    private SpriteRenderer m_SpriteRenderer;
    private ManagerVars vars;
    private void Awake()
    {
        //调用 GetManagerVars 获得资源管理器
        vars = ManagerVars.GetManagerVars();
        //获得SpriteRenderer组件
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        //获得随机索引值
        int ranValue = Random.Range(0,vars.bgThemeSpriteList.Count);
        ////提取随机索引值的背景
        m_SpriteRenderer.sprite = vars.bgThemeSpriteList[ranValue];
    }
}
