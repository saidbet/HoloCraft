using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyHolder : MonoBehaviour
{
    public Properties property;
    public Toggle toggle;
    public GameObject plusButton;
    public GameObject minButton;
    public Text input;

    public Block currentObject;
    public Property currentProperty;

    private void Start()
    {
        currentObject = MainManager.Instance.hoveredObject;
        currentProperty = currentObject.properties.Find(prop => prop.property == property);
        input.text = currentProperty.value.ToString();
    }

    public void OnButtonClick(GameObject button)
    {
        float value = float.Parse(input.text);

        if(button == plusButton)
        {
            value += 1;
        }
        else if(button == minButton)
        {
            value -= 1;
        }

        currentProperty.value = value;
    }

    public void OnToggleChange()
    {
        if(toggle.isOn == true)
        {
            currentProperty.value = 1;
        }
        else
        {
            currentProperty.value = 0;
        }
    }
}