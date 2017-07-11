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
    public PropertyPrefab[] propertiesArray;
    public Dictionary<Property, GameObject> propertiesPrefabs = new Dictionary<Property, GameObject>();

    private void Awake()
    {
        for(int i = 0; i < propertiesArray.Length; i++)
        {
            propertiesPrefabs.Add(propertiesArray[i].property, propertiesArray[i].prefab);
        }
    }

    private void OnEnable()
    {
        Block currentBlock = MainManager.Instance.HoveredObject;
        if (currentBlock == null) return;

        for (int i = 0; i < currentBlock.type.properties.Length; i++)
        {
            Debug.Log(currentBlock.type.properties[i]);
            GameObject currentGo = Instantiate(propertiesPrefabs[currentBlock.type.properties[i]], transform);
            currentGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -i * 40);
        }
    }

    private void OnDisable()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
