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
        StartCoroutine(nm.joinNewMatch());
    }

    public void updatePlayerHealth() {
        lc.playerHealth = (int)healthSlider.value;
        nm.DEBUG_setHealth(lc.playerHealth);
    }
    public void addPlayerAP() {
        lc.playerAP += 1;
        StartCoroutine(nm.DEBUG_setAP(lc.playerAP));
        
    }
    public void removePlayerAP() {
        lc.playerAP -= 1;
        StartCoroutine(nm.DEBUG_setAP(lc.playerAP));
    }
}
