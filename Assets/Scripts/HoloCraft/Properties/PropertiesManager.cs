using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PropertyPrefab
{
    public Property property;
    public GameObject prefab;
}

public class PropertiesManager : MonoBehaviour
{
    public Block currentBlock;
    public GameObject background;
    private GameObject current;
    private RectTransform rect;
    public PropertyPrefab[] propertiesArray;
    public Dictionary<Property, GameObject> propertiesPrefabs;

    private void Awake()
    {
        for(int i = 0; i < propertiesArray.Length; i++)
        {
            //propertiesPrefabs.Add(propertiesArray[i].property, propertiesArray[i].prefab);
        }
    }

    private void Start()
    {
        //rect = current.GetComponent<RectTransform>();
        //rect.pivot = new Vector2(0, 1);
        //rect.anchoredPosition = new Vector2(0, 1);
        //for(int i = 0; i< currentBlock.type.boolProperties.Length; i++)
        //{

        //}
    }
}
