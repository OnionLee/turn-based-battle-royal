using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public IEnumerable<Player> AlivePlayers
    {
        get
        {
            return Players.Where(p => p.IsDead == false);
        }
    }

    public bool IsGameOver
    {
        get
        {
            return AlivePlayers.Count() == 1;
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
        while (IsGameOver == false)
        {
            turnCount++;

            foreach (var player in AlivePlayers)
            {
                CurrentPlayer = player;

                StartTurn();

                while (CurrentPlayer.IsTurnDone == false) yield return null;
            }

            yield return null;

            DoCommand(AlivePlayers.Select(p => p.Command));

            yield return new WaitForSeconds(2.0f);
        }

        hud.ShowMsg(CurrentPlayer.Name + "승리!");
    }

    private void DoCommand(IEnumerable<Command> commands)
    {
        var sb = new StringBuilder();

        var defensees = commands.Where(c => c is DefenseCommand).Cast<DefenseCommand>();

        foreach (var defense in defensees)
        {
            sb = sb.Append(defense.Do());
            sb = sb.Append("\n");
        }

        //이동의 경우는 섞는다.
        var moves = commands.Where(c => c is MoveCommand).Cast<MoveCommand>().ToList();
        moves.Shuffle();

        foreach (var move in moves)
        {
            sb = sb.Append(move.Do());
            sb = sb.Append("\n");
        }

        var attacks = commands.Where(c => c is AttackCommand).Cast<AttackCommand>().ToList();

        foreach (var attack in attacks)
        {
            sb = sb.Append(attack.Do());
            sb = sb.Append("\n");
        }

        hud.ShowMsg(sb.ToString());
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

    public Player FindEnemy(IntVector2 target)
    {
        return AlivePlayers.FirstOrDefault(p => p.Position == target);
    }

    private void OnTileClicked(Tile tile)
    {
        if (CurrentPlayer.Command == null)
        {
            hud.ShowMsg("커맨드를 선택해주세요.");
        }
        else if (CurrentPlayer.Command.TrySetTarget(tile) == false)
        {
            hud.ShowMsg(CurrentPlayer.Command.GetInValidClickMsg());
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