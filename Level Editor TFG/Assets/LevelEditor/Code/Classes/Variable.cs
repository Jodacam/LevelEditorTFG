using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public abstract class IData
{
    [SerializeField]
    public string varName;

    [SerializeField]
    public Level.VariableTypes type {get; protected set;}

    public void Init (string name)
    {
        varName = name;
    }
#if UNITY_EDITOR
    public abstract void ShowGUI();
#endif
}


[Serializable]
public class VariableString : IData
{
    [SerializeField]
    public string value;

    public new void Init(string name)
    {
        base.Init(name);
        type = Level.VariableTypes.String;
    }

#if UNITY_EDITOR
    public override void ShowGUI()
    {
        value = EditorGUILayout.TextField("Value",value);
    }
    #endif
}

[Serializable]
public class VariableInt : IData
{
    [SerializeField]
    public int value;

    public new void Init(string name)
    {
        base.Init(name);
        type = Level.VariableTypes.Int;
    }

#if UNITY_EDITOR
    public override void ShowGUI()
    {
        value = EditorGUILayout.IntField("Value",value);
    }
    #endif
}
[Serializable]
public class VariableBool : IData
{
    [SerializeField]
    public bool value;

    public new void Init(string name)
    {
        base.Init(name);
        type = Level.VariableTypes.Boolean;
    }

#if UNITY_EDITOR
    public override void ShowGUI()
    {
        value = EditorGUILayout.Toggle("Value",value);
    }
    #endif
}
[Serializable]
public class VariableFloat : IData
{
    [SerializeField]
    public float value;

    public new void Init(string name)
    {
        base.Init(name);
        type = Level.VariableTypes.Float;
    }

#if UNITY_EDITOR
    public override void ShowGUI()
    {
        value = EditorGUILayout.FloatField("Value",value);
    }
    #endif
}


