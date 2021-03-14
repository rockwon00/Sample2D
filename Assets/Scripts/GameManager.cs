using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;

    public void NextStage()
    {
        stageIndex += stageIndex;
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
            health--;
        else
        {
            //Player Die Effect
            player.OnDie();
            //Result UI
            Debug.Log("죽었습니다!");
            //Retry Button UI
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            HealthDown();

        //Player Reposition
        collision.attachedRigidbody.velocity = Vector2.zero;
        collision.transform.position = new Vector2(0, 0);

    }
}
