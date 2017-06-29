using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CloudDataManager : Singleton<CloudDataManager> {

    public string address = "http://52.233.157.150:8080/";
    //private string address = "http://10.120.31.169:8080/";
    public string path = "products";
    public string pathAsset = "/asset";
    public Dictionary<int, VerandaData> listVerandas;
    public Dictionary<int, GameObject> prefabCache;

    public List<GameObject> selectedWalls;
    public Color baseWallWireColor;

    private bool firstPull = true;

    void Start () {
        ShareManager.Instance.onMessageEvent += Instance_onMessageEvent;
        listVerandas = new Dictionary<int, VerandaData>();
        selectedWalls = new List<GameObject>();
        prefabCache = new Dictionary<int, GameObject>();
        GetListVerandas();
    }

    private void Instance_onMessageEvent(MessageSynchronizer syncMessage)
    {
        switch (syncMessage.messageType.Value)
        {
            case ShareManager.SELECTED_WALLS:
                UpdateSelectedWallsList(syncMessage.stringData.Value);
                break;
        }
    }

    public GameObject GetPrefab(int id)
    {
        if (prefabCache.ContainsKey(id))
        {
            return prefabCache[id];
        } else
        {
            return null;
        }
        
    }

    public bool IsVerandaValid(int id)
    {
        return listVerandas[id].Valid;
    }

    public void GetListVerandas(List<GameObject> listWalls)
    {
        Debug.Log("GetListVerandas");
        // Compute segments
        //List<Dictionary<string, Dictionary<string, string>>> segments = new List<Dictionary<string, Dictionary<string, string>>>();

        List<Segment> segments = new List<Segment>();

        foreach(GameObject wall in listWalls)
        {
            float x1, x2, y1, y2;
            Bounds bound = wall.GetComponent<Renderer>().bounds;
            Vector3 min = bound.min;
            Vector3 max = bound.max;
            x1 = min.x;
            x2 = max.x;
            y1 = min.z;
            y2 = max.z;

            Segment segment = new Segment();
            segment.point1 = new Point(x1, y1);
            segment.point2 = new Point(x2, y2);
            segments.Add(segment);
        }

        

        WWWForm form = new WWWForm();
        // ADD Infos
        //JsonSerializerSettings settings = new JsonSerializerSettings();
        if(segments.Count > 0)
        {
            string jsonstring = "{\"segments\":[";
            foreach (Segment s in segments)
            {
                jsonstring += "{\"point1\":{\"x\":\"" + s.point1.x + "\",\"y\":\"" + s.point1.y + "\"},\"point2\":{\"x\":\"" + s.point2.x + "\",\"y\":\"" + s.point2.y + "\"}},";
            }
            jsonstring = jsonstring.Substring(0, jsonstring.Length - 1);
            jsonstring += "]}";
            Debug.Log(jsonstring);
            StartCoroutine(PostData(address + path, jsonstring));
        } else
        {
            StartCoroutine(PostData(address + path, "[]"));
        }
    }

    public void GetListVerandas()
    {
        List<GameObject> emptyList = new List<GameObject>();
        GetListVerandas(emptyList);
    }

    private void ParseData(string data)
    {
        listVerandas = new Dictionary<int, VerandaData>();
        Debug.Log("Download finished: " + data);
        int idx = data.IndexOf("},{");
        List<string> json = new List<string>();
        if(idx == -1)
        {
            string newdata = data.Substring(1, data.Length -2);
            json.Add(newdata);
        }
        else
        {
            while (idx > -1)
            {
                string newdata = data.Substring(1, idx);
                json.Add(newdata);
                data = data.Substring(idx + 2);
                idx = data.IndexOf("},{");
            }
            json.Add(data.Substring(0, data.Length - 1));
        }
        foreach(string ver in json)
        {
            VerandaData verandaData = DeserializeVeranda(ver);
            listVerandas.Add(verandaData.Veranda.Id, verandaData);
        }

        if (firstPull)
        {
            firstPull = false;
            foreach(VerandaData veranda in listVerandas.Values)
            {
                StartCoroutine(GetPrefabFromServer(veranda.Veranda.Id));
            }
        }
    }

    public IEnumerator PostData(string url, string jsonData)
    {
        UnityWebRequest www = new UnityWebRequest(url);
        www.SetRequestHeader("Content-Type", "application/json");
        if (jsonData.Equals("[]"))
        {
            jsonData = "{}";
        }

        
        www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
        www.downloadHandler = new DownloadHandlerBuffer();
        www.method = UnityWebRequest.kHttpVerbPOST;
        yield return www.Send();
        if (www.isError)
        {
            Debug.Log("Erreur serveur: " + www.error);
            // Fetch verandas from OfflineVerandaHolder
            OfflineVerandaHolder holder = gameObject.GetComponent<OfflineVerandaHolder>();
            foreach(VerandaData v in holder.verandas)
            {
                prefabCache.Add(v.Veranda.Id, holder.prefabs[v.Veranda.Id -1]);
                listVerandas.Add(v.Veranda.Id, v);
            }
        } else
        {
            ParseData(www.downloadHandler.text);
        }
    }

    public VerandaData DeserializeVeranda(string data)
    {
        VerandaData verandaData = null;
        int idx = data.IndexOf("product");
        string newData = data.Substring(idx + 9);
        newData = newData.Substring(1, newData.Length - 3);
        if(data[0] != '{')
        {
            data = "{" + data;
        }
        Debug.Log("data:" + data);
        
        verandaData = JsonUtility.FromJson<VerandaData>(data);
        Product product = new Product();
        string[] properties = newData.Split(',');
        string[] element = properties[0].Split(':');
        product.Name = element[1].Substring(1, element[1].Length - 2);
        element = properties[1].Split(':');
        product.Description = element[1].Substring(1, element[1].Length - 2);
        element = properties[2].Split(':');
        product.Document = element[1].Substring(1, element[1].Length - 2);
        element = properties[3].Split(':');
        product.Thumbnail = element[1].Substring(1, element[1].Length - 2);
        element = properties[4].Split(':');
        product.Id = int.Parse(element[1]);
        verandaData.Veranda = product;
        return verandaData;
    }

    private void UpdateSelectedWallsList(string wallList)
    {
        List<GameObject> tempWalls = new List<GameObject>();
        if (wallList.Length > 0)
        {
            string[] wallNames = wallList.Split('*');
            foreach (string name in wallNames)
            {
                tempWalls.Add(GameObject.Find(name));
            }
        }

        foreach(GameObject wall in selectedWalls)
        {
            if (!tempWalls.Contains(wall))
            {
                wall.GetComponent<Renderer>().material.SetColor("_WireColor", baseWallWireColor);
            }
        }
        selectedWalls = new List<GameObject>();
        foreach(GameObject wall in tempWalls)
        {
            wall.GetComponent<Renderer>().material.SetColor("_WireColor", Color.red);
            selectedWalls.Add(wall);
        }

        GetListVerandas(selectedWalls);
        
    }

    private IEnumerator GetPrefabFromServer(int id)
    {
        string url = address + path + "/" + id + pathAsset;
        // Download the file from the URL. It will not be saved in the Cache
        using (WWW www = new WWW(url))
        {
            AssetBundleRequest asset = null;
            yield return www;
            if (www.error != null)
                throw new Exception("WWW download had an error:" + www.error);

            AssetBundle bundle = www.assetBundle;
            string[] names = bundle.GetAllAssetNames();
            foreach (string name in names)
            {
                if (name.Contains(".prefab"))
                {
                    asset = bundle.LoadAssetAsync(name);
                    break;
                }
            }
            GameObject prefab = asset.asset as GameObject;

            if (prefab != null && !prefabCache.ContainsKey(id))
            {
                prefabCache.Add(id, prefab);
                Debug.Log(prefabCache[id].name);
            }
            // Unload the AssetBundles compressed contents to conserve memory
            bundle.Unload(false);
        } // memory is freed from the web stream (www.Dispose() gets called implicitly)
    }
}
