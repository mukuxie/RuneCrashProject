using UnityEngine;
using UnityEngine.UI; //Button
using UnityEngine.Advertisements; //Advertisement
using System;

public class ADSManager : MonoBehaviour,IUnityAdsInitializationListener, IUnityAdsLoadListener,IUnityAdsShowListener
{
    
    private Button btnAdaGoGame;
    private string gameIDAndroid = "4825779";
    //private string gameIDIOS = "4825778";
    private string gameID;

    private string adsIDAndroid = "GoGame";
    //private string adsIDIOS = "GoGame";
    private string adsID ;


    public void OnInitializationComplete()
    {
        print("<color=green>�s�i��l�Ʀ��\</color>");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        print("<color=red>�s�i��l�ƥ���</color>");
    }
    private void Awake()
    {
        btnAdaGoGame = GameObject.Find("�L���Ҧ�").GetComponent<Button>();
        btnAdaGoGame = GameObject.Find("���ɼҦ�").GetComponent<Button>();
        btnAdaGoGame.onClick.AddListener(LoadAds);
        InitializeAds();
#if UNITY_IOS

        adsID = adsIDIOS;
#elif UNITY_Android
        adsID = adsIDAndroid;
#endif
        adsID = adsIDAndroid;



    }

    //�s�i���J����k
    public void OnUnityAdsAdLoaded(string placementId)
    {
        print("<color=green>2. �s�i���J���\" + placementId + "</color>");
    } 

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        print("<color=red>�s�i���J����" + message + "</color>");
    }
    //�s�i���J����k

    // �s�i���񪺤�k
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        print("<color=red>2. �s�i����" + placementId + "</color>");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        print("<color=green>2. �s�i�}�l" + placementId + "</color>");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        print("<color=green>2. �s�i����I��" + placementId + "</color>");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        print("<color=green>2. �s�i��ܧ���" + placementId + "</color>");
    }
    // �s�i���񪺤�k


    private void LoadAds()
    {
        print("���J�s�i . ID:" + adsID);
        Advertisement.Load(adsID, this);
        ShowAds();
    }

    private void ShowAds()
    {
        Advertisement.Show(adsID, this);
    }

    private void InitializeAds()
    {
        gameID = gameIDAndroid;
        Advertisement.Initialize(gameID, true, this);
    }

    
}