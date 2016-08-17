using System;
using System.Threading;

namespace GoldDigger
{
	class Program
	{
		#region Тип: Item

		/// <summary>
		/// Описва съдържанието на дадена позиция от полето
		/// </summary>
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

		#endregion

		#region Тип: Direction

		/// <summary>
		/// Описва посока на движение
		/// </summary>
		enum Direction
		{
			Up,
			Right,
			Down,
			Left,
		}

		#endregion

		/// <summary>
		/// Подканва потребителя да въведе размерите на полето
		/// </summary>
		/// <param name="maxX">максимална широчина на полето</param>
		/// <param name="maxY">максимална височина на полето</param>
		/// <param name="x">въведена широчина на полето</param>
		/// <param name="y">въведена височина на полето</param>
		/// <returns>true, ако потребителят е въвел размерите на полето, иначе - false</returns>
		static bool GetFieldBounds(int maxX, int maxY, out int x, out int y)
		{
			// константи за минималния размер на полето
			const int MinX = 10, MinY = 10;

			x = 0;
			y = 0;

			bool success;

			#region Взима стойност за x

			do
			{
				// подканваме потребителя да въведе число между MinX и maxX
				Console.Write("x ({0} <= x <= {1}) = ", MinX, maxX);
				// изчитаме въведения от потребителя ред
				string line = Console.ReadLine();
				// ако потребителят не е въвел нищо, значи иска да излезе
				if (string.IsNullOrWhiteSpace(line))
					return false;
				// опитваме да обработим въведения низ
				success = int.TryParse(line, out x);
			}
			// повтаряме, докато обработката не мине успешно и числото не влезе в зададените граници
			while (!success || x < MinX || x > maxX);

			#endregion

			#region Взима стойност за y

			do
			{
				// подканваме потребителя да въведе число между MinY и maxY
				Console.Write("y ({0} <= y <= {1}) = ", MinY, maxY);
				// изчитаме въведения от потребителя ред
				string line = Console.ReadLine();
				// ако потребителят не е въвел нищо, значи иска да излезе
				if (string.IsNullOrWhiteSpace(line))
					return false;
				// опитваме да обработим въведения низ
				success = int.TryParse(line, out y);
			}
			// повтаряме, докато обработката не мине успешно и числото не влезе в зададените граници
			while (!success || y < MinY || y > maxY);

			#endregion

			return true;
		}

		/// <summary>
		/// Взима произволна празна позиция от полето
		/// </summary>
		/// <param name="field">поле</param>
		/// <param name="x">позиция по хоризонтала</param>
		/// <param name="y">позиция по вертикала</param>
		static void GetEmptyPosition(Item[,] field, out int x, out int y)
		{
			// взимаме размерите на полето
			int m = field.GetLength(0);
			int n = field.GetLength(1);

			// създаваме генератор на произволни числа
			var random = new Random();

			do
			{
				// генерираме произволни стойности за x и y
				x = random.Next(m);
				y = random.Next(n);
			}
			// продължаваме, докато не попаднем на позиция, на която няма нищо
			while (field[x, y] != Item.Empty);
		}

