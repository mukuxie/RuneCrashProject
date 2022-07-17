using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRune : MonoBehaviour
{
    /// <summary>
    /// 給按鈕呼叫用，更改RuneListContainer的編號與Output
    /// </summary>
    /// <param name="runeListID">資料表編號</param>
    public void ChangeRuneForButton(int runeListID)
    {
        RuneListContainer.Instance.ChangePickID(runeListID);
    }
}
