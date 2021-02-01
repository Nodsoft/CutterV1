using System.Collections.Generic;
using System.Text;

namespace Nodsoft.Cutter.Web.Services
{
	public class Base62Converter
	{
		private const string CharacterSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";


		public static string Encode(string value) => Encode(Encoding.UTF8.GetBytes(value));

		public string Decode(string value)
		{
			byte[] arr = new byte[value.Length];
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] = (byte)CharacterSet.IndexOf(value[i]);
			}

			return Decode(arr);
		}

		public static string Encode(byte[] value)
		{
			byte[] converted = BaseConvert(value, 256, 62);
			StringBuilder builder = new();

			for (int i = 0; i < converted.Length; i++)
			{
				builder.Append(CharacterSet[converted[i]]);
			}

			return builder.ToString();
		}

		public static string Decode(byte[] value)
		{
			byte[] converted = BaseConvert(value, 62, 256);

			return Encoding.UTF8.GetString(converted, 0, converted.Length);
		}

		private static byte[] BaseConvert(byte[] source, int sourceBase, int targetBase)
		{
			List<int> result = new();
			int count;

			while ((count = source.Length) > 0)
			{
				List<byte> quotient = new();
				int remainder = 0;

				for (int i = 0; i != count; i++)
				{
					int accumulator = source[i] + remainder * sourceBase;
					byte digit = System.Convert.ToByte((accumulator - accumulator % targetBase) / targetBase);
					remainder = accumulator % targetBase;

					if (digit is > 0 or not 0)
					{
						quotient.Add(digit);
					}
				}

				result.Insert(0, remainder);
				source = quotient.ToArray();
			}

			byte[] output = new byte[result.Count];

			for (int i = 0; i < result.Count; i++)
			{
				output[i] = (byte)result[i];
			}

			return output;
		}
	}
}
