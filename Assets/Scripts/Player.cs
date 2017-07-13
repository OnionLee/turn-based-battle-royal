using System;

public class Player
{
    public string Name { get; private set; }

    public IntVector2 Position { get; private set; }

    public bool IsTurnDone { get; private set; }

    private Stage stage;

    public Command Command { get; private set; }

    public event Action CommandChanged;

    public int HP { get; private set; }

    public bool IsDead { get { return HP <= 0; } }

    public Player(string name, IntVector2 position, Stage stage)
    {
        this.HP = 4;
        this.Name = name;
        this.Position = position;

        this.stage = stage;
    }

    public void OnStartTurn()
    {
        IsTurnDone = false;

        // 최조에 커맨드를 없앤다.
        OnCommandCancled();
    }

    public void OnCommandSelected(CommandType type)
    {
        Command = CommandFactory.CreateCommand(type, stage, this);

        if (CommandChanged != null)
            CommandChanged();
    }

    public void OnCommandCancled()
    {
        Command = null;

        if (CommandChanged != null)
            CommandChanged();
    }

    public void OnEndTurn()
    {
        IsTurnDone = true;

        if (CommandChanged != null)
            CommandChanged();
    }

    #region Action

    public void Move(IntVector2 target)
    {
        Position = target;
    }

    public void Defense()
    {
        // TODO
    }

    public void Hit(int damage)
    {
        HP -= damage;
    }

    #endregion Action
}