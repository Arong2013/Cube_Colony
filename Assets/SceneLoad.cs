using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoad : MonoBehaviour
{
    public Slider progressbar;
    public Text loadtext;
    private void Start()
    {
        StartCoroutine(LoadScene());
    }
    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation operation = SceneManager.LoadSceneAsync("Play");
        operation.allowSceneActivation = false;

        while(!operation.isDone)
        {
            yield return null;
            if(progressbar.value<0.9f)
            {
                progressbar.value=Mathf.MoveTowards(progressbar.value,0.9f,Time.deltaTime);
            }

            if (operation.progress >= 0.9f)
            {
                progressbar.value = Mathf.MoveTowards(progressbar.value, 1f, Time.deltaTime);
            }
            else if(operation.progress >= 0.9f)
            {
                progressbar.value = Mathf.MoveTowards(progressbar.value, 1f, Time.deltaTime);
            }
            if(progressbar.value>1f)
            {
                loadtext.text = "Press SpaceBar";
            }
            if(Input.GetKeyDown(KeyCode.Space)&&progressbar.value>=1f&&operation.progress>=0.9f)
            {
                operation.allowSceneActivation = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
