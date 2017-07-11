using System.Collections;
using System.Collections.Generic;
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
        if (creationDict.ContainsKey(position))
        {
            Destroy(GetBlock(position).gameObject);
        }
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

    public void MoveWorkspace()
    {
        MainManager.Instance.mode = MainManager.Mode.Moving;
        MenuManager.Instance.ToggleMenu(gameObject, false);
    }

    public void ScaleWorkspace()
    {
        MainManager.Instance.mode = MainManager.Mode.Scaling;
        MenuManager.Instance.ToggleMenu(gameObject, false);
    }
}
