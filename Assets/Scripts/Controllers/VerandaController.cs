using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerandaController : Singleton<VerandaController> {

    public bool verandaPlaced;
    public GameObject veranda = null;

    public void ValidateVeranda(int verandaId)
    {
        if (verandaId != 0)
        {
            if(veranda != null)
            {
                ShareManager.Instance.spawnManager.Delete(veranda);
            }

            SyncSpawnedObject verandaDataModel = new SyncSpawnedObject();
            veranda = ShareManager.Instance.spawnManager.SpawnFromCache(verandaDataModel, verandaId, NetworkSpawnManager.EVERYONE, null);

            AdjustVerandaPos();

            ShareManager.Instance.SendSyncMessage(ShareManager.VERANDA_PLACED, veranda.name);

            if (!verandaPlaced)
                SpaceManager.Instance.HideWallSurfaces();

            verandaPlaced = true;
        }
    }

    public void ValidateVeranda(GameObject verandaPrefab)
    {
        if (veranda != null)
        {
            ShareManager.Instance.spawnManager.Delete(veranda);
        }

        veranda = ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), verandaPrefab, 0, "");

        AdjustVerandaPos();

        ShareManager.Instance.SendSyncMessage(ShareManager.VERANDA_PLACED, veranda.name);

        if(!verandaPlaced)
            SpaceManager.Instance.HideWallSurfaces();

        verandaPlaced = true;
    }

    private List<GameObject> FindModelAnchors(GameObject model)
    {
        List<GameObject> listAnchorRefs = new List<GameObject>();
        listAnchorRefs.Add(model.transform.Find("O").gameObject);
        listAnchorRefs.Add(model.transform.Find("X").gameObject);
        listAnchorRefs.Add(model.transform.Find("Y").gameObject);
        listAnchorRefs.Add(model.transform.Find("Z").gameObject);

        return listAnchorRefs;
    }

    public void AdjustVerandaPos()
    {
        List<GameObject> listAnchors = AnchorController.Instance.anchorsList;
        List<GameObject> listAnchorRefs = FindModelAnchors(veranda);

        float scaleX = (Vector3.Distance(listAnchors[0].transform.position, listAnchors[1].transform.position) / Vector3.Distance(listAnchorRefs[0].transform.localPosition, listAnchorRefs[1].transform.localPosition));
        float scaleY = (Vector3.Distance(listAnchors[0].transform.position, listAnchors[2].transform.position) / Vector3.Distance(listAnchorRefs[0].transform.localPosition, listAnchorRefs[2].transform.localPosition));
        float scaleZ = scaleX;


        veranda.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        veranda.transform.localRotation = listAnchors[0].transform.localRotation;

        veranda.transform.position = listAnchors[0].transform.position;
        Debug.Log("Veranda (and Origin) Pos: " + veranda.transform.position.x + ", " + veranda.transform.position.y + ", " + veranda.transform.position.z +
            "\nAnchor O pos: " + listAnchorRefs[0].transform.position.x + ", " + listAnchorRefs[0].transform.position.y + ", " + listAnchorRefs[0].transform.position.z);

        veranda.transform.position -= (listAnchorRefs[0].transform.position - veranda.transform.position);
    }

    public void RemoveVeranda()
    {
        ShareManager.Instance.spawnManager.Delete(veranda);
        verandaPlaced = false;
    }

}
