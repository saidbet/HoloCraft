using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Model
{
    public GameObject modelPrefab;
    public Texture2D modelImage;
}

public class ModelsHolder : Singleton<ModelsHolder>
{

    [SerializeField]
    public List<Model> models;

    public List<Color> colors;
}
