using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformScript : MonoBehaviour
{
    public SpriteRenderer[] SpriteRenderers;
    //障碍物
    public GameObject obstacle;
    //开始计时标志
    private bool startTimer;
    //倒计时
    private float fallTime;
    private Rigidbody2D my_Body;
    private void Awake()
    {
        my_Body = GetComponent<Rigidbody2D>();
    }
    public void Init(Sprite sprite ,float fallTime, int obstaclesDir)
    {
        my_Body.bodyType = RigidbodyType2D.Static;
        this.fallTime = fallTime;
        startTimer = true;
        for (int i = 0; i < SpriteRenderers.Length; i++)
        {
            SpriteRenderers[i].sprite = sprite;
        }

        if(obstaclesDir == 0)//朝右边
        {

            if(obstacle != null)
            {
                //使用localposition 要不然生成的障碍物会到屏幕中间去
                obstacle.transform.localPosition = new Vector3(-obstacle.transform.localPosition.x,
                    obstacle.transform.localPosition.y,
                     0
                    );
            }
        }
    }
    private void Update()
    {
        if (GameManager.Instance.IsGameStarted==false||GameManager.Instance.PlayerIsMove==false)
        {
            return;
        }
        if (startTimer)
        {
            //倒计时减少
            fallTime -= Time.deltaTime;
            if (fallTime < 0)//倒计时结束
            {
                startTimer = false;
                if(my_Body.bodyType != RigidbodyType2D.Dynamic)
                {
                    my_Body.bodyType = RigidbodyType2D.Dynamic;
                    StartCoroutine(DealyHide());
                }
                
            }
        }
        if(transform.position.y - Camera.main.transform.position.y < -6)
        {
            StartCoroutine(DealyHide());
        }
    }
    private IEnumerator DealyHide()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
