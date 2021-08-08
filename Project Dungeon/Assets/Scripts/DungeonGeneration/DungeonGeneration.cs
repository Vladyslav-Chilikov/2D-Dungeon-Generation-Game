using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//https://youtu.be/nADIYwgKHv4

public class DungeonGeneration : MonoBehaviour
{
    public int nRoomQuantity;
    public int nDungeonType;
    public Vector2 v2Worldsize = new Vector2(4, 4);

    public Tilemap DungeonMap;
    public Tilemap TokenMap;
    public Tilemap ObstacleMap;

    // TileBase MainTile;
    public TileBase Obstacle;
    public TileBase[] Tokens;
    public TileBase[]
        v1_R_U, v1_R_D, v1_R_R, v1_R_L,         //4       I
        v1_R_UD, v1_R_LR,                       //2       I
        v1_R_UL, v1_R_LD, v1_R_DR, v1_R_UR,     //4       L          
        v1_R_ULD, v1_R_LDR, v1_R_UDR, v1_R_ULR, //4       T
        v1_R_ULDR,                              //1       X         //15 tiles
        v1_WR_UD, v1_WR_LR,                     //2
        v1_WR_UL, v1_WR_LD, v1_WR_DR, v1_WR_UR; //4                 //21 tiles    



    Room[,] rooms;
    List<Vector2> lv2TakenPositions = new List<Vector2>();
    List<Room> RoomList;
    int nGridSizeX, nGridSizeY;

    
    // Start is called before the first frame update
    void Start()
    {
        if (nRoomQuantity >= (v2Worldsize.x * 2) * (v2Worldsize.y * 2))
        {
            nRoomQuantity = Mathf.RoundToInt((v2Worldsize.x * 2) * (v2Worldsize.y * 2));
        }
        nGridSizeX = Mathf.RoundToInt(v2Worldsize.x);
        nGridSizeY = Mathf.RoundToInt(v2Worldsize.y);
        CreateRooms();
        SetRoomDoors();
        DrawDungeon();
        SetObstacles();
    }

 

    void CreateRooms()
    {
        rooms = new Room[nGridSizeX * 2, nGridSizeY * 2];
        rooms[nGridSizeX, nGridSizeY] = new Room( 1, Vector2.zero); // 1st/enter room generation
        lv2TakenPositions.Insert(0, Vector2.zero);
        Vector2 checkPosition = Vector2.zero;
        //number settings
        float fRandomCompare = 0.2f, fRandomCompareStart = 0.2f, fRandomCompareEnd = 0.01f;
        //add rooms
        for(int i = 0; i< nRoomQuantity - 1; i++)
        {
            float fRandomPerc = ((float)i) / ((float)nRoomQuantity - 1);
            fRandomCompare = Mathf.Lerp(fRandomCompareStart, fRandomCompareEnd, fRandomPerc);
            //grab new position
            checkPosition = NewPosition();
            //test new position
            if (NumberOfNeighbors(checkPosition, lv2TakenPositions) > 1 && Random.value > fRandomCompare)
            {
                int iterations = 0;
                do
                {
                    checkPosition = SelectiveNewPosition();
                    iterations++;
                } while (NumberOfNeighbors(checkPosition, lv2TakenPositions) > 1 && iterations < 100);
                if (iterations >= 50)
                    print("error: could not generate with fewer neighbours then: " + NumberOfNeighbors(checkPosition, lv2TakenPositions));
            }
            //finalize position
            rooms[(int)checkPosition.x + nGridSizeX, (int)checkPosition.y + nGridSizeY] = new Room( 0, checkPosition);
            lv2TakenPositions.Insert(0, checkPosition);
        }
    }

    Vector2 NewPosition()
    {
        int x = 0, y = 0;
        Vector2 checkigPos = Vector2.zero;
        do
        {
            int index = Mathf.RoundToInt(Random.value * (lv2TakenPositions.Count - 1));
            x = (int)lv2TakenPositions[index].x;
            y = (int)lv2TakenPositions[index].y;
            bool UpDown = (Random.value < 0.5f);
            bool Positive = (Random.value < 0.5f);
            if (UpDown)
            {
                if (Positive)
                {
                    y += 1;
                }
                else { y -= 1; }

            }
            else
            {
                if (Positive)
                {
                    x += 1;
                }
                else { x -= 1; }
            }
            checkigPos = new Vector2(x, y);
        } while (lv2TakenPositions.Contains(checkigPos)
                || x >= nGridSizeX || x < -nGridSizeX || y >= nGridSizeY || y < -nGridSizeY);
        return checkigPos;
    }

