using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRune : MonoBehaviour
{
    /// <summary>
    /// �����s�I�s�ΡA���RuneListContainer���s���POutput
    /// </summary>
    /// <param name="runeListID">��ƪ�s��</param>
    public void ChangeRuneForButton(int runeListID)
    {
        RuneListContainer.Instance.ChangePickID(runeListID);
    }
}
