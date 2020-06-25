using System;
using System.Linq;
using LNE.IO;

namespace Empatica2Ledalab
{
	class Program
	{
		private static int DELETE_TAG { get; } = -1;

		static void Main(string[] args)
		{
			// load files
			var edaCSV = LoadCSVFile("EDA file path:");
			var tagCSV = LoadCSVFile("Tag file path:");

			if (edaCSV == null || tagCSV == null)
				return;

			// load necessary data
			var success = true;
			success &= edaCSV.TryGetCell(0, 0, out float startTime);
			success &= edaCSV.TryGetCell(0, 1, out float frequency);
			success &= edaCSV.TryGetColumn(0, out float[] column);
			success &= tagCSV.TryGetColumn(0, out float[] tags);

			if (success == false)
				return;

			// convert tag times from utc to the start of recording
			tags = ConvertTagTimes(tags, startTime);

			// output columns (time, eda + tag)
			var outTimes = new float[column.Length - 2];
			var outEdas = new float[column.Length - 2];
			var outTags = new float[column.Length - 2];

			// convert from empatica -> ledalab
			for (int i = 2; i < column.Length; i++)
			{
				var im2 = i - 2;

				var time = CalcTime(im2, frequency);
				outTimes[im2] = time;		// time
				outEdas[im2] = column[i];	// eda

				// tag
				for (int j = 0; j < tags.Length; j++)
				{
					var halfSampleRate = 1 / (frequency / 2);
					if (tags[j] >= time - halfSampleRate && tags[j] <= time + halfSampleRate)
					{
						outTags[im2] = j + 1;
						tags[j] = DELETE_TAG;
						break;
					}
					else
					{
						outTags[im2] = 0;
					}
				}
			}

			// save ledalab file
			var ledalabCSV = new CSV();
			ledalabCSV.Separator = "\t";
			ledalabCSV.AppendColumn(outTimes.Cast<object>().ToArray());
			ledalabCSV.AppendColumn(outEdas.Cast<object>().ToArray());
			ledalabCSV.AppendColumn(outTags.Cast<object>().ToArray());

			Console.WriteLine("Output file path:");
			var outputPath = Console.ReadLine();
			ledalabCSV.SaveAs(outputPath);
		}

		private static CSV LoadCSVFile(string prompt)
		{
			Console.WriteLine(prompt);
			var path = Console.ReadLine();

			var csv = new CSV();
			csv.Separator = "\t";
			var success = csv.LoadFromFile(path);

			return success ? csv : null;
		}

		private static float[] ConvertTagTimes(float[] tags, float startTime)
		{
			for (int i = 0; i < tags.Length; i++)
			{
				tags[i] -= startTime;
			}

			return tags;
		}

		private static float CalcTime(int row, float frequency)
		{
			return row * (1 / frequency);
		}
	}
}
