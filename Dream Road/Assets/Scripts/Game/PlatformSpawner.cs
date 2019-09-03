using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformGroupType
{
    Grass,
    Winter
}
public class PlatformSpawner : MonoBehaviour
{
    //系数
    public float multiple;
    //最小掉落时间
    public float minFallTime;
    //掉落时间
    public float fallTime;
    //里程碑数
    public int milestoneCount = 10;
    //平台初始位置（0,-2.4,0）
    public Vector3 startSpawnPos;
    //平台生成次数
    private int spawnPlatformCount;
    private ManagerVars vars;
    private bool isLeftSpwan = false;
    //平台生成位置
    private Vector3 platformSpawnPositon;
    //选择的平台主题
    private Sprite selectedPlatformSprite;
    //组合平台类型
    private PlatformGroupType groupType;
    /// <summary>
    /// 钉子组合平台是否生成在左边
    /// </summary>
    private bool spikeSpwanLeft = false;
    /// <summary>
    /// 钉子方向平台的位置
    /// </summary>
    private Vector3 spikeDirPlatfromPos;
    /// <summary>
    /// 生成钉子平台之后需要在钉子方向生成的平台数量
    /// </summary>
    private int afterSpawnSpikeSpawnCount;
    private bool isSpawnSpike;
    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        EventCenter.AddListener(EventDefine.DecidePath, DecidePath);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.DecidePath, DecidePath);
    }
    private void Start()
    {
        RandomPlotformTheme();
        //给第一个平台生成的位置赋值
        platformSpawnPositon = startSpawnPos;
        //先向右生成5个平台
        for (int i = 0; i < 5; i++)
        {
            spawnPlatformCount = 5;
            DecidePath();
        }
        //生成人物
        GameObject go = Instantiate(vars.charcterPre);
        go.transform.position = new Vector3(0, -1.8f, 0);
    }
    private void Update()
    {
        if (GameManager.Instance.IsGameStarted && GameManager.Instance.IsGameOver == false)
        {
            UpdataFallTime();
        }
        
    }
    /// <summary>
    /// 更新平台掉落时间
    /// </summary>
    private void UpdataFallTime()
    {
        if(GameManager.Instance.GetGameScore()> milestoneCount)
        {
            //里程碑数增加
            milestoneCount *= 2;
            //掉落时间更新
            fallTime *= multiple;
            //如果掉落时间小于最小时间，则将掉落时间=最小时间
            if (fallTime < minFallTime)
            {
                fallTime = minFallTime;
            }
        }
    } 
    /// <summary>
    /// 确定路径
    /// </summary>
    private void DecidePath()
    {
        if (isSpawnSpike)
        {
            AfterSpawnSpike();
            return;
        }
        if(spawnPlatformCount > 0)
        {
            //每次生成平台后，平台生成次数减少
            spawnPlatformCount--;
            SpawnPlatform();
        }
        else
        {
            //当平台次数减少到0的时候会改变方向，并且重新赋值给平台生成次数
            isLeftSpwan = !isLeftSpwan;
            spawnPlatformCount = Random.Range(1, 4);
            SpawnPlatform();
        }
    }
    /// <summary>
    /// 随机平台主题
    /// </summary>
    private void RandomPlotformTheme()
    {
        int ran = Random.Range(0, vars.platformThemeSpriteList.Count);
        selectedPlatformSprite = vars.platformThemeSpriteList[ran];
        //冬季主题
        if (ran == 2)
        {
            groupType = PlatformGroupType.Winter;
        }
        else
        {
            groupType = PlatformGroupType.Grass;
        }
    }
    /// <summary>
    /// 生成平台
    /// </summary>
    private void SpawnPlatform()
    {
        int ranObstacleDir = Random.Range(0, 2);
        //生成单个平台
        if(spawnPlatformCount >= 1)
        {
            SpawnNormalPlatform(ranObstacleDir);
        }
        //生成组合平台
        else if(spawnPlatformCount == 0)
        {
            int ran = Random.Range(0, 3);
            //生成通用组合平台
            if (ran==0)
            {
                SpawnCommonPlatformGroup(ranObstacleDir);
            }
            //生成主题组合平台
            else if (ran == 1)
            {
                switch (groupType)
                {
                    case PlatformGroupType.Grass:
                        SpawnGrassPlatformGroup(ranObstacleDir);
                        break;
                    case PlatformGroupType.Winter:
                        SpawnWinterPlatformGroup(ranObstacleDir);
                        break;
                    default:
                        break;
                }
            }
            //生成钉子的组合平台
            else 
            {
                int value = -1;
                //根据游戏设定 路径向左生成 钉子向右生成
                if (isLeftSpwan)
                {
                    value = 0;//生成右边方向的钉子
                }
                else
                {
                    value = 1;//生成左边方向的钉子
                }
                SpawnSpikePlatformGroup(value);

                isSpawnSpike = true;
                afterSpawnSpikeSpawnCount = 4;
                if (spikeSpwanLeft)//钉子生成在左边
                {
                    //记录钉子方向平台的位置
                    spikeDirPlatfromPos = new Vector3(platformSpawnPositon.x - 1.65f
                        , platformSpawnPositon.y + vars.nextYPos,
                        0);
                }
                else
                {
                    spikeDirPlatfromPos = new Vector3(platformSpawnPositon.x + 1.65f
                        , platformSpawnPositon.y + vars.nextYPos,
                        0);
                }
            }
        }
        int ranSpawnDiamond = Random.Range(0, 9);
        if(ranSpawnDiamond >= 6&& GameManager.Instance.PlayerIsMove)
        {
            GameObject go = ObjecPool.Instance.GetDiamond();
            go.transform.position = new Vector3(platformSpawnPositon.x, 
                platformSpawnPositon.y + 0.5f, 0);
            go.SetActive(true);
        }
        if (isLeftSpwan)//向左生成
        {
            platformSpawnPositon = new Vector3(platformSpawnPositon.x - vars.nextXPos,
                platformSpawnPositon.y + vars.nextYPos,
                0);
        }
       else//向右生成
        {
            platformSpawnPositon = new Vector3(platformSpawnPositon.x + vars.nextXPos,
                platformSpawnPositon.y + vars.nextYPos,
                0);
        }
    }
    /// <summary>
    /// 生成普通平台（单个）
    /// </summary>
    private void SpawnNormalPlatform(int ranObstacleDir)
    {
        GameObject go = ObjecPool.Instance.GetNormalPlatform();
        go.transform.position = platformSpawnPositon;
        //随机了样式，拿到了图片之后，获取单个平台的替换图片脚本来完成样式随机
        go.GetComponent<platformScript>().Init(selectedPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成通用组合平台
    /// </summary>
    private void SpawnCommonPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjecPool.Instance.GetCommonPlatform();
        go.transform.position = platformSpawnPositon;
        go.GetComponent<platformScript>().Init(selectedPlatformSprite,fallTime, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成草地组合平台
    /// </summary>
    private void SpawnGrassPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjecPool.Instance.GetGrassPlatform();
        go.transform.position = platformSpawnPositon;
        go.GetComponent<platformScript>().Init(selectedPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成冬季组合平台
    /// </summary>
    private void SpawnWinterPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjecPool.Instance.GetWinterPlatform();
        go.transform.position = platformSpawnPositon;
        go.GetComponent<platformScript>().Init(selectedPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成钉子组合平台
    /// </summary>
    private void SpawnSpikePlatformGroup(int dir)
    {
        GameObject temp = null; 
        if (dir == 0)
        {
            spikeSpwanLeft = false;
            temp = ObjecPool.Instance.GetRightSpikePlatform();
        }
        else
        {
            spikeSpwanLeft = true;
            temp = ObjecPool.Instance.GetLeftSpikePlatform();
        }
        temp.transform.position = platformSpawnPositon;
        temp.GetComponent<platformScript>().Init(selectedPlatformSprite, fallTime, dir);
        temp.SetActive(true);
    }
    /// <summary>
    /// 生成钉子平台之后需要生成的平台
    /// 包括钉子方向 也包括原来的方向
    /// </summary>
    private void AfterSpawnSpike()
    {
        if (afterSpawnSpikeSpawnCount > 0)
        {
            afterSpawnSpikeSpawnCount--;
            for (int i = 0; i < 2; i++)
            {
                GameObject temp = ObjecPool.Instance.GetNormalPlatform();
                if (i == 0)//原来方向
                {
                    temp.transform.position = platformSpawnPositon;
                    //如果钉子在左边，原先路径就是右边
                    if (spikeSpwanLeft)
                    {
                        platformSpawnPositon = new Vector3(platformSpawnPositon.x + vars.nextXPos
                            , platformSpawnPositon.y + vars.nextYPos,0);

                    }
                    else
                    {
                        platformSpawnPositon = new Vector3(platformSpawnPositon.x - vars.nextXPos
                            , platformSpawnPositon.y + vars.nextYPos, 0);
                    }
                }
                else//生成钉子方向
                {
                    temp.transform.position = spikeDirPlatfromPos;
                    if (spikeSpwanLeft)
                    {
                        
                        spikeDirPlatfromPos = new Vector3(spikeDirPlatfromPos.x - vars.nextXPos
                            , spikeDirPlatfromPos.y + vars.nextYPos, 0);

                    }
                    else
                    {
                        spikeDirPlatfromPos = new Vector3(spikeDirPlatfromPos.x + vars.nextXPos
                            , spikeDirPlatfromPos.y + vars.nextYPos, 0);
                    }
                }
                temp.GetComponent<platformScript>().Init(selectedPlatformSprite, fallTime, 1);
                temp.SetActive(true);
            }
        }
        else
        {
            isSpawnSpike = false;
            DecidePath();
        }
    }
}
