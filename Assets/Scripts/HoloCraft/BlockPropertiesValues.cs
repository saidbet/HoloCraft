using System.Collections.Generic;
using UnityEngine;

public class BlockPropertiesValues : MonoBehaviour
{
    public List<Property> properties;

    public void CreateProperties()
    {
        BlockType type = GetComponent<Block>().type;

        properties = new List<Property>();

        foreach (PropertyValue prop in type.properties)
        {
            properties.Add(new Property(prop.property, prop.value));
        }
    }
}
