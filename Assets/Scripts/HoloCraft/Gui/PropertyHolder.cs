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
        currentObject = MainManager.Instance.creator.HoveredObject;
        currentProperty = currentObject.GetComponent<BlockPropertiesValues>().properties.Find(prop => prop.property == property);

        if (input != null)
            input.text = currentProperty.value.ToString();
        else
        {
            if (currentProperty.value == 0)
                toggle.isOn = false;
            else
                toggle.isOn = true;
        }

    }

    public void OnButtonClick(GameObject button)
    {
        float value = float.Parse(input.text);

        if (button == plusButton)
        {
            value += 1;
        }
        else if (button == minButton)
        {
            value -= 1;
        }

        currentProperty.value = value;
        input.text = value.ToString();
    }

    public void OnToggleChange()
    {
        if (toggle.isOn == false)
        {
            toggle.isOn = true;
            currentProperty.value = 1;
        }
        else
        {
            toggle.isOn = false;
            currentProperty.value = 0;
        }
    }
}