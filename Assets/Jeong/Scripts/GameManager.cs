using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameStart(int id)
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.jump);
    }

    public void GameOver()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.jump);
    }

    public void GameVictroy()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.jump);
    }
}
