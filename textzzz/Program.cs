using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AesEncryption
{
    public static (string encryptedText, byte[] key, byte[] iv) Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.GenerateKey();
            aes.GenerateIV();

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    return (Convert.ToBase64String(ms.ToArray()), aes.Key, aes.IV);
                }
            }
        }
    }

    public static string Decrypt(string cipherText, byte[] key, byte[] iv)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Welcome to TextZZZ - Encryption/Decryption Tool!");
        Console.WriteLine("                  by vlaqqxz");
        while (true)
        {
            Console.WriteLine("\nDo you want to encrypt or decrypt? (e/d) or 'q' to quit:");

            string choice = Console.ReadLine().ToLower();

            if (choice == "e")
            {
                Console.WriteLine("Enter the phrase to encrypt:");
                string phraseToEncrypt = Console.ReadLine();
                var (encrypted, key, iv) = AesEncryption.Encrypt(phraseToEncrypt);
                Console.WriteLine($"Encrypted phrase: {encrypted}");
                Console.WriteLine($"Key (Base64): {Convert.ToBase64String(key)}");
                Console.WriteLine($"IV (Base64): {Convert.ToBase64String(iv)}");
            }
            else if (choice == "d")
            {
                Console.WriteLine("Enter the phrase to decrypt:");
                string phraseToDecrypt = Console.ReadLine();
                Console.WriteLine("Enter the Base64-encoded key:");
                string keyInput = Console.ReadLine();
                Console.WriteLine("Enter the Base64-encoded IV:");
                string ivInput = Console.ReadLine();

                try
                {
                    byte[] key = Convert.FromBase64String(keyInput);
                    byte[] iv = Convert.FromBase64String(ivInput);
                    string decrypted = AesEncryption.Decrypt(phraseToDecrypt, key, iv);
                    Console.WriteLine($"Decrypted phrase: {decrypted}");
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input for decryption. Please ensure you are using valid Base64 strings for the key and IV.");
                }
                catch (CryptographicException)
                {
                    Console.WriteLine("Decryption failed. The key or IV may be incorrect.");
                }
            }
            else if (choice == "q")
            {
                Console.WriteLine("Exiting TextZZZ. Goodbye!");
                break;
            }
            else
            {
                Console.WriteLine("Invalid choice. Please enter 'e' for encrypt, 'd' for decrypt, or 'q' to quit.");
            }
        }
    }
}