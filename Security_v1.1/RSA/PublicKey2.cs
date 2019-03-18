using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PublicKey2
{
	class CipherMessage
	{
		public byte[] cipherBytes;  // RC2 encrypted message text
		public byte[] tdesKey;       // RSA encrypted rc2 key
		public byte[] tdesIV;        // RC2 initialization vector
	}

	class Person
	{
		private RSACryptoServiceProvider rsa;
		private TripleDESCryptoServiceProvider tdes;
		private string name;

		// Maximum key size for the tdes algorithm
		const int keySize = 192;

		// Person constructor
		public Person(string p_Name)
		{
			rsa = new RSACryptoServiceProvider();
			Console.WriteLine(rsa.KeySize.ToString());
			Console.WriteLine(rsa.ToXmlString(true));

			tdes = new TripleDESCryptoServiceProvider();
			tdes.KeySize = keySize;
			
			name = p_Name;
		}

		// Used to send the rsa public key parameters
		public RSAParameters SendPublicKey()
		{
			RSAParameters result = new RSAParameters();
			try
			{
				result = rsa.ExportParameters(false);

				// 공개키를 내보낼때 개인키를 함께 내보내면 받는 쪽에서 메시지 복호화가 가능해진다.
				//result = rsa.ExportParameters(true);
			}
			catch (CryptographicException e)
			{
				Console.WriteLine(e.Message);
			}
			return result;
		}

		// Used to import the rsa public key parameters
		public void GetPublicKey(Person receiver)
		{
			try 
			{
				rsa.ImportParameters(receiver.SendPublicKey()); 
			}
			catch (CryptographicException e)
			{
				Console.WriteLine(e.Message);
			}
		}

		public CipherMessage EncryptMessage(string text)
		{
			// Convert string to a byte array
			CipherMessage message = new CipherMessage();
			byte[] plainBytes = Encoding.Default.GetBytes(text.ToCharArray());

			// A new key and iv are generated for every message
			tdes.GenerateKey();
			tdes.GenerateIV();

			// The rc2 initialization doesnt need to be encrypted, but will
			// be used in conjunction with the key to decrypt the message.
			message.tdesIV = tdes.IV;
			try 
			{
				// Encrypt the RC2 key using RSA encryption
				message.tdesKey = rsa.Encrypt(tdes.Key, false);
			}
			catch (CryptographicException e)
			{
				// The High Encryption Pack is required to run this  sample
				// because we are using a 128-bit key. See the readme for
				// additional information.
				Console.WriteLine("Encryption Failed. Ensure that the" + 
					" High Encryption Pack is installed.");
				Console.WriteLine("Error Message: " + e.Message);
				Environment.Exit(0);
			}
			// Encrypt the Text Message using RC2 (Symmetric algorithm)
			ICryptoTransform sse = tdes.CreateEncryptor();
			MemoryStream ms = new MemoryStream();
			CryptoStream cs = new CryptoStream(ms, sse, CryptoStreamMode.Write);
			try
			{
				cs.Write(plainBytes, 0, plainBytes.Length);
				cs.FlushFinalBlock();
				message.cipherBytes = ms.ToArray();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				ms.Close();
				cs.Close();
			}
			return message;
		} // method EncryptMessage


		public void DecryptMessage(CipherMessage message)
		{
			// Get the RC2 Key and Initialization Vector
			tdes.IV = message.tdesIV;
			try 
			{
				// Try decrypting the rc2 key
				tdes.Key = rsa.Decrypt(message.tdesKey, false);
			}
			catch (CryptographicException e)
			{
				Console.WriteLine("Decryption Failed: " + e.Message);
				return;
			}
      
			ICryptoTransform ssd = tdes.CreateDecryptor();
			// Put the encrypted message in a memorystream
			MemoryStream ms = new MemoryStream(message.cipherBytes);
			// the CryptoStream will read cipher text from the MemoryStream
			CryptoStream cs = new CryptoStream(ms, ssd, CryptoStreamMode.Read);

			byte[] initialText = new Byte[message.cipherBytes.Length];

			try 
			{
				// Decrypt the message and store in byte array
				cs.Read(initialText, 0, initialText.Length);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally 
			{
				ms.Close();
				cs.Close();
			}

			// Display the message received
			Console.WriteLine(name + " received the following message:");
			Console.WriteLine("  " + Encoding.Unicode.GetString(initialText));
		} // method DecryptMessage
	} // class Person
}
