using System;
using System.Security.Cryptography;
using System.IO;


public class AESEncrypt
{
    private readonly byte[] _key = { 11, 176, 127, 81, 211, 67, 243, 175, 227, 137, 44, 159, 235, 146, 149, 168, 213, 205, 27, 192, 99, 58, 78, 231, 119, 81, 138, 116, 76, 211, 242, 67 };
    private readonly byte[] _vector = { 174, 152, 100, 60, 9, 213, 129, 173, 109, 212, 196, 230, 91, 131, 212, 96 };


    private readonly ICryptoTransform _encryptorTransform, _decriptorTransform;
    private readonly System.Text.UTF8Encoding _utfEncoder;

    public AESEncrypt()
    {
        // This is the encryption method
        RijndaelManaged rm = new RijndaelManaged();
        // Create an encryptor and a decryptor using our encryption method, key, and vector.
        _encryptorTransform = rm.CreateEncryptor(_key, _vector);
        _decriptorTransform = rm.CreateDecryptor(_key, _vector);
        // Used to translate bytes to text and vice versa
        _utfEncoder = new System.Text.UTF8Encoding();
    }

    
    /// <summary>
    /// Encrypt text
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public byte[] Encrypt(string text)
    {
        // Translates our text value into a byte array.
        byte[] bytes = _utfEncoder.GetBytes(text);
        // Used to stream the data in and out of the CryptoStream.
        MemoryStream memoryStream = new MemoryStream();
        CryptoStream cs = new CryptoStream(memoryStream, _encryptorTransform, CryptoStreamMode.Write);
        cs.Write(bytes, 0, bytes.Length);
        cs.FlushFinalBlock();
        memoryStream.Position = 0;
        byte[] encrypted = new byte[memoryStream.Length];
        memoryStream.Read(encrypted, 0, encrypted.Length);
        //Clean up.
        cs.Close();
        memoryStream.Close();
        return encrypted;
    }
    
    /// <summary>
    /// Decrypt the value
    /// </summary>
    /// <param name="encryptedValue"></param>
    /// <returns></returns>
    public string Decrypt(byte[] encryptedValue)
    {
        MemoryStream encryptedStream = new MemoryStream();
        CryptoStream decryptStream = new CryptoStream(encryptedStream, _decriptorTransform, CryptoStreamMode.Write);
        decryptStream.Write(encryptedValue, 0, encryptedValue.Length);
        decryptStream.FlushFinalBlock();
        encryptedStream.Position = 0;
        byte[] decryptedBytes = new byte[encryptedStream.Length];
        encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
        encryptedStream.Close();
        return _utfEncoder.GetString(decryptedBytes);
    }
    
}