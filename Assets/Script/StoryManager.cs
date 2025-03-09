using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    public Text storyText; // 用于显示故事的UI Text
    public Button nextButton; // Next按钮
    public string[] storyLines; // 故事内容的每一行
    public float textDelay = 1f; // 每行文本显示的时间间隔

    private int currentLine = 0; // 当前显示的行数

    void Start()
    {
        nextButton.gameObject.SetActive(false); // 初始隐藏Next按钮
        StartCoroutine(ShowStory()); // 开始逐句显示故事
    }

    IEnumerator ShowStory()
    {
        // 逐行显示故事
        while (currentLine < storyLines.Length)
        {
            storyText.text = storyLines[currentLine]; // 更新文本内容
            currentLine++;
            yield return new WaitForSeconds(textDelay); // 等待指定时间
        }

        // 故事结束后显示Next按钮
        nextButton.gameObject.SetActive(true);
    }

  public void NextButton()
    {
        // 加载Jason scene
        SceneManager.LoadScene("Jason scene");
    }

}