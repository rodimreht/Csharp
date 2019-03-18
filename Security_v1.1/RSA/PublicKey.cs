/*=====================================================================
  File:      PublicKey.cs

  Summary:   Demonstrates public key cryptography using the .NET
			 Framework implementation of RSA. 

---------------------------------------------------------------------
  This file is part of the Microsoft .NET Framework SDK Code Samples.

  Copyright (C) 2001 Microsoft Corporation.  All rights reserved.

This source code is intended only as a supplement to Microsoft
Development Tools and/or on-line documentation.  See these other
materials for detailed information regarding Microsoft code samples.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.
=====================================================================*/

using System;
using System.Security.Cryptography;
using System.IO; 
using System.Text;

namespace PublicKey
{
	class App
	{
		static void Main(string[] args)
		{
			//main();
			//PKTest.Run();
			PKTest.Run2();
		}

		// Main entry point
		static void main()
		{
			// 3명의 인스턴스를 생성한다.
			Person alice = new Person("Alice");
			Person bob = new Person("Bob");
			Person steve = new Person("Steve");

			PublicKey2.Person thermidor = new PublicKey2.Person("thermidor");

			// 각 사람들이 주고받을 메시지.
			CipherMessage aliceMessage;
			CipherMessage bobMessage;
			CipherMessage steveMessage;

			//============================================
			// 1. 자기 스스로 메시지를 암호화/복호화한다.
			// ------------------------------------------
			Console.WriteLine("Encrypting/Decrypting Your Own Message");
			Console.WriteLine("-----------------------------------------");

			// 자신의 Public Key와 Private Key로 암호화/복호화가 가능하다.
			aliceMessage = alice.EncryptMessage("Alice wrote this message");
			alice.DecryptMessage(aliceMessage);

			bobMessage = bob.EncryptMessage("Hi Alice! - Bob.");
			bob.DecryptMessage(bobMessage);
			//============================================

			
			//============================================
			// 2. 메시지를 주고 받는다.
			// ------------------------------------------
			Console.WriteLine();
			Console.WriteLine("Exchanging Keys and Messages");
			Console.WriteLine("-----------------------------------------");

			// Alice가 다른 사람들에게 자신의 Public Key를 준다.
			// (Alice가 메일로 Bob과 Steve에게 자신의 공개키(디지털 서명)를 전송한다고 간주한다.)
			bob.GetPublicKey(alice);
			steve.GetPublicKey(alice);

			// Bob과 Steve는 Alice로부터 받은 Public Key를 이용하여 자신의 메시지를 암호화한다.
			// (받은 공개키를 이용하여 메시지를 암호화하여 Alice에게 전송한다고 간주한다.)
			bobMessage = bob.EncryptMessage("Hi Alice! - Bob.");
			steveMessage = steve.EncryptMessage("How are you? - Steve");

			// Alice는 Bob과 Steve로부터 받은 메시지가 자신의 Public Key로 암호화된 메시지이므로
			// 자신의 Private Key를 이용하여 복호화할 수 있다.
			alice.DecryptMessage(bobMessage);
			alice.DecryptMessage(steveMessage);
			//============================================


			//============================================
			// 3. Private Key가 필요하여 실패하는 경우
			// ------------------------------------------
			Console.WriteLine();
			Console.WriteLine("Private Key required to read the messages");
			Console.WriteLine("-----------------------------------------");

			// Steve는 Bob이 암호화한 메시지를 복호화할 수 없다.
			// (Steve가 Bob이 Alice에게 보낸 메시지를 중간에 가로채더라도 내용을 알 수 없다.)
			steve.DecryptMessage(bobMessage);

			// Bob 역시 자신이 암호화한 메시지더라도 복호화할 수 없다.
			bob.DecryptMessage(bobMessage);
			//============================================
		} // method Main
	} // class App

	class CipherMessage
	{
		public byte[] cipherBytes;  // RC2 encrypted message text
		public byte[] rc2Key;       // RSA encrypted rc2 key
		public byte[] rc2IV;        // RC2 initialization vector
	}

	class Person
	{
		private RSACryptoServiceProvider rsa;
		private RC2CryptoServiceProvider rc2;
		private string name;

		// Maximum key size for the RC2 algorithm
		const int keySize = 128;

		// Person constructor
		public Person(string p_Name)
		{
			rsa = new RSACryptoServiceProvider();
			rc2 = new RC2CryptoServiceProvider();

			rc2.KeySize = keySize;
			
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
			byte[] plainBytes = Encoding.Unicode.GetBytes(text.ToCharArray());

			// A new key and iv are generated for every message
			rc2.GenerateKey();
			rc2.GenerateIV();

			// The rc2 initialization doesnt need to be encrypted, but will
			// be used in conjunction with the key to decrypt the message.
			message.rc2IV = rc2.IV;
			try 
			{
				// Encrypt the RC2 key using RSA encryption
				message.rc2Key = rsa.Encrypt(rc2.Key, false);
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
			ICryptoTransform sse = rc2.CreateEncryptor();
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
			rc2.IV = message.rc2IV;
			try 
			{
				// Try decrypting the rc2 key
				rc2.Key = rsa.Decrypt(message.rc2Key, false);
			}
			catch (CryptographicException e)
			{
				Console.WriteLine("Decryption Failed: " + e.Message);
				return;
			}
      
			ICryptoTransform ssd = rc2.CreateDecryptor();
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
} // namespace PublicKey
