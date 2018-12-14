namespace Csg
{
    public class Vertex
    {
        public Vector3D Pos;
        int tag = 0;

        public Vertex(Vector3D pos)
        {
            Pos = pos;
        }

        public int Tag
        {
            get
            {
                if (tag == 0)
                {
                    tag = Solid.GetTag();
                }
                return tag;
            }
        }

        public Vertex Flipped()
        {
            return this;
        }

        public override string ToString() => Pos.ToString();

        public Vertex Transform(Matrix4x4 matrix4x4)
        {
            var newpos = Pos * matrix4x4;
            return new Vertex(newpos);
        }
    }
}

