using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField]
    private int startX;

    [SerializeField]
    private int startY;

    [SerializeField]
    private int tileSize;

    [SerializeField]
    private int mapSize;

    [SerializeField]
    private int spacing;

    [SerializeField]
    private Tile tilePrefab;

    private List<Tile> tiles = new List<Tile>();

    private Player player;
    private Player enemy;

    private bool isGameOver;
    private int turnCount;

    [SerializeField]
    private HUD hud;

    public IEnumerable<Player> Players
    {
        get
        {
            yield return player;
            yield return enemy;
        }
    }

    public Player CurrentPlayer { get; private set; }

    private int playerPositionCounter = 0;
    private List<IntVector2> playerPositions = new List<IntVector2>();

    private void Start()
    {
        InitPlayers();

        CreateTiles();

        StartCoroutine(UpdateTurn());
    }

    private void InitPlayers()
    {
        InitPlayerPositions();

        player = new Player("강희", GetRandomPosition(), this);
        player.CommandChanged += UpdateTiles;

        enemy = new Player("병조", GetRandomPosition(), this);
        enemy.CommandChanged += UpdateTiles;
    }

    private void InitPlayerPositions()
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
                playerPositions.Add(new IntVector2(x, y));
        }

        playerPositions.Shuffle();
    }

    private IEnumerator UpdateTurn()
    {
        while (isGameOver == false)
        {
            turnCount++;

            foreach (var player in Players)
            {
                CurrentPlayer = player;

                StartTurn();

                while (CurrentPlayer.IsTurnDone == false) yield return null;
            }

            yield return null;

            DoCommand(Players.Select(p => p.Command));
        }
    }

    private void DoCommand(IEnumerable<Command> commands)
    {
        foreach (var command in commands)
        {
            command.Do();
        }
    }

    private void CreateTiles()
    {
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                var tile = CreateTile(new IntVector2(y, x), tileSize);

                tiles.Add(tile);
            }
        }
    }

    // TOOD 팩토리로 분리
    private Tile CreateTile(IntVector2 pos, float scale)
    {
        var tile = Instantiate(tilePrefab);
        tile.transform.SetParent(transform, false);

        tile.SetPosition(pos);
        tile.SetScale(scale);
        tile.SetState(TileState.None);
        tile.Clicked += OnTileClicked;

        return tile;
    }

    private void UpdateTiles()
    {
        foreach (var tile in tiles)
        {
            if (CurrentPlayer.Command != null && CurrentPlayer.Command.ValidClick(tile))
                tile.SetState(TileState.Actionable);
            else if (tile.Postion == CurrentPlayer.Position)
                tile.SetState(TileState.Player);
            else
                tile.SetState(TileState.None);
        }
    }

    public IntVector2 GetRandomPosition()
    {
        return playerPositions[playerPositionCounter++];
    }

    private void StartTurn()
    {
        CurrentPlayer.OnStartTurn();
        hud.OnStartTurn();
    }

    private void OnTileClicked(Tile tile)
    {
        if (CurrentPlayer.Command == null)
        {
            hud.OnInvaildTileClicked("커맨드를 선택해주세요.");
        }
        else if (CurrentPlayer.Command.TrySetTarget(tile) == false)
        {
            hud.OnInvaildTileClicked(CurrentPlayer.Command.GetInValidClickMsg());
        }
        else
        {
            CurrentPlayer.OnEndTurn();
        }
    }

    public static Vector3 TilePosToStagePos(IntVector2 pos)
    {
        var stage = FindObjectOfType<Stage>();
        var xPos = stage.startX + (stage.tileSize * pos.x) + (stage.spacing * pos.x);
        var yPos = stage.startY + (stage.tileSize * pos.y) + (stage.spacing * pos.y);

        return new Vector3(xPos, yPos, 1);
    }
}