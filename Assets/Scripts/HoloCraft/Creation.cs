using System;
using System.Collections.Generic;
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

        this.date = DateTime.Now;

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
        var data = new CreationData(this);
        list.AddCreation(data);
    }

    public void SetUpFromLoadData(CreationData data)
    {
        maxHeight = data.maxHeight;
        maxWidth = data.maxWidth;
        maxDepth = data.maxDepth;

        creationName = data.creationName;
        date = data.date;
        creationDict.Clear();
    }
}

[Serializable]
public struct BlockData
{
    //Position
    public float posX;
    public float posY;
    public float posZ;
    //Rotation
    public float rotX;
    public float rotY;
    public float rotZ;
    public BlockType.BlockTypes type;
}

[Serializable]
public class CreationData
{
    public string creationName;

    public DateTime date;

    public int maxHeight;
    public int maxWidth;
    public int maxDepth;

    public BlockData[] savedBlocks;

    public CreationData(Creation creation)
    {
        this.creationName = creation.creationName;

        maxHeight = creation.maxHeight;
        maxWidth = creation.maxWidth;
        maxDepth = creation.maxDepth;

        date = creation.date;
        DictToArray(creation.creationDict);
    }

    public CreationData()
    {

    }

    private void DictToArray(Dictionary<Vector3, Block> dict)
    {
        savedBlocks = new BlockData[dict.Count];
        int index = 0;
        foreach (var item in dict)
        {
            savedBlocks[index].posX = item.Key.x;
            savedBlocks[index].posY = item.Key.y;
            savedBlocks[index].posZ = item.Key.z;

            savedBlocks[index].rotX = item.Value.rotation.eulerAngles.x;
            savedBlocks[index].rotY = item.Value.rotation.eulerAngles.y;
            savedBlocks[index].rotZ = item.Value.rotation.eulerAngles.z;

            savedBlocks[index].type = item.Value.type.blockType;
            index++;
        }
    }
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