		/// <summary>
		/// Създава поле, попълва го и връща информация за него
		/// </summary>
		/// <param name="m">широчина на полето</param>
		/// <param name="n">височина на полето</param>
		/// <param name="ourGuyX">хоризонтална позиция на "нашия човек"</param>
		/// <param name="ourGuyY">вертикална позиция на "нашия човек"</param>
		/// <param name="diamondsTotal">общ брой диаманти в полето</param>
		/// <returns>поле</returns>
		static Item[,] CreateField(int m, int n, out int ourGuyX, out int ourGuyY, out int diamondsTotal)
		{
			// създаваме празно поле
			var field = new Item[m, n];

			// намираме подходяща позиция за "нашия човек"
			GetEmptyPosition(field, out ourGuyX, out ourGuyY);
			// поставяме нашия човек на намерената позиция
			field[ourGuyX, ourGuyY] = Item.OurGuy;

			// изчисляваме броя на "чуждите хора"
			var someGuyCount = (int)(m * n * 0.05);
			for (int i = 0; i < someGuyCount; i++)
			{
				// намираме подходяща позиция за "чужд човек"
				int someGuyX, someGuyY;
				GetEmptyPosition(field, out someGuyX, out someGuyY);
				// поставяме "чуждия човек" на намерената позиция
				field[someGuyX, someGuyY] = Item.SomeGuy;
			}

			// изчисляваме броя на диамантите
			diamondsTotal = (int)(m * n * 0.10);
			for (int i = 0; i < diamondsTotal; i++)
			{
				// намираме подходяща позиция за диамант
				int diamondX, diamondY;
				GetEmptyPosition(field, out diamondX, out diamondY);
				// поставяме диаманта на намерената позиция
				field[diamondX, diamondY] = Item.Diamond;
			}

			// създаваме генератор на случайни числа
			var random = new Random();

			// обхождаме всички позиции в полето
			for (int y = 0; y < n; y++)
			{
				for (int x = 0; x < m; x++)
				{
					// проверяваме дали съответното място е празно
					if (field[x, y] == Item.Empty)
					{
						// генерираме произволно число от 0 до 99
						int num = random.Next(100);
						// с 40% вероятност поставяме земя (40 = 0 + 40)
						if (num < 40)
							field[x, y] = Item.Ground;
						// с 30% вероятност поставяме трева (70 = 40 + 30)
						else if (num < 70)
							field[x, y] = Item.Grass;
						// с 20% вероятност поставяме дърво (90 = 70 + 20)
						else if (num < 90)
							field[x, y] = Item.Tree;
						// с 10% вероятност поставяме дърво (100 = 90 + 10)
						else
							field[x, y] = Item.Stone;
					}
				}
			}

			return field;
		}

		/// <summary>
		/// Изрисува полето в горната лява част на екрана
		/// </summary>
		/// <param name="field">поле</param>
		static void DrawField(Item[,] field)
		{
			// записваме оригиналните цветове на конзолата
			ConsoleColor backColor = Console.BackgroundColor;
			ConsoleColor foreColor = Console.ForegroundColor;

			// сменяме фона на тъмно червен
			Console.BackgroundColor = ConsoleColor.DarkRed;

			// взимаме размерите на полето
			int m = field.GetLength(0);
			int n = field.GetLength(1);
			// обхождаме всеки ред от полето
			for (int y = 0; y < n; y++)
			{
				// за всеки ред от полето поставяме курсора в началото на съответния ред на конзолата
				Console.SetCursorPosition(0, y);

				// обхождаме всяка позиция от избрания ред
				for (int x = 0; x < m; x++)
				{
					// проверяваме какво има на позицията в полето
					switch (field[x, y])
					{
						// ако няма нищо...
						case Item.Empty:
							Console.Write(' ');
							break;
						// ако е "нашият човек"...
						case Item.OurGuy:
							Console.ForegroundColor = ConsoleColor.Blue;
							Console.Write('\u263A');
							break;
						// ако е "чужд човек"...
						case Item.SomeGuy:
							Console.ForegroundColor = ConsoleColor.Red;
							Console.Write('\u263B');
							break;
						// ако е диамант...
						case Item.Diamond:
							Console.ForegroundColor = ConsoleColor.Gray;
							Console.Write('\u2666');
							break;
						// ако е земя...
						case Item.Ground:
							Console.ForegroundColor = ConsoleColor.Cyan;
							Console.Write('\u2592');
							break;
						// ако е трева...
						case Item.Grass:
							Console.ForegroundColor = ConsoleColor.Green;
							Console.Write('\u2593');
							break;
						// ако е дърво...
						case Item.Tree:
							Console.ForegroundColor = ConsoleColor.DarkGreen;
							Console.Write('\u2663');
							break;
						// ако е камък...
						case Item.Stone:
							Console.ForegroundColor = ConsoleColor.DarkGray;
							Console.Write('\u0665');
							break;
						// в противен случай...
						default:
							Console.ForegroundColor = ConsoleColor.Red;
							Console.Write('?');
							break;
					}
				}
			}

			// възстановяваме основните цветове на конзолата
			Console.ForegroundColor = foreColor;
			Console.BackgroundColor = backColor;
		}

