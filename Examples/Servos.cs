using Csg;
using static Csg.Solids;

public class Servo
{
    public double BodyWidth = 40.3;
    public double BodyHeight = 36.1;
    public double BodyDepth = 20;
    public double BodyDescent = 26.6;
    public double TotalWidth = 55.5;
    public double BracketHeight = 2.54;
    public double TotalHeight = 44.1;
    public double HoleSpacing = 10.0;
    public double HoleDiameter = 4.32;
    public double HoleInset = 2.61;
    public Solid GetSolid()
    {
        Solid body =
            Cube(size: new Vector3D(BodyWidth, BodyHeight, BodyDepth)).
            Translate(-BodyWidth/2, 0, -BodyDepth/2);
        Solid bracket =
            Cube(TotalWidth, BracketHeight, BodyDepth, center: false).
            Translate(-TotalWidth/2, BodyDescent, -BodyDepth/2);
        Solid hole1 =
            Cylinder(HoleDiameter/2, BracketHeight*4, center: true).
            Translate(-TotalWidth/2 + HoleInset, BodyDescent, -HoleSpacing/2); 
        Solid hole2 =
            Cylinder(HoleDiameter/2, BracketHeight*4, center: true).
            Translate(-TotalWidth/2 + HoleInset, BodyDescent, HoleSpacing/2); 
        Solid hole3 =
            Cylinder(HoleDiameter/2, BracketHeight*4, center: true).
            Translate(TotalWidth/2 - HoleInset, BodyDescent, -HoleSpacing/2); 
        Solid hole4 =
            Cylinder(HoleDiameter/2, BracketHeight*4, center: true).
            Translate(TotalWidth/2 - HoleInset, BodyDescent, HoleSpacing/2); 
        return Union(body, Difference(bracket, hole1, hole2, hole3, hole4)).Translate(y: -TotalHeight/2);
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

