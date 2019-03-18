using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using PnPeople.Security;

namespace NPKI
{
	public class nPKCS12 : PKCS12
	{
		public const string pbeWithSHAAndSEEDCBC = "1.2.410.200004.1.15";

		public nPKCS12()
		{
		}

		/// <summary>
		/// SEED/CBC에서 Password는 입력 문자열을 그대로 사용한다
		/// </summary>
		public string Password
		{
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					int size = value.Length;
					if (size > MaximumPasswordLength) size = MaximumPasswordLength;
					_password = new byte[size];
					Encoding.Default.GetBytes(value, 0, size, _password, 0);
				}
				else
				{
					// no password
					_password = null;
				}
			}
		}
	
		// Key, IV를 각각 64바이트로 만들어서 합친다.... 이상한 로직: 사용하면 안될 듯
		private SEED GetSymmetricAlgorithm(string algorithmOid, byte[] salt, int iterationCount)
		{
			int keyLength = 16;	// 128 bits (default)
			int ivLength = 16;	// 128 bits (default)

			DeriveBytes pd = new DeriveBytes();
			pd.Password = base._password;
			pd.Salt = salt;
			pd.IterationCount = iterationCount;

			switch (algorithmOid)
			{
				case pbeWithSHAAndSEEDCBC:			// no unit test available
					pd.HashName = "SHA";
					break;
			}

			SEED seed = new SEED();
			seed.KeyBytes = pd.DeriveKey(keyLength);
			// IV required only for block ciphers (not stream ciphers)
			if (ivLength > 0)
			{
				seed.IV = pd.DeriveIV(ivLength);
				seed.ModType = SEED.MODE.AI_CBC; // CBC
			}
			return seed;
		}

		private SEED GetSymmetricAlgorithm(byte[] salt, int iterationCount)
		{
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(_password, salt, "SHA1", iterationCount); // PBKDF1
			byte[] derivedKey = pdb.GetBytes(20);

			SEED seed = new SEED();
			seed.KeyBytes = getKey(derivedKey);
			seed.IV = getIV(derivedKey);
			seed.ModType = SEED.MODE.AI_CBC; // CBC
			return seed;
		}

		private byte[] getKey(byte[] derivedKey)
		{
			byte[] key = new byte[16];
			Buffer.BlockCopy(derivedKey, 0, key, 0, 16);
			return key;
		}

		private byte[] getIV(byte[] derivedKey)
		{
			byte[] iv = new byte[16];
			byte[] ivTemp = new byte[4];
			SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
			Buffer.BlockCopy(derivedKey, 16, ivTemp, 0, 4);
			byte[] derivedIV = sha1.ComputeHash(ivTemp);
			Buffer.BlockCopy(derivedIV, 0, iv, 0, 16);
			return iv;
		}

		public new byte[] Decrypt(string algorithmOid, byte[] salt, int iterationCount, byte[] encryptedData)
		{
			if (algorithmOid != pbeWithSHAAndSEEDCBC) return null; // Only for SHA1/SEED/CBC

			// Mono DeriveBytes 사용: 에러남
			//SEED seed1 = GetSymmetricAlgorithm(algorithmOid, salt, iterationCount);
			//byte[] result1 = seed1.Decrypt(encryptedData);

			SEED seed2 = GetSymmetricAlgorithm(salt, iterationCount);
			byte[] result2 = seed2.Decrypt(encryptedData);

			// SeedCs.cs 사용: 정상
			//PasswordDeriveBytes pdb = new PasswordDeriveBytes(_password, salt, "SHA1", iterationCount);  // PBKDF1
			//byte[] derivedKey = pdb.GetBytes(20);
			//byte[] result3 = SEED2.Decrypt(encryptedData, getKey(derivedKey), true, getIV(derivedKey));

			// Rfc2898DeriveBytes 사용: 에러남
			//Rfc2898DeriveBytes pdb2 = new Rfc2898DeriveBytes(_password, salt, iterationCount); // PBKDF2
			//byte[] derivedKey2 = pdb2.GetBytes(20);
			//byte[] result4 = SEED2.Decrypt(encryptedData, getKey(derivedKey2), true, getIV(derivedKey2));


			// CFB 테스트
			seed2.ModType = SEED.MODE.AI_CFB;
			byte[] enc = seed2.Encrypt(result2);
			byte[] dec = seed2.Decrypt(enc);

			// ECB 테스트
			seed2.ModType = SEED.MODE.AI_ECB;
			enc = seed2.Encrypt(result2);
			dec = seed2.Decrypt(enc);

			// OFB 테스트
			seed2.ModType = SEED.MODE.AI_OFB;
			enc = seed2.Encrypt(result2);
			dec = seed2.Decrypt(enc);

			return result2;
		}
	}
}
