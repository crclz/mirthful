using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Leopard.Seed
{
	class RandomText
	{
		private Random Random = new Random();

		private string SourceText;

		public RandomText()
		{
			SourceText = File.ReadAllText("random-text.txt");
			SourceText = SourceText.Replace("\r\n", " ");
			SourceText = SourceText.Replace("\n", " ");
		}

		public string Generate(int requiredLength)
		{
			int start = Random.Next(0, SourceText.Length - 1);

			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < requiredLength; i++)
			{
				char c = SourceText[(start + i) % SourceText.Length];
				builder.Append(c);
			}

			return builder.ToString();
		}

		public string Generate(int minlength, int maxlength)
		{
			int requiredLength = Random.Next(minlength, maxlength);

			return Generate(requiredLength);
		}
	}
}
