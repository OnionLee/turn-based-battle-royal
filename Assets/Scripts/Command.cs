using System;
using System.Collections.Generic;
using System.Linq;

public enum CommandType
{
    Defense,
    Moving,
}

public abstract class Command
{
    public Player Actor { get; private set; }
    public IntVector2 Target { get; private set; }

    public Command(Player actor)
    {
        Actor = actor;
    }

    public abstract void Do();

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
    public DefenseCommand(Player actor) : base(actor)
    {
    }

    public override void Do()
    {
        Actor.Defense();
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
    public MoveCommand(Player actor) : base(actor)
    {
    }

    public override void Do()
    {
        Actor.Move(Target);
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
    public static Command CreateCommand(CommandType type, Player actor)
    {
        switch (type)
        {
            case CommandType.Moving:
                return new MoveCommand(actor);
        }

        return new DefenseCommand(actor);
    }
}