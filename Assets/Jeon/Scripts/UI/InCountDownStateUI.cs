using System;
using UnityEngine;
using UnityEngine.UI;
public class InCountDownStateUI : MonoBehaviour, IObserver
{
    [SerializeField] Slider Hp,O2,Eng;
    [SerializeField] Slider explorationBar;
    [SerializeField] Button survalStartBtn;
    [SerializeField] CubeControllerUI cubeControllerUI; 
    private Action survalStartAction;
    private Action<Cubie, CubeAxisType, bool> cubeControllAction;
    private Func<bool> canRotate;
    public void Initialize(Action survalStartAction, Action<Cubie, CubeAxisType, bool> cubeControllAction,Func<bool> canRoate)
    {
        this.survalStartAction = survalStartAction;
        this.cubeControllAction = cubeControllAction;
        this.canRotate = canRoate;
        cubeControllerUI.SetRotateAction(cubeControllAction);   
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        cubeControllerUI.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    public void UpdateObserver()
    {

        //float currentHp = playerStat.GetStat(EntityStatName.HP);
        //float maxHp = playerStat.GetStat(EntityStatName.MaxHP);
        //Hp.value = currentHp / maxHp;

        //float currentO2 = playerStat.GetStat(EntityStatName.O2);
        //float maxO2 = playerStat.GetStat(EntityStatName.MaxO2);
        //O2.value = currentO2 / maxO2;

        //float currentEng = playerStat.GetStat(EntityStatName.Eng);
        //float maxEng = playerStat.GetStat(EntityStatName.MaxEng);
        //Eng.value = currentEng / maxEng;
    }
    public void RotateCubeAction(Cubie selectedCubie, CubeAxisType axis, bool isClock)
    {
        cubeControllAction?.Invoke(selectedCubie,axis,isClock);
        if(!canRotate())
        {
            cubeControllerUI.gameObject.SetActive(false);   
        }
    }
    public void SuvalStartAction() => survalStartAction?.Invoke();  
}