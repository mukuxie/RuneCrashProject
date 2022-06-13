using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FruitSpawner))]
public class AddButtonToResetTesting : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FruitSpawner m_fS = (FruitSpawner)target;

        if (GUILayout.Button("_RESET TESTING_"))
        {
            m_fS.ResetAllFruit();
        }
    }
}

