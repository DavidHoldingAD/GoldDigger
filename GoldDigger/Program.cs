using System;
using System.Threading;

namespace GoldDigger
{
	class Program
	{
		enum Item
		{
			Empty,
			OurGuy,
			SomeGuy,
			Diamond,
			Ground,
			Grass,
			Tree,
			Stone,
		}

		enum Direction
		{
			Up,
			Right,
			Down,
			Left,
		}

		static bool GetFieldBounds(int maxX, int maxY, out int x, out int y)
		{
			const int MinX = 10;
			const int MinY = 10;

			x = 0;
			y = 0;

			bool success;

			#region Взима стойност за x

			do
			{
				Console.Write("x ({0} <= x <= {1}) = ", MinX, maxX);
				string line = Console.ReadLine();
				if (string.IsNullOrWhiteSpace(line))
					return false;

				success = int.TryParse(line, out x);
			}
			while (!success || x < MinX || x > maxX);

			#endregion

			#region Взима стойност за y

			do
			{
				Console.Write("y ({0} <= y <= {1}) = ", MinY, maxY);
				string line = Console.ReadLine();
				if (string.IsNullOrWhiteSpace(line))
					return false;

				success = int.TryParse(line, out y);
			}
			while (!success || y < MinY || y > maxY);

			#endregion

			return true;
		}

		static void GetEmptyPosition(Item[,] field, out int x, out int y)
		{
			int maxX = field.GetLength(0);
			int maxY = field.GetLength(1);

			Random random = new Random();

			do
			{
				x = random.Next(maxX);
				y = random.Next(maxY);
			}
			while (field[x, y] != Item.Empty);
		}

		static Item[,] CreateField(int m, int n, out int ourGuyX, out int ourGuyY, out int diamondsLeft)
		{
			Item[,] field = new Item[m, n];

			GetEmptyPosition(field, out ourGuyX, out ourGuyY);
			field[ourGuyX, ourGuyY] = Item.OurGuy;

			int someGuyCount = (int)(m * n * 0.05);
			for (int i = 0; i < someGuyCount; i++)
			{
				int someGuyX, someGuyY;
				GetEmptyPosition(field, out someGuyX, out someGuyY);
				field[someGuyX, someGuyY] = Item.SomeGuy;
			}

			diamondsLeft = (int)(m * n * 0.10);
			for (int i = 0; i < diamondsLeft; i++)
			{
				int diamondX, diamondY;
				GetEmptyPosition(field, out diamondX, out diamondY);
				field[diamondX, diamondY] = Item.Diamond;
			}

			Random random = new Random();
			for (int x = 0; x < m; x++)
				for (int y = 0; y < n; y++)
				{
					if (field[x, y] == Item.Empty)
					{
						int num = random.Next(100);
						if (num < 40)
							field[x, y] = Item.Ground;
						else if (num < 70)
							field[x, y] = Item.Grass;
						else if (num < 90)
							field[x, y] = Item.Tree;
						else
							field[x, y] = Item.Stone;
					}
				}

			return field;
		}

		static void DrawField(Item[,] field)
		{
			ConsoleColor backColor = Console.BackgroundColor;
			ConsoleColor foreColor = Console.ForegroundColor;

			Console.BackgroundColor = ConsoleColor.DarkRed;

			int m = field.GetLength(0);
			int n = field.GetLength(1);
			for (int y = 0; y < n; y++)
			{
				Console.SetCursorPosition(0, y);
				for (int x = 0; x < m; x++)
				{
					switch (field[x, y])
					{
						case Item.Empty:
							Console.Write(' ');
							break;
						case Item.OurGuy:
							Console.ForegroundColor = ConsoleColor.Blue;
							Console.Write('\u263A');
							break;
						case Item.SomeGuy:
							Console.ForegroundColor = ConsoleColor.Red;
							Console.Write('\u263B');
							break;
						case Item.Diamond:
							Console.ForegroundColor = ConsoleColor.Gray;
							Console.Write('\u2666');
							break;
						case Item.Ground:
							Console.ForegroundColor = ConsoleColor.Cyan;
							Console.Write('\u2592');
							break;
						case Item.Grass:
							Console.ForegroundColor = ConsoleColor.Green;
							Console.Write('\u2593');
							break;
						case Item.Tree:
							Console.ForegroundColor = ConsoleColor.DarkGreen;
							Console.Write('\u2663');
							break;
						case Item.Stone:
							Console.ForegroundColor = ConsoleColor.DarkGray;
							Console.Write('\u0665');
							break;
						default:
							Console.ForegroundColor = ConsoleColor.Red;
							Console.Write('?');
							break;
					}
				}
			}

			Console.ForegroundColor = foreColor;
			Console.BackgroundColor = backColor;
		}

