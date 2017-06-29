using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IpInputPanel : BasePanel {

    public List<BaseButton> listNumbers;
    public BaseButton dot;
    public BaseButton back;
    public List<TextMesh> listIpFields;
    public TextMesh statusConnection;
    public GameObject underLine;
    public BaseButton connectButton;
    public BaseButton offlineButton;
    public int selectedField;

    private void Awake()
    {
        StartCoroutine(BlinkUnderline());
        SetIpFields(NetworkController.Instance.foundIp);
        selectedField = 0;
        statusConnection.text = "Sharing server not found.\nPlease enter IP of the server or select offline mode.";
    }

    IEnumerator BlinkUnderline()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            underLine.GetComponent<Renderer>().enabled = !underLine.GetComponent<Renderer>().enabled;
        }
    }

    public override void OnClick(BaseButton button)
    {
        if (listNumbers.Contains(button))
        {
            AddNumberToCurrentField(listNumbers.IndexOf(button));
        } else if(button == dot)
        {
            AdvanceToNextField();
        } else if(button == back)
        {
            GoBack();
        } else if(button == connectButton)
        {
            TryToConnect();
        } else if(button == offlineButton)
        {
            StayOffline();
        }
    }

    private void AddNumberToCurrentField(int nb)
    {
        string field = listIpFields[selectedField].text;
        if(field == "0")
        {
            field = "" + nb;
            if(nb == 0)
            {
                AdvanceToNextField();
            }
        } else if(field.Length < 3)
        {
            field += nb;
        }
        listIpFields[selectedField].text = field;

        if(field.Length == 3)
        {
            AdvanceToNextField();
        }
    }

    private void TryToConnect()
    {
        string ip = listIpFields[0].text + "." + listIpFields[1].text + "." + listIpFields[2].text + "." + listIpFields[3].text;
        if (SharedController.ValidateIp(ip))
        {
            NetworkController.Instance.ConnectToServer(ip);
            Destroy(gameObject);
        } else
        {
            statusConnection.text = "Error.\nIP invalid, please enter a valid IP.";
        }
    }

    private void AdvanceToNextField()
    {
        if(selectedField < 3)
        {
            selectedField++;
            selectIpField(listIpFields[selectedField]);
        }
    }

    private void GoBack()
    {
        int field = int.Parse(listIpFields[selectedField].text);
        if(field == 0 && selectedField > 0)
        {
            selectedField--;
            selectIpField(listIpFields[selectedField]);
        } else if(field > 9)
        {
            string textIp = listIpFields[selectedField].text;
            textIp = textIp.Substring(0, textIp.Length - 1);
            listIpFields[selectedField].text = textIp;
        } else
        {
            listIpFields[selectedField].text = "0";
        }
    }

    private void OnDestroy()
    {
        StopCoroutine(BlinkUnderline());
    }

    private void SetIpFields(string ip)
    {
        if(ip != "" && SharedController.ValidateIp(ip))
        {
            string[] ips = ip.Split('.');
            for(int i = 0; i < ips.Length; i++)
            {
                listIpFields[i].text = ips[i];
            }
        }
        else
        {
            foreach(TextMesh t in listIpFields)
            {
                t.text = "0";
            }
        }
    }

    public void selectIpField(TextMesh ipField)
    {
        if (listIpFields.Contains(ipField))
        {
            underLine.transform.position = ipField.transform.position;
        }
    }

    private void StayOffline()
    {
        MessageObserver.Instance.StartOfflineMode();
        MainController.Instance.SetPanelsReady();
        Destroy(gameObject);
    }
}
