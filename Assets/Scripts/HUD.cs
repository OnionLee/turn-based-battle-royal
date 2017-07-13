using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private Button moveButton;

    [SerializeField]
    private Button attackButton;

    [SerializeField]
    private Button defenseButton;

    [SerializeField]
    private Text notiText;

    private Stage stage;

    private void Awake()
    {
        moveButton.onClick.AddListener(OnMoveButtonClicked);
        defenseButton.onClick.AddListener(OnDefenseButtonClicked);
        attackButton.onClick.AddListener(OnAttackButtonClicked);

        stage = FindObjectOfType<Stage>();
    }

    private void OnDestroy()
    {
        moveButton.onClick.RemoveAllListeners();
    }

    private void ResetNoti()
    {
        notiText.text = string.Format("{0}의 턴 입니다.", stage.CurrentPlayer.Name);
    }

    public void OnMoveButtonClicked()
    {
        stage.CurrentPlayer.OnCommandSelected(CommandType.Move);
    }

    public void OnAttackButtonClicked()
    {
        stage.CurrentPlayer.OnCommandSelected(CommandType.Attack);
    }

    public void OnDefenseButtonClicked()
    {
        stage.CurrentPlayer.OnCommandSelected(CommandType.Defense);
    }

    public void OnStartTurn()
    {
        ResetNoti();
    }

    public void ShowMsg(string msg)
    {
        StartCoroutine(PrintMsg(msg));
    }

    private IEnumerator PrintMsg(string msg)
    {
        notiText.text = msg;

        yield return new WaitForSeconds(2.0f);

        ResetNoti();
    }
}