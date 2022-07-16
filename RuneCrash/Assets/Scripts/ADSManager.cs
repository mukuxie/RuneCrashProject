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
        print("<color=green>廣告初始化成功</color>");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        print("<color=red>廣告初始化失敗</color>");
    }
    private void Awake()
    {
        btnAdaGoGame = GameObject.Find("無限模式").GetComponent<Button>();
        btnAdaGoGame = GameObject.Find("限時模式").GetComponent<Button>();
        btnAdaGoGame.onClick.AddListener(LoadAds);
        InitializeAds();
#if UNITY_IOS

        adsID = adsIDIOS;
#elif UNITY_Android
        adsID = adsIDAndroid;
#endif
        adsID = adsIDAndroid;



    }

    //廣告載入的方法
    public void OnUnityAdsAdLoaded(string placementId)
    {
        print("<color=green>2. 廣告載入成功" + placementId + "</color>");
    } 

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        print("<color=red>廣告載入失敗" + message + "</color>");
    }
    //廣告載入的方法

    // 廣告播放的方法
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        print("<color=red>2. 廣告失敗" + placementId + "</color>");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        print("<color=green>2. 廣告開始" + placementId + "</color>");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        print("<color=green>2. 廣告顯示點擊" + placementId + "</color>");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        print("<color=green>2. 廣告顯示完成" + placementId + "</color>");
    }
    // 廣告播放的方法


    private void LoadAds()
    {
        print("載入廣告 . ID:" + adsID);
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