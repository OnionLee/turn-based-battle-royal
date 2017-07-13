public struct IntVector2
{
    public int x;
    public int y;

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(IntVector2 a, IntVector2 b)
    {
        return (a.x == b.x) && (a.y == b.y);
    }

    public static bool operator !=(IntVector2 a, IntVector2 b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return x ^ y;
    }

    public override bool Equals(object obj)
    {
        // If parameter cannot be cast to Point return false.
        IntVector2 p = (IntVector2)obj;

        // Return true if the fields match:
        return (x == p.x) && (y == p.y);
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", x, y);
    }
}