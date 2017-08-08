using UnityEngine;
using System.Collections;

public class SideWalls : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Collider2D>().CompareTag("Ball"))
        {
            GameManager.Score(transform.name);
            GameManager.RestartRoundStatic();
        }
    }
}