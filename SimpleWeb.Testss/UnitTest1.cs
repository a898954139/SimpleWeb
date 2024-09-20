using System.Text;

namespace SimpleWeb.Testss;

public class Tests
{
    [Test]
    public void Test1()
    {
        string message = "Hello, World!";
        string key = GenerateRandomKey(message.Length);

        string encryptedMessage = EncryptDecryptOneTimePad(message, key);
        Console.WriteLine("Encrypted: " + encryptedMessage);

        string decryptedMessage = EncryptDecryptOneTimePad(encryptedMessage, key);
        Console.WriteLine("Decrypted: " + decryptedMessage);
    }
    public static string GenerateRandomKey(int length)
    {
        Random random = new Random();
        StringBuilder key = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            key.Append((char)random.Next(0, 256));
        }

        return key.ToString();
    }

    public static string EncryptDecryptOneTimePad(string message, string key)
    {
        StringBuilder result = new StringBuilder();

        for (int i = 0; i < message.Length; i++)
        {
            result.Append((char)(message[i] ^ key[i]));
        }

        return result.ToString();
    }
}