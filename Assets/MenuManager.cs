using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    public void GoToScene(string sceneName){
        SceneManager.LoadScene(sceneName);
    }
    public void quitGame(){
        Application.Quit();
    }
}
