using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RuneList", menuName = "Rune/Runelist", order = 0)]
public class RuneData : ScriptableObject
{
    [Header("��ǳƦn���Ť�w�s����i��")]
    public GameObject[] runeList = new GameObject[6];
}
