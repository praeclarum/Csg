using Csg;
using static Csg.Solids;

public class Servo
{
    public Solid GetSolid()
    {
        return Cube();
    }
}


public static class Servos
{
    public static Solid Main()
    {
        var s = new Servo();
        return s.GetSolid();
    }
}

