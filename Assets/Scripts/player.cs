using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class player : MonoBehaviour
{
    public int health;
    public int playerid;
    public int playerAP;
    public Color color;
    public int x;
    public int y;
    
    PauseMenu pauseMenu;
    RawImage[] hearts;
    Text APText;
    LevelController levelController; // cached reference to level controller
    NotificationManager notificationManager; // cached reference to notification manager
    NetworkManager networkManager;
    cam cam; // cached reference to main camera script
    Spot spot; // cached reference to spot
    Spot selectedSpot; //cached reference to selected spot
    AudioManager audioManager;


    /*

        <summary>
            Returns distance between two points
        </summary>
    */
    private float distaceFrom(int x1, int y1, int x2, int y2) { return Mathf.Sqrt(Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2)); }
    

    /*
        <summary>
            Updates the player based on the health. If health is 0, then the player is removed.
        </summary>
    */
    public void updatePlayer(int health, int playerAP) {
        if (health != this.health) { //heath updates
            if (health == 0) {
                playerid = -1;
                color = new Color(1, 1, 1, 1);
                spot.transform.localScale = new Vector3(1, 1, 1);
                spot.reloadColor();
                notificationManager.say("Player at (" + x + ", " + y + ") died.");
            } else {
                if (health > this.health) { //this should never happen unless debug is on
                    notificationManager.say("Player at (" + x + ", " + y + ") healed " + (health - this.health) + " health.");
                    this.health = health;
                } else {
                    notificationManager.say("Player at (" + x + ", " + y + ") took " + (this.health - health) + " damage.");
                    audioManager.playSound("hurt");
                    this.health = health;
                }
            }
            if (this.health >= 3) {
                hearts[0].color = new Color(1, 1, 1, 1);
                hearts[1].color = new Color(1, 1, 1, 1);
                hearts[2].color = new Color(1, 1, 1, 1);
            } else if (this.health == 2) {
                hearts[0].color = new Color(1, 1, 1, 1);
                hearts[1].color = new Color(1, 1, 1, 1);
                hearts[2].color = new Color(0, 0, 0, 1);
            } else if (this.health == 1) {
                hearts[0].color = new Color(1, 1, 1, 1);
                hearts[1].color = new Color(0, 0, 0, 1);
                hearts[2].color = new Color(0, 0, 0, 1);
            } else {
                hearts[0].color = new Color(0, 0, 0, 1);
                hearts[1].color = new Color(0, 0, 0, 1);
                hearts[2].color = new Color(0, 0, 0, 1);
                PlayerPrefs.SetInt("playerid", -1);
                SceneManager.LoadScene(3);
            }
        }
        if (playerAP != this.playerAP) { //AP updates
            if (playerAP > this.playerAP) { //this should never happen unless debug is on
                //notificationManager.say("You recieved " + (playerAP - this.playerAP) + " action points.");
                this.playerAP = playerAP;
            } else {
                //notificationManager.say("You lost " + (this.playerAP - playerAP) + " action points.");
                this.playerAP = playerAP;
            }
            APText.text = this.playerAP.ToString();
        }
    }




    void attack(int x, int y) {
        Spot target = levelController.findSpot(x, y);
        int tID = target.playerid;
        target.health--;
        int tHealth = target.health;
        target.playerid = -1;
        networkManager.sendAttack(tID, tHealth, playerAP);
        audioManager.playSound("attack");
        target.reloadColor();
    }



    /*
        <summary>
            Spawns player at x and y. Also moves camera to that spot. Initializes playerid, color, health, and spot.
        </summary>
    */
    public void spawn(int x, int y, int playerid, Color color, int health, int playerAP) {
        
        levelController = GameObject.Find("LevelController").GetComponent<LevelController>();
        notificationManager = GameObject.Find("NotificationManager").GetComponent<NotificationManager>();
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        pauseMenu = levelController.pauseMenu;
        cam = Camera.main.GetComponent<cam>();
        hearts = levelController.hearts;
        APText = levelController.APText;


        this.playerid = playerid;
        this.color = color;
        this.health = health;
        this.x = x;
        this.y = y;
        this.playerAP = playerAP;

        spot = levelController.findSpot(x, y);
        spot.playerid = playerid;
        spot.color = color;
        spot.reloadColor();

        selectedSpot = spot;
        select(x, y);
        notificationManager.say("Player spawned at (" + x + ", " + y + ") with playerid " + playerid + " and health " + health + ".");
    }

    /*
        <summary>
            Selects spot specified using x and y and moves the camera to it
        </summary>
    */
    void select(int x, int y) { 
        Spot newSpot = levelController.findSpot(x, y);
        if (newSpot != null) {
            selectedSpot.reloadColor();
            selectedSpot = newSpot;
            Vector2 RealWorldpos = new Vector2(selectedSpot.transform.position.x, selectedSpot.transform.position.y);

            cam.target = selectedSpot;
            
        } else {
            notificationManager.say("Spot (" + x + ", " + y + ") does not exist.");
        }
    }

    /*
        <summary>
            Moves player to spot specified using x and y
        </summary>
    */
    void move(int x, int y) {
        if(playerAP > 0) {
            if (selectedSpot != spot && distaceFrom(x, y, spot.x, spot.y) <= levelController.moveRange) {
                playerAP--;
            if (selectedSpot.playerid != -1 && selectedSpot.health > 0) {
                notificationManager.say("You atatcked (" + x + ", " + y + ").");
                attack(x, y);
            } else {
                networkManager.move(x, y);
                //uninhabitting old spot
                spot.playerid = -1;
                spot.color = new Color(1, 1, 1, 1);
                spot.transform.localScale = new Vector2(1.0f, 1.0f);
                spot.reloadColor(); 

                //inhabitting new spot
                spot = selectedSpot;
                spot.playerid = playerid;
                spot.color = color;
                spot.reloadColor();
                this.x = x;
                this.y = y;
                audioManager.playSound("move");
                select(x, y);
            }
            } else
                notificationManager.say("You cannot inhabit spot (" + x + ", " + y + ").");
        } else
            notificationManager.say("You do not have enough action points");
    }
    
    void giveAP(int x, int y) {
        if (playerAP > 0) {
            Spot target = levelController.findSpot(x, y);
            if (target.playerid != -1 && target.playerid != playerid) {
                playerAP--;
                networkManager.giveAP(target.playerid, playerAP);
                notificationManager.say("You gave action points to player " + target.playerid + ".");
            } else
                notificationManager.say("You cannot give action points to spot (" + x + ", " + y + ").");
        } else
            notificationManager.say("You do not have enough action points to give action points.");
    }
    // Update is called once per frame
    void Update()
    {
        APText.text = playerAP.ToString(); 
        //send to main menu if died
        if(spot.playerid == -1) SceneManager.LoadScene(3);

        //pause menu
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!pauseMenu.isPaused) { 
                pauseMenu.pause();
            }  
            else {
                if(pauseMenu.settingsMenu.gameObject.activeSelf)
                    pauseMenu.settingsMenu.gameObject.SetActive(false);
                else
                    pauseMenu.resume();
            }
        }
        
        //if press f11, toggle fullscreen
        if (Input.GetKeyDown(KeyCode.F11)) {
            Screen.fullScreen = !Screen.fullScreen;
        }

        
        if(!pauseMenu.isPaused) {
            //selecting code
            if (Input.GetKeyDown(KeyCode.W)) 
                select(selectedSpot.x, selectedSpot.y + 1);
            if (Input.GetKeyDown(KeyCode.A))
                select(selectedSpot.x - 1, selectedSpot.y);
            if (Input.GetKeyDown(KeyCode.S))
                select(selectedSpot.x, selectedSpot.y - 1);
            if (Input.GetKeyDown(KeyCode.D))
                select(selectedSpot.x + 1, selectedSpot.y);
            
            //movement code
            if (Input.GetKeyDown(KeyCode.Space))
                move(selectedSpot.x, selectedSpot.y);
            if(Input.GetKeyDown(KeyCode.Return))
                giveAP(selectedSpot.x, selectedSpot.y);
        }
        
    }
}