		static bool MoveGuy(Item[,] field, int x, int y, Direction direction,
			out int newX, out int newY, out bool hadDiamond)
		{
			int m = field.GetLength(0);
			int n = field.GetLength(1);

			newX = x;
			newY = y;
			hadDiamond = false;

			switch (direction)
			{
				case Direction.Up:
					newY--;
					break;
				case Direction.Right:
					newX++;
					break;
				case Direction.Down:
					newY++;
					break;
				case Direction.Left:
					newX--;
					break;
				default:
					return false;
			}

			if (newX < 0)
				return false;
			if (newX >= m)
				return false;
			if (newY < 0)
				return false;
			if (newY >= n)
				return false;

			switch (field[newX, newY])
			{
				case Item.Empty:
				case Item.Ground:
				case Item.Grass:
					field[newX, newY] = field[x, y];
					field[x, y] = Item.Ground;
					return true;
				case Item.Diamond:
					hadDiamond = true;

					field[newX, newY] = field[x, y];
					field[x, y] = Item.Ground;
					return true;
				case Item.OurGuy:
				case Item.SomeGuy:
				case Item.Tree:
				case Item.Stone:
				default:
					return false;
			}
		}

		static void MoveSomeGuys(Item[,] field, ref int diamondsLeft)
		{
			int m = field.GetLength(0);
			int n = field.GetLength(1);

			Random random = new Random();

			for (int y = 0; y < n; y++)
			{
				for (int x = 0; x < m; x++)
				{
					if (field[x, y] == Item.SomeGuy)
					{
						Direction direction;

						int num = random.Next(4);
						if (num == 0)
							direction = Direction.Up;
						else if (num == 1)
							direction = Direction.Right;
						else if (num == 2)
							direction = Direction.Down;
						else
							direction = Direction.Left;

						int newX, newY;
						bool hadDiamond;
						MoveGuy(field, x, y, direction, out newX, out newY, out hadDiamond);
						if (hadDiamond)
							diamondsLeft--;
					}
				}
			}
		}

		static void MoveOurGuy(Item[,] field, ref int x, ref int y, Direction direction, ref int diamondsCollected, ref int diamondsLeft)
		{
			bool hadDiamond;
			int newX, newY;
			if (MoveGuy(field, x, y, direction, out newX, out newY, out hadDiamond))
			{
				x = newX;
				y = newY;

				if (hadDiamond)
				{
					diamondsCollected++;
					diamondsLeft--;
				}
			}
		}

		static bool ProcessInput(Item[,] field, ref int ourGuyX, ref int ourGuyY,
			ref int diamondsCollected, ref int diamondsLeft, out bool escape)
		{
			escape = false;
			if (!Console.KeyAvailable)
				return false;

			ConsoleKeyInfo info = Console.ReadKey(true);

			switch (info.Key)
			{
				case ConsoleKey.UpArrow:
					MoveOurGuy(field, ref ourGuyX, ref ourGuyY, Direction.Up, ref diamondsCollected, ref diamondsLeft);
					break;
				case ConsoleKey.RightArrow:
					MoveOurGuy(field, ref ourGuyX, ref ourGuyY, Direction.Right, ref diamondsCollected, ref diamondsLeft);
					break;
				case ConsoleKey.DownArrow:
					MoveOurGuy(field, ref ourGuyX, ref ourGuyY, Direction.Down, ref diamondsCollected, ref diamondsLeft);
					break;
				case ConsoleKey.LeftArrow:
					MoveOurGuy(field, ref ourGuyX, ref ourGuyY, Direction.Left, ref diamondsCollected, ref diamondsLeft);
					break;
				case ConsoleKey.Escape:
					escape = true;
					break;
			}

			return true;
		}

		static void Main(string[] args)
		{
			int maxX = Console.WindowWidth - 1;
			int maxY = Console.WindowHeight - 1;

			int x, y;
			if (!GetFieldBounds(maxX, maxY, out x, out y))
				return;

			int ourGuyX, ourGuyY, diamondsTotal;
			Item[,] field = CreateField(x, y, out ourGuyX, out ourGuyY, out diamondsTotal);

			Console.Clear();

			bool escape = false;
			int diamondsLeft = diamondsTotal;
			int diamondsCollected = 0;
			while (!escape && diamondsLeft > 0)
			{
				DrawField(field);

				MoveSomeGuys(field, ref diamondsLeft);

				ProcessInput(field, ref ourGuyX, ref ourGuyY, ref diamondsCollected, ref diamondsLeft, out escape);

				Thread.Sleep(100);
			}

			Console.WriteLine("Diamonds:");
			Console.WriteLine("* total:\t{0}", diamondsTotal);
			Console.WriteLine("* collected:\t {0} ({1}%)", diamondsCollected, 100 * diamondsCollected / diamondsTotal);
			Console.WriteLine("* stolen:\t {0} ({1}%)", diamondsTotal - diamondsCollected, 100 * (diamondsTotal - diamondsCollected) / diamondsTotal);
			Console.WriteLine("* left:\t{0} ({1}%)", diamondsLeft, 100 * diamondsLeft / diamondsTotal);
			Console.WriteLine();
			Console.Write("Press [Enter] to exit...");
			Console.ReadLine();
		}
	}
}
