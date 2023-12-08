using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    int recentScene;
    void Update(){
        getMostRecentScene();
    }
    public void GoToScene(string sceneName){
        SceneManager.LoadScene(sceneName);
    }
    public void restartLevel(){
        if(recentScene != null){
            SceneManager.LoadScene(recentScene);
        }
        Debug.Log("No Scene Found");
    }
    public void getMostRecentScene(){
        recentScene = BasicMovement.mostRecentScene;
    }
    public void quitGame(){
        Application.Quit();
    }
}
