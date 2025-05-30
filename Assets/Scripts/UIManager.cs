using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(1);
    }


    public void NextLeft()
    {
        SceneManager.LoadScene(0);
    }    

    public void Level1()
    {
        SceneManager.LoadScene(2);
    }    

    public void Level2()
    {
        SceneManager.LoadScene(3);
    }    

    public void Level3()
    {
        SceneManager.LoadScene(4);
    }    

    public void Home()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
