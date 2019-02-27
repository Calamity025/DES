using System;
using System.Collections;
using System.Text;

namespace DES {
	public static class Calculus {
		//всякие кодировки, блоки перестановок, подстановок и тд
		private static readonly Encoding cp866 = Encoding.GetEncoding(866);
		private static int[] P10 = {3, 5, 2, 7, 4, 10, 1, 9, 8, 6};
		private static int[] P8 = {6, 3, 7, 4, 8, 5, 10, 9};
		private static int[] IP = {2, 6, 3, 1, 4, 8, 5, 7};
		private static int[] BIP = {4, 1, 3, 5, 7, 2, 8, 6};
		private static int[] EP = {4, 1, 2, 3, 2, 3, 4, 1};
		private static int[] P4 = {2, 4, 3, 1};
		private static int[,] S0 = {
			{1, 0, 3, 2},
			{3, 2, 1, 0},
			{0, 2, 1, 3},
			{3, 1, 3, 1}
		};
		private static int[,] S1 = {
			{1, 1, 2, 3},
			{2, 0, 1, 3},
			{3, 0, 1, 0},
			{2, 1, 0, 3}
		};
		//раундовые ключи тут, потому что я могу
		private static BitArray[] roundKeys = new BitArray[2];
		//функция определения позиции символа в кодировке ср866
		private static int GetCharPosition(char ch) {
			byte[] pos = cp866.GetBytes(Convert.ToString(ch));
			return pos[0];
		}
		//функция оборота битового массива, потому что по дефолту младшие биты в нашем понимании тут - старшие и vise versa
		private static void Reverse(ref BitArray array)
		{
			int length = array.Length;
			int mid = (length / 2);
			for (int i = 0; i < mid; i++)
			{
				bool bit = array[i];
				array[i] = array[length - i - 1];
				array[length - i - 1] = bit;
			}
		}
		//инициализация символа (превращение из числа в битовый массив + блок перестановки)
		private static BitArray InitInput(int value, int[] pBlock) {
			BitArray bitArray = NumberToBitArray(value, 8);
			Shaffle(ref bitArray, pBlock);
			return bitArray;
		}
		//функция "резания" массива. По дефолту битовый массив для инта создает 32 бита, но нам так много не надо,
		//а что бы нормально обрезать его до нужных нам 8ми битов и используем эту функцию
		//Если юзать превращение в массив битов через byte то у нас будет 8 бит, а для ключа нужно уже 10
		private static BitArray CutBitArray(BitArray array, int numberOfBits) {
			BitArray tempArray = new BitArray(numberOfBits);
			for (int i = 0; i < numberOfBits; i++) {
				tempArray.Set(i, array[i]);
			}
			return tempArray;
		}
		//превращаем число в битовый массив с помощью встроенной функции (самого понимания и применения битового массива)
		private static BitArray NumberToBitArray(int number, int numberOfBits) {
			BitArray numberInBits = new BitArray(new [] {number});
			numberInBits = CutBitArray(numberInBits, numberOfBits);
			Reverse(ref numberInBits);
			return numberInBits;
		}
		//делим один массив на 2 заданной длины
		private static void SliceBitArray(BitArray numberInBits, int length, out BitArray left, out BitArray right) {
			left = new BitArray(length, false);
			right = new BitArray(length, false);
			for (int i = 0; i < numberInBits.Length; i++) {
				if (i < length) {
					left.Set(i, numberInBits[i]);
				}
				else {
					right.Set(i-length, numberInBits[i]);
				}
			}
		}
		//объединяем массивы
		private static BitArray UniteArrays(BitArray left, BitArray right) {
			BitArray unitedArray = new BitArray(left.Length*2);
			for (int i = 0; i < unitedArray.Length; i++) {
				if (i < left.Length) {
					unitedArray.Set(i, left[i]);
				}
				else {
					unitedArray.Set(i, right[i - left.Length]);
				}
			}

			return unitedArray;
		}
		//инициилизируем сразу оба раундовых ключа
		private static void InitKeys(int key) {
			BitArray keyInBits = NumberToBitArray(key, 10);
			Shaffle(ref keyInBits, P10);
			SliceBitArray(keyInBits,5, out BitArray left, out BitArray right);
			for (int i = 0; i < 2; i++) {
				Shift(ref left, i + 1);
				Shift(ref right, i+1);
				BitArray roundKey = UniteArrays(left, right);
				Shaffle(ref roundKey, P8);
				roundKeys[i] = roundKey;
			}
		}
		//фунция перестановки заданого значения и блока
		private static void Shaffle(ref BitArray value, int[] pBlock) {
			BitArray temp = new BitArray(pBlock.Length);
			for (int i = 0; i < pBlock.Length; i++) {
				temp[i] = value[pBlock[i] - 1];
			}
			value = temp;
		}
		private static BitArray Shaffle(BitArray value, int[] pBlock) {
			BitArray temp = new BitArray(pBlock.Length);
			for (int i = 0; i < pBlock.Length; i++) {
				temp[i] = value[pBlock[i] - 1];
			}
			return temp;
		}
		//битовый сдвиг
		//стандартный битовый сдвиг не работает с этим типом данных
		//вопрос в том, что проще: своя функция или конвертирование в другой тип данных? :hmmm: 
		private static void Shift(ref BitArray array, int value) {
			for (int i = 0; i < value; i++) {
				bool temp = array[0];
				for (int j = 1; j < array.Length; j++) {
					array[j - 1] = array[j];
				}
				array[array.Length - 1] = temp;
			}
		}
		//подстановка заданого значения по заданому с-блоку 
		private static void Substitute(ref BitArray bitArray, int[,] sBlock) {
			BitArray first = new BitArray(2);
			first.Set(0, bitArray[0]);
			first.Set(1, bitArray[3]);
			BitArray second = new BitArray(2);
			second.Set(0, bitArray[1]);
			second.Set(1, bitArray[2]);
			int[] comparison = new int[2];
			first.CopyTo(comparison, 0);
			second.CopyTo(comparison, 1);
			int[] temp = {sBlock[comparison[0], comparison[1]]};
			bitArray = new BitArray(temp);
			bitArray = CutBitArray(bitArray, 2);
			Reverse(ref bitArray);
		}

