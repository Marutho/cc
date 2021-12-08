using System;
using UnityEngine;

/// <summary>
/// Hex is a util class for hexadecimal operations
/// </summary>
public static class Hex 
{
	private static readonly uint[] Lookup32 = CreateLookup32();

	private static uint[] CreateLookup32()
	{
		uint[] result = new uint[256];
		for (int i = 0; i < 256; i++)
		{
			string s = i.ToString("X2");
			result[i] = s[0] + ((uint)s[1] << 16);
		}
		return result;
	}
	
	/// <summary>
	/// Super fast function which does not compute the tuple bytes hex value because it was already computed at the beginning.
	/// </summary>
	/// <param name="bytes"></param>
	/// <returns></returns>
	public static string ByteArrayToHexViaLookup32(byte[] bytes)
	{
		uint[] lookup32 = Lookup32;
		char[] result = new char[bytes.Length * 2];
		for (int i = 0; i < bytes.Length; i++)
		{
			uint val = lookup32[bytes[i]];
			result[2 * i] = (char)val;
			result[2 * i + 1] = (char) (val >> 16);
		}
		return new string(result);
	}
	
	public static byte[] HexadecimalStringToByteArray(string input)
	{
		if (input.Length % 2 != 0) throw new Exception("Input length must be of even length");
		int outputLength = input.Length / 2;
		byte[] output = new byte[outputLength];
		char[] numeral = new char[2];
		for (int i = 0; i < outputLength; i++)
		{
			input.CopyTo(i * 2, numeral, 0, 2);
			output[i] = Convert.ToByte(new string(numeral), 16);
		}
		return output;
	}
}
