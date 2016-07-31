using System;

namespace Csg
{
	public struct Vector3D
	{
		public double X, Y, Z;

		public Vector3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}
		public double Length
		{
			get { throw new NotImplementedException(); }
		}

		public double Dot(Vector3D other)
		{
			throw new NotImplementedException();
		}

		public Vector3D Cross(Vector3D other)
		{
			throw new NotImplementedException();
		}

		public Vector3D Unit
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Vector3D Min(Vector3D other)
		{
			return new Vector3D(Math.Min(X, other.X), Math.Min(Y, other.Y), Math.Min(Z, other.Z));
		}

		public Vector3D Max(Vector3D other)
		{
			return new Vector3D(Math.Max(X, other.X), Math.Max(Y, other.Y), Math.Max(Z, other.Z));
		}

		public static Vector3D operator +(Vector3D a, Vector3D b)
		{
			throw new NotImplementedException();
		}
		public static Vector3D operator -(Vector3D a, Vector3D b)
		{
			throw new NotImplementedException();
		}
		public static Vector3D operator *(Vector3D a, double b)
		{
			throw new NotImplementedException();
		}

}
}

