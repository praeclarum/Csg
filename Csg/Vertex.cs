namespace Csg
{
    public class Vertex
    {
		/// <summary>
		/// The world position of this vertex.
		/// </summary>
        public readonly Vector3D Pos;

		/// <summary>
		/// The texture coordinate of this vertex.
		/// </summary>
		public readonly Vector2D Tex;
        int tag = 0;

        public Vertex(Vector3D pos, Vector2D tex)
        {
            Pos = pos;
			Tex = tex;
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
            return new Vertex(newpos, Tex);
        }
    }
}

