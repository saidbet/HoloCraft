using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class Creation
{
    //size of the workspace
    public int maxHeight;
    public int maxWidth;
    public int maxDepth;

    public string creationName;
    public Dictionary<Vector3, Block> creationDict = new Dictionary<Vector3, Block>();
    public DateTime date;
    public Texture2D thumbnail;

    public Creation(int height, int width, int depth, string name)
    {
        Initialize(height, width, depth, name);
    }

    public Creation(int height, int width, int depth)
    {
        string randomName = Utility.GenerateName("Creation");
        Initialize(height, width, depth, randomName);
    }

    public Creation()
    {
        string randomName = Utility.GenerateName("Creation");
        Initialize(20, 20, 20, randomName);
    }

    public void Initialize(int height, int width, int depth, string name)
    {
        this.maxHeight = height;
        this.maxWidth = width;
        this.maxDepth = depth;
        this.creationName = name;
        date = DateTime.Now;
        creationDict = new Dictionary<Vector3, Block>();
    }

    public void AddBlock(Vector3 key, Block block)
    {
        creationDict.Add(key, block);
    }

    public void RemoveBlock(Vector3 position)
    {
        ShareManager.Instance.spawnManager.Delete(GetBlock(position).gameObject);
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

    public void AddToCreationsList(CreationsList list)
    {
		int nbr = list.nbrCreations + 1;
        string name = "Creation" + nbr;
        var data = new CreationData(creationDict, name);
        list.AddCreation(data);
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
