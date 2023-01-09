using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private Text notificationText;
    
    // Start is called before the first frame update
    void Start()
    {
        notificationText.text = "";
    }

    public void say(string notification, float delay=2f)
    {
        StartCoroutine(ShowNotificationCoroutine(notification, delay));
    }

    private IEnumerator ShowNotificationCoroutine(string notification, float delay)
    {
        notificationText.text = notification;
        yield return new WaitForSeconds(delay);
        notificationText.text = "";
    }
}
