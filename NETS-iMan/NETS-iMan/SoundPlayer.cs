using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NETS_iMan
{
	/// <summary>
	/// Helper for playing wav files and System Events stored under HKEY_CURRENT_USER\AppEvents\Schemes\Apps\.Default
	/// </summary>
	public class SoundPlayer
	{
		[DllImport("winmm.dll", EntryPoint = "PlaySound", CharSet = CharSet.Auto)]
		private static extern int PlaySound(String pszSound, int hmod, int falgs);

		/// <summary>
		/// API Parameter Flags for PlaySound method
		/// </summary>
		[Flags]
		public enum SND
		{
			SND_SYNC = 0x0000,/* play synchronously (default) */
			SND_ASYNC = 0x0001, /* play asynchronously */
			SND_NODEFAULT = 0x0002, /* silence (!default) if sound not found */
			SND_MEMORY = 0x0004, /* pszSound points to a memory file */
			SND_LOOP = 0x0008, /* loop the sound until next sndPlaySound */
			SND_NOSTOP = 0x0010, /* don't stop any currently playing sound */
			SND_NOWAIT = 0x00002000, /* don't wait if the driver is busy */
			SND_ALIAS = 0x00010000,/* name is a registry alias */
			SND_ALIAS_ID = 0x00110000, /* alias is a pre d ID */
			SND_FILENAME = 0x00020000, /* name is file name */
			SND_RESOURCE = 0x00040004, /* name is resource name or atom */
			SND_PURGE = 0x0040,  /* purge non-static events for task */
			SND_APPLICATION = 0x0080  /* look for application specific association */
		}

		/// <summary>
		/// Plays specified wav file.
		/// </summary>
		/// <param name="pszSound">Path points to the wave file.</param>
		/// <returns></returns>
		public static void PlaySound(string pszSound)
		{
			string path = Environment.GetEnvironmentVariable("TEMP") + @"\" + pszSound;
			if (File.Exists(path))
			{
				PlaySound(path, 0, (int)(SND.SND_ASYNC | SND.SND_FILENAME | SND.SND_NOWAIT));
			}
			else
			{
				Stream wav = getWavResourceStream(pszSound);
				if ((wav == null) || (wav.Length == 0)) return;
				byte[] bytes = new byte[wav.Length];
				wav.Read(bytes, 0, (int)wav.Length);
				wav.Close();

				FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
				stream.Write(bytes, 0, bytes.Length);
				stream.Flush();
				stream.Close();

				PlaySound(path, 0, (int)(SND.SND_ASYNC | SND.SND_FILENAME | SND.SND_NOWAIT));
			}
		}

		private static Stream getWavResourceStream(string pszSound)
		{
			string resourceName = "NETS_iMan.Sounds." + pszSound;
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
		}

		/// <summary>
		/// Play System Event stored under HKEY_CURRENT_USER\AppEvents\Schemes\Apps\.Default
		/// </summary>
		/// <param name="pszSound">SystemEvent Verb</param>
		public static void PlaySoundEvent(string pszSound)
		{
			PlaySound(pszSound, 0, (int)(SND.SND_ASYNC | SND.SND_ALIAS | SND.SND_NOWAIT));
		}
	}
}
