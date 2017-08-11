using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectPicker : MonoBehaviour, IMenu
{
    public BlocksArray blocksArray;
    public GameObject elementPrefab;
    public GameObject[] guiElements;
    public int nbrElementsPerLine;
    public int elementSize;
    public int currentIndex;

    private void Awake()
    {
        guiElements = new GameObject[blocksArray.array.Length];

        for (int i = 0; i < blocksArray.array.Length; i++)
        {
            PlaceElement(i);
            guiElements[i].transform.GetChild(0).GetComponent<Image>().sprite = blocksArray.array[i].thumbnail;
        }
    }

    private void OnEnable()
    {
        currentIndex = 0;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(guiElements[currentIndex]);
    }

    protected void PlaceElement(int index)
    {
        guiElements[index] = Instantiate(elementPrefab, transform);
        guiElements[index].GetComponent<RectTransform>().anchoredPosition = GetValidPosition(index);
        guiElements[index].GetComponent<ObjectPickerOnSubmit>().blockType = blocksArray.array[index];
    }

    protected Vector2 GetValidPosition(int index)
    {
        int x = index % nbrElementsPerLine;

        int y = index / nbrElementsPerLine;

        return new Vector2(x * elementSize, -y * elementSize);
    }

    public void MoveSelection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Down:
                if ((currentIndex + nbrElementsPerLine) < guiElements.Length)
                    currentIndex += nbrElementsPerLine;
                break;

            case Direction.Up:
                if (currentIndex >= nbrElementsPerLine)
                    currentIndex -= nbrElementsPerLine;
                break;

            case Direction.Right:
                if (currentIndex < guiElements.Length - 1)
                    currentIndex += 1;
                break;

            case Direction.Left:
                if (currentIndex > 0)
                    currentIndex -= 1;
                break;
        }

        EventSystem.current.SetSelectedGameObject(guiElements[currentIndex]);
    }
}
