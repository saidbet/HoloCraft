using UnityEngine;

public class VerandaPrefab {
    public int id;
    public GameObject prefab;

    public VerandaPrefab()
    {
        id = 0;
        prefab = null;
    }

    public VerandaPrefab(int id, GameObject prefab)
    {
        this.id = id;
        this.prefab = prefab;
    }
}
