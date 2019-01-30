using System;
using System.Collections;
using System.Text;

namespace DES {
	public static class Calculus {
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

		private static int GetCharPosition(char ch) {
			byte[] pos = cp866.GetBytes(Convert.ToString(ch));
			return pos[0];
		}
		
		private static BitArray Reverse(BitArray array)
		{
			int length = array.Length;
			int mid = (length / 2);
			for (int i = 0; i < mid; i++)
			{
				bool bit = array[i];
				array[i] = array[length - i - 1];
				array[length - i - 1] = bit;
			}

			return array;
		}
		private static BitArray InitInput(int value) {
			BitArray bitArray = NumberToBitArray(value, 8);
			bitArray = Shaffle(bitArray, IP);
			return bitArray;
		}

		private static BitArray CutBitArray(BitArray array, int numberOfBits) {
			BitArray tempArray = new BitArray(numberOfBits);
			for (int i = 0; i < numberOfBits; i++) {
				tempArray.Set(i,array[i]);
			}
			return tempArray;
		}
		private static BitArray NumberToBitArray(int number, int numberOfBits) {
			int[] numberInBytes = {number};
			BitArray numberInBits = new BitArray(numberInBytes);
			numberInBits = CutBitArray(numberInBits, numberOfBits);
			numberInBits = Reverse(numberInBits);
			return numberInBits;
		}
		
		private static BitArray[] SliceBitArray(BitArray numberInBits, int length) {
			BitArray left = new BitArray(length, false);
			BitArray right = new BitArray(length, false);
			for (int i = 0; i < numberInBits.Length; i++) {
				if (i < length) {
					left.Set(i, numberInBits[i]);
				}
				else {
					right.Set(i-length, numberInBits[i]);
				}
			}
			BitArray[] bitArrays = {left, right};	
			return bitArrays;
		}

		private static BitArray UniteArrays(BitArray left, BitArray right) {
			BitArray united = new BitArray(left.Length*2);
			for (int i = 0; i < united.Length; i++) {
				if (i < left.Length) {
					united.Set(i, left[i]);
				}
				else {
					united.Set(i, right[i - left.Length]);
				}
			}
			return united;
		}

		private static BitArray InitKey(int key) {
			BitArray keyInBits = NumberToBitArray(key, 10);
			return Shaffle(keyInBits, P10);
		}
		private static BitArray KeyGen(BitArray keyArray, int shift) {
			BitArray[] temp = SliceBitArray(keyArray,5);
			BitArray left = temp[0];
			BitArray right = temp[1];
			left = Shift(left, shift);
			right = Shift(right, shift);
			return Shaffle(UniteArrays(left,right),P8);
		}
		
		private static BitArray Shaffle(BitArray key, int[] pBlock) {
			BitArray temp = new BitArray(pBlock.Length);
			for (int i = 0; i < pBlock.Length; i++) {
				temp[i] = key[pBlock[i] - 1];
			}
			return temp;
		}

		private static BitArray Shift(BitArray array, int value) {
			for (int i = 0; i < value; i++) {
				bool temp = array[0];
				for (int j = array.Length - 1; j > 0; j--) {
					array[j] = array[j - 1];
				}
				array[array.Length - 1] = temp;
			}
			return array;
		}

		private static BitArray Substitute(BitArray bitArray, int[,] sBlock) {
			BitArray first = new BitArray(2);
			first.Set(0, bitArray[0]);
			first.Set(1, bitArray[3]);
			BitArray second = new BitArray(2);
			second.Set(0, bitArray[1]);
			second.Set(0, bitArray[2]);
			int[] comparison = new int[2];
			first.CopyTo(comparison, 0);
			second.CopyTo(comparison, 1);
			int[] temp = {sBlock[comparison[0], comparison[1]]};
			BitArray output = new BitArray(temp);
			output = CutBitArray(output, 2);
			output = Reverse(output);
			return output;
		}
		
		//методы с порядком выполнения операций и проверок сразу после нажатия кнопки
		public static char Encrypt(char input, int key) {
			int value = GetCharPosition(input);
			BitArray initKey = InitKey(key);
			BitArray[] initInput = SliceBitArray(InitInput(value), 4);
			BitArray left = initInput[0];
			BitArray right = initInput[1];
			BitArray result = new BitArray(8);
			
			for (int i = 0; i < 2; i++) {
				BitArray roundKey = KeyGen(initKey, i+1);
				BitArray extRight = Shaffle(right, EP);
				extRight = extRight.Xor(roundKey);
				BitArray[] temp = SliceBitArray(extRight, 4);
				BitArray toS0 = temp[0];
				BitArray toS1 = temp[1];
				toS0 = Substitute(toS0, S0);
				toS1 = Substitute(toS1, S1);
				BitArray output = UniteArrays(toS0, toS1);
				output = Shaffle(output, P4);
				output = output.Xor(left);
				if (i == 0) {
					left = right;
					right = output;
				}
				else {
					result = UniteArrays(output, right);
					result = Shaffle(result, BIP);
				}
			}
			int[] final = new int[1];
			result.CopyTo(final, 0);
			char[] ch = cp866.GetChars(new [] {(byte)final[0]});
			return ch[0];
		}
		public static string Decrypt(string text, string key) {
			return null;
		}
	}
}
