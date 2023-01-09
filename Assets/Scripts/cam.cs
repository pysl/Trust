using UnityEngine;

public class cam : MonoBehaviour
{
    public Spot target;
    [SerializeField] private float smoothSpeed = 0.125f;

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = Vector3.Lerp(transform.position, target.transform.position, smoothSpeed);
        transform.position = new Vector3(targetPos.x, targetPos.y, -10);
    }
}
