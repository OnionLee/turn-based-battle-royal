using System.Collections.Generic;
using System.Linq;

public enum CommandType
{
    Defense,
    Move,
    Attack,
}

public abstract class Command
{
    public Stage Stage { get; private set; }
    public Player Actor { get; private set; }
    public IntVector2 Target { get; private set; }

    public Command(Stage stage, Player actor)
    {
        Stage = stage;
        Actor = actor;
    }

    public abstract string Do();

    public bool TrySetTarget(Tile tile)
    {
        if (ValidClick(tile))
        {
            Target = tile.Postion;

            return true;
        }

        return false;
    }

    public abstract bool ValidClick(Tile tile);

    protected abstract IEnumerable<IntVector2> GetClickablePositions();

    public abstract string GetInValidClickMsg();
}

public class DefenseCommand : Command
{
    public DefenseCommand(Stage stage, Player actor) : base(stage, actor)
    {
    }

    public override string Do()
    {
        Actor.Defense();

        return string.Format("[{0}]{1}이(가) 해당지역을 방어했습니다.", Target.ToString(), Actor.Name);
    }

    public override bool ValidClick(Tile tile)
    {
        return GetClickablePositions().Any(p => p == tile.Postion);
    }

    protected override IEnumerable<IntVector2> GetClickablePositions()
    {
        yield return Actor.Position;
    }

    public override string GetInValidClickMsg()
    {
        return "해당 타일을 방어할 수 없습니다.";
    }
}

public class MoveCommand : Command
{
    public MoveCommand(Stage stage, Player actor) : base(stage, actor)
    {
    }

    public override string Do()
    {
        var enemy = Stage.FindEnemy(Target);
        if (enemy != null)
        {
            enemy.Hit(1);
            Actor.Hit(1);

            return string.Format("[{0}]{1}와 {2}이(가) 해당지역에서 서로 만나 {3}의 피해를 입히고 돌아갔습니다.", Target.ToString(), Actor.Name, enemy.Name, 1);
        }
        else
        {
            Actor.Move(Target);

            return string.Format("[{0}]{1}이(가) 해당지역으로 이동했습니다.", Target.ToString(), Actor.Name);
        }
    }

    public override bool ValidClick(Tile tile)
    {
        return GetClickablePositions().Any(p => p == tile.Postion);
    }

    protected override IEnumerable<IntVector2> GetClickablePositions()
    {
        yield return new IntVector2(Actor.Position.x + 1, Actor.Position.y);
        yield return new IntVector2(Actor.Position.x - 1, Actor.Position.y);
        yield return new IntVector2(Actor.Position.x, Actor.Position.y + 1);
        yield return new IntVector2(Actor.Position.x, Actor.Position.y - 1);
    }

    public override string GetInValidClickMsg()
    {
        return "해당 타일로 이동할 수 없습니다.";
    }
}

public class AttackCommand : Command
{
    public AttackCommand(Stage stage, Player actor) : base(stage, actor)
    {
    }

    public override string Do()
    {
        var enemy = Stage.FindEnemy(Target);
        if (enemy != null)
        {
            enemy.Hit(2);
            return string.Format("[{0}]{1}이(가) {2}을(를) 해당지역에서 공격해 {3}의 피해를 입혔습니다.", Target.ToString(), Actor.Name, enemy.Name, 2);
        }
        else
        {
            return string.Format("[{0}]{1}이(가) 해당지역에서 공격했으나 아무도 없었습니다.", Target.ToString(), Actor.Name);
        }
    }

    public override bool ValidClick(Tile tile)
    {
        return GetClickablePositions().Any(p => p == tile.Postion);
    }

    protected override IEnumerable<IntVector2> GetClickablePositions()
    {
        yield return new IntVector2(Actor.Position.x + 1, Actor.Position.y);
        yield return new IntVector2(Actor.Position.x - 1, Actor.Position.y);
        yield return new IntVector2(Actor.Position.x, Actor.Position.y + 1);
        yield return new IntVector2(Actor.Position.x, Actor.Position.y - 1);
    }

    public override string GetInValidClickMsg()
    {
        return "해당 타일로 이동할 수 없습니다.";
    }
}

public static class CommandFactory
{
    public static Command CreateCommand(CommandType type, Stage stage, Player actor)
    {
        switch (type)
        {
            case CommandType.Move:
                return new MoveCommand(stage, actor);

            case CommandType.Attack:
                return new AttackCommand(stage, actor);

            case CommandType.Defense:
                return new DefenseCommand(stage, actor);
        }

        return new DefenseCommand(stage, actor);
    }
}