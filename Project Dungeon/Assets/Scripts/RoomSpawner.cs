using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public char cOpeningDirection;
    //N - north door ...
    public int rnd;

    public static int nRoomCounter = 0;
    public int nnRoomCounter = 0;         // удалить после отладки

    private TileTemplatesScript templates;
    private bool bSpawned = false;

    private DungeonGenerationScript MapSeed;

   

    public void Start()
    {
        templates = GameObject.Find("Tileset Templates").GetComponent<TileTemplatesScript>();
        MapSeed = GameObject.Find("Tileset Templates").GetComponent<DungeonGenerationScript>();
        if (MapSeed.bRandomSeed == true) { MapSeed.nMapSeed = Random.Range(0, 100000); }
        //Random.InitState(MapSeed.nMapSeed);
        Invoke("SpawnTiles", 0.9f);
    }


    public void SpawnTiles()
    {
        if((bSpawned == false) && (nRoomCounter < 50))
        {
            if (cOpeningDirection == 'N')
            {
                //spawn a room with S door
                if (MapSeed.nDungeonTileType == 1)
                {
                    rnd = Random.Range(0, templates.SV1Tileset.Length);
                    Instantiate(templates.SV1Tileset[rnd], transform.position, templates.SV1Tileset[rnd].transform.rotation);
                }
            }
            else if (cOpeningDirection == 'E')
            {
                //spawn a room with W door
                if (MapSeed.nDungeonTileType == 1)
                {
                    rnd = Random.Range(0, templates.WV1Tileset.Length);
                    Instantiate(templates.WV1Tileset[rnd], transform.position, templates.WV1Tileset[rnd].transform.rotation);
                }
            }
            else if (cOpeningDirection == 'S')
            {
                //spawn a room with N door
                if (MapSeed.nDungeonTileType == 1)
                {
                    rnd = Random.Range(0, templates.NV1Tileset.Length);
                    Instantiate(templates.NV1Tileset[rnd], transform.position, templates.NV1Tileset[rnd].transform.rotation);
                }
            }
            else if (cOpeningDirection == 'W')
            {
                //spawn a room with E door
                if (MapSeed.nDungeonTileType == 1)
                {
                    rnd = Random.Range(0, templates.EV1Tileset.Length);
                    Instantiate(templates.EV1Tileset[rnd], transform.position, templates.EV1Tileset[rnd].transform.rotation);
                }   
            }
        }
        bSpawned = true;
        nRoomCounter += 1;
        nnRoomCounter = nRoomCounter;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("SpawnPoint"))
        {
            if(collision.GetComponent<RoomSpawner>().bSpawned == false && bSpawned == false)
            {

            }
            Destroy(gameObject);
           // bSpawned = true;
        }
        if (collision.CompareTag("Tile"))
        {
            Destroy(gameObject);
            bSpawned = true;
        }

    }
}
