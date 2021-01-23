using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TreeStructGeneration/TreeStructSettings")]
public class TreeStructSettings : ScriptableObject
{
    public Vector2Int size;

    public Vector2Int Size { get => size; }
}