		/// <summary>
		/// Премества човек
		/// </summary>
		/// <param name="field">поле</param>
		/// <param name="x">хоризонтална позиция на човека</param>
		/// <param name="y">вертикална позиция на човека</param>
		/// <param name="direction">посока на преместване</param>
		/// <param name="newX">нова хоризонтална позиция на човека</param>
		/// <param name="newY">нова вертикална позиция на човека</param>
		/// <param name="hadDiamond">true, ако човекът е прибрал диамант, иначе - false</param>
		/// <returns>true, ако човекът се е преместил, иначе - false</returns>
		static bool MoveGuy(Item[,] field, int x, int y, Direction direction,
			out int newX, out int newY, out bool hadDiamond)
		{
			// взимаме размерите на полето
			int m = field.GetLength(0);
			int n = field.GetLength(1);

			// човекът още се намира на старата си позиция
			newX = x;
			newY = y;
			// човекът още не е взел диамант
			hadDiamond = false;

			// в зависимост от посоката...
			switch (direction)
			{
				// опитваме да преместим човека нагоре
				case Direction.Up:
					newY--;
					break;
				// опитваме да преместим човека надясно
				case Direction.Right:
					newX++;
					break;
				// опитваме да преместим човека надолу
				case Direction.Down:
					newY++;
					break;
				// опитваме да преместим човека наляво
				case Direction.Left:
					newX--;
					break;
				default:
					return false;
			}

			// проверяваме дали няма да излезе от полето
			if (newX < 0 || newX >= m)
				return false;
			if (newY < 0 || newY >= n)
				return false;

			// в зависимост от това, което има на новата позиция...
			switch (field[newX, newY])
			{
				// нищо
				case Item.Empty:
				// земя
				case Item.Ground:
				// трева
				case Item.Grass:
					// преместваме човека
					field[newX, newY] = field[x, y];
					// зад него остава просто земя
					field[x, y] = Item.Ground;
					// казваме, че сме го преместили
					return true;
				// диамант
				case Item.Diamond:
					// казваме, че е взел диамант
					hadDiamond = true;
					// преместваме човека
					field[newX, newY] = field[x, y];
					// зад него остава просто земя
					field[x, y] = Item.Ground;
					// казваме, че сме го преместили
					return true;
				// "наш човек"
				case Item.OurGuy:
				// "чужд човек"
				case Item.SomeGuy:
				// "дърво"
				case Item.Tree:
				// камък
				case Item.Stone:
				// нещо друго
				default:
					// не правим нищо и не местим човека
					return false;
			}
		}

		/// <summary>
		/// Премества всички "чужди хора"
		/// </summary>
		/// <param name="field">поле</param>
		/// <param name="diamondsLeft">брой останали диаманти</param>
		static void MoveSomeGuys(Item[,] field, ref int diamondsLeft)
		{
			// взимаме размерите на полето
			int m = field.GetLength(0);
			int n = field.GetLength(1);

			// създаваме генератор на произволни числа
			var random = new Random();

			// обхождаме всички позиции в полето
			for (int y = 0; y < n; y++)
			{
				for (int x = 0; x < m; x++)
				{
					// ако на съответната позиция има "чужд човек"
					if (field[x, y] == Item.SomeGuy)
					{
						// избираме му произволна посока
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

						// опитваме се да преместим човека
						int newX, newY;
						bool hadDiamond;
						MoveGuy(field, x, y, direction, out newX, out newY, out hadDiamond);
						// ако е взел диамант, намаляваме бройката на диамантите
						if (hadDiamond)
							diamondsLeft--;
					}
				}
			}
		}

		/// <summary>
		/// Премества "нашия човек"
		/// </summary>
		/// <param name="field">поле</param>
		/// <param name="x">хоризонтална позиция на "нашия човек"</param>
		/// <param name="y">вертикална позиция на "нашия човек"</param>
		/// <param name="direction">посока на преместване</param>
		/// <param name="diamondsCollected">брой събрани диаманти</param>
		/// <param name="diamondsLeft">брой останали диаманти</param>
		static void MoveOurGuy(Item[,] field, ref int x, ref int y, Direction direction, ref int diamondsCollected, ref int diamondsLeft)
		{
			// опитваме се да преместим човека
			int newX, newY;
			bool hadDiamond;
			if (MoveGuy(field, x, y, direction, out newX, out newY, out hadDiamond))
			{
				x = newX;
				y = newY;

				// ако е взел диамант, намаляваме бройката на диамантите
				// и увеличаваме тази на прибраните диаманти
				if (hadDiamond)
				{
					diamondsLeft--;
					diamondsCollected++;
				}
			}
		}

