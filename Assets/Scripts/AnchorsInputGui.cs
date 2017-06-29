using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorsInputGui : BasePanel {

    public const int XAXIS = 1;
    public const int YAXIS = 2;

    public SettingsGui settingsGUI;

    public List<BaseButton> inputNumbersButtons;
    public List<BaseButton> uppers;
    public List<BaseButton> lowers;

    public List<BaseButton> numbers;
    public BaseButton backButton;

    public List<Texture2D> textureNumbers;

    public int[] inputNumbers;

    public float xAxisDistance;
    public float yAxisDistance;
    public float currentAxis;

    public BaseButton cancelAnchorEdit;
    public BaseButton saveAnchorEdit;

    public GameObject caret;
    public int actualIndex;
    public int type;

    protected override void Start()
    {
        actualIndex = 0;
        //inputNumbers = new int[5];
        caret.transform.localPosition = new Vector3(inputNumbersButtons[0].transform.localPosition.x, inputNumbersButtons[0].transform.localPosition.y, -1);
        StartCoroutine(FlashCaret());
    }

    public override void OnClick(BaseButton button)
    {
        if (uppers.Contains(button))
        {
            AddNumberToIndex(uppers.IndexOf(button));
        }else if (lowers.Contains(button))
        {
            SubstractNumberToIndex(lowers.IndexOf(button));
        } else if (numbers.Contains(button))
        {
            ModifyNumberAtCurrentIndex(numbers.IndexOf(button));
        }
        else if(button == backButton)
        {
            DeleteNumberAtCurrentIndex();
        }
        else if(inputNumbersButtons.Contains(button))
        {
            actualIndex = inputNumbersButtons.IndexOf(button);
            UpdateCaretPosition();
        }
        else if (button == cancelAnchorEdit)
        {
            settingsGUI.HideAnchorEdit();
        }
        else if (button == saveAnchorEdit)
        {
            SaveAndApplyAnchor();
            settingsGUI.HideAnchorEdit();
        }
    }

    private void AddNumberToIndex(int index)
    {
        if(inputNumbers[index] == 9)
        {
            inputNumbers[index] = 0;
        }
        else
        {
            inputNumbers[index]++;
        }

        UpdateNumber();
    }

    private void SubstractNumberToIndex(int index)
    {
        if(inputNumbers[index] == 0)
        {
            inputNumbers[index] = 9;
        }
        else
        {
            inputNumbers[index]--;
        }
        UpdateNumber();
    }

    private void ModifyNumberAtCurrentIndex(int number)
    {
        inputNumbers[actualIndex] = number;
        if(actualIndex < 4)
        {
            actualIndex++;
        }
        UpdateNumber();
        UpdateCaretPosition();
    }

    private void DeleteNumberAtCurrentIndex()
    {
        inputNumbers[actualIndex] = 0;
        if (actualIndex > 0)
        {
            actualIndex--;
        }
        UpdateNumber();
        UpdateCaretPosition();
    }

    private void UpdateNumber()
    {
        GetInputNumbersForCurrentAxis();
    }

    private void UpdateCaretPosition()
    {
        caret.transform.localPosition = new Vector3(inputNumbersButtons[actualIndex].transform.localPosition.x, 
                                                    inputNumbersButtons[actualIndex].transform.localPosition.y, -1);
    }

    public void ActivateBehaviour(int type)
    {
        this.type = type;
        UpdateAnchorsData();
        if(type == XAXIS)
        {
            currentAxis = xAxisDistance;
        } else if(type == YAXIS)
        {
            currentAxis = yAxisDistance;
        }
        SetInputNumbersFromCurrentAxis();
    }

    public void UpdateAnchorsData()
    {
        List<GameObject> listAnchors = AnchorController.Instance.anchorsList;
        float tempX = Vector3.Distance(listAnchors[0].transform.position, listAnchors[1].transform.position);
        float tempY = Vector3.Distance(listAnchors[0].transform.position, listAnchors[2].transform.position);
        string xString = tempX.ToString("F3");
        string yString = tempY.ToString("F3");
        xAxisDistance = float.Parse(xString);
        yAxisDistance = float.Parse(yString);
    }

    private void GetInputNumbersForCurrentAxis()
    {
        int number = inputNumbers[0] * 10000;
        number += inputNumbers[1] * 1000;
        number += inputNumbers[2] * 100;
        number += inputNumbers[3] * 10;
        number += inputNumbers[4];
        currentAxis = number / 1000f;

        SetInputButtonsFromNumbers();
    }

    private void SetInputNumbersFromCurrentAxis()
    {
        string number = currentAxis.ToString("F3");
        Debug.Log("inputFromCurrentAxis: " + number);
        string units;
        string decimals;
        units = number.Split('.')[0];
        decimals = number.Split('.')[1];

        char[] unitNums = units.ToCharArray();
        char[] decNums = decimals.ToCharArray();
        if(unitNums.Length > 1)
        {
            inputNumbers[0] = int.Parse(unitNums[0].ToString());
            inputNumbers[1] = int.Parse(unitNums[1].ToString());
        } else
        {
            inputNumbers[0] = 0;
            inputNumbers[1] = int.Parse(unitNums[0].ToString());
        }

        int i = 0;
        for(; i < decNums.Length; i++)
        {
            inputNumbers[i + 2] = int.Parse(decNums[i].ToString());
            Debug.Log("input numbers: " + inputNumbers[i + 2] + ", decNum: " + int.Parse(decNums[i].ToString()));
        }
        for(i+=2; i < inputNumbers.Length; i++)
        {
            inputNumbers[i] = 0;
        }
        Debug.Log(inputNumbers[0] + " " + inputNumbers[1] + " " + inputNumbers[2] + " " + inputNumbers[3] + " " + inputNumbers[4]);
        SetInputButtonsFromNumbers();
    }

    private void SetInputButtonsFromNumbers()
    {
        for(int i = 0; i < inputNumbers.Length; i++)
        {
            inputNumbersButtons[i].GetComponent<MeshRenderer>().material.SetTexture("_MainTex", textureNumbers[inputNumbers[i]]);
        }
    }

    public void SaveAndApplyAnchor()
    {
        if(type == XAXIS)
        {
            AnchorController.Instance.AdjustAnchorsPositions(currentAxis, yAxisDistance);
        } else if(type == YAXIS)
        {
            AnchorController.Instance.AdjustAnchorsPositions(xAxisDistance, currentAxis);
        }
        VerandaController.Instance.AdjustVerandaPos();
    }

    private IEnumerator FlashCaret()
    {
        Renderer caretRenderer = caret.GetComponent<Renderer>();
        while (true)
        {
            caretRenderer.enabled = !caretRenderer.enabled;
            yield return new WaitForSeconds(1.5f);
        }
    }

    private void OnDestroy()
    {
        StopCoroutine(FlashCaret());
    }
}
