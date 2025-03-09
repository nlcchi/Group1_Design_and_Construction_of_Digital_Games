using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartManager : MonoBehaviour
{
    public void StartGame()
    {
        // 加载游戏场景，确保已将游戏场景添加到Build Settings
        SceneManager.LoadScene("StoryScene");
    }

    public void QuitGame()
    {
        // 在编辑器中输出日志
        Debug.Log("退出游戏");

        // 在打包后的游戏中退出
        Application.Quit();
    }

}
