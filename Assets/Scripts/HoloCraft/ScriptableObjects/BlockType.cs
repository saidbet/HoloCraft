using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "holocraft/blocktype", order = 1)]
public class BlockType : ScriptableObject
{
    public string blockName;
    public GameObject prefab;
    public Sprite thumbnail;
    public Property[] properties;
}
