using UnityEngine;
using UnityEngine.UI;

public class HPBarView : MonoBehaviour,
    IView
{
    [SerializeField] private Image _lostBar;

    public void UpdateView(int hp)
    {
        Debug.Log($"[HpBarView] --> updateview {hp}");
        _lostBar.fillAmount = 1.0f - (float) hp / Constant.CharacterModel.MaxHP;
    }

    public void UpdatePos(Vector3 worldPos)
    {
        transform.position = worldPos;
    }
}