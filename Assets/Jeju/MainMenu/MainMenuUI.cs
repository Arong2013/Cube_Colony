using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void OnClickStartGame()
    {
        SceneManager.LoadScene("LoadingScene");
    }
}