		public static char Encrypt(char input, int key) {
			//инициализация всего что нужно вначале инициализировать
			int value = GetCharPosition(input);
			InitKeys(key);
			SliceBitArray(InitInput(value, IP), 4, out BitArray left, out BitArray right);
			BitArray result = new BitArray(8);
			//раунды
			for (int i = 0; i < 2; i++) {
				BitArray extRight = Shaffle(right, EP); //расширение текста
				extRight = extRight.Xor(roundKeys[i]); //ксор с ключом
				SliceBitArray(extRight, 4, out BitArray toS0, out BitArray toS1); 
				Substitute(ref toS0, S0);
				Substitute(ref toS1, S1);
				BitArray output = UniteArrays(toS0, toS1);
				Shaffle(ref output, P4);
				output = output.Xor(left);
				if (i == 0) { //если первый раунд то меняем местами
					left = right;
					right = output;
				}
				else { //если нет, то делаем конечную перестановку
					result = UniteArrays(output, right);
					result = Shaffle(result, BIP);
				}
			}
			//страшная форма записи, которая по простому переводит битовый массив в число, а потом в символ
			int[] final = new int[1];
			result.CopyTo(final, 0);
			char[] ch = cp866.GetChars(new [] {(byte)final[0]});
			return ch[0];
		}

		public static char Decrypt(char input, int key) { //тут все так же как в зашифровке
			int value = GetCharPosition(input);
			InitKeys(key);
			SliceBitArray(InitInput(value, BIP), 4, out BitArray left, out BitArray right);
			BitArray result = new BitArray(8);

			for (int i = 1; i >= 0; i--) {
				BitArray extRight = Shaffle(right, EP);
				extRight = extRight.Xor(roundKeys[i]);
				SliceBitArray(extRight, 4, out BitArray toS0, out BitArray toS1);
				Substitute(ref toS0, S0);
				Substitute(ref toS1, S1);
				BitArray output = UniteArrays(toS0, toS1);
				Shaffle(ref output, P4);
				output = output.Xor(left);
				if (i == 1) {
					left = right;
					right = output;
				}
				else {
					result = UniteArrays(output, right);
					Shaffle(ref result, IP);
					Reverse(ref result);
				}
			}

			//страшная форма записи, которая по простому переводит битовый массив в число, а потом в символ
			int[] final = new int[1];
			result.CopyTo(final, 0);
			char[] ch = cp866.GetChars(new[] {(byte) final[0]});
			return ch[0];
		}
	}
}