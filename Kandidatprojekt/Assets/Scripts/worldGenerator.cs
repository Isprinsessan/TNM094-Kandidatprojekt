using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worldGenerator : MonoBehaviour {

    //Arrys to hold objects that we will spawn in world:

    [SerializeField]
    private GameObject[] trees;

    [SerializeField]
    private GameObject[] rocks;

    [SerializeField]
    private GameObject[] terrain;

    private int stoneChanceAmt = 5;

    // Marker objects:
    [SerializeField]
    private GameObject BLMarker;

    [SerializeField]
    private GameObject TRMarker;

    //Spawning grid values / variables to control world size:
    private Vector3 currentPos;
    private Vector3 worldObjectStartPos;
    private Vector3 terrainStartPos;

    private float groundWidth;
    private float groundHeight;

    private float worldObjectIncAmt;
    private float worldObjectIncAmtRows;
    private float terrainIncAmt;
    private float terrainIncAmtRows;

    private float worldObjectRandAmt;
    private float worldObjectRandAmtRows;
    private float terrainRandAmt;
    private float terrainAngle;

    // Values to control spawn loop through grid

    [SerializeField] // Gör så att vi kan ändra värdet i editorn.
    private int worldObjectRowsAndCols;

    [SerializeField]
    private int terrainRowsAndCols;
    
    /* Test
    [SerializeField] 
    private int worldObjectRows;
    [SerializeField]
    private int worldObjectCols;

    [SerializeField]
    private int terrainRows;
    [SerializeField]
    private int terrainCols;
    */

    [SerializeField]
    private int reapetPasses;

    private int currentPass;

    [SerializeField]
    private float worldObjectSphereRad;

    [SerializeField]
    private float terrainSphereRad;


    // LayerMasks
    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private LayerMask terrainLayer;

    [SerializeField]
    private LayerMask worldObjectLayer;

    // Use this for initialization
    void Start ()
    {
        StartCoroutine("SpawnWorld");
	}

    IEnumerator SpawnWorld()
    {
        //Bestämmer storleken på marken 
        groundWidth = TRMarker.transform.position.x - BLMarker.transform.position.x;
        //groundHeight = TRMarker.transform.position.z - BLMarker.transform.position.z;

        //Delar storlken av mappen med antalet kolumner och rader som användaren själv får välja
        worldObjectIncAmt = groundWidth / worldObjectRowsAndCols;
        // Bestämmer hur långt från vår punkt vi ska randoma nästa objeket 
        worldObjectRandAmt = worldObjectIncAmt / 2f;

        //test
        //worldObjectIncAmt = groundWidth / worldObjectCols;

        /* test 
        worldObjectIncAmtRows = groundHeight / worldObjectRows;
        worldObjectRandAmtRows = worldObjectIncAmtRows / 2f;
        */
        


        //Samma som ovanstående fast för terängen
        terrainIncAmt = groundWidth / terrainRowsAndCols;
        terrainRandAmt = terrainIncAmt / 2f;

        /*test
        terrainIncAmt = groundWidth / terrainCols;
        terrainIncAmtRows = groundHeight / terrainRows;
        */



        //Startpositionen för objeketen gör så att vi hamnar i mitten av varje ruta snarare än på kanten av den
        worldObjectStartPos = new Vector3(BLMarker.transform.position.x - (worldObjectIncAmt / 2f), BLMarker.transform.position.y, BLMarker.transform.position.z + (worldObjectIncAmt / 2f));

        //Samma som ovanstående
        terrainStartPos = new Vector3(BLMarker.transform.position.x - (terrainIncAmt / 2f), BLMarker.transform.position.y, BLMarker.transform.position.z + (terrainIncAmt / 2f));


        // Antalet gånger vi ska gå igenom banan
        for(int rp = 0; rp <= reapetPasses; rp++)
        {
            currentPass = rp;
            //Första gången vi går igenom banan skapas endast teräng
            if (currentPass == 0)
            {
                currentPos = terrainStartPos;
                // Går igenom varje kolumn och rad av banan
                for(int cols = 1; cols <= terrainRowsAndCols /*terrainRows*/; cols++)
                {
                    for(int rows = 1; rows <= terrainRowsAndCols /*terrainCols*/; rows++)
                    {
                        //Bestämmer nuvarnade postitionen
                        currentPos = new Vector3(currentPos.x + terrainIncAmt, currentPos.y, currentPos.z);
                        // Bestämmer vilken typ av objekt som ska skapas.
                        GameObject newSpawn = terrain[Random.Range(0, terrain.Length)];
                        //Skickar iväg oss till funktionen där objeketets spawn plats skapas.
                        spawnHere(currentPos, newSpawn, terrainSphereRad, true);
                        // returnerar det som hänt och väntar innan den 0.01s innan den körs nästa gång
                        yield return new WaitForSeconds(0.01f);
                        
                    }
                    //När en rad gåtts igenom hoppar currentPos upp en rad med; currentPos.z + terrainIncAmt
                    currentPos = new Vector3(terrainStartPos.x, currentPos.y, currentPos.z + terrainIncAmt);
                }
            }
            // Körs när terängen är skapad.
            else if(currentPass > 0)
            {
                currentPos = worldObjectStartPos;
                //Kör igenom alla rader och kolumner av banan.
                for(int cols = 1; cols <= worldObjectRowsAndCols /*worldObjectRows*/; cols++)
                {
                     for (int rows = 1; rows <= worldObjectRowsAndCols /*worldObjectCols*/; rows++)
                     {
                        //Sätter den nuvarande postionen till en framåt med hjälp av worldObjectIncAmt
                        currentPos = new Vector3(currentPos.x + worldObjectIncAmt, currentPos.y, currentPos.z);

                        int spawnChance = Random.Range(1, stoneChanceAmt + 1);
                        //Bestämmer om det är en sten eller träd som ska skapas.
                        if (spawnChance == 1)
                        {
                            GameObject newSpawn = rocks[Random.Range(0, rocks.Length)];

                            spawnHere(currentPos, newSpawn, worldObjectSphereRad, false);

                            yield return new WaitForSeconds(0.01f);
                        }
                        else
                        {
                            GameObject newSpawn = trees[Random.Range(0, trees.Length)];

                            spawnHere(currentPos, newSpawn, worldObjectSphereRad, false);

                            yield return new WaitForSeconds(0.01f);

                        }
                    }
                    //När en rad gåtts igenom hoppar currentPos upp en rad med; currentPos.z + worldObjectIncAmt
                    currentPos = new Vector3(worldObjectStartPos.x, currentPos.y, currentPos.z + worldObjectIncAmt);
                    //test 
                    //currentPos = new Vector3(worldObjectStartPos.x, currentPos.y, currentPos.z + worldObjectIncAmtRows);
                }
            }

        }
        // Vad som ska göras när världen har skapats.
        worldGenDone();

    }
    //funktionen som bestämmer vart ett objekt ska skapas. Det som skickas är(nuvarande positionen, objeketet som ska skapas, radien runt objeketet där man inte vill ha ett annat objekt. Samt om objeket är terräng eller inte
    void spawnHere(Vector3 newSpawnPos, GameObject objectToSpawn, float radiusOfSphere, bool isObjectTerrain)
    {
        // Om det är ett terräng objeket eller inte.
        if (isObjectTerrain == true)
        {
            //Random fram en position utifrån terrainRandAmt runt positionen.
            Vector3 randPos = new Vector3(newSpawnPos.x + Random.Range(-terrainRandAmt, terrainRandAmt + 1), newSpawnPos.y, newSpawnPos.z + Random.Range(-terrainRandAmt, terrainRandAmt + 1));
            //Skapar en raycast som startar 10 enheter över den nuvarande positionen
            Vector3 rayPos = new Vector3(randPos.x, 100, randPos.z);

            RaycastHit hit;

            //Körs om en raycast som tittar ner mot marken träffar marken eller inte. Körs om den träffar marken
            if (Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                // Array för alla worldObjekt som blivit träffade inom svären OverlapSphere med en radie som bestäms av av användaren.
                Collider[] objectsHit = Physics.OverlapSphere(randPos, radiusOfSphere, terrainLayer);

                randPos = new Vector3(randPos.x, hit.point.y, randPos.z);

                // Om inga objekt har träffats.
                if (objectsHit.Length == 0)
                {
                    
                    //Skapar en kopia av objeket som ska skapas på den randomade positionen utan rotation.
                    GameObject terrainObject = (GameObject)Instantiate(objectToSpawn, randPos, Quaternion.identity);

                    //OM det är ett kull objekt.
                    if (objectToSpawn == terrain[1])
                    {
                        terrainObject.transform.position = new Vector3(terrainObject.transform.position.x, terrainObject.transform.position.y + (terrainObject.GetComponent<Renderer>().bounds.extents.y * -0.4f), terrainObject.transform.position.z);
                        
                    }
                    if (Vector3.Angle(hit.normal, Vector3.up) > 10 && Vector3.Angle(hit.normal, Vector3.up) < 20)
                    {
                        terrainObject.transform.position = new Vector3(terrainObject.transform.position.x, terrainObject.transform.position.y + (terrainObject.GetComponent<Renderer>().bounds.extents.y * 0.05f), terrainObject.transform.position.z);


                    }
                    else
                    {
                        //Ser till att terängen sjunker ner i marken efter terrainObject.GetComponent<Renderer>().bounds.extents.y* 0.7f
                        terrainObject.transform.position = new Vector3(terrainObject.transform.position.x, terrainObject.transform.position.y + (terrainObject.GetComponent<Renderer>().bounds.extents.y * 0.7f), terrainObject.transform.position.z);
                        
                        //Randomade rotation kring y-axeln av objeket.
                        terrainObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0, 360), transform.eulerAngles.z);
                    }

                    //Randomade rotation kring y-axeln av objeket.
                    terrainObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0, 360), transform.eulerAngles.z);

                }
            }
        }
        else
        {
            // Random position som gör att den spawnar någonstans inom rutan alltså [-worldObjectRandAmt,worldObjectRandAmt] 
            Vector3 randPos = new Vector3(newSpawnPos.x + Random.Range(-worldObjectRandAmt, worldObjectRandAmt + 1), newSpawnPos.y, newSpawnPos.z + Random.Range(-worldObjectRandAmt, worldObjectRandAmt + 1));
            //Skapar raycast positionen högre upp så att vi inte hamnar mitt i ett teräng objekt.
            Vector3 rayPos = new Vector3(randPos.x, 110, randPos.z);

            // Om ett objekt träffas
            RaycastHit hit;
            //Om raycasten som tittar neråt träffar och träffar marken, går man vidare. Man sparar även information om den kolliderar med ett teräng objekt.
            if(Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                //Skapar en ny positionen ovanpå terrängen med hit.point.y 
                randPos = new Vector3(randPos.x, hit.point.y, randPos.z);

                //Tittar om det finns några objekt med radien radiusOfSphere runt objektet.
                Collider[] objectsHit = Physics.OverlapSphere(randPos, radiusOfSphere, worldObjectLayer); 

                // Om inget objekt träffas.
                if(objectsHit.Length == 0)
                {
                    //skapar en kopia av objeketet på den randomade positionen utan rotation.
                    GameObject worldObject = (GameObject)Instantiate(objectToSpawn, randPos, Quaternion.identity);
                    
                    // Sänker ner objektet lite i marken om ytan den spawnar på skulle vara lutande
                    worldObject.transform.position = new Vector3(worldObject.transform.position.x, worldObject.transform.position.y + (worldObject.GetComponent<Renderer>().bounds.extents.y * 0.7f), worldObject.transform.position.z);
                    // Randomar fram en rotation kring y för objeket.
                    worldObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0, 360), transform.eulerAngles.z);

                }

            }
        }
    }
    void worldGenDone()
    {
        print("World has been created!!!");
    }
	
}
