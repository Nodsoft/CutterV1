namespace Nodsoft.Cutter.Web.Services;

public static class Base62Generator
{
	private const string CharacterSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
	private static readonly Random Random = new();

	public static string GenerateString(int length)
	{
		Span<char> arr = stackalloc char[length];

		for (int i = 0; i < length; i++)
		{
			arr[i] = CharacterSet[Random.Next(CharacterSet.Length)];
		}

		return new(arr);
	}
}