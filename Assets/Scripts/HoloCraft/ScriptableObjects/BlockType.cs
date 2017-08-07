using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "holocraft/blocktype", order = 1)]
public class BlockType : ScriptableObject
{
    public enum BlockTypes
    {
        Cube,
        Triangle,
        Wheel
    }

    public BlockTypes blockType;
    public GameObject buildPrefab;
    public GameObject playPrefab;
    public Sprite thumbnail;
    public PropertyValue[] properties;
}
