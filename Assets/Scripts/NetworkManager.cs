using System.Collections;
using UnityEngine;
using System.Net;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public string settings;
    public string url = "";
    private LevelController lc;
    public int playerid;
    string otherPlayers;
    // Start is called before the first frame update
    void Start()
    {
        
        //get ip and port from PlayerPrefs
        url = "http://" + PlayerPrefs.GetString("ip") + ":" + PlayerPrefs.GetInt("port");
        if (PlayerPrefs.GetString("ip") == "") url = "http://localhost:3000";

        
        try {
            getFromServer("/ping");
        } catch (WebException) {
            //redirect to main menu
            SceneManager.LoadScene(2);
        }
        if (PlayerPrefs.GetInt("playerid") != -1) playerid = PlayerPrefs.GetInt("playerid");
        else playerid = -1;
        
        
    
        lc = GameObject.Find("LevelController").GetComponent<LevelController>();
        
        //download settings and map
        settings = getFromServer("/settings");
        otherPlayers = getFromServer("/map"); 

        if (playerid == -1) {
            //playerid is not set
            //redirect to joinNewMatch()
            lc.startGame(settings, joinNewMatch(), otherPlayers);
        } else {
            //playerid is set
            //redirect to joinMatch()
            joinMatch(playerid);
        }
    }
    public void sendAttack(int targetID, int targetHealth, int playerAP) {
        //post "playerid": playerid, "targetid": targetID, "targetHealth": targetHealth to /attack
        try {
            using (WebClient client = new WebClient())
            {
                byte[] o = client.UploadValues(url + "/attack", "POST", new System.Collections.Specialized.NameValueCollection() { { "playerid", playerid.ToString() }, { "targetid", targetID.ToString() }, { "targetHealth", targetHealth.ToString() }, { "AP", playerAP.ToString() } });
                //loop through byte array and convert to string
                string oString = "";
                foreach (byte b in o) {
                    oString += (char)b;
                }
                
            }
        } catch (WebException) {
            //redirect to main menu
            SceneManager.LoadScene(2);
        }
    }

    public void giveAP(int targetID, int playerAP) {
        //post "playerid": playerid, "targetid": targetID to /giveap
        try {
            using (WebClient client = new WebClient())
            {
                byte[] o = client.UploadValues(url + "/giveap", "POST", new System.Collections.Specialized.NameValueCollection() { { "playerid", playerid.ToString() }, { "targetid", targetID.ToString() }, { "AP", playerAP.ToString() } });
                //loop through byte array and convert to string
                string oString = "";
                foreach (byte b in o) {
                    oString += (char)b;
                }
                //StartCoroutine(updateMap());
            }
        } catch (WebException)
        {
            //redirect to main menu
            SceneManager.LoadScene(2);
        }
    }

    public void joinMatch(int playerid) {
        //post playerid to /join
        //post "playerid": playerid to /join
        try {
            using (WebClient client = new WebClient())
            {
                string output = postToServer("playerid", playerid.ToString(), "/join");
                //remove first and last characters
                output = output.Substring(1, output.Length - 2);
                if (output == "false") {
                    //the player does not exist
                    //redirect to joinNewMatch()
                    lc.startGame(settings, joinNewMatch(), otherPlayers);
                } else {
                    lc.startGame(settings, output, otherPlayers);
                }
            }
        } catch (WebException) {
            //redirect to main menu
            SceneManager.LoadScene(2);
        }
    }
    //public string getMap() => getFromServer("/map");

    public void updateMap() {
        //fetch map from server asynchronously and update map when download is complete
        using (WebClient client = new WebClient()) {
            string o = getFromServer("/map");
            Debug.Log("downloaded map");
            lc.updateMap(lc.serializeOtherPlayers(o));
        }
    }


    public string joinNewMatch() => getFromServer("/newjoin");

    public void move(int x, int y) {
        //post "playerid": playerid, "x": x, "y": y to /move
        try { 
            using (WebClient client = new WebClient())
            {
                byte[] o = client.UploadValues(url + "/move", "POST", new System.Collections.Specialized.NameValueCollection() { { "playerid", playerid.ToString() }, { "x", x.ToString() }, { "y", y.ToString() },  {"AP", lc.player.playerAP.ToString()}});
                //loop through byte array and convert to string
                string oString = "";
                foreach (byte b in o) {
                    oString += (char)b;
                }
            }
        } catch (WebException) {
            //redirect to main menu
            SceneManager.LoadScene(2);
        }
    }
    //makes a post request to server
    private string postToServer(string name, string data, string address) {
        try {
            using (WebClient client = new WebClient()) {
                byte[] o = client.UploadValues(url + address, "POST", new System.Collections.Specialized.NameValueCollection() { { name, data } });
                //loop through byte array and convert to string
                string oString = "";
                foreach (byte b in o) {
                    oString += (char)b;
                }
                return oString;
            }
        } catch (WebException) {
            //redirect to main menu
            SceneManager.LoadScene(2);
            return "";
        }
    }
    //makes a get request to server
    private string getFromServer(string address) {
        try {
            using (WebClient client = new WebClient()) {
                return client.DownloadString(url + address);
            }
        } catch (WebException) {
            //redirect to main menu
            SceneManager.LoadScene(2);
            return "";
        }
    }

}
