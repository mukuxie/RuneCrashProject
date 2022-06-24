using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RuneList", menuName = "Rune/Runelist", order = 0)]
public class RuneData : ScriptableObject
{
    [Header("把準備好的符文預製物放進來")]
    public GameObject[] runeList = new GameObject[6];
}
