using System;
namespace Csg.Viewer.Mac
{
	public class Workspace
	{
		byte[] bytes = new byte[0];
		string path = "";

		public byte[] Bytes => bytes;
		public string FilePath => path;

		public event Action BytesChanged;

		public void SetBytes(byte[] bytes, string path)
		{
			this.bytes = bytes;
			this.path = path;
			BytesChanged?.Invoke();
		}
	}
}

