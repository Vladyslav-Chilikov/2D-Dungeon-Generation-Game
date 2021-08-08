using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Room
{
    public Room(int _type, Vector2 _gridPosition)
    {
        nType = _type;
        v2GridPosition = _gridPosition;
    }
    public int nType;
    public Vector2 v2GridPosition;
    public bool bDoorTop, bDoorBottom, bDoorLeft, bDoorRight;
    public bool bContainRoom = false;
    public TileBase tile;
}
