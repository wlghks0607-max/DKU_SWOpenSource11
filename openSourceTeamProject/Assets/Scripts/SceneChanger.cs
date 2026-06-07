using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void gameStart(){
        SceneManager.LoadScene("Level Selector");
    }

    public void level_1(){
        SceneManager.LoadScene("TestScene");
    }
    
}
