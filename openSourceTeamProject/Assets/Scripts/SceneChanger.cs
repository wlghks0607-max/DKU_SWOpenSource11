using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void gameStart(){
        SceneManager.LoadScene("Level Selector");
    }

    public void level_1(){
        SceneManager.LoadScene("1_stage");
    }

    public void level_2(){
        SceneManager.LoadScene("2_stage");
    }

    public void level_3(){
        SceneManager.LoadScene("3_stage");
    }

    public void go_main()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void level_select()
    {
        SceneManager.LoadScene("Level Selector");
    }
    
}
