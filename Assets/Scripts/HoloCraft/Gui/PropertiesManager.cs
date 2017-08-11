using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PropertiesManager : MonoBehaviour, IMenu
{
    public List<PropertyPrefab> propertiesPrefabs;
    public Selectable[] selectables;
    public int currentIndex;

    private void OnEnable()
    {
        Block currentBlock = MainManager.Instance.HoveredObject;
        if (currentBlock == null) return;


        for (int i = 0; i < currentBlock.type.properties.Length; i++)
        {
            GameObject current = Instantiate(propertiesPrefabs.Find(prop => prop.property == currentBlock.type.properties[i].property).prefab, transform);
            current.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -i * 40);
        }

        selectables = GetComponentsInChildren<Selectable>();

    }

    private void OnDisable()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void MoveSelection(MainManager.Direction direction)
    {
        switch (direction)
        {
            case MainManager.Direction.Down:
                if (currentIndex < selectables.Length - 1)
                    currentIndex += 1;
                break;

            case MainManager.Direction.Up:
                if (currentIndex > 0)
                    currentIndex -= 1;
                break;

            case MainManager.Direction.Right:
                if (currentIndex < selectables.Length - 1)
                    currentIndex += 1;
                break;

            case MainManager.Direction.Left:
                if (currentIndex > 0)
                    currentIndex -= 1;
                break;
        }

        EventSystem.current.SetSelectedGameObject(selectables[currentIndex].gameObject);
    }
}
