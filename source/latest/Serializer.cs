using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

public class Serializer
{
	public static void Serialize<T>(string pPath, T pObject)
	{
		BinarySerialize(pPath, pObject);
	}

	public static T Deserialize<T>(string pPath)
	{
		return BinaryDeserialize<T>(pPath);
	}

	public static void BinarySerialize<T>(string pPath, T pObject)
	{
		Stream stream = null;
		try
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			stream = File.OpenWrite(pPath);
			binaryFormatter.Serialize(stream, pObject);
		}
		catch (Exception)
		{
		}
		finally
		{
			if (stream != null)
			{
				stream.Close();
			}
		}
	}

	public static T BinaryDeserialize<T>(string pPath)
	{
		Stream stream = null;
		T result = default(T);
		try
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			stream = File.OpenRead(pPath);
			result = (T)binaryFormatter.Deserialize(stream);
			return result;
		}
		catch (Exception)
		{
			return result;
		}
		finally
		{
			if (stream != null)
			{
				stream.Close();
			}
		}
	}

	public static void XMLSerialize<T>(string pPath, T pObject, Type[] pIncludeTypes)
	{
		Stream stream = null;
		try
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), pIncludeTypes);
			stream = File.Open(pPath, FileMode.Create);
			xmlSerializer.Serialize(stream, pObject);
		}
		catch (Exception)
		{
		}
		finally
		{
			if (stream != null)
			{
				stream.Close();
			}
		}
	}

	public static T XMLDeserialize<T>(string pPath, Type[] pIncludeTypes)
	{
		Stream stream = null;
		T result = default(T);
		try
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), pIncludeTypes);
			stream = File.OpenRead(pPath);
			result = (T)xmlSerializer.Deserialize(stream);
			return result;
		}
		catch (Exception)
		{
			return result;
		}
		finally
		{
			if (stream != null)
			{
				stream.Close();
			}
		}
	}
}
