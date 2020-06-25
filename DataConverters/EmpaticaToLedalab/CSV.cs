using System;
using System.Collections.Generic;
using System.IO;

namespace EmpaticaToLedalab
{
	/// <summary>
	/// Represents a CSV file
	/// </summary>
	public class CSV
	{
		public string Separator { get; set; } = ";";

		public string AsString
		{
			get
			{
				var retval = "";
				for (int i = 0; i <= height; i++)
				{
					for (int j = 0; j <= width; j++)
					{
						var hasValue = cells.TryGetValue((j, i), out var val);
						retval += hasValue && val != null ? val.ToString() : "";
						retval += Separator;
					}

					retval += "\n";
				}

				return retval;
			}
		}

		private Dictionary<(int, int), string> cells = new Dictionary<(int, int), string>();
		private int width = 0;
		private int height = 0;

		public CSV() : this(";")
		{

		}

		public CSV(string separator)
		{
			Separator = separator;
		}

		public bool LoadFromFile(string path)
		{
			if (File.Exists(path))
			{
				LoadFromString(File.ReadAllText(path));
				return true;
			}
			else
			{
				DebugConsole.WriteError($"The path ({path}) does not exist.");
				return false;
			}
		}

		public void LoadFromString(string csv)
		{
			var rows = csv.Split("\n");
			foreach (var row in rows)
			{
				AppendRow(row.Split(Separator));
			}
		}

		public void AppendRow(params object[] row)
		{
			SetRow(height, row);
		}

		public void AppendColumn(params object[] col)
		{
			SetColumn(width, col);
		}

		public void SetRow(int y, params object[] row)
		{
			for (int i = 0; i < row.Length; i++)
			{
				SetCell(i, y, row[i]);
			}

			width = Math.Max(width, row.Length);
			height = Math.Max(height, y + 1);
		}

		public void SetColumn(int x, params object[] col)
		{
			for (int i = 0; i < col.Length; i++)
			{
				SetCell(x, i, col[i]);
			}

			width = Math.Max(width, x + 1);
			height = Math.Max(height, col.Length);
		}

		public void SetCell(int x, int y, object val)
		{
			cells.Add((x, y), val.ToString());
		}

		public bool TryGetRow<T>(int y, out T[] result)
		{
			result = default;

			if (y >= height)
			{
				DebugConsole.WriteError($"No row at position ({y})");
				return false;
			}

			var row = new List<T>();
			for (int i = 0; i < width; i++)
			{
				var success = TryGetCell(i, y, out T cell);
				if (success)
				{
					row.Add(cell);
				}
			}

			result = row.ToArray();
			return true;
		}

		public bool TryGetColumn<T>(int x, out T[] result)
		{
			result = default;

			if (x >= width)
			{
				DebugConsole.WriteError($"No column at position ({x})");
				return false;
			}

			var col = new List<T>();
			for (int i = 0; i < height; i++)
			{
				var success = TryGetCell(x, i, out T cell);
				if (success)
				{
					col.Add(cell);
				}
			}

			result = col.ToArray();
			return true;
		}

		public bool TryGetCell<T>(int x, int y, out T result)
		{
			result = default;

			// check cell
			var success = cells.TryGetValue((x, y), out string cell);
			if (success == false)
			{
				DebugConsole.WriteError($"No cell at position ({x}, {y}).");
				return false;
			}

			// convert to T
			object value = default;
			if (typeof(T) == typeof(float))
			{
				success = float.TryParse(cell, out float f);
				value = f;
			}
			else if (typeof(T) == typeof(int))
			{
				success = int.TryParse(cell, out int i);
				value = i;
			}
			else if (typeof(T) == typeof(bool))
			{
				success = bool.TryParse(cell, out bool b);
				value = b;
			}
			else if (typeof(T) == typeof(string))
			{
				success = true;
				value = cell;
			}

			// output
			if (success)
			{
				result = (T)value;
				return true;
			}
			else
			{
				DebugConsole.WriteWarning($"Cell ({x}, {y}) could not be parsed to a {typeof(T)}.");
				return false;
			}
		}

		public bool SaveAs(string path)
		{
			var directory = Path.GetDirectoryName(path);
			if (Directory.Exists(directory))
			{
				File.WriteAllText(path, AsString);
				return true;
			}
			else
			{
				DebugConsole.WriteError($"Directory ({directory}) does not exist.");
				return false;
			}
		}
	}
}
