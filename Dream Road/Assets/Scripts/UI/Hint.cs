using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Hint : MonoBehaviour
{
    private Image img_Bg;
    private Text txt_Hint;

    private void Awake()
    {
        img_Bg = GetComponent<Image>();
        txt_Hint = GetComponentInChildren<Text>();
        //设置初始透明度
        img_Bg.color = new Color(img_Bg.color.r, img_Bg.color.g, img_Bg.color.b, 0);
        txt_Hint.color = new Color(txt_Hint.color.r, txt_Hint.color.g, txt_Hint.color.b, 0);

        EventCenter.AddListener<string>(EventDefine.Hint, Show);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<string>(EventDefine.Hint, Show);
    }
    private void Show(string text)
    {
        //防止提示框还没有上移就再次进行show方法，导致移动方向出错
        StopCoroutine("Delay");
        //设置初始位置
        transform.localPosition = new Vector3(0, -70, 0);
        //使用DOTweeing来控制Y轴移动
        transform.DOLocalMoveY(0, 0.3f).OnComplete(() => 
        {
            StartCoroutine("Delay");
        });
        //增加a值
        img_Bg.DOColor(new Color(img_Bg.color.r, img_Bg.color.g, img_Bg.color.b, 0.4f), 0.1f);
        txt_Hint.DOColor(new Color(txt_Hint.color.r, txt_Hint.color.g, txt_Hint.color.b, 1), 0.1f);
    }
    private IEnumerator Delay()
    {
        //延时1s，提示框消失
        yield return new WaitForSeconds(1f);
        transform.DOLocalMoveY(70, 0.3f);
        img_Bg.DOColor(new Color(img_Bg.color.r, img_Bg.color.g, img_Bg.color.b, 0), 0.1f);
        txt_Hint.DOColor(new Color(txt_Hint.color.r, txt_Hint.color.g, txt_Hint.color.b, 0), 0.1f);

    }
}
