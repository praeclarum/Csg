using Csg;
using static Csg.Script;

public static class Servos
{
    public static Csg.Csg Servo()
    {
        return Difference(Sphere(100, center: true), Sphere(100, center:false));
    }
    public static Csg.Csg Main()
    {
        return Servo();
    }
}

