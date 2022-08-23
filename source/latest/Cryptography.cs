using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Cryptography : MonoBehaviour
{
	private static byte[] _salt = Encoding.ASCII.GetBytes("o6806642kbM7c5");

	private static string _sharedKey = "13mth3k3yt0us3";

	private void Start()
	{
	}

	private void Update()
	{
	}

	public static string Encrypt(string pDataToEncrypt)
	{
		return Encrypt(pDataToEncrypt, _sharedKey);
	}

	public static string Encrypt(string pDataToEncrypt, string pSharedKey)
	{
		if (string.IsNullOrEmpty(pDataToEncrypt))
		{
			throw new ArgumentNullException("pDataToEncrypt");
		}
		if (string.IsNullOrEmpty(pSharedKey))
		{
			throw new ArgumentNullException("pSharedKey");
		}
		string text = null;
		SymmetricAlgorithm symmetricAlgorithm = null;
		try
		{
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(pSharedKey, _salt);
			symmetricAlgorithm = Aes.Create();
			symmetricAlgorithm.Key = rfc2898DeriveBytes.GetBytes(symmetricAlgorithm.KeySize / 8);
			ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor(symmetricAlgorithm.Key, symmetricAlgorithm.IV);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.Write(BitConverter.GetBytes(symmetricAlgorithm.IV.Length), 0, 4);
				memoryStream.Write(symmetricAlgorithm.IV, 0, symmetricAlgorithm.IV.Length);
				using (CryptoStream stream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
				{
					using (StreamWriter streamWriter = new StreamWriter(stream))
					{
						streamWriter.Write(pDataToEncrypt);
					}
				}
				return Convert.ToBase64String(memoryStream.ToArray());
			}
		}
		finally
		{
			if (symmetricAlgorithm != null)
			{
				symmetricAlgorithm.Clear();
			}
		}
	}

	public static string Decrypt(string pDataToEncrypt)
	{
		return Decrypt(pDataToEncrypt, _sharedKey);
	}

	public static string Decrypt(string pDataToDecrypt, string pSharedKey)
	{
		if (string.IsNullOrEmpty(pDataToDecrypt))
		{
			throw new ArgumentNullException("pDataToDecrypt");
		}
		if (string.IsNullOrEmpty(pSharedKey))
		{
			throw new ArgumentNullException("pSharedKey");
		}
		SymmetricAlgorithm symmetricAlgorithm = null;
		string text = null;
		try
		{
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(pSharedKey, _salt);
			byte[] buffer = Convert.FromBase64String(pDataToDecrypt);
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				symmetricAlgorithm = Aes.Create();
				symmetricAlgorithm.Key = rfc2898DeriveBytes.GetBytes(symmetricAlgorithm.KeySize / 8);
				symmetricAlgorithm.IV = ReadByteArray(memoryStream);
				ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor(symmetricAlgorithm.Key, symmetricAlgorithm.IV);
				using (CryptoStream stream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
				{
					using (StreamReader streamReader = new StreamReader(stream))
					{
						return streamReader.ReadToEnd();
					}
				}
			}
		}
		finally
		{
			if (symmetricAlgorithm != null)
			{
				symmetricAlgorithm.Clear();
			}
		}
	}

	private static byte[] ReadByteArray(Stream pStream)
	{
		byte[] array = new byte[4];
		if (pStream.Read(array, 0, array.Length) != array.Length)
		{
			throw new SystemException("Stream did not contain properly formatted byte array");
		}
		byte[] array2 = new byte[BitConverter.ToInt32(array, 0)];
		if (pStream.Read(array2, 0, array2.Length) != array2.Length)
		{
			throw new SystemException("Did not read byte array properly");
		}
		return array2;
	}
}
