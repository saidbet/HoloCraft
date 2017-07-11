using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiMenu : MonoBehaviour {

    public HighlightManager highlight;
    public GameObject[] guiElements;
    public int elementCoeff = 110;
    public int nbrElementPerLine = 6;
    protected int currentIndex;

    protected void Start()
    {
        currentIndex = 0;
        highlight.SetMaxSiblingIndex();
        highlight.SetHighlight(guiElements[currentIndex].gameObject);
    }

    public void GetNewPos(MainManager.Direction direction)
    {
        if (direction == MainManager.Direction.Right)
        {
            if (currentIndex < guiElements.Length - 1)
                currentIndex += 1;
        }
        else if (direction == MainManager.Direction.Left)
        {
            if (currentIndex > 0)
                currentIndex -= 1;
        }
        else if (direction == MainManager.Direction.Up)
        {
            if ((currentIndex - nbrElementPerLine) >= 0)
                currentIndex -= nbrElementPerLine;
        }
        else if (direction == MainManager.Direction.Down)
        {
            if ((currentIndex + nbrElementPerLine) < guiElements.Length)
                currentIndex += nbrElementPerLine;
        }

        highlight.SetHighlight(guiElements[currentIndex].gameObject);
    }
}
