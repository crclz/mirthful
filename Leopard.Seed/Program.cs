using System;

namespace Leopard.Seed
{
	class Program
	{
		static void Main(string[] args)
		{
			var randomTextGenerator = new RandomText();

			for(int i = 1; i <= 10; i++)
			{
				Console.WriteLine(randomTextGenerator.Generate(100, 200));
				Console.WriteLine();
			}
		}
	}
}
