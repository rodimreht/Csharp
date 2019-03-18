using System.Collections.Specialized;

namespace iManService
{
	public class ManagedMimes
	{
		private static NameValueCollection mimeCollection;
		private static readonly object lockObj = new object();

		static ManagedMimes()
		{
			lock (lockObj)
			{
				if ((mimeCollection == null) || (mimeCollection.Count == 0))
					initMimes();
			}
		}

		private static void initMimes()
		{
			mimeCollection = new NameValueCollection();
			mimeCollection.Add(".art", "image/x-jg");
			mimeCollection.Add(".bmp", "image/bmp");
			mimeCollection.Add(".cmx", "image/x-cmx");
			mimeCollection.Add(".cod", "image/cis-cod");
			mimeCollection.Add(".dib", "image/bmp");
			mimeCollection.Add(".gif", "image/gif");
			mimeCollection.Add(".ico", "image/x-icon");
			mimeCollection.Add(".ief", "image/ief");
			mimeCollection.Add(".jfif", "image/pjpeg");
			mimeCollection.Add(".jpe", "image/jpeg");
			mimeCollection.Add(".jpeg", "image/jpeg");
			mimeCollection.Add(".jpg", "image/jpeg");
			mimeCollection.Add(".pbm", "image/x-portable-bitmap");
			mimeCollection.Add(".pgm", "image/x-portable-graymap");
			mimeCollection.Add(".png", "image/png");
			mimeCollection.Add(".pnm", "image/x-portable-anymap");
			mimeCollection.Add(".pnz", "image/png");
			mimeCollection.Add(".ppm", "image/x-portable-pixmap");
			mimeCollection.Add(".ras", "image/x-cmu-raster");
			mimeCollection.Add(".rf", "image/vnd.rn-realflash");
			mimeCollection.Add(".rgb", "image/x-rgb");
			mimeCollection.Add(".tif", "image/tiff");
			mimeCollection.Add(".tiff", "image/tiff");
			mimeCollection.Add(".wbmp", "image/vnd.wap.wbmp");
			mimeCollection.Add(".wbm", "image/x-xbitmap");
			mimeCollection.Add(".xpm", "image/x-xpixmap");
			mimeCollection.Add(".xwd", "image/x-xwindowdump");
		}

		/// <summary>
		/// 지정된 확장자에 해당하는 Mime 종류 문자열을 반환한다.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static string GetMimeType(string fileName)
		{
			string fileExt = fileName.Substring(fileName.LastIndexOf("."));
			string lowerExt = fileExt.ToLower().Trim();
			for (int i = 0; i < mimeCollection.Count; i++)
			{
				if (mimeCollection.GetKey(i).Equals(lowerExt))
					return mimeCollection.Get(i);
			}

			// 목록에 없으면 확장자 그대로 리턴한다.
			return "image/" + fileName.Substring(1);
		}

		public static bool IsManagedType(string fileName)
		{
			int pos = fileName.LastIndexOf(".");
			if (pos < 0) return false;

			string fileExt = fileName.Substring(pos);
			string lowerExt = fileExt.ToLower().Trim();
			for (int i = 0; i < mimeCollection.Count; i++)
			{
				if (mimeCollection.GetKey(i).Equals(lowerExt))
					return true;
			}
			return false;
		}
	}
}
