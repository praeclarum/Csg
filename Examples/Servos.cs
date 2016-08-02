using Csg;
using static Csg.Script;

public static class Servos
{
    public static Solid Servo()
    {
        return Difference(Sphere(200, center: true), Sphere(90, center:false));
    }
    public static Solid Main()
    {
        return Servo();
    }
}

