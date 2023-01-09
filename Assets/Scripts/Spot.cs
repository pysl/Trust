using UnityEngine;

public class Spot : MonoBehaviour
{
    public int x;
    public int y;
    public int playerid = -1;
    public Color color = new Color(1, 1, 1, 1); // default color
    public int health;


    /*
        <summary>
            When called, the spot reloads its color. Used when playerid is changed.
        </summary>
    */
    public void reloadColor() {
        GetComponent<SpriteRenderer>().color = color;
    }

    
    void Update() {
        if (playerid != -1) {
            color = new Color(1, 1, 1, 1);
        }
        if (health >= 3) {
            transform.localScale = new Vector3(1f, 1f, 1f);
        } else if (health == 2) {
            transform.localScale = new Vector3(0.75f, 0.75f, 1);
        } else if (health == 1) {
            transform.localScale = new Vector3(0.5f, 0.5f, 1);
        } else {
            //transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }


    /*
        <summary>
            Called when spot is spawned. If playerid is not -1, then it is a player spot and it will set the color to the player's color.
        </summary>
    */
    public void spawned() {

        if (playerid != -1) {
            reloadColor();  
        }
    }

    /*
        <summary>
            When called, the spot will be highlighted.
        </summary>
    */
    
    
}
