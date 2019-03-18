using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace ExecSQL
{
	class Which
	{
		private bool printPaths = false;
		private bool printFullPath = false;
		private string additionalExtensionsString = null;
		private string[] pathExt = null; //contains the list of strings that are the parts of the PARTEXT environment variable.  
		private string firstFoundPath = null;

		public string FirstFoundPath
		{
			get { return firstFoundPath; }
		}

		public bool IsExists(string executableName)
		{
			string pathString = "";
			IDictionary  environmentVariables = Environment.GetEnvironmentVariables();
			foreach (DictionaryEntry de in environmentVariables)
			{
				if(de.Key.ToString().ToUpper().Equals("PATH"))
				{
					pathString = de.Value.ToString().Trim();
				}
				if (de.Key.ToString().ToUpper().Equals("PATHEXT"))
				{
					pathExt = de.Value.ToString().Trim().Split(new char[]{';'});
				}
			}
			
			Regex regEx = new Regex(GetRegExString(executableName), RegexOptions.IgnoreCase);
            
			string[] paths = this.GetPathsSplit(pathString);
			foreach(string onePath in paths)
			{
				string onePath2 = onePath.Trim();
				if (onePath2.Length == 0) continue;
				DirectoryInfo di = new DirectoryInfo(onePath2);
				if (!di.Exists) continue;
				foreach(FileInfo fi in di.GetFiles())
				{
					if (regEx.IsMatch(fi.Name))
					{
						firstFoundPath = fi.FullName;
						return true;
					}
				}
			}
			return false;
		}

		public void Search(string executableName)
		{
			string pathString = "";
			IDictionary  environmentVariables = Environment.GetEnvironmentVariables();
			foreach (DictionaryEntry de in environmentVariables)
			{
				if(de.Key.ToString().ToUpper().Equals("PATH"))
				{
					pathString = de.Value.ToString().Trim();
				}
				if (de.Key.ToString().ToUpper().Equals("PATHEXT"))
				{
					pathExt = de.Value.ToString().Trim().Split(new char[]{';'});
				}
			}
            
			if (this.printFullPath)
			{
				Console.WriteLine("Full Search Path = "+pathString);
			}

			//if there are additional extensions, add them to the existing list
			if (this.additionalExtensionsString != null && this.additionalExtensionsString.Length>0)
			{
				//a simple precaution to remove the last semicolon if there is one
				if (this.additionalExtensionsString.EndsWith(";"))
				{
					this.additionalExtensionsString = this.additionalExtensionsString.Remove(this.additionalExtensionsString.Length-1, 1);
				}
				string[] aes = this.additionalExtensionsString.Split(new char[]{';'});
				ArrayList al = new ArrayList();
				al.AddRange(this.pathExt);
				al.AddRange(aes);
				this.pathExt = (string[])al.ToArray(typeof(string));
			}
			
			Regex regEx = new Regex(GetRegExString(executableName), RegexOptions.IgnoreCase);
            
			string[] paths = this.GetPathsSplit(pathString);
			int count = 0;
			foreach(string onePath in paths)
			{
				if (this.printPaths)
				{
					Console.WriteLine(onePath);
				}
				string onePath2 = onePath.Trim();
				if (onePath2.Length == 0) continue;
				DirectoryInfo di = new DirectoryInfo(onePath2);
				if (!di.Exists) continue;
				foreach(FileInfo fi in di.GetFiles())
				{
					if (regEx.IsMatch(fi.Name))
					{
						if (firstFoundPath == null)
							firstFoundPath = fi.FullName;

						Console.WriteLine(" "+ ++count + " " +fi.FullName);
					}
				}
			}
			if (count == 0)
			{
				Console.WriteLine("No matches");
			}
		}

		//Insert the startup path at the beginning
		//Remove other instances of the startup path
		//Return an array of the paths.
		private string[] GetPathsSplit(string pathString)
		{
			string[] paths = pathString.Split(new char[]{Path.PathSeparator});
			string startupPath = System.Environment.CurrentDirectory.ToLower();

			ArrayList al = new ArrayList();
			foreach (string oneString in paths)
			{
				if (!(oneString.ToLower().Equals(startupPath) || 
					oneString.Equals(".")))
				{
					al.Add(oneString);
				}
			}

			al.Insert(0, ".");
			return (string[])al.ToArray(typeof(string));
		}

		/// <summary>
		/// Form the regular expression string for the matching file.
		/// </summary>
		/// <param name="executableName">The name of the executable</param>
		/// <returns>string that is the regex pattern.</returns>
		private string GetRegExString(string executableName)
		{
			executableName = executableName.ToLower(); 
			string regexString = "^"+executableName;
			bool execNameHasExtension = false;
			foreach(string oneExt in this.pathExt)
			{
				if (executableName.EndsWith(oneExt.ToLower()))
				{
					execNameHasExtension = true;
					break;
				}
			}
			if (execNameHasExtension || this.pathExt == null || this.pathExt.Length == 0)
			{
				regexString += "$";
			}
			else
			{
				regexString += "(?:";
				foreach(string oneExt in this.pathExt)
				{
					regexString += "\\"+oneExt+"|";
				}
				regexString += ")$";
				//regexString += "\\."+"(?:exe|com|bat|cmd)$"; //removed the hard coding
			}
			return regexString;
		}

		/// <summary>
		/// Print the description of the program.
		/// Print the usage string.
		/// Provide version and author info.
		/// </summary>
		public static void PrintUsage()
		{
			Console.WriteLine(
				"Description: WinWhich searches for an executable in all PATH locations.  Apart from the default executable extensions, the user may specify others with the -ae option." +
				"\r\n\r\n"+
				"WinWhich <executable name> [-p|-printPaths] [-fp|-fullPath] [-ae|-additionalExtensions] <semi colon separated extensions>"+
				"\r\n\r\n" + "Eg. WinWhich notepad -ae .txt;.rtf;.doc" + 
				"\r\n\r\n"+
				"Version: 1.0" +
				"\r\n"+
				"Written by Sathish V J (sathishvj@gmail.com)"
				);
		}

		/// <summary>
		/// Scan the arguments for all options and assign internal values.
		/// </summary>
		/// <param name="args">The arguments provided to the program.</param>
		public void ScanArgs(string[] args)
		{
			int i = 1; //ignore the executable name option
			while (i<args.Length)
			{
				string oneArg = args[i];
				if (oneArg.ToLower().Equals("-p") || oneArg.ToUpper().Equals("-printpaths"))
				{
					this.printPaths = true;
				}
				else if (oneArg.ToLower().Equals("-fp") || oneArg.ToUpper().Equals("-fullPath"))
				{
					this.printFullPath = true;
				}
				else if (oneArg.ToLower().Equals("-ae") || oneArg.ToLower().Equals("-additionalExtensions"))
				{
					i++;
					if (i>=args.Length)
					{
						Console.WriteLine("Error: missing value for additional extensions.");
						Environment.Exit(0);
					}
					this.additionalExtensionsString = args[i].Trim();
				}
				else
				{
					Console.WriteLine("Warning!: Unidentified argument: "+oneArg+"\r\n");
				}
				i++;
			}
		}
	}
}
