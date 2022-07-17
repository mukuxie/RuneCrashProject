using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneListContainer : MonoBehaviour
{
    // 資料表 a.k.a 腳本化物件

    /// <summary>
    /// 放所有的資料表
    /// </summary>
    [SerializeField]
    private List<RuneData> runeDatas = new List<RuneData>();

    [Header("AutoChange")]
    /// <summary>
    /// 要選到的ListIndex
    /// </summary>
    public int pickID = 0;
    /// <summary>
    /// OUTPUT的盧恩資料表
    /// </summary>
    public RuneData opRuneData;

    private static RuneListContainer _instance;
    public static RuneListContainer Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
        else if (Instance != null)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        opRuneData = runeDatas[pickID];
    }

    /// <summary>
    /// 更改OUTPUT的資料表
    /// </summary>
    private void ChangeOutputData()
    {
        opRuneData = runeDatas[pickID];
    }

    /// <summary>
    /// 更改ID
    /// </summary>
    /// <param name="id">List編號</param>
    public void ChangePickID(int id)
    {
        pickID = id;
        ChangeOutputData();
    }
}
