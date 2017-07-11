using System;
using UnityEngine;

using UnityEngine.EventSystems;

public enum TileState
{
    None,
    Player,
    Actionable,
}

// 나중에 비주얼이랑 로직 분리
public class Tile : MonoBehaviour, IPointerClickHandler
{
    public event Action<Tile> Clicked;

    public TileState State { get; private set; }

    [SerializeField]
    private tk2dSprite sprite;

    [SerializeField]
    private Color32 playerColor;

    [SerializeField]
    private Color32 actionAbleColor;

    [SerializeField]
    private Color32 noneColor;

    public IntVector2 Postion { get; private set; }

    public void SetState(TileState state)
    {
        this.State = state;

        UpdateColor();
    }

    private void UpdateColor()
    {
        sprite.color = GetCurColor();
    }

    private Color32 GetCurColor()
    {
        switch (State)
        {
            case TileState.Player:
                return playerColor;

            case TileState.Actionable:
                return actionAbleColor;

            case TileState.None:
                return noneColor;
        }

        return new Color32(0, 0, 0, 100);
    }

    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, 1);
    }

    public void SetPosition(IntVector2 pos)
    {
        Postion = pos;

        transform.localPosition = Stage.TilePosToStagePos(Postion);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Clicked != null)
            Clicked(this);
    }
}