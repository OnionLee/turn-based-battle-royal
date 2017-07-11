using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private Button moveButton;

    [SerializeField]
    private Text notiText;

    private Stage stage;

    private void Awake()
    {
        moveButton.onClick.AddListener(OnMoveButtonClicked);

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
        stage.CurrentPlayer.OnCommandSelected(CommandType.Moving);
    }

    public void OnStartTurn()
    {
        ResetNoti();
    }

    public void OnInvaildTileClicked(string msg)
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