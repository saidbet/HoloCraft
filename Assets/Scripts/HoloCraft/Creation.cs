using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    public void SaveCreation(string name)
    {
        //TODO
    }

    public string GetFilePath(string fileName)
    {
        return Path.Combine(Application.streamingAssetsPath, fileName + ".JSON");
    }


    public void FindAdjacents()
    {
        //List<Vector3> positions = creationDict.Keys.ToList();
    }
}
