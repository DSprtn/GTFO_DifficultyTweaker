using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_DIfficulty_Tweaker.Util
{
	public static class PathUtil
	{

		public static bool CheckFile(string pathToFile)
		{
			return File.Exists(pathToFile);
		}

		public static string MakeRelativeDirectory(string path)
		{
			string text = Path.Combine(Path.Combine(Paths.ConfigPath, "GameModes"), path);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}
}