		/// <summary>
		/// Обработва потребителскиа вход
		/// </summary>
		/// <param name="field">поле</param>
		/// <param name="ourGuyX">хоризонтална позиция на "нашия човек"</param>
		/// <param name="ourGuyY">вертикална позиция на "нашия човек"</param>
		/// <param name="diamondsCollected">брой събрани диаманти</param>
		/// <param name="diamondsLeft">брой останали диаманти</param>
		/// <param name="escape">true, ако потребителят иска да излезе, иначе - false</param>
		/// <returns>true, ако е имало вход, иначе - false</returns>
		static bool ProcessInput(Item[,] field, ref int ourGuyX, ref int ourGuyY,
			ref int diamondsCollected, ref int diamondsLeft, out bool escape)
		{
			// по принцип потребителят не иска да излиза
			escape = false;

			// ако потребителят не е натиснат клавиш, се връщаме обратно
			if (!Console.KeyAvailable)
				return false;

			// взимаме информация за натиснатия клавиш
			ConsoleKeyInfo info = Console.ReadKey(true);

			// според натиснатия клавиш...
			switch (info.Key)
			{
				// стрелка нагоре
				case ConsoleKey.UpArrow:
					// преместваме "нашия човек" нагоре
					MoveOurGuy(field, ref ourGuyX, ref ourGuyY, Direction.Up, ref diamondsCollected, ref diamondsLeft);
					break;
				// стрелка надясно
				case ConsoleKey.RightArrow:
					// преместваме "нашия човек" надясно
					MoveOurGuy(field, ref ourGuyX, ref ourGuyY, Direction.Right, ref diamondsCollected, ref diamondsLeft);
					break;
				// стрелка надолу
				case ConsoleKey.DownArrow:
					// преместваме "нашия човек" надолу
					MoveOurGuy(field, ref ourGuyX, ref ourGuyY, Direction.Down, ref diamondsCollected, ref diamondsLeft);
					break;
				// стрелка наляво
				case ConsoleKey.LeftArrow:
					// преместваме "нашия човек" наляво
					MoveOurGuy(field, ref ourGuyX, ref ourGuyY, Direction.Left, ref diamondsCollected, ref diamondsLeft);
					break;
				// escape
				case ConsoleKey.Escape:
					// потребителят иска да излезе
					escape = true;
					break;
			}

			return true;
		}

		static void Main(string[] args)
		{
			// максималните размери на полето ще бъдат малко по-малки от максималните размери на прозореца
			int maxX = Console.WindowWidth - 1;
			int maxY = Console.WindowHeight - 1;

			// взимаме размерите на полето
			int m, n;
			if (!GetFieldBounds(maxX, maxY, out m, out n))
				// ако потребителят е отказал да въведе размер на полето, си тръгваме
				return;

			// създаваме полето и взимаме информация за него
			int ourGuyX, ourGuyY, diamondsTotal;
			Item[,] field = CreateField(m, n, out ourGuyX, out ourGuyY, out diamondsTotal);

			// изчистваме екрана
			Console.Clear();

			// първоначално потребителят не иска да излиза
			bool escape = false;
			// първоначално имаме пълен брой диаманти
			int diamondsLeft = diamondsTotal;
			// първоначално нямаме събрани диаманти
			int diamondsCollected = 0;
			// докато потребителят не иска да излезе и не са събрани всички диаманти
			while (!escape && diamondsLeft > 0)
			{
				// изрисуваме полето
				DrawField(field);
				// преместваме всички "чужди хора"
				MoveSomeGuys(field, ref diamondsLeft);
				// обработваме входа от потребителя
				ProcessInput(field, ref ourGuyX, ref ourGuyY, ref diamondsCollected, ref diamondsLeft, out escape);
				// изчакваме малко
				Thread.Sleep(100);
			}

			// извеждаме статистика
			Console.WriteLine();
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
