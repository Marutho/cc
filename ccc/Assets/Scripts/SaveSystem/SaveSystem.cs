using System;
using System.IO;
using System.Threading;
using UnityEngine;

/// <summary>
/// Save system is a class that MUST be included inside the scene you're saving/loading data.
///
/// Sample use in the bottom of the file.
///
/// </summary>
public class SaveSystem : MonoBehaviour

{
	/// <summary>
	/// Informer is a util class which serves the purpose of informing when are the threads done loading or saving data.
	/// Do not change loaded and data, just try to read them to check if the loading is already completed.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Informer<T>
	{
		public bool loaded;
		public T data;
		public bool error;
	}
	
	#region Private Variables
	private static readonly AESEncrypt EncryptionTool = new AESEncrypt();
	private static string _persistentDataPath;
	#endregion

	#region Public Variables

	#endregion

	#region Properties

	public static string PersistentDataPath
	{
		set { _persistentDataPath = value; }
		get => _persistentDataPath;
	}
	
	#endregion

	#region MonoBehaviour

	private void Awake()
	{
		_persistentDataPath = Application.persistentDataPath;
		if (!Directory.Exists(_persistentDataPath))
		{
			Directory.CreateDirectory(_persistentDataPath);
		}
	}

	#endregion

    #region Public Methods
    

    public static Informer<T> LoadOnAnotherThread<T>(string path)
    {
	    _persistentDataPath = Application.persistentDataPath;
	    Informer<T> informer = new Informer<T>();
	    Thread thread = new Thread(delegate() { Load(path, informer); });
	    thread.Start();
	    return informer;
    }

    /// <summary>
    /// Saves in another thread the data.
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Informer which will fill the boolean error if there has been an error and will tell when this is finished</returns>
    public static Informer<T> SaveOnAnotherThread<T>(string path, T data)
    {
	    _persistentDataPath = Application.persistentDataPath;
	    Informer<T> informer = new Informer<T> {data = data};
	    Thread thread = new Thread(delegate()
	    {
		    bool ok = Save(path, data);
		    informer.data = data;
		    informer.error = !ok;
		    informer.loaded = true;
	    });
	    thread.Start();
	    return informer;
    }

    /// <summary>
    /// Saves data into path.
    /// Highly recommended to use SaveInAnotherThread method for better performance.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="system">The system must be System.Serializable</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>If successful</returns>
    public static bool Save<T>(string path, T system)
    {
	    // NOTE (GABI): Maybe specify that T should be a class where T : class
	    try
	    {
		    path = $"{_persistentDataPath}/{path}.json";
		    string s = JsonUtility.ToJson(system);
		    if (!File.Exists(path))
		    {
			    File.Create(path).Close();
		    }
		    byte[] encryptedSystem = EncryptionTool.Encrypt(s);
		    string hex = Hex.ByteArrayToHexViaLookup32(encryptedSystem);
		    StreamWriter writer = new StreamWriter(path);
		    writer.Write(hex);
		    writer.Flush();
		    writer.Close();
		    Debug.Log($"Saved into {path}");
		    return true;
	    }
	    catch (Exception exception)
	    {
			Debug.LogError($"Error parsing system data {system} because of exception: {exception.Message}");
	    }
	    
	    return false;
    }
    
    private static void Load<T>(string path, Informer<T> information)
    {
	    try
	    {
		    T data = Load<T>(path);
		    information.data = data;
		    information.loaded = true;
	    }
	    catch (Exception err)
	    {
		    Debug.LogWarning($"Error loading file in another thread: {err.Message}");
		    information.error = true;
	    }
    }

    /// <summary>
    /// Loads the specified system from the file system.
    /// Might throw an exception if things go wrong.
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Load<T>(string path)
    {
	    path = $"{_persistentDataPath}/{path}.json";
	    StreamReader reader = new StreamReader(path);
	    string data = reader.ReadToEnd();
	    reader.Close();
	    byte[] encryptedData = Hex.HexadecimalStringToByteArray(data);
	    string decryptedData = EncryptionTool.Decrypt(encryptedData);
	    T system = JsonUtility.FromJson<T>(decryptedData);
	    return system;
    }

    /// <summary>
    /// Check if file exists.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool Exists(string path)
    {
	    path = $"{_persistentDataPath}/{path}.json";
	    return File.Exists(path);
    }
    
    #endregion

    #region Private Methods

    #endregion
}

/**
 *
 *  Sample use
 * 
	Informer<TestData> data = null;
	private Informer<TestData> saveInformer = null;

	private void Start()
	{
		saveInformer = SaveOnAnotherThread("data", new TestData());
	}

	private void Update()
	{
		if (saveInformer != null && saveInformer.loaded)
		{
			data = LoadOnAnotherThread<TestData>("data");
			saveInformer = null;
		}
		
		if (data != null && data.loaded)
		{
			Debug.Log(data.data.data);
			Debug.Log(data.data.moreData);
			Debug.Log(data.data.datas);
			data = null;
		}
	}
 */
