using UnityEngine;

public class Goal : MonoBehaviour
{
    public GameObject gameClearUI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameClearUI.SetActive(true);

            other.GetComponent<Player>().enabled = false;

            Debug.Log("GAME CLEAR!");
        }
    }
}