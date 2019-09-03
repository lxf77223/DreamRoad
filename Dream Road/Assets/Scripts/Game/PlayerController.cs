using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    //射线位置
    public Transform rayDown, rayLeft, rayRight;
    //平台层，用于射线检测
    public LayerMask platformLayer, obstacleLayer;
    //是否向左移动 反之向右
    private bool isMoveLeft = false;
    //是否在跳跃
    private bool isJumping = false;
    //左右两边的下次位置
    private Vector3 nextPlatformLeft, nextPlatformRight;
    private ManagerVars vars;
    private Rigidbody2D my_Body;
    private SpriteRenderer spriteRenderer;
    private AudioSource my_AudioSource;
    //private bool isMove= false;
    private void Awake()
    {
        EventCenter.AddListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
        EventCenter.AddListener<int>(EventDefine.ChangeSkin, ChangeSkin);
        vars = ManagerVars.GetManagerVars();
        spriteRenderer = GetComponent<SpriteRenderer>();
        my_Body = GetComponent<Rigidbody2D>();
        my_AudioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        ChangeSkin(GameManager.Instance.GetCurrentSkin());
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<int>(EventDefine.ChangeSkin, ChangeSkin);
    }
    /// <summary>
    /// 音效是否开启
    /// </summary>
    private void IsMusicOn(bool value)
    {
        my_AudioSource.mute = !value;
    }
    /// <summary>
    /// 更换皮肤调用
    /// </summary>
    /// <param name="skinIndex"></param>
    private void ChangeSkin(int skinIndex)
    {
        spriteRenderer.sprite = vars.CharacterSpriteList[skinIndex];
    }
    private bool IsPointerOverGameObject(Vector2 mousePostion)
    {
        //创建一个点击事件
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePostion;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        //向点击位置发射一条射线，检测是否点击UI
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults.Count > 0;
    }
    private void Update()
    {
        Debug.DrawRay(rayDown.position, Vector2.down * 1,Color.red);
        Debug.DrawRay(rayLeft.position, Vector2.left * 0.15f, Color.red);
        Debug.DrawRay(rayRight.position, Vector2.right * 0.15f, Color.red);

        //当运行平台为安卓和ios时
        //if (Application.platform==RuntimePlatform.Android||
        //    Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    int fingerId = Input.GetTouch(0).fingerId;
        //    //EventSystem.current.IsPointerOverGameObject 是否碰到UI
        //    if (EventSystem.current.IsPointerOverGameObject(fingerId)) return;
        //}
        //else
        //{
        //    if (EventSystem.current.IsPointerOverGameObject()) return;
        //}
        if (IsPointerOverGameObject(Input.mousePosition)) return;

        //角色不在跳跃状态下才能点击鼠标
        if (GameManager.Instance.IsGameStarted == false || GameManager.Instance.IsGameOver == true||GameManager.Instance.IsPause == true)
            return;

        if (Input.GetMouseButtonDown(0) && isJumping== false && nextPlatformLeft != Vector3.zero)
        {
            //if(isMove == false)
            //{
            //    EventCenter.Broadcast(EventDefine.PlayerMove);
            //    isMove = true;
            //}
            GameManager.Instance.PlayerIsMove = true;
            my_AudioSource.PlayOneShot(vars.jumpClip);
            EventCenter.Broadcast(EventDefine.DecidePath);
            isJumping = true;
            //通过input下的mousePositon变量获得鼠标点击的位置
            Vector3 mousePos = Input.mousePosition;
            //点击的是左边屏幕
            if (mousePos.x <= Screen.width / 2)
            {
                isMoveLeft = true;
            }
            //点击的是右边屏幕
            else if (mousePos.x > Screen.width / 2)
            {
                isMoveLeft = false;
            }
            Jump();
        }
        //人物掉落游戏结束
        if (my_Body.velocity.y < 0 && IsRayPlatform() == false && GameManager.Instance.IsGameOver == false)//y轴速度小于0，表示人物正在向下落
        {
            my_AudioSource.PlayOneShot(vars.fallClip);
            //调整人物的sorting Layer
            spriteRenderer.sortingLayerName = "Default";
            //将人物碰撞器禁用
            GetComponent<BoxCollider2D>().enabled = false;
            GameManager.Instance.IsGameOver = true;
            //调用结束面板
            StartCoroutine(DealyShowGameOver());
        }
        //碰到障碍物游戏结束
        if(isJumping && IsRayObstacle()&&GameManager.Instance.IsGameOver == false)
        {
            my_AudioSource.PlayOneShot(vars.hitClip);
            GameObject go = ObjecPool.Instance.GetDeathEffect();
            go.SetActive(true);
            go.transform.position = transform.position;
            GameManager.Instance.IsGameOver = true;
            //不显示人物
            spriteRenderer.enabled = false;
            //调用结束面板
            StartCoroutine(DealyShowGameOver());
        }
        //平台掉落游戏判断
        if(transform.position.y -Camera.main.transform.position.y< -6&& GameManager.Instance.IsGameOver == false)
        {
            my_AudioSource.PlayOneShot(vars.fallClip);
            GameManager.Instance.IsGameOver = true;
            //调用结束面板
            StartCoroutine(DealyShowGameOver());
        }
    }
    IEnumerator DealyShowGameOver()
    {
        yield return new WaitForSeconds(1f);
        EventCenter.Broadcast(EventDefine.ShowGameOver);
    }
    /// <summary>
    /// 人物跳跃
    /// </summary>
    private void Jump()
    {
        if (isMoveLeft)
        {
            //人物翻转朝向左边
            transform.localScale = new Vector3(-1, 1, 1);
            transform.DOMoveX(nextPlatformLeft.x, 0.2f);
            transform.DOMoveY(nextPlatformLeft.y+0.8f, 0.15f);
        }
        else
        {
            //人物翻转朝向右边
            transform.DOMoveX(nextPlatformRight.x, 0.2f);
            transform.DOMoveY(nextPlatformRight.y + 0.8f, 0.15f);
            transform.localScale = Vector3.one;
            
        }

    }
    //用来存放上一次碰到的平台
    private GameObject lastHitGo = null;
    /// <summary>
    /// 射线检测平台
    /// </summary>
    /// <returns>人物是否碰撞到平台</returns>
    private bool IsRayPlatform()
    {
        //参数：起始点，方向，距离，检测哪一层
        RaycastHit2D hit = Physics2D.Raycast(rayDown.position, Vector2.down, 1f, platformLayer);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Platform")
            {
                //判断碰到的平台是不是上一平台
                if(lastHitGo != hit.collider.gameObject)
                {
                    if (lastHitGo == null)
                    {
                        //将当前平台赋值给上一平台变量
                        lastHitGo = hit.collider.gameObject;
                        return true;
                    }
                    //广播加分
                    EventCenter.Broadcast(EventDefine.AddScore);
                    //将当前平台赋值给上一平台变量
                    lastHitGo = hit.collider.gameObject;
                }
                return true;
            }
                
        }
        return false;
    }
    /// <summary>
    /// 射线检测障碍物
    /// </summary>
    /// <returns>人物是否碰撞到障碍物</returns>
    private bool IsRayObstacle()
    {
        RaycastHit2D lefthit = Physics2D.Raycast(rayLeft.position, Vector2.left, 0.15f, obstacleLayer);
        RaycastHit2D righthit = Physics2D.Raycast(rayRight.position, Vector2.right, 0.15f, obstacleLayer);
        //判断左边
        if (lefthit.collider != null)
        {
            if (lefthit.collider.tag =="Obstacle")
            {
                return true;
            }
        }
        //判断右边
        if (righthit.collider != null)
        {
            if (righthit.collider.tag == "Obstacle")
            {
                return true;
            }

        }
        return false;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //当人物碰撞到平台的时候，得到人物下次跳的位置
        if (collision.tag == "Platform")
        {
            isJumping = false;
            Vector3 currentPlatformPos = collision.gameObject.transform.position;
            //人物跳向左边的位置
            nextPlatformLeft = new Vector3(
                currentPlatformPos.x - vars.nextXPos,
                currentPlatformPos.y + vars.nextYPos,
                0);
            //人物跳向右边的位置
            nextPlatformRight = new Vector3(
                currentPlatformPos.x + vars.nextXPos,
                currentPlatformPos.y + vars.nextYPos,
                0);
        }
        if (collision.tag == "Pickup")
        {
            my_AudioSource.PlayOneShot(vars.diamondClip);
            EventCenter.Broadcast(EventDefine.AddDiamond);
            //吃到钻石了
            collision.gameObject.SetActive(false);
        }
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.collider.tag == "Pickup")
    //    {
    //        EventCenter.Broadcast(EventDefine.AddDiamond);
    //        //吃到钻石了
    //        collision.gameObject.SetActive(false);
    //    }
    //}
}
