using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{ //i want to fucking kill myself
    [SerializeField] GameObject spot;
    [SerializeField] GameObject playerGO;

    private NetworkManager nm;
    public player player;


    public int[] mapSize;
    public float moveRange;
    public bool isLoading = true;
    public int playerHealth;
    public PauseMenu pauseMenu;
    public int playerAP;
    public RawImage[] hearts;
    public Text APText;
    public Text playeridText;
    public bool gameLoopRunning = false;
    public float rps = 10.0f; //requests per second


    /*
        <summary>
            Initializes the level and player
        </summary>
    */
    public void startGame(string s, string p, string otherPlayers)
    {   
        nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>(); // get NetworkManager

        //deserialize settings
        Settings settings = JsonConvert.DeserializeObject<Settings>(s);
        playerStats playerStats = JsonConvert.DeserializeObject<playerStats>(p); // deserialize playerStats

        mapSize = settings.mapSize;
        moveRange = (float) settings.range;


        int[] playerSpawn = {playerStats.x, playerStats.y};
        int playerid = playerStats.playerid;
        Color playerColor = new Color(playerStats.colorR, playerStats.colorG, playerStats.colorB, 1);
        int playerHealth = playerStats.health;
        int playerAP = playerStats.AP;
        PlayerPrefs.SetInt("playerid", playerid);
        rps = settings.rps;


        GenerateMap(mapSize[0], mapSize[1]);
        playerGO = Instantiate(playerGO, new Vector2(0, 0), Quaternion.identity); // spawn player object at 0, 0
        player = playerGO.GetComponent<player>();
        playeridText.text = "PlayerID: " + playerid.ToString();
        nm.playerid = playerid;
        player.spawn(playerSpawn[0], playerSpawn[1], playerid, playerColor, playerHealth, playerAP); // spawn player at playerSpawn with playerid, playerColor, and playerHealth
        
        //enter into game loop
        StartCoroutine(gameLoop());
        

    }

    //updates the players position
    public void updateMap(playerStats[] otherPlayersStats) {
        Spot[] spots = new Spot[mapSize[0] * mapSize[1]];
        for (int x = 0; x < mapSize[0]; x++)
        {
            for (int y = 0; y < mapSize[1]; y++)
            {
                Spot spot = findSpot(x, y); //findSpot returns the spot at x and y
                spots[x * mapSize[1] + y] = spot;

            }
        }

        //remove all spots that appear in otherPlayersStats 
        foreach (playerStats op in otherPlayersStats)
        {
            int[] otherPlayerSpawn = {op.x, op.y};
            Spot spot = findSpot(otherPlayerSpawn[0], otherPlayerSpawn[1]);
            spots[otherPlayerSpawn[0] * mapSize[1] + otherPlayerSpawn[1]] = null;
        }


        //loop through all spots remaining in spots and set them to default
        foreach (Spot s in spots)
        {
            if (s != null)
            {
                s.playerid = -1;
                s.health = 0;
                spot.transform.localScale = new Vector2(1.0f, 1.0f);
                s.reloadColor();
            }
        }
            
           
        //find player stats in otherPlayersStats and update player
        foreach (playerStats op in otherPlayersStats)
        {
            if (op.playerid == player.playerid)
            {
                player.updatePlayer(op.health, op.AP);
            } else {
                Spot opSpot = findSpot(op.x, op.y);
                opSpot.health = op.health;
            }
        }
    }
    //game loop
    IEnumerator gameLoop()
    {
        
        while (true)
        {
            if (!gameLoopRunning)
            {
                gameLoopRunning = true;
                nm.updateMap();
                yield return new WaitForSeconds(1.0f / rps);
                gameLoopRunning = false;
            }
        }
    }
    //returns serialized version of otherPlayers array
    public playerStats[] serializeOtherPlayers(string otherPlayers) {
        playerStats[] otherPlayersStats = JsonConvert.DeserializeObject<playerStats[]>(otherPlayers);
        foreach (playerStats op in otherPlayersStats)
        {
            int[] otherPlayerSpawn = {op.x, op.y};
            int otherPlayerid = op.playerid;
            Color otherPlayerColor = new Color(op.colorR, op.colorG, op.colorB, 1);
            int otherPlayerHealth = op.health;
            int otherPlayerAP = op.AP;
            Spot spot = findSpot(otherPlayerSpawn[0], otherPlayerSpawn[1]);
            spot.playerid = otherPlayerid;
            spot.color = otherPlayerColor;
            spot.health = otherPlayerHealth;
            spot.reloadColor();
        }
        return otherPlayersStats;
    }

    /*
        <summary>
            Finds spot at x and y and returns it. If it doesn't exist, returns null.
        </summary>
    */
    public Spot findSpot(int x, int y)
    {
        GameObject[] spots = GameObject.FindGameObjectsWithTag("spot");
        foreach (GameObject spot in spots)
        {
            if (spot.GetComponent<Spot>().x == x && spot.GetComponent<Spot>().y == y)
            {
                return spot.GetComponent<Spot>();
            }
        }
        return null;
    }



    /*
        <summary>
            Finds spot with playerid and returns it. If it doesn't exist, returns null.
        </summary>
    */
    public Spot findPlayer(int id)
    {
        GameObject[] spots = GameObject.FindGameObjectsWithTag("spot");
        foreach (GameObject spot in spots)
        {
            if (spot.GetComponent<Spot>().playerid == id)
            {
                return spot.GetComponent<Spot>();
            }
        }
        return null;
    }

    /*
        <summary>
            Generates map of size x by y. Sets isLoading to true while generating map.
        </summary>
    */
    void GenerateMap(int x, int y)
    {
        isLoading = true;
        float separation = 1.5f;
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject newSpot = Instantiate(spot, new Vector2(i * separation, j * separation), Quaternion.identity);
                newSpot.GetComponent<Spot>().x = i;
                newSpot.GetComponent<Spot>().y = j;
                newSpot.tag = "spot";
                newSpot.GetComponent<Spot>().spawned();
            }
        }
        isLoading = false;
    }
}

public class Settings
{
    public int[] mapSize;
    public int range;
    public int rps;
}

public class playerStats
{
    public int x;
    public int y;
    public int playerid; 
    public float[] color;
    public int health;
    public int AP;
    public float colorR;
    public float colorG;
    public float colorB;
    
}


