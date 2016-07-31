using System;

namespace Csg
{
	public struct Vector3D : IEquatable<Vector3D>
	{
		public double X, Y, Z;

		public Vector3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public bool Equals(Vector3D a)
		{
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
			return X == a.X && Y == a.Y && Z == a.Z;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
		}

		public double Length
		{
			get { return Math.Sqrt(X * X + Y * Y + Z * Z); }
		}

		public double DistanceToSquared(Vector3D a)
		{
			var dx = X - a.X;
			var dy = Y - a.Y;
			var dz = Z - a.Z;
			return dx * dx + dy * dy + dz * dz;
		}

		public double Dot(Vector3D a)
		{
			return X * a.X + Y * a.Y + Z * a.Z;
		}

		public Vector3D Cross(Vector3D a)
		{
			return new Vector3D(
				Y * a.Z - Z * a.Y,
				Z * a.X - X * a.Z,
				X * a.Y - Y * a.X);
		}

		public Vector3D Unit
		{
			get
			{
				var d = Length;
				return new Vector3D(X / d, Y / d, Z / d);
			}
		}

		public Vector3D Negated
		{
			get
			{
				return new Vector3D(-X, -Y, -Z);
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
			return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}
		public static Vector3D operator -(Vector3D a, Vector3D b)
		{
			return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}
		public static Vector3D operator *(Vector3D a, double b)
		{
			return new Vector3D(a.X * b, a.Y * b, a.Z * b);
		}

		public override string ToString()
		{
			return $"[{X}, {Y}, {Z}]";
		}
	}

	public struct Vector2D : IEquatable<Vector2D>
	{
		public double X, Y;

		public Vector2D(double x, double y)
		{
			X = x;
			Y = y;
		}

		public bool Equals(Vector2D a)
		{
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
			return X == a.X && Y == a.Y;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
		}

		public double Length
		{
			get { return Math.Sqrt(X * X + Y * Y); }
		}

		public double DistanceTo(Vector2D a)
		{
			var dx = X - a.X;
			var dy = Y - a.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}
	}

	public class OrthoNormalBasis
	{
		public OrthoNormalBasis(Plane plane)
		{
		}
		public Vector2D To2D(Vector3D a)
		{
			throw new NotImplementedException();
		}
		public Vector3D To3D(Vector2D a)
		{
			throw new NotImplementedException();
		}
	}

	public class Line2D
	{
		public Vector2D Direction()
		{
			throw new NotImplementedException();
		}
		public static Line2D FromPoints(Vector2D a, Vector2D b)
		{
			throw new NotImplementedException();
		}
	}
}

