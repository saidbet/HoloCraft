using UnityEngine;

public enum Properties
{
    Weight,
    Speed,
    Steerable,
    Direction
}

[System.Serializable]
public struct PropertyValue
{
    public Properties property;
    public float value;
}

[System.Serializable]
public struct PropertyPrefab
{
    public Properties property;
    public GameObject prefab; 
}