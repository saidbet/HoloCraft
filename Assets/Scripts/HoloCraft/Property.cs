using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Property
{
    public Properties property;
    public float value;

    public Property(Properties prop, float value)
    {
        property = prop;
        this.value = value;
    }
}
