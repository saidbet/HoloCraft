using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class Creation : MonoBehaviour
{
    //size of the workspace
    public int maxHeight = 20;
    public int maxWidth = 20;
    public int maxDepth = 20;

    public string creationName;
    public Dictionary<Vector3, Block> creationDict = new Dictionary<Vector3, Block>();
    public string date;
    public Texture2D thumbnail;

    public CreationsList creationsList;

    private void Start()
    {
        DeserializeCreationsList();

        if (creationsList == null)
            creationsList = new CreationsList();
    }

    public void RemoveBlock(Vector3 position)
    {
        Destroy(GetBlock(position).gameObject);
        creationDict.Remove(position);
    }

    public Block GetBlock(Vector3 position)
    {
        if (creationDict.ContainsKey(position))
        {
            return (creationDict[position]);
        }
        else
            return null;
    }

    public bool CheckKey(Vector3 position)
    {
        return creationDict.ContainsKey(position);
    }

    public void AddToDict(Vector3 key, Block block)
    {
        creationDict.Add(key, block);
    }

    public void AddToCreationsList()
    {
		int nbr = creationsList.nbrCreations + 1;
        string name = "Creation" + nbr;
        var data = new CreationData(creationDict, name);
        creationsList.AddCreation(data);
        SerializeCreationsList();
    }

    public void SerializeCreationsList()
    {
        using (FileStream file = File.Open(GetFilePath("CreationsList"), FileMode.OpenOrCreate))
        {
            var serializer = new XmlSerializer(typeof(CreationsList));
            serializer.Serialize(file, creationsList);
        }
    }


    public void DeserializeCreationsList()
    {
		if (!File.Exists(GetFilePath("CreationsList")))
        {
            Debug.Log("File does not exist");
            return;
        }

        using (FileStream file = File.Open(GetFilePath("CreationsList"), FileMode.Open))
        {
            var serializer = new XmlSerializer(typeof(CreationsList));
            creationsList = serializer.Deserialize(file) as CreationsList;
        }
    }


    public string GetFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName + ".xml");
    }

    public void LoadCreation(string name)
    {
        CreationData creationToLoad = creationsList.creations.Find(data => data.name == name);
        CleanUpWorkspace();
        PopulateWorkspace(creationToLoad);
    }

    private void CleanUpWorkspace()
    {
        foreach (var item in creationDict)
        {
            Destroy(item.Value.gameObject);
        }

        creationDict = new Dictionary<Vector3, Block>();
    }

    private void PopulateWorkspace(CreationData data)
    {
        foreach (var item in data.dataToSave)
        {
            GameObject toInstantiate = MainManager.Instance.listOfBlocks.Find(block => block.blockType == item.type).prefab;
            toInstantiate = Instantiate(toInstantiate, MainManager.Instance.workspaceHolder.transform);
            Vector3 position = new Vector3(item.posX, item.posY, item.posZ);
            toInstantiate.transform.localPosition = position;
            MainManager.Instance.Validate(position, toInstantiate);
        }
    }
}

[Serializable]
public class CreationData
{
    public string name;
    public BlockData[] dataToSave;

	public CreationData()
	{
		
	}

    public CreationData(Dictionary<Vector3, Block> dict, string name)
    {
        dataToSave = new BlockData[dict.Count];
        int index = 0;
        foreach (var item in dict)
        {
            dataToSave[index].posX = item.Key.x;
            dataToSave[index].posY = item.Key.y;
            dataToSave[index].posZ = item.Key.z;

            dataToSave[index].type = item.Value.type.blockType;
            index++;
        }

        this.name = name;
    }
}

[Serializable]
public struct BlockData
{
    public float posX;
    public float posY;
    public float posZ;
    public BlockType.BlockTypes type;
}

[XmlRoot("CreationList"), Serializable]
public class CreationsList
{
    public int nbrCreations;
    [XmlArray("Creations"), XmlArrayItem("Creation")]
    public List<CreationData> creations;

    public CreationsList()
    {
        if (creations == null)
            creations = new List<CreationData>();
    }

    public void AddCreation(CreationData data)
    {
        creations.Add(data);
        nbrCreations++;
    }

    public CreationData GetCreation(int index)
    {
        return creations[index];
    }

    public void RemoveCreation(int index)
    {
        creations.RemoveAt(index);
    }
}
