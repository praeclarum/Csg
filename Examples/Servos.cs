using Csg;
using static Csg.Script;

public static class Servos
{
    public static Csg.Csg Servo()
    {
        return Difference(Cube(200, center: true), Sphere(80, center:false));
    }
    public static Csg.Csg Main()
    {
        return Servo();
    }
}

