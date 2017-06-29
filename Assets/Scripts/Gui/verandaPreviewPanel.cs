using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class verandaPreviewPanel : MonoBehaviour {
    public TextMesh txtVerandaName;
    public TextMesh txtVerandaMeasures;
    public TextMesh txtVerandaType;
    public TextMesh txtVerandaPop;

    public GameObject background;

    private void Start()
    {
        background.GetComponent<MeshRenderer>().material.renderQueue = 2999;
    }

    public void updatePreviewText(string name, string measures, string type, string pop)
    {
        txtVerandaName.text = name;
        txtVerandaMeasures.text = measures;
        txtVerandaType.text = type;
        txtVerandaPop.text = pop;
    }
}
