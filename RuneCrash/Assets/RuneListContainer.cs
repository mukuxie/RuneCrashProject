using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneListContainer : MonoBehaviour
{
    // ��ƪ� a.k.a �}���ƪ���

    /// <summary>
    /// ��Ҧ�����ƪ�
    /// </summary>
    [SerializeField]
    private List<RuneData> runeDatas = new List<RuneData>();

    [Header("AutoChange")]
    /// <summary>
    /// �n��쪺ListIndex
    /// </summary>
    public int pickID = 0;
    /// <summary>
    /// OUTPUT���c����ƪ�
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
    /// ���OUTPUT����ƪ�
    /// </summary>
    private void ChangeOutputData()
    {
        opRuneData = runeDatas[pickID];
    }

    /// <summary>
    /// ���ID
    /// </summary>
    /// <param name="id">List�s��</param>
    public void ChangePickID(int id)
    {
        pickID = id;
        ChangeOutputData();
    }
}