    Vector2 SelectiveNewPosition()
    {
        int index = 0, inc = 0;
        int x = 0, y = 0;
        Vector2 checkigPos = Vector2.zero;
        do
        {
            inc = 0;
            do
            {
                index = Mathf.RoundToInt(Random.value * (lv2TakenPositions.Count - 1));
                inc++;
            } while (NumberOfNeighbors(lv2TakenPositions[index], lv2TakenPositions) > 1 && inc < 100);
            x = (int)lv2TakenPositions[index].x;
            y = (int)lv2TakenPositions[index].y;
            bool UpDown = (Random.value < 0.5f);
            bool Positive = (Random.value < 0.5f);
            if (UpDown)
            {
                if (Positive)
                {
                    y += 1;
                }
                else { y -= 1; }

            }
            else
            {
                if (Positive)
                {
                    x += 1;
                }
                else { x -= 1; }
            }
            checkigPos = new Vector2(x, y);
        } while (lv2TakenPositions.Contains(checkigPos)
                || x >= nGridSizeX || x < -nGridSizeX || y >= nGridSizeY || y < -nGridSizeY);
        if(inc >= 100)
        {
            print("Error could not find position with only 1 neighbor");
        }
        return checkigPos;
    }
  
    int NumberOfNeighbors(Vector2 chekingPos, List<Vector2> usedPositions)
    {
        int result = 0;
        if(usedPositions.Contains(chekingPos + Vector2.right))
        {
            result++;
        }
        if (usedPositions.Contains(chekingPos + Vector2.left))
        {
            result++;
        }
        if (usedPositions.Contains(chekingPos + Vector2.up))
        {
            result++;
        }
        if (usedPositions.Contains(chekingPos + Vector2.down))
        {
            result++;
        }
        return result;
    }

    void SetRoomDoors()
    {
        for(int x = 0; x < (nGridSizeX * 2); x++ )
        {
            for (int y = 0; y < (nGridSizeY * 2); y++)
            {
                if(rooms[x,y] == null) { continue; }
                Vector2 gridPosition = new Vector2(x,y);
                //check above
                if(y - 1 < 0) { rooms[x, y].bDoorBottom = false; } else { rooms[x, y].bDoorBottom = rooms[x, y - 1] != null; }
                //check bellow
                if (y + 1 >= nGridSizeY * 2) { rooms[x, y].bDoorTop = false; } else { rooms[x, y].bDoorTop = rooms[x, y + 1] != null; }
                //check left
                if(x - 1 < 0) { rooms[x, y].bDoorLeft = false; } else { rooms[x, y].bDoorLeft = rooms[x - 1, y] != null; }
                //check right
                if (x + 1 >= nGridSizeX * 2) { rooms[x, y].bDoorRight = false; } else { rooms[x, y].bDoorRight = rooms[x + 1, y] != null; }
            }
        }
    }

