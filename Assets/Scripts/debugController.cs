using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class debugController : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    public void addDebugPlayer() { //used to add fake players for testing because i dont have friends 
        NetworkManager nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
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
        LevelController lc = GameObject.Find("LevelController").GetComponent<LevelController>();
        lc.playerHealth = (int)healthSlider.value;
    }
    public void addPlayerAP() {
        LevelController lc = GameObject.Find("LevelController").GetComponent<LevelController>();
        lc.playerAP += 1;
    }
    public void removePlayerAP() {
        LevelController lc = GameObject.Find("LevelController").GetComponent<LevelController>();
        lc.playerAP -= 1;
    }
}
