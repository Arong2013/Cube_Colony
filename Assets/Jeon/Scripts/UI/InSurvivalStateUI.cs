using UnityEngine;
public class InSurvivalStateUI: MonoBehaviour
{
    [SerializeField] PlayerIonsAndBar bar;
    private void OnEnable()
    {
        bar.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        bar.gameObject.SetActive(false);
    }

    public void Initialize()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}