    public Room PickTile(Room rom)
    {
        Room temp = rom;
        bool up = temp.bDoorTop, down = temp.bDoorBottom, left = temp.bDoorLeft, right = temp.bDoorRight;
        if (up)
        {
            if (down)
            {
                if (right)
                {
                    if (left)
                    {
                        temp.tile = v1_R_ULDR[nDungeonType - 1];
                        temp.bContainRoom = true;
                    }
                    else
                    {
                        temp.tile = v1_R_UDR[nDungeonType - 1];
                        temp.bContainRoom = true;
                    }
                }
                else if (left)
                {
                    temp.tile = v1_R_ULD[nDungeonType - 1];
                    temp.bContainRoom = true;
                }
                else
                {
                    int nTemp = Random.Range(0, 2);
                    if(nTemp == 0)
                    {
                        temp.tile = v1_R_UD[nDungeonType - 1];         /////////////////////////////////////////////////////
                        temp.bContainRoom = true;
                    } else { temp.tile = v1_WR_UD[nDungeonType - 1]; }
                    
                }
            }
            else
            {
                if (right)
                {
                    if (left)
                    {
                        temp.tile = v1_R_ULR[nDungeonType - 1];
                        temp.bContainRoom = true;
                    }
                    else
                    {
                        int nTemp = Random.Range(0, 2);
                        if (nTemp == 0)
                        {
                            temp.tile = v1_R_UR[nDungeonType - 1]; ////////////////////////////////////////////////////////////
                            temp.bContainRoom = true;
                        } else { temp.tile = v1_WR_UR[nDungeonType - 1]; }
                            
                    }
                }
                else if (left)
                {
                    int nTemp = Random.Range(0, 2);
                    if (nTemp == 0)
                    {
                        temp.tile = v1_R_UL[nDungeonType - 1]; //////////////////////////////////////////////////////////////////
                        temp.bContainRoom = true;
                    } else { temp.tile = v1_WR_UL[nDungeonType - 1]; }
                }
                else
                {
                    temp.tile = v1_R_U[nDungeonType - 1];
                    temp.bContainRoom = true;
                }
            }
            return temp;
        }
        if (down)
        {
            if (right)
            {
                if (left)
                {
                    temp.tile = v1_R_LDR[nDungeonType - 1];
                    temp.bContainRoom = true;
                }
                else
                {
                    int nTemp = Random.Range(0, 1);
                    if (nTemp == 0)
                    {
                        temp.tile = v1_R_DR[nDungeonType - 1]; /////////////////////////////////////////////////
                        temp.bContainRoom = true;
                    } else { temp.tile = v1_WR_DR[nDungeonType - 1]; }
                }
            }
            else if (left)
            {
                int nTemp = Random.Range(0, 1);
                if (nTemp == 0)
                {
                    temp.tile = v1_R_LD[nDungeonType - 1]; /////////////////////////////////////////////
                    temp.bContainRoom = true;
                } else { temp.tile = v1_WR_LD[nDungeonType - 1]; }
            }
            else
            {
                temp.tile = v1_R_D[nDungeonType - 1];
                temp.bContainRoom = true;
            }
            return temp;
        }
        if (right)
        {
            if (left)
            {
                int nTemp = Random.Range(0, 1);
                if (nTemp == 0)
                {
                    temp.tile = v1_R_LR[nDungeonType - 1]; ////////////////////////////////////////////////
                    temp.bContainRoom = true;
                } else { temp.tile = v1_WR_LR[nDungeonType - 1]; }
            }
            else
            {
                temp.tile = v1_R_R[nDungeonType - 1];
                temp.bContainRoom = true;
            }
        }
        else
        {
            temp.tile = v1_R_L[nDungeonType - 1];
            temp.bContainRoom = true;
        }
        return temp;
    }

    void DrawDungeon()
    {
        for (int i = 0; i < nGridSizeX * 2; i++)
        {
            for (int j = 0; j < nGridSizeY * 2; j++)
            {
                if (rooms[i, j] == null) { continue; }
                Vector3Int tileAxxis = new Vector3Int((int)rooms[i, j].v2GridPosition.x, (int)rooms[i, j].v2GridPosition.y, 0);
                rooms[i, j] = PickTile(rooms[i, j]);
                DungeonMap.SetTile(tileAxxis, rooms[i, j].tile);
                if (rooms[i, j].bContainRoom == true && rooms[i, j].nType != 1)
                {
                    int rnd = Random.Range(0, Tokens.Length);
                    TokenMap.SetTile(tileAxxis, Tokens[rnd]);
                }
            }
        }
   }

    void SetObstacles()
    {
        for (int i = 0; i < nGridSizeX * 2; i++)
        {
            for (int j = 0; j < nGridSizeY * 2; j++)
            {
                if (rooms[i, j] == null) { continue; }
                if (rooms[i, j].bDoorTop == false)
                {
                    Vector3Int tileAxxis = new Vector3Int((int)rooms[i, j].v2GridPosition.x, (int)rooms[i, j].v2GridPosition.y + 1, 0);
                    ObstacleMap.SetTile(tileAxxis, Obstacle);
                }
                if (rooms[i, j].bDoorBottom == false)
                {
                    Vector3Int tileAxxis = new Vector3Int((int)rooms[i, j].v2GridPosition.x, (int)rooms[i, j].v2GridPosition.y - 1, 0);
                    ObstacleMap.SetTile(tileAxxis, Obstacle);
                }
                if (rooms[i, j].bDoorRight == false)
                {
                    Vector3Int tileAxxis = new Vector3Int((int)rooms[i, j].v2GridPosition.x + 1, (int)rooms[i, j].v2GridPosition.y, 0);
                    ObstacleMap.SetTile(tileAxxis, Obstacle);
                }
                if (rooms[i, j].bDoorLeft == false)
                {
                    Vector3Int tileAxxis = new Vector3Int((int)rooms[i, j].v2GridPosition.x - 1, (int)rooms[i, j].v2GridPosition.y, 0);
                    ObstacleMap.SetTile(tileAxxis, Obstacle);
                }
            }
        }
    }
}
