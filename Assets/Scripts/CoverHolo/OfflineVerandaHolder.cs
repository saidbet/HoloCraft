using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineVerandaHolder : MonoBehaviour {

    public List<VerandaData> verandas;
    public List<GameObject> prefabs;

	// Use this for initialization
	void Start () {
        verandas = new List<VerandaData>();
        Product p1 = new Product();
        p1.Id = 1;
        p1.Name = "Conservatory";
        p1.Description = "";
        VerandaData ver1 = new VerandaData("", true, p1);
        Product p2 = new Product();
        p2.Id = 2;
        p2.Name = "Veranda Tef";
        p2.Description = "";
        VerandaData ver2 = new VerandaData("", true, p2);
        verandas.Add(ver1);
        verandas.Add(ver2);
    }
}
