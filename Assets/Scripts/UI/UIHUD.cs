using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIHUD : MonoBehaviour
{
    [SerializeField] private GameObject objMenu;
    private bool isMove = false;

    [SerializeField] private Button btn_Menu;
    private void Start()
    {
        objMenu.gameObject.SetActive(false);
        btn_Menu.onClick.AddListener(OnMenu);
    }

    public void OnUpgrade()
    {

    }
    public void OnMenu()
    {
        if (isMove == false)
        {
            isMove = true;
            if (objMenu.activeSelf == false)
            {
                objMenu.SetActive(true);
                objMenu.transform.DOLocalMoveY(objMenu.transform.localPosition.y - 220f, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    isMove = false;
                });
            }
            else
            {
                objMenu.transform.DOLocalMoveY(objMenu.transform.localPosition.y + 220f, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    isMove = false;
                    objMenu.SetActive(false);
                });
            }
        }
    }

    public void MainScrean()
    {

    }

}
