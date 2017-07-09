using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "holocraft/properties/floatProperty")]
public class FloatProperty : ScriptableObject {

    public string text;
    public float value;
    public int minValue;
    public int maxValue;
}
