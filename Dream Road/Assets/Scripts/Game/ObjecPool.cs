using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjecPool : MonoBehaviour
{
    public static ObjecPool Instance;
    //list中的物体数
    public int initSpawnCount = 5;
    private List<GameObject> normalPlotformList = new List<GameObject>();
    private List<GameObject> commonPlotformList = new List<GameObject>();
    private List<GameObject> grassPlotformList = new List<GameObject>();
    private List<GameObject> winterPlotformList = new List<GameObject>();
    private List<GameObject> spikePlotformLeftList = new List<GameObject>();
    private List<GameObject> spikePlotformRightList = new List<GameObject>();
    private List<GameObject> deathEffectList = new List<GameObject>();
    private List<GameObject> diamondList = new List<GameObject>();
    private ManagerVars vars;
    private void Awake()
    {
        Instance = this;
        vars = ManagerVars.GetManagerVars();
        Init();
    }
    private void Init()
    {
        //普通平台
        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObject(vars.normalPlatformPre, ref normalPlotformList);
        }
        //普通组合平台
        for (int i = 0; i < initSpawnCount; i++)
        {
            for (int j = 0; j < vars.commonPlatformGroup.Count; j++)
            {
                InstantiateObject(vars.commonPlatformGroup[j], ref commonPlotformList);
            }
        }
        //草地组合平台
        for (int i = 0; i < initSpawnCount; i++)
        {
            for (int j = 0; j < vars.grassPlatformGroup.Count; j++)
            {
                InstantiateObject(vars.grassPlatformGroup[j], ref grassPlotformList);
            }
        }
        //雪地组合平台
        for (int i = 0; i < initSpawnCount; i++)
        {
            for (int j = 0; j < vars.winterPlatformGroup.Count; j++)
            {
                InstantiateObject(vars.winterPlatformGroup[j], ref winterPlotformList);
            }
        }
        //左边的刺平台
        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObject(vars.spikePlatformLeft, ref spikePlotformLeftList);
        }
        //右边的刺平台
        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObject(vars.spikePlatformRight, ref spikePlotformRightList);
        }
        //死亡特效
        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObject(vars.deathEffect, ref deathEffectList);
        }
        //钻石
        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObject(vars.diamondPre, ref diamondList);
        }
    }
    
    /// <summary>
    /// 设置list
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="addlist"></param>
    private GameObject InstantiateObject(GameObject prefab,ref List<GameObject> addlist)//因为对list进行了修改，需要加ref关键字
    {
        //将生成出来的平台放在对象池下
        GameObject go = Instantiate(prefab, transform);
        //物体默认是隐藏的
        go.SetActive(false);
        //将go加进去
        addlist.Add(go);
        return go;
    }
    /// <summary>
    /// 获得普通单个平台list的方法
    /// </summary>
    /// <returns></returns>
    public GameObject GetNormalPlatform()
    {
        for (int i = 0; i < normalPlotformList.Count; i++)
        {
            if(normalPlotformList[i].activeInHierarchy == false)//如果list中有平台没有显示代表可以使用
            {
                return normalPlotformList[i];
            }
        }
        //对象池中没有物体了，再生成一个
        return  InstantiateObject(vars.normalPlatformPre, ref normalPlotformList);
    }
    /// <summary>
    /// 获得通用组合平台list的方法
    /// </summary>
    /// <returns></returns>
    public GameObject GetCommonPlatform()
    {
        for (int i = 0; i < commonPlotformList.Count; i++)
        {
            if (commonPlotformList[i].activeInHierarchy == false)//如果list中有平台没有显示代表可以使用
            {
                return commonPlotformList[i];
            }
        }
        //对象池中没有物体了，再生成一个随机的组合平台
        int ran = Random.Range(0, vars.commonPlatformGroup.Count);
        return InstantiateObject(vars.commonPlatformGroup[ran], ref commonPlotformList);
    }
    /// <summary>
    /// 获得草地组合平台list的方法
    /// </summary>
    /// <returns></returns>
    public GameObject GetGrassPlatform()
    {
        for (int i = 0; i < grassPlotformList.Count; i++)
        {
            if (grassPlotformList[i].activeInHierarchy == false)//如果list中有平台没有显示代表可以使用
            {
                return grassPlotformList[i];
            }
        }
        //对象池中没有物体了，再生成一个随机的组合平台
        int ran = Random.Range(0, vars.grassPlatformGroup.Count);
        return InstantiateObject(vars.grassPlatformGroup[ran], ref grassPlotformList);
    }
    /// <summary>
    /// 获得冬季组合平台list的方法
    /// </summary>
    /// <returns></returns>
    public GameObject GetWinterPlatform()
    {
        for (int i = 0; i < winterPlotformList.Count; i++)
        {
            if (winterPlotformList[i].activeInHierarchy == false)//如果list中有平台没有显示代表可以使用
            {
                return winterPlotformList[i];
            }
        }
        //对象池中没有物体了，再生成一个随机的组合平台
        int ran = Random.Range(0, vars.winterPlatformGroup.Count);
        return InstantiateObject(vars.winterPlatformGroup[ran], ref winterPlotformList);
    }
    /// <summary>
    /// 获得左边钉子组合平台list的方法
    /// </summary>
    /// <returns></returns>
    public GameObject GetLeftSpikePlatform()
    {
        for (int i = 0; i < spikePlotformLeftList.Count; i++)
        {
            if (spikePlotformLeftList[i].activeInHierarchy == false)//如果list中有平台没有显示代表可以使用
            {
                return spikePlotformLeftList[i];
            }
        }
        //对象池中没有物体了，再生成一个
        return InstantiateObject(vars.spikePlatformLeft, ref spikePlotformLeftList);
    }
    /// <summary>
    /// 获得右边钉子组合平台list的方法
    /// </summary>
    /// <returns></returns>
    public GameObject GetRightSpikePlatform()
    {
        for (int i = 0; i < spikePlotformRightList.Count; i++)
        {
            if (spikePlotformRightList[i].activeInHierarchy == false)//如果list中有平台没有显示代表可以使用
            {
                return spikePlotformRightList[i];
            }
        }
        //对象池中没有物体了，再生成一个
        return InstantiateObject(vars.spikePlatformRight, ref spikePlotformRightList);
    }
    /// <summary>
    /// 获得死亡特效
    /// </summary>
    /// <returns></returns>
    public GameObject GetDeathEffect()
    {
        for (int i = 0; i < deathEffectList.Count; i++)
        {
            if (deathEffectList[i].activeInHierarchy == false)
            {
                return deathEffectList[i];
            }
        }
        //对象池中没有物体了，再生成一个
        return InstantiateObject(vars.deathEffect, ref deathEffectList);
    }
    /// <summary>
    /// 获得钻石
    /// </summary>
    /// <returns></returns>
    public GameObject GetDiamond()
    {
        for (int i = 0; i < diamondList.Count; i++)
        {
            if (diamondList[i].activeInHierarchy == false)
            {
                return diamondList[i];
            }
        }
        //对象池中没有物体了，再生成一个
        return InstantiateObject(vars.diamondPre, ref diamondList);
    }
}
