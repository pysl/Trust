using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class debugController : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    LevelController lc;
    NetworkManager nm;

    void Start()
    {
        lc = GameObject.Find("LevelController").GetComponent<LevelController>();
        nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }
    public void addDebugPlayer() { //used to add fake players for testing because i dont have friends 
        string n = nm.joinNewMatch();
        playerStats ps = JsonConvert.DeserializeObject<playerStats>(n);
        int[] otherPlayerSpawn = {ps.x, ps.y};
        int otherPlayerid = ps.playerid;
        Color otherPlayerColor = new Color(ps.color[0], ps.color[1], ps.color[2], 1);
        int otherPlayerHealth = ps.health;
        int otherPlayerAP = ps.AP;
        Spot spot = GameObject.Find("LevelController").GetComponent<LevelController>().findSpot(otherPlayerSpawn[0], otherPlayerSpawn[1]);
        spot.playerid = otherPlayerid;
        spot.color = otherPlayerColor;
        spot.health = otherPlayerHealth;
        spot.reloadColor();
    }

    public void updatePlayerHealth() {
        lc.playerHealth = (int)healthSlider.value;
        nm.DEBUG_setHealth(lc.playerHealth);
    }
    public void addPlayerAP() {
        lc.playerAP += 1;
        nm.DEBUG_setAP(lc.playerAP);
    }
    public void removePlayerAP() {
        lc.playerAP -= 1;
        nm.DEBUG_setAP(lc.playerAP);
    }
}
