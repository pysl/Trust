using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    public string settings;
    public string url = "";
    private LevelController lc;
    public int playerid;
    public byte[] results;
    string otherPlayers;
    // Start is called before the first frame update
    void Start()
    {
        
        //get ip and port from PlayerPrefs
        url = "http://" + PlayerPrefs.GetString("ip") + ":" + PlayerPrefs.GetInt("port");
        if (PlayerPrefs.GetString("ip") == "") url = "http://localhost:3000";

        StartCoroutine(pingServer(url));
        
        


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

    private IEnumerator pingServer(string url) {
        UnityWebRequest request = UnityWebRequest.Get(url + "/ping");
        // set timeout to 5 seconds
        request.timeout = 5;
        yield return request.SendWebRequest();
        

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {

            //redirect to main menu
            SceneManager.LoadScene(2);
        }
    }

    public IEnumerator DEBUG_setAP(int newAP) {
        
        UnityWebRequest request = UnityWebRequest.Post(url + "/debug_setAP", new Dictionary<string, string> { { "playerid", playerid.ToString() }, { "AP", newAP.ToString() } });
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
            //redirect to main menu
            SceneManager.LoadScene(2);
        }


    }

    public IEnumerator DEBUG_setHealth(int newHealth) {
        

        UnityWebRequest request = UnityWebRequest.Post(url + "/debug_setHealth", new Dictionary<string, string> { { "playerid", playerid.ToString() }, { "health", newHealth.ToString() } });
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
            //redirect to main menu
            SceneManager.LoadScene(2);
        }
    }



    public IEnumerator sendAttack(int targetID, int targetHealth, int playerAP) {
        

        UnityWebRequest request = UnityWebRequest.Post(url + "/attack", new Dictionary<string, string> { { "playerid", playerid.ToString() }, { "targetid", targetID.ToString() }, { "targetHealth", targetHealth.ToString() }, { "AP", playerAP.ToString() } });
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
            //redirect to main menu
            SceneManager.LoadScene(2);
        }
    }

    public IEnumerator giveAP(int targetID, int playerAP) {

        UnityWebRequest request = UnityWebRequest.Post(url + "/giveap", new Dictionary<string, string> { { "playerid", playerid.ToString() }, { "targetid", targetID.ToString() }, { "AP", playerAP.ToString() } });
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
            //redirect to main menu
            SceneManager.LoadScene(2);
        }
    }

    public IEnumerator joinMatch(int playerid) { 
        // post playerid to /join
        

        UnityWebRequest request = UnityWebRequest.Post(url + "/join", new Dictionary<string, string> { { "playerid", playerid.ToString() } });
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
            SceneManager.LoadScene(2);
        }


        string output = request.downloadHandler.text;
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
    //public string getMap() => getFromServer("/map");

    public void updateMap() {
        //fetch map from server asynchronously and update map when download is complete
        
        string o = getFromServer("/map");
        lc.updateMap(lc.serializeOtherPlayers(o));
        
    }


    public string joinNewMatch() => getFromServer("/newjoin");

    

    public IEnumerator move(int x, int y) {
        UnityWebRequest request = UnityWebRequest.Post(url + "/move", new Dictionary<string, string> { { "playerid", playerid.ToString() }, { "x", x.ToString() }, { "y", y.ToString() }, { "AP", lc.player.playerAP.ToString() } });
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
            //redirect to main menu
            SceneManager.LoadScene(2);
        }
    }

    //makes a post request to server
    private string postToServer(string name, string data, string address) {
        string oString = "";
            foreach (byte b in results) {
                oString += (char)b;
            }
        return oString;
    }

    private IEnumerator postToServerIEnumerator(string name, string data, string address) {
        UnityWebRequest request = UnityWebRequest.Post(url + address, new Dictionary<string, string> { { name, data } });
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
            //redirect to main menu
            SceneManager.LoadScene(2);
        } else {
            // Show results as text
            Debug.Log(request.downloadHandler.text);
            // Or retrieve results as binary data
            byte[] results = request.downloadHandler.data;
        }
    }
    //makes a get request to server
    private string getFromServer(string address) {
        StartCoroutine(getFromServerIEnumerator(address));
        string oString = "";

        //loop through byte array and convert to string
        foreach (byte b in results) {
            oString += (char)b;
        }
        return oString;
    }
    private IEnumerator getFromServerIEnumerator(string address) {
        UnityWebRequest request = UnityWebRequest.Get(url + address);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
            Debug.LogError(request.error);
        } else {
            // Show results as text
            Debug.Log(request.downloadHandler.text);
            // Or retrieve results as binary data
            results = request.downloadHandler.data;
            
        }
    }

}
