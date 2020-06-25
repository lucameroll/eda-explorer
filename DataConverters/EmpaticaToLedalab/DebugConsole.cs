using System;

namespace EmpaticaToLedalab
{
	public static class DebugConsole
	{
		public static void WriteWithColour(string message, ConsoleColor colour)
		{
			var lastColour = Console.ForegroundColor;
			Console.ForegroundColor = colour;
			Console.WriteLine(message);
			Console.ForegroundColor = lastColour;
		}

		public static void Write(string message)
		{
			WriteWithColour(message, Console.ForegroundColor);
		}

		public static void WriteWarning(string message)
		{
			WriteWithColour(message, ConsoleColor.Yellow);
		}

		public static void WriteError(string message)
		{
			WriteWithColour(message, ConsoleColor.Red);
		}

		public static void WriteSuccess(string message)
		{
			WriteWithColour(message, ConsoleColor.Green);
		}
	}
}
