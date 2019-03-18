using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NETS_iMan
{

	#region "OLE definitions"

	// STGM
	[Flags, ComVisible(false)]
	public enum STGM
	{
		STGM_DIRECT = 0x0,
		STGM_TRANSACTED = 0x10000,
		STGM_SIMPLE = 0x8000000,
		STGM_READ = 0x0,
		STGM_WRITE = 0x1,
		STGM_READWRITE = 0x2,
		STGM_SHARE_DENY_NONE = 0x40,
		STGM_SHARE_DENY_READ = 0x30,
		STGM_SHARE_DENY_WRITE = 0x20,
		STGM_SHARE_EXCLUSIVE = 0x10,
		STGM_PRIORITY = 0x40000,
		STGM_DELETEONRELEASE = 0x4000000,
		STGM_NOSCRATCH = 0x100000,
		STGM_CREATE = 0x1000,
		STGM_CONVERT = 0x20000,
		STGM_FAILIFTHERE = 0x0,
		STGM_NOSNAPSHOT = 0x200000,
	}

	// DVASPECT
	[Flags, ComVisible(false)]
	public enum DVASPECT
	{
		DVASPECT_CONTENT = 1,
		DVASPECT_THUMBNAIL = 2,
		DVASPECT_ICON = 4,
		DVASPECT_DOCPRINT = 8,
		DVASPECT_OPAQUE = 16,
		DVASPECT_TRANSPARENT = 32,
	}

	// CLIPFORMAT
	[ComVisible(false)]
	public enum CLIPFORMAT
	{
		CF_TEXT = 1,
		CF_BITMAP = 2,
		CF_METAFILEPICT = 3,
		CF_SYLK = 4,
		CF_DIF = 5,
		CF_TIFF = 6,
		CF_OEMTEXT = 7,
		CF_DIB = 8,
		CF_PALETTE = 9,
		CF_PENDATA = 10,
		CF_RIFF = 11,
		CF_WAVE = 12,
		CF_UNICODETEXT = 13,
		CF_ENHMETAFILE = 14,
		CF_HDROP = 15,
		CF_LOCALE = 16,
		CF_MAX = 17,
		CF_OWNERDISPLAY = 0x80,
		CF_DSPTEXT = 0x81,
		CF_DSPBITMAP = 0x82,
		CF_DSPMETAFILEPICT = 0x83,
		CF_DSPENHMETAFILE = 0x8E,
	}

	// Object flags
	[Flags, ComVisible(false)]
	public enum REOOBJECTFLAGS : uint
	{
		REO_NULL = 0x00000000, // No flags
		REO_READWRITEMASK = 0x0000003F, // Mask out RO bits
		REO_DONTNEEDPALETTE = 0x00000020, // Object doesn't need palette
		REO_BLANK = 0x00000010, // Object is blank
		REO_DYNAMICSIZE = 0x00000008, // Object defines size always
		REO_INVERTEDSELECT = 0x00000004, // Object drawn all inverted if sel
		REO_BELOWBASELINE = 0x00000002, // Object sits below the baseline
		REO_RESIZABLE = 0x00000001, // Object may be resized
		REO_LINK = 0x80000000, // Object is a link (RO)
		REO_STATIC = 0x40000000, // Object is static (RO)
		REO_SELECTED = 0x08000000, // Object selected (RO)
		REO_OPEN = 0x04000000, // Object open in its server (RO)
		REO_INPLACEACTIVE = 0x02000000, // Object in place active (RO)
		REO_HILITED = 0x01000000, // Object is to be hilited (RO)
		REO_LINKAVAILABLE = 0x00800000, // Link believed available (RO)
		REO_GETMETAFILE = 0x00400000 // Object requires metafile (RO)
	}

	// OLERENDER
	[ComVisible(false)]
	public enum OLERENDER
	{
		OLERENDER_NONE = 0,
		OLERENDER_DRAW = 1,
		OLERENDER_FORMAT = 2,
		OLERENDER_ASIS = 3,
	}

	// TYMED
	[Flags, ComVisible(false)]
	public enum TYMED
	{
		TYMED_NULL = 0,
		TYMED_HGLOBAL = 1,
		TYMED_FILE = 2,
		TYMED_ISTREAM = 4,
		TYMED_ISTORAGE = 8,
		TYMED_GDI = 16,
		TYMED_MFPICT = 32,
		TYMED_ENHMF = 64,
	}

	[StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct FORMATETC
	{
		public CLIPFORMAT cfFormat;
		public IntPtr ptd;
		public DVASPECT dwAspect;
		public int lindex;
		public TYMED tymed;
	}

	[StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct STGMEDIUM
	{
		//[MarshalAs(UnmanagedType.I4)]
		public int tymed;
		public IntPtr unionmember;
		public IntPtr pUnkForRelease;
	}

	[ComVisible(true),
	 ComImport,
	 Guid("00000103-0000-0000-C000-000000000046"),
	 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IEnumFORMATETC
	{
		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int Next(
			[In, MarshalAs(UnmanagedType.U4)] int celt,
			[Out] FORMATETC rgelt,
			[In, Out, MarshalAs(UnmanagedType.LPArray)] int[] pceltFetched);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int Skip(
			[In, MarshalAs(UnmanagedType.U4)] int celt);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int Reset();

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int Clone(
			[Out, MarshalAs(UnmanagedType.LPArray)] IEnumFORMATETC[] ppenum);
	}

	[ComVisible(true), StructLayout(LayoutKind.Sequential)]
	public class COMRECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;

		public COMRECT()
		{
		}

		public COMRECT(int left, int top, int right, int bottom)
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}

		public static COMRECT FromXYWH(int x, int y, int width, int height)
		{
			return new COMRECT(x, y, x + width, y + height);
		}
	}

	public enum GETOBJECTOPTIONS
	{
		REO_GETOBJ_NO_INTERFACES = 0x00000000,
		REO_GETOBJ_POLEOBJ = 0x00000001,
		REO_GETOBJ_PSTG = 0x00000002,
		REO_GETOBJ_POLESITE = 0x00000004,
		REO_GETOBJ_ALL_INTERFACES = 0x00000007,
	}

	public enum GETCLIPBOARDDATAFLAGS
	{
		RECO_PASTE = 0,
		RECO_DROP = 1,
		RECO_COPY = 2,
		RECO_CUT = 3,
		RECO_DRAG = 4
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct CHARRANGE
	{
		public int cpMin;
		public int cpMax;
	}

	[StructLayout(LayoutKind.Sequential)]
	public class REOBJECT
	{
		public int cbStruct = Marshal.SizeOf(typeof (REOBJECT)); // Size of structure
		public int cp; // Character position of object
		public Guid clsid; // Class ID of object
		public IntPtr poleobj; // OLE object interface
		public IStorage pstg; // Associated storage interface
		public IOleClientSite polesite; // Associated client site interface
		public Size sizel; // Size of object (may be 0,0)
		public uint dvAspect; // Display aspect to use
		public uint dwFlags; // Object status flags
		public uint dwUser; // Dword for user's use
	}

	[ComVisible(true), Guid("0000010F-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAdviseSink
	{
		//C#r: UNDONE (Field in interface) public static readonly    Guid iid;
		void OnDataChange(
			[In] FORMATETC pFormatetc,
			[In] STGMEDIUM pStgmed);

		void OnViewChange(
			[In, MarshalAs(UnmanagedType.U4)] int dwAspect,
			[In, MarshalAs(UnmanagedType.I4)] int lindex);

		void OnRename(
			[In, MarshalAs(UnmanagedType.Interface)] object pmk);

		void OnSave();


		void OnClose();
	}

	[ComVisible(false), StructLayout(LayoutKind.Sequential)]
	public sealed class STATDATA
	{
		[MarshalAs(UnmanagedType.U4)] public int advf;
		[MarshalAs(UnmanagedType.U4)] public int dwConnection;
	}

	[ComVisible(false), StructLayout(LayoutKind.Sequential)]
	public sealed class tagOLEVERB
	{
		[MarshalAs(UnmanagedType.I4)] public int lVerb;

		[MarshalAs(UnmanagedType.LPWStr)] public String lpszVerbName;

		[MarshalAs(UnmanagedType.U4)] public int fuFlags;

		[MarshalAs(UnmanagedType.U4)] public int grfAttribs;
	}

	[ComVisible(true), ComImport, Guid("00000104-0000-0000-C000-000000000046"),
	 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IEnumOLEVERB
	{
		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int Next(
			[MarshalAs(UnmanagedType.U4)] int celt,
			[Out] tagOLEVERB rgelt,
			[Out, MarshalAs(UnmanagedType.LPArray)] int[] pceltFetched);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int Skip(
			[In, MarshalAs(UnmanagedType.U4)] int celt);

		void Reset();


		void Clone(
			out IEnumOLEVERB ppenum);
	}

	[ComVisible(true), Guid("00000105-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IEnumSTATDATA
	{
		//C#r: UNDONE (Field in interface) public static readonly    Guid iid;

		void Next(
			[In, MarshalAs(UnmanagedType.U4)] int celt,
			[Out] STATDATA rgelt,
			[Out, MarshalAs(UnmanagedType.LPArray)] int[] pceltFetched);


		void Skip(
			[In, MarshalAs(UnmanagedType.U4)] int celt);


		void Reset();


		void Clone(
			[Out, MarshalAs(UnmanagedType.LPArray)] IEnumSTATDATA[] ppenum);
	}

	[ComVisible(true), Guid("0000011B-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IOleContainer
	{
		void ParseDisplayName(
			[In, MarshalAs(UnmanagedType.Interface)] object pbc,
			[In, MarshalAs(UnmanagedType.BStr)] string pszDisplayName,
			[Out, MarshalAs(UnmanagedType.LPArray)] int[] pchEaten,
			[Out, MarshalAs(UnmanagedType.LPArray)] object[] ppmkOut);


		void EnumObjects(
			[In, MarshalAs(UnmanagedType.U4)] int grfFlags,
			[Out, MarshalAs(UnmanagedType.LPArray)] object[] ppenum);


		void LockContainer(
			[In, MarshalAs(UnmanagedType.I4)] int fLock);
	}

	[ComVisible(true),
	 ComImport,
	 Guid("0000010E-0000-0000-C000-000000000046"),
	 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IDataObject2
	{
		[PreserveSig]
		uint GetData(
			ref FORMATETC a,
			ref STGMEDIUM b);

		[PreserveSig]
		uint GetDataHere(
			ref FORMATETC pFormatetc,
			out STGMEDIUM pMedium);

		[PreserveSig]
		uint QueryGetData(
			ref FORMATETC pFormatetc);

		[PreserveSig]
		uint GetCanonicalFormatEtc(
			ref FORMATETC pformatectIn,
			out FORMATETC pformatetcOut);

		[PreserveSig]
		uint SetData(
			ref FORMATETC pFormatectIn,
			ref STGMEDIUM pmedium,
			[In, MarshalAs(UnmanagedType.Bool)] bool fRelease);

		[PreserveSig]
		uint EnumFormatEtc(
			uint dwDirection, IEnumFORMATETC penum);

		[PreserveSig]
		uint DAdvise(
			ref FORMATETC pFormatetc,
			int advf,
			[In, MarshalAs(UnmanagedType.Interface)] IAdviseSink pAdvSink,
			out uint pdwConnection);

		[PreserveSig]
		uint DUnadvise(
			uint dwConnection);

		[PreserveSig]
		uint EnumDAdvise(
			[Out, MarshalAs(UnmanagedType.Interface)] out IEnumSTATDATA ppenumAdvise);
	}

	[ComVisible(true), Guid("00000118-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IOleClientSite
	{
		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int SaveObject();

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int GetMoniker(
			[In, MarshalAs(UnmanagedType.U4)] int dwAssign,
			[In, MarshalAs(UnmanagedType.U4)] int dwWhichMoniker,
			[Out, MarshalAs(UnmanagedType.Interface)] out object ppmk);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int GetContainer([MarshalAs(UnmanagedType.Interface)] out IOleContainer container);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int ShowObject();

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int OnShowWindow(
			[In, MarshalAs(UnmanagedType.I4)] int fShow);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int RequestNewObjectLayout();
	}

	[ComVisible(false), StructLayout(LayoutKind.Sequential)]
	public sealed class tagLOGPALETTE
	{
		[MarshalAs(UnmanagedType.U2) /*leftover(offset=0, palVersion)*/] public short palVersion;

		[MarshalAs(UnmanagedType.U2) /*leftover(offset=2, palNumEntries)*/] public short palNumEntries;

		// UNMAPPABLE: palPalEntry: Cannot be used as a structure field.
		//   /** @com.structmap(UNMAPPABLE palPalEntry) */
		//  public UNMAPPABLE palPalEntry;
	}

	[ComVisible(true), ComImport, Guid("00000112-0000-0000-C000-000000000046"),
	 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IOleObject
	{
		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int SetClientSite(
			[In, MarshalAs(UnmanagedType.Interface)] IOleClientSite pClientSite);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int GetClientSite(out IOleClientSite site);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int SetHostNames(
			[In, MarshalAs(UnmanagedType.LPWStr)] string szContainerApp,
			[In, MarshalAs(UnmanagedType.LPWStr)] string szContainerObj);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int Close(
			[In, MarshalAs(UnmanagedType.I4)] int dwSaveOption);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int SetMoniker(
			[In, MarshalAs(UnmanagedType.U4)] int dwWhichMoniker,
			[In, MarshalAs(UnmanagedType.Interface)] object pmk);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int GetMoniker(
			[In, MarshalAs(UnmanagedType.U4)] int dwAssign,
			[In, MarshalAs(UnmanagedType.U4)] int dwWhichMoniker,
			out object moniker);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int InitFromData(
			[In, MarshalAs(UnmanagedType.Interface)] IDataObject2 pDataObject,
			[In, MarshalAs(UnmanagedType.I4)] int fCreation,
			[In, MarshalAs(UnmanagedType.U4)] int dwReserved);

		int GetClipboardData(
			[In, MarshalAs(UnmanagedType.U4)] int dwReserved,
			out IDataObject2 data);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int DoVerb(
			[In, MarshalAs(UnmanagedType.I4)] int iVerb,
			[In] IntPtr lpmsg,
			[In, MarshalAs(UnmanagedType.Interface)] IOleClientSite pActiveSite,
			[In, MarshalAs(UnmanagedType.I4)] int lindex,
			[In] IntPtr hwndParent,
			[In] COMRECT lprcPosRect);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int EnumVerbs(out IEnumOLEVERB e);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int Update();

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int IsUpToDate();

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int GetUserClassID(
			[In, Out] ref Guid pClsid);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int GetUserType(
			[In, MarshalAs(UnmanagedType.U4)] int dwFormOfType,
			[Out, MarshalAs(UnmanagedType.LPWStr)] out string userType);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int SetExtent(
			[In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect,
			[In] Size pSizel);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int GetExtent(
			[In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect,
			[Out] Size pSizel);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int Advise([In, MarshalAs(UnmanagedType.Interface)] IAdviseSink pAdvSink, out int cookie);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int Unadvise([In, MarshalAs(UnmanagedType.U4)] int dwConnection);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int EnumAdvise(out IEnumSTATDATA e);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int GetMiscStatus([In, MarshalAs(UnmanagedType.U4)] int dwAspect, out int misc);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int SetColorScheme([In] tagLOGPALETTE pLogpal);
	}

	[ComImport]
	[Guid("0000000d-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IEnumSTATSTG
	{
		// The user needs to allocate an STATSTG array whose size is celt.
		[PreserveSig]
		uint
			Next(
			uint celt,
			[MarshalAs(UnmanagedType.LPArray), Out] STATSTG[] rgelt,
			out uint pceltFetched
			);

		void Skip(uint celt);

		void Reset();

		[return: MarshalAs(UnmanagedType.Interface)]
		IEnumSTATSTG Clone();
	}

	[ComImport]
	[Guid("0000000b-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IStorage
	{
		int CreateStream(
			/* [string][in] */ string pwcsName,
			                   /* [in] */ uint grfMode,
			                   /* [in] */ uint reserved1,
			                   /* [in] */ uint reserved2,
			                   /* [out] */ out IStream ppstm);

		int OpenStream(
			/* [string][in] */ string pwcsName,
			                   /* [unique][in] */ IntPtr reserved1,
			                   /* [in] */ uint grfMode,
			                   /* [in] */ uint reserved2,
			                   /* [out] */ out IStream ppstm);

		int CreateStorage(
			/* [string][in] */ string pwcsName,
			                   /* [in] */ uint grfMode,
			                   /* [in] */ uint reserved1,
			                   /* [in] */ uint reserved2,
			                   /* [out] */ out IStorage ppstg);

		int OpenStorage(
			/* [string][unique][in] */ string pwcsName,
			                           /* [unique][in] */ IStorage pstgPriority,
			                           /* [in] */ uint grfMode,
			                           /* [unique][in] */ IntPtr snbExclude,
			                           /* [in] */ uint reserved,
			                           /* [out] */ out IStorage ppstg);

		int CopyTo(
			/* [in] */ uint ciidExclude,
			           /* [size_is][unique][in] */ Guid rgiidExclude,
			           /* [unique][in] */ IntPtr snbExclude,
			           /* [unique][in] */ IStorage pstgDest);

		int MoveElementTo(
			/* [string][in] */ string pwcsName,
			                   /* [unique][in] */ IStorage pstgDest,
			                   /* [string][in] */ string pwcsNewName,
			                   /* [in] */ uint grfFlags);

		int Commit(
			/* [in] */ uint grfCommitFlags);

		int Revert();

		int EnumElements(
			/* [in] */ uint reserved1,
			           /* [size_is][unique][in] */ IntPtr reserved2,
			           /* [in] */ uint reserved3,
			           /* [out] */ out IEnumSTATSTG ppenum);

		int DestroyElement(
			/* [string][in] */ string pwcsName);

		int RenameElement(
			/* [string][in] */ string pwcsOldName,
			                   /* [string][in] */ string pwcsNewName);

		int SetElementTimes(
			/* [string][unique][in] */ string pwcsName,
			                           /* [unique][in] */ FILETIME pctime,
			                           /* [unique][in] */ FILETIME patime,
			                           /* [unique][in] */ FILETIME pmtime);

		int SetClass(
			/* [in] */ Guid clsid);

		int SetStateBits(
			/* [in] */ uint grfStateBits,
			           /* [in] */ uint grfMask);

		int Stat(
			/* [out] */ out STATSTG pstatstg,
			            /* [in] */ uint grfStatFlag);
	}

	[ComImport]
	[Guid("0000000a-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface ILockBytes
	{
		int ReadAt(
			/* [in] */ ulong ulOffset,
			           /* [unique][out] */ IntPtr pv,
			           /* [in] */ uint cb,
			           /* [out] */ out IntPtr pcbRead);

		int WriteAt(
			/* [in] */ ulong ulOffset,
			           /* [size_is][in] */ IntPtr pv,
			           /* [in] */ uint cb,
			           /* [out] */ out IntPtr pcbWritten);

		int Flush();

		int SetSize(
			/* [in] */ ulong cb);

		int LockRegion(
			/* [in] */ ulong libOffset,
			           /* [in] */ ulong cb,
			           /* [in] */ uint dwLockType);

		int UnlockRegion(
			/* [in] */ ulong libOffset,
			           /* [in] */ ulong cb,
			           /* [in] */ uint dwLockType);

		int Stat(
			/* [out] */ out STATSTG pstatstg,
			            /* [in] */ uint grfStatFlag);
	}

	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0c733a30-2a1c-11ce-ade5-00aa0044773d")]
	public interface ISequentialStream
	{
		int Read(
			/* [length_is][size_is][out] */ IntPtr pv,
			                                /* [in] */ uint cb,
			                                /* [out] */ out uint pcbRead);

		int Write(
			/* [size_is][in] */ IntPtr pv,
			                    /* [in] */ uint cb,
			                    /* [out] */ out uint pcbWritten);
	} ;

	[ComImport]
	[Guid("0000000c-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IStream : ISequentialStream
	{
		int Seek(
			/* [in] */ ulong dlibMove,
			           /* [in] */ uint dwOrigin,
			           /* [out] */ out ulong plibNewPosition);

		int SetSize(
			/* [in] */ ulong libNewSize);

		int CopyTo(
			/* [unique][in] */ [In] IStream pstm,
			                   /* [in] */ ulong cb,
			                   /* [out] */ out ulong pcbRead,
			                   /* [out] */ out ulong pcbWritten);

		int Commit(
			/* [in] */ uint grfCommitFlags);

		int Revert();

		int LockRegion(
			/* [in] */ ulong libOffset,
			           /* [in] */ ulong cb,
			           /* [in] */ uint dwLockType);

		int UnlockRegion(
			/* [in] */ ulong libOffset,
			           /* [in] */ ulong cb,
			           /* [in] */ uint dwLockType);

		int Stat(
			/* [out] */ out STATSTG pstatstg,
			            /* [in] */ uint grfStatFlag);

		int Clone(
			/* [out] */ out IStream ppstm);
	} ;

	/// <summary>
	/// Definition for interface IPersist.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0000010c-0000-0000-C000-000000000046")]
	public interface IPersist
	{
		/// <summary>
		/// getClassID
		/// </summary>
		/// <param name="pClassID"></param>
		void GetClassID( /* [out] */ out Guid pClassID);
	}

	/// <summary>
	/// Definition for interface IPersistStream.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000109-0000-0000-C000-000000000046")]
	public interface IPersistStream : IPersist
	{
		/// <summary>
		/// GetClassID
		/// </summary>
		/// <param name="pClassID"></param>
		new void GetClassID(out Guid pClassID);

		/// <summary>
		/// isDirty
		/// </summary>
		/// <returns></returns>
		[PreserveSig]
		int IsDirty();

		/// <summary>
		/// Load
		/// </summary>
		/// <param name="pStm"></param>
		void Load([In] UCOMIStream pStm);

		/// <summary>
		/// Save
		/// </summary>
		/// <param name="pStm"></param>
		/// <param name="fClearDirty"></param>
		void Save([In] UCOMIStream pStm, [In, MarshalAs(UnmanagedType.Bool)] bool fClearDirty);

		/// <summary>
		/// GetSizeMax
		/// </summary>
		/// <param name="pcbSize"></param>
		void GetSizeMax(out long pcbSize);
	}

	[ComImport, Guid("00020D00-0000-0000-c000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IRichEditOle
	{
		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int GetClientSite(out IOleClientSite site);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int GetObjectCount();

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int GetLinkCount();

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int GetObject(int iob, [In, Out] REOBJECT lpreobject, [MarshalAs(UnmanagedType.U4)] GETOBJECTOPTIONS flags);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int InsertObject(REOBJECT lpreobject);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int ConvertObject(int iob, Guid rclsidNew, string lpstrUserTypeNew);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int ActivateAs(Guid rclsid, Guid rclsidAs);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int SetHostNames(string lpstrContainerApp, string lpstrContainerObj);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int SetLinkAvailable(int iob, bool fAvailable);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int SetDvaspect(int iob, uint dvaspect);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int HandsOffStorage(int iob);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int SaveCompleted(int iob, IStorage lpstg);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int InPlaceDeactivate();

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int ContextSensitiveHelp(bool fEnterMode);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int GetClipboardData([In, Out] ref CHARRANGE lpchrg, [MarshalAs(UnmanagedType.U4)] GETCLIPBOARDDATAFLAGS reco,
		                     out IDataObject2 lplpdataobj);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int ImportDataObject(IDataObject2 lpdataobj, int cf, IntPtr hMetaPict);
	}

	#endregion

	public class myDataObject : IDataObject2
	{
		private const uint E_FAIL = 0x80004005;
		private const uint E_NOTIMPL = 0x80004001;
		private const uint E_POINTER = 0x80004003;
		private const uint S_OK = 0;
		private Bitmap mBitmap;
		public FORMATETC mpFormatetc;

		public myDataObject()
		{
			mBitmap = new Bitmap(16, 16);
			mpFormatetc = new FORMATETC();
		}

		#region IDataObject2 Members

		public uint GetData(ref FORMATETC pFormatetc, ref STGMEDIUM pMedium)
		{
			IntPtr hDst = mBitmap.GetHbitmap();

			pMedium.tymed = (int) TYMED.TYMED_GDI;
			pMedium.unionmember = hDst;
			pMedium.pUnkForRelease = IntPtr.Zero;

			return S_OK;
		}

		public uint GetDataHere(ref FORMATETC pFormatetc, out STGMEDIUM pMedium)
		{
			Trace.WriteLine("GetDataHere");

			pMedium = new STGMEDIUM();

			return E_NOTIMPL;
		}

		public uint QueryGetData(ref FORMATETC pFormatetc)
		{
			Trace.WriteLine("QueryGetData");

			return E_NOTIMPL;
		}

		public uint GetCanonicalFormatEtc(ref FORMATETC pFormatetcIn, out FORMATETC pFormatetcOut)
		{
			Trace.WriteLine("GetCanonicalFormatEtc");

			pFormatetcOut = new FORMATETC();

			return E_NOTIMPL;
		}

		public uint SetData(ref FORMATETC a, ref STGMEDIUM b, bool fRelease)
		{
			//mpFormatetc = pFormatectIn;
			//mpmedium = pmedium;

			Trace.WriteLine("SetData");

			return (int) S_OK;
		}

		public uint EnumFormatEtc(uint dwDirection, IEnumFORMATETC penum)
		{
			Trace.WriteLine("EnumFormatEtc");

			return (int) S_OK;
		}

		public uint DAdvise(ref FORMATETC a, int advf, IAdviseSink pAdvSink, out uint pdwConnection)
		{
			Trace.WriteLine("DAdvise");

			pdwConnection = 0;

			return E_NOTIMPL;
		}

		public uint DUnadvise(uint dwConnection)
		{
			Trace.WriteLine("DUnadvise");

			return E_NOTIMPL;
		}

		public uint EnumDAdvise(out IEnumSTATDATA ppenumAdvise)
		{
			Trace.WriteLine("EnumDAdvise");

			ppenumAdvise = null;

			return E_NOTIMPL;
		}

		#endregion

		public void SetImage(string strFilename)
		{
			try
			{
				mBitmap = (Bitmap) Image.FromFile(strFilename, true);

				mpFormatetc.cfFormat = CLIPFORMAT.CF_BITMAP; // Clipboard format = CF_BITMAP
				mpFormatetc.ptd = IntPtr.Zero; // Target Device = Screen
				mpFormatetc.dwAspect = DVASPECT.DVASPECT_CONTENT; // Level of detail = Full content
				mpFormatetc.lindex = -1; // Index = Not applicaple
				mpFormatetc.tymed = TYMED.TYMED_GDI; // Storage medium = HBITMAP handle
			}
			catch
			{
			}
		}

		public void SetImage(Image image)
		{
			try
			{
				mBitmap = new Bitmap(image);

				mpFormatetc.cfFormat = CLIPFORMAT.CF_BITMAP; // Clipboard format = CF_BITMAP
				mpFormatetc.ptd = IntPtr.Zero; // Target Device = Screen
				mpFormatetc.dwAspect = DVASPECT.DVASPECT_CONTENT; // Level of detail = Full content
				mpFormatetc.lindex = -1; // Index = Not applicaple
				mpFormatetc.tymed = TYMED.TYMED_GDI; // Storage medium = HBITMAP handle
			}
			catch
			{
			}
		}
	}

	public class RichTextBoxEx : RichTextBox
	{
		#region Imports and structs

		// It makes no difference if we use PARAFORMAT or
		// PARAFORMAT2 here, so I have opted for PARAFORMAT2.

		private const int EM_SETEVENTMASK = 1073;
		private const int WM_SETREDRAW = 11;

		[DllImport("user32", CharSet = CharSet.Auto)]
		private static extern int SendMessage(HandleRef hWnd,
		                                      int msg,
		                                      int wParam,
		                                      int lParam);

		[DllImport("user32", CharSet = CharSet.Auto)]
		private static extern int SendMessage(HandleRef hWnd,
		                                      int msg,
		                                      int wParam,
		                                      ref PARAFORMAT lp);

		[DllImport("user32", CharSet = CharSet.Auto)]
		private static extern int SendMessage(HandleRef hWnd,
		                                      int msg,
		                                      int wParam,
		                                      ref CHARFORMAT lp);

		[DllImport("User32.dll", CharSet = CharSet.Auto, PreserveSig = false)]
		public static extern IRichEditOle SendMessage(IntPtr hWnd, int message, int wParam);

		[DllImport("user32", CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern bool GetClientRect(IntPtr hWnd, [In, Out] ref Rectangle rect);

		[DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern bool GetWindowRect(IntPtr hWnd, [In, Out] ref Rectangle rect);

		[DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr GetParent(IntPtr hWnd);

		[DllImport("ole32.dll")]
		private static extern int OleSetContainedObject([MarshalAs(UnmanagedType.IUnknown)] object pUnk, bool fContained);

		[DllImport("ole32.dll")]
		private static extern int OleLoadPicturePath(
			[MarshalAs(UnmanagedType.LPWStr)] string lpszPicturePath,
			[MarshalAs(UnmanagedType.IUnknown)] [In] object pIUnknown,
			uint dwReserved,
			uint clrReserved,
			ref Guid riid,
			[MarshalAs(UnmanagedType.IUnknown)] out object ppvObj);

		[DllImport("ole32.dll")]
		private static extern int OleCreateFromFile([In] ref Guid rclsid,
		                                            [MarshalAs(UnmanagedType.LPWStr)] string lpszFileName, [In] ref Guid riid,
		                                            uint renderopt, ref FORMATETC pFormatEtc, IOleClientSite pClientSite,
		                                            IStorage pStg, [MarshalAs(UnmanagedType.IUnknown)] out object ppvObj);

		[DllImport("ole32.dll")]
		private static extern int OleCreateFromData(IDataObject2 pSrcDataObj,
		                                            [In] ref Guid riid, uint renderopt, ref FORMATETC pFormatEtc,
		                                            IOleClientSite pClientSite, IStorage pStg,
		                                            [MarshalAs(UnmanagedType.IUnknown)] out object ppvObj);

		[DllImport("ole32.dll")]
		private static extern int OleCreateStaticFromData([MarshalAs(UnmanagedType.Interface)] IDataObject2 pSrcDataObj,
		                                                  [In] ref Guid riid, uint renderopt, ref FORMATETC pFormatEtc,
		                                                  IOleClientSite pClientSite, IStorage pStg,
		                                                  [MarshalAs(UnmanagedType.IUnknown)] out object ppvObj);

		[DllImport("ole32.dll")]
		private static extern int OleCreateLinkFromData([MarshalAs(UnmanagedType.Interface)] IDataObject2 pSrcDataObj,
		                                                [In] ref Guid riid, uint renderopt, ref FORMATETC pFormatEtc,
		                                                IOleClientSite pClientSite, IStorage pStg,
		                                                [MarshalAs(UnmanagedType.IUnknown)] out object ppvObj);

		#region Nested type: CHARFORMAT

		[StructLayout(LayoutKind.Sequential)]
		public struct CHARFORMAT
		{
			public int cbSize;
			public UInt32 dwMask;
			public UInt32 dwEffects;
			public Int32 yHeight;
			public Int32 yOffset;
			public Int32 crTextColor;
			public byte bCharSet;
			public byte bPitchAndFamily;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public char[] szFaceName;

			// CHARFORMAT2 from here onwards.
			public short wWeight;
			public short sSpacing;
			public Int32 crBackColor;
			public uint lcid;
			public uint dwReserved;
			public short sStyle;
			public short wKerning;
			public byte bUnderlineType;
			public byte bAnimation;
			public byte bRevAuthor;
			public byte bReserved1;
		}

		#endregion

		#region Nested type: PARAFORMAT

		[StructLayout(LayoutKind.Sequential)]
		public struct PARAFORMAT
		{
			public int cbSize;
			public uint dwMask;
			public short wNumbering;
			public short wReserved;
			public int dxStartIndent;
			public int dxRightIndent;
			public int dxOffset;
			public short wAlignment;
			public short cTabCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public int[] rgxTabs;

			// PARAFORMAT2 from here onwards.
			public int dySpaceBefore;
			public int dySpaceAfter;
			public int dyLineSpacing;
			public short sStyle;
			public byte bLineSpacingRule;
			public byte bOutlineLevel;
			public short wShadingWeight;
			public short wShadingStyle;
			public short wNumberingStart;
			public short wNumberingStyle;
			public short wNumberingTab;
			public short wBorderSpace;
			public short wBorderWidth;
			public short wBorders;
		}

		#endregion

		#endregion

		#region LINK Extension

		private const int WM_USER = 0x0400;
		private const int EM_GETCHARFORMAT = WM_USER + 58;
		private const int EM_SETCHARFORMAT = WM_USER + 68;

		private const int SCF_SELECTION = 0x0001;
		private const int SCF_WORD = 0x0002;
		private const int SCF_ALL = 0x0004;

		#region CHARFORMAT2 Flags

		private const UInt32 CFE_BOLD = 0x0001;
		private const UInt32 CFE_ITALIC = 0x0002;
		private const UInt32 CFE_UNDERLINE = 0x0004;
		private const UInt32 CFE_STRIKEOUT = 0x0008;
		private const UInt32 CFE_PROTECTED = 0x0010;
		private const UInt32 CFE_LINK = 0x0020;
		private const UInt32 CFE_AUTOCOLOR = 0x40000000;
		private const UInt32 CFE_SUBSCRIPT = 0x00010000;		/* Superscript and subscript are */
		private const UInt32 CFE_SUPERSCRIPT = 0x00020000;		/*  mutually exclusive			 */

		private const int CFM_SMALLCAPS = 0x0040;			/* (*)	*/
		private const int CFM_ALLCAPS = 0x0080;			/* Displayed by 3.0	*/
		private const int CFM_HIDDEN = 0x0100;			/* Hidden by 3.0 */
		private const int CFM_OUTLINE = 0x0200;			/* (*)	*/
		private const int CFM_SHADOW = 0x0400;			/* (*)	*/
		private const int CFM_EMBOSS = 0x0800;			/* (*)	*/
		private const int CFM_IMPRINT = 0x1000;			/* (*)	*/
		private const int CFM_DISABLED = 0x2000;
		private const int CFM_REVISED = 0x4000;

		private const int CFM_BACKCOLOR = 0x04000000;
		private const int CFM_LCID = 0x02000000;
		private const int CFM_UNDERLINETYPE = 0x00800000;		/* Many displayed by 3.0 */
		private const int CFM_WEIGHT = 0x00400000;
		private const int CFM_SPACING = 0x00200000;		/* Displayed by 3.0	*/
		private const int CFM_KERNING = 0x00100000;		/* (*)	*/
		private const int CFM_STYLE = 0x00080000;		/* (*)	*/
		private const int CFM_ANIMATION = 0x00040000;		/* (*)	*/
		private const int CFM_REVAUTHOR = 0x00008000;


		private const UInt32 CFM_BOLD = 0x00000001;
		private const UInt32 CFM_ITALIC = 0x00000002;
		private const UInt32 CFM_UNDERLINE = 0x00000004;
		private const UInt32 CFM_STRIKEOUT = 0x00000008;
		private const UInt32 CFM_PROTECTED = 0x00000010;
		private const UInt32 CFM_LINK = 0x00000020;
		private const UInt32 CFM_SIZE = 0x80000000;
		private const UInt32 CFM_COLOR = 0x40000000;
		private const UInt32 CFM_FACE = 0x20000000;
		private const UInt32 CFM_OFFSET = 0x10000000;
		private const UInt32 CFM_CHARSET = 0x08000000;
		private const UInt32 CFM_SUBSCRIPT = CFE_SUBSCRIPT | CFE_SUPERSCRIPT;
		private const UInt32 CFM_SUPERSCRIPT = CFM_SUBSCRIPT;

		private const byte CFU_UNDERLINENONE = 0x00000000;
		private const byte CFU_UNDERLINE = 0x00000001;
		private const byte CFU_UNDERLINEWORD = 0x00000002; /* (*) displayed as ordinary underline	*/
		private const byte CFU_UNDERLINEDOUBLE = 0x00000003; /* (*) displayed as ordinary underline	*/
		private const byte CFU_UNDERLINEDOTTED = 0x00000004;
		private const byte CFU_UNDERLINEDASH = 0x00000005;
		private const byte CFU_UNDERLINEDASHDOT = 0x00000006;
		private const byte CFU_UNDERLINEDASHDOTDOT = 0x00000007;
		private const byte CFU_UNDERLINEWAVE = 0x00000008;
		private const byte CFU_UNDERLINETHICK = 0x00000009;
		private const byte CFU_UNDERLINEHAIRLINE = 0x0000000A; /* (*) displayed as ordinary underline	*/

		#endregion

		#endregion


		public RichTextBoxEx()
		{
			// Otherwise, non-standard links get lost when user starts typing
			// next to a non-standard link
			this.DetectUrls = false;
		}

		[DefaultValue(false)]
		public new bool DetectUrls
		{
			get { return base.DetectUrls; }
			set { base.DetectUrls = value; }
		}

		/// <summary>
		/// Insert a given text as a link into the RichTextBox at the current insert position.
		/// </summary>
		/// <param name="text">Text to be inserted</param>
		public void InsertLink(string text)
		{
			InsertLink(text, this.SelectionStart);
		}

		/// <summary>
		/// Insert a given text at a given position as a link. 
		/// </summary>
		/// <param name="text">Text to be inserted</param>
		/// <param name="position">Insert position</param>
		public void InsertLink(string text, int position)
		{
			if (position < 0 || position > this.Text.Length)
				throw new ArgumentOutOfRangeException("position");

			this.SelectionStart = position;
			this.SelectedText = text;
			this.Select(position, text.Length);
			this.SetSelectionLink(true);
			this.Select(position + text.Length, 0);
		}

		/// <summary>
		/// Insert a given text at at the current input position as a link.
		/// The link text is followed by a hash (#) and the given hyperlink text, both of
		/// them invisible.
		/// When clicked on, the whole link text and hyperlink string are given in the
		/// LinkClickedEventArgs.
		/// </summary>
		/// <param name="text">Text to be inserted</param>
		/// <param name="hyperlink">Invisible hyperlink string to be inserted</param>
		public void InsertLink(string text, string hyperlink)
		{
			InsertLink(text, hyperlink, this.SelectionStart);
		}

		/// <summary>
		/// Insert a given text at a given position as a link. The link text is followed by
		/// a hash (#) and the given hyperlink text, both of them invisible.
		/// When clicked on, the whole link text and hyperlink string are given in the
		/// LinkClickedEventArgs.
		/// </summary>
		/// <param name="text">Text to be inserted</param>
		/// <param name="hyperlink">Invisible hyperlink string to be inserted</param>
		/// <param name="position">Insert position</param>
		public void InsertLink(string text, string hyperlink, int position)
		{
			if (position < 0 || position > this.Text.Length)
				throw new ArgumentOutOfRangeException("position");

			this.SelectionStart = position;
			this.SelectedRtf = @"{\rtf1\ansi\deff0{\fonttbl{\f0\fnil\fcharset129 \'b1\'bc\'b8\'b2;}}" + "\n" +
							   @"\viewkind4\uc1\pard\lang1042\f0\fs18 " + text + @"\v #" + hyperlink + @"\v0}";
			this.Select(position, text.Length + hyperlink.Length + 1);
			this.SetSelectionLink(true);
			this.Select(position + text.Length + hyperlink.Length + 1, 0);
		}

		/// <summary>
		/// Set the current selection's link style
		/// </summary>
		/// <param name="link">true: set link style, false: clear link style</param>
		public void SetSelectionLink(bool link)
		{
			SetSelectionStyle(CFM_LINK, link ? CFE_LINK : 0);
		}
		/// <summary>
		/// Get the link style for the current selection
		/// </summary>
		/// <returns>0: link style not set, 1: link style set, -1: mixed</returns>
		public int GetSelectionLink()
		{
			return GetSelectionStyle(CFM_LINK, CFE_LINK);
		}


		private void SetSelectionStyle(UInt32 mask, UInt32 effect)
		{
			CHARFORMAT cf = new CHARFORMAT();
			cf.cbSize = Marshal.SizeOf(cf);
			cf.dwMask = mask;
			cf.dwEffects = effect;

			IntPtr wpar = new IntPtr(SCF_SELECTION);
			IntPtr lpar = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf));
			Marshal.StructureToPtr(cf, lpar, false);

			IntPtr res = SendMessage(Handle, EM_SETCHARFORMAT, wpar, lpar);

			Marshal.FreeCoTaskMem(lpar);
		}

		private int GetSelectionStyle(UInt32 mask, UInt32 effect)
		{
			CHARFORMAT cf = new CHARFORMAT();
			cf.cbSize = Marshal.SizeOf(cf);
			cf.szFaceName = new char[32];

			IntPtr wpar = new IntPtr(SCF_SELECTION);
			IntPtr lpar = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf));
			Marshal.StructureToPtr(cf, lpar, false);

			SendMessage(Handle, EM_GETCHARFORMAT, wpar, lpar);

			cf = (CHARFORMAT)Marshal.PtrToStructure(lpar, typeof(CHARFORMAT));

			int state;
			// dwMask holds the information which properties are consistent throughout the selection:
			if ((cf.dwMask & mask) == mask)
			{
				if ((cf.dwEffects & effect) == effect)
					state = 1;
				else
					state = 0;
			}
			else
			{
				state = -1;
			}

			Marshal.FreeCoTaskMem(lpar);
			return state;
		}


		public void InsertOleObject(IOleObject oleObj)
		{
			RichEditOle ole = new RichEditOle(this);
			ole.InsertOleObject(oleObj);
		}

		public void InsertControl(Control control)
		{
			RichEditOle ole = new RichEditOle(this);
			ole.InsertControl(control);
		}

		public void InsertMyDataObject(myDataObject mdo)
		{
			RichEditOle ole = new RichEditOle(this);
			ole.InsertMyDataObject(mdo);
		}

		public void UpdateObjects()
		{
			RichEditOle ole = new RichEditOle(this);
			ole.UpdateObjects();
		}

		public void InsertImage(Image image)
		{
			myDataObject mdo = new myDataObject();

			mdo.SetImage(image);

			InsertMyDataObject(mdo);
		}

		public void InsertImage(string imageFile)
		{
			myDataObject mdo = new myDataObject();

			mdo.SetImage(imageFile);

			InsertMyDataObject(mdo);
		}

		public void InsertImageFromFile(string strFilename)
		{
			RichEditOle ole = new RichEditOle(this);
			ole.InsertImageFromFile(strFilename);
		}

		public void InsertActiveX(string strProgID)
		{
			Type t = Type.GetTypeFromProgID(strProgID);
			if (t == null)
				return;

			object o = Activator.CreateInstance(t);

			bool b = (o is IOleObject);

			if (b)
				InsertOleObject((IOleObject) o);
		}

		// RichEditOle wrapper and helper

		#region Nested type: RichEditOle

		private class RichEditOle
		{
			public const int EM_GETOLEINTERFACE = WM_USER + 60;
			public const int WM_USER = 0x0400;

			private readonly RichTextBoxEx _richEdit;
			private IRichEditOle _RichEditOle;

			public RichEditOle(RichTextBoxEx richEdit)
			{
				_richEdit = richEdit;
			}

			private IRichEditOle IRichEditOle
			{
				get
				{
					if (_RichEditOle == null)
					{
						_RichEditOle = SendMessage(_richEdit.Handle, EM_GETOLEINTERFACE, 0);
					}

					return _RichEditOle;
				}
			}

			[DllImport("ole32.dll", PreserveSig = false)]
			internal static extern int CreateILockBytesOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease,
			                                                     [Out] out ILockBytes ppLkbyt);

			[DllImport("ole32.dll")]
			private static extern int StgCreateDocfileOnILockBytes(ILockBytes plkbyt, uint grfMode,
			                                                       uint reserved, out IStorage ppstgOpen);

			public void InsertControl(Control control)
			{
				if (control == null)
					return;

				Guid guid = Marshal.GenerateGuidForType(control.GetType());

				//-----------------------
				ILockBytes pLockBytes;
				CreateILockBytesOnHGlobal(IntPtr.Zero, true, out pLockBytes);

				IStorage pStorage;
				StgCreateDocfileOnILockBytes(pLockBytes, (uint) (STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_CREATE | STGM.STGM_READWRITE),
				                             0, out pStorage);

				IOleClientSite pOleClientSite;
				IRichEditOle.GetClientSite(out pOleClientSite);
				//-----------------------

				//-----------------------
				REOBJECT reoObject = new REOBJECT();

				reoObject.cp = _richEdit.TextLength;

				reoObject.clsid = guid;
				reoObject.pstg = pStorage;
				reoObject.poleobj = Marshal.GetIUnknownForObject(control);
				reoObject.polesite = pOleClientSite;
				reoObject.dvAspect = (uint) (DVASPECT.DVASPECT_CONTENT);
				reoObject.dwFlags = (uint) (REOOBJECTFLAGS.REO_BELOWBASELINE);
				reoObject.dwUser = 1;

				IRichEditOle.InsertObject(reoObject);
				//-----------------------

				//-----------------------
				Marshal.ReleaseComObject(pLockBytes);
				Marshal.ReleaseComObject(pOleClientSite);
				Marshal.ReleaseComObject(pStorage);
				//-----------------------
			}

			public bool InsertImageFromFile(string strFilename)
			{
				//-----------------------
				ILockBytes pLockBytes;
				CreateILockBytesOnHGlobal(IntPtr.Zero, true, out pLockBytes);

				IStorage pStorage;
				StgCreateDocfileOnILockBytes(pLockBytes, (uint) (STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_CREATE | STGM.STGM_READWRITE),
				                             0, out pStorage);

				IOleClientSite pOleClientSite;
				IRichEditOle.GetClientSite(out pOleClientSite);
				//-----------------------


				//-----------------------
				FORMATETC formatEtc = new FORMATETC();

				formatEtc.cfFormat = 0;
				formatEtc.ptd = IntPtr.Zero;
				formatEtc.dwAspect = DVASPECT.DVASPECT_CONTENT;
				formatEtc.lindex = -1;
				formatEtc.tymed = TYMED.TYMED_NULL;

				Guid IID_IOleObject = new Guid("{00000112-0000-0000-C000-000000000046}");
				Guid CLSID_NULL = new Guid("{00000000-0000-0000-0000-000000000000}");

				object pOleObjectOut;

				// I don't sure, but it appears that this function only loads from bitmap
				// You can also try OleCreateFromData, OleLoadPictureIndirect, etc.
				int hr = OleCreateFromFile(ref CLSID_NULL, strFilename, ref IID_IOleObject, (uint) OLERENDER.OLERENDER_DRAW,
				                           ref formatEtc, pOleClientSite, pStorage, out pOleObjectOut);

				if (pOleObjectOut == null)
				{
					Marshal.ReleaseComObject(pLockBytes);
					Marshal.ReleaseComObject(pOleClientSite);
					Marshal.ReleaseComObject(pStorage);

					return false;
				}

				IOleObject pOleObject = (IOleObject) pOleObjectOut;
				//-----------------------


				//-----------------------
				Guid guid = new Guid();

				//guid = Marshal.GenerateGuidForType(pOleObject.GetType());
				pOleObject.GetUserClassID(ref guid);
				//-----------------------

				//-----------------------
				OleSetContainedObject(pOleObject, true);

				REOBJECT reoObject = new REOBJECT();

				reoObject.cp = _richEdit.TextLength;

				reoObject.clsid = guid;
				reoObject.pstg = pStorage;
				reoObject.poleobj = Marshal.GetIUnknownForObject(pOleObject);
				reoObject.polesite = pOleClientSite;
				reoObject.dvAspect = (uint) (DVASPECT.DVASPECT_CONTENT);
				reoObject.dwFlags = (uint) (REOOBJECTFLAGS.REO_BELOWBASELINE);
				reoObject.dwUser = 0;

				IRichEditOle.InsertObject(reoObject);
				//-----------------------

				//-----------------------
				Marshal.ReleaseComObject(pLockBytes);
				Marshal.ReleaseComObject(pOleClientSite);
				Marshal.ReleaseComObject(pStorage);
				Marshal.ReleaseComObject(pOleObject);
				//-----------------------

				return true;
			}

			public void InsertMyDataObject(myDataObject mdo)
			{
				if (mdo == null)
					return;

				//-----------------------
				ILockBytes pLockBytes;
				int sc = CreateILockBytesOnHGlobal(IntPtr.Zero, true, out pLockBytes);

				IStorage pStorage;
				sc = StgCreateDocfileOnILockBytes(pLockBytes,
				                                  (uint) (STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_CREATE | STGM.STGM_READWRITE), 0,
				                                  out pStorage);

				IOleClientSite pOleClientSite;
				IRichEditOle.GetClientSite(out pOleClientSite);
				//-----------------------

				Guid guid = Marshal.GenerateGuidForType(mdo.GetType());

				Guid IID_IOleObject = new Guid("{00000112-0000-0000-C000-000000000046}");
				Guid IID_IDataObject2 = new Guid("{0000010e-0000-0000-C000-000000000046}");
				Guid IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");

				object pOleObject;

				int hr = OleCreateStaticFromData(mdo, ref IID_IOleObject, (uint) OLERENDER.OLERENDER_FORMAT, ref mdo.mpFormatetc,
				                                 pOleClientSite, pStorage, out pOleObject);

				if (pOleObject == null)
					return;
				//-----------------------


				//-----------------------
				OleSetContainedObject(pOleObject, true);

				REOBJECT reoObject = new REOBJECT();

				reoObject.cp = _richEdit.TextLength;

				reoObject.clsid = guid;
				reoObject.pstg = pStorage;
				reoObject.poleobj = Marshal.GetIUnknownForObject(pOleObject);
				reoObject.polesite = pOleClientSite;
				reoObject.dvAspect = (uint) (DVASPECT.DVASPECT_CONTENT);
				reoObject.dwFlags = (uint) (REOOBJECTFLAGS.REO_BELOWBASELINE);
				reoObject.dwUser = 0;

				IRichEditOle.InsertObject(reoObject);
				//-----------------------

				//-----------------------
				Marshal.ReleaseComObject(pLockBytes);
				Marshal.ReleaseComObject(pOleClientSite);
				Marshal.ReleaseComObject(pStorage);
				Marshal.ReleaseComObject(pOleObject);
				//-----------------------
			}

			public void InsertOleObject(IOleObject oleObject)
			{
				if (oleObject == null)
					return;

				//-----------------------
				ILockBytes pLockBytes;
				CreateILockBytesOnHGlobal(IntPtr.Zero, true, out pLockBytes);

				IStorage pStorage;
				StgCreateDocfileOnILockBytes(pLockBytes, (uint) (STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_CREATE | STGM.STGM_READWRITE),
				                             0, out pStorage);

				IOleClientSite pOleClientSite;
				IRichEditOle.GetClientSite(out pOleClientSite);
				//-----------------------

				//-----------------------
				Guid guid = new Guid();

				oleObject.GetUserClassID(ref guid);
				//-----------------------

				//-----------------------
				OleSetContainedObject(oleObject, true);

				REOBJECT reoObject = new REOBJECT();

				reoObject.cp = _richEdit.TextLength;

				reoObject.clsid = guid;
				reoObject.pstg = pStorage;
				reoObject.poleobj = Marshal.GetIUnknownForObject(oleObject);
				reoObject.polesite = pOleClientSite;
				reoObject.dvAspect = (uint) DVASPECT.DVASPECT_CONTENT;
				reoObject.dwFlags = (uint) REOOBJECTFLAGS.REO_BELOWBASELINE;

				IRichEditOle.InsertObject(reoObject);
				//-----------------------

				//-----------------------
				Marshal.ReleaseComObject(pLockBytes);
				Marshal.ReleaseComObject(pOleClientSite);
				Marshal.ReleaseComObject(pStorage);
				//-----------------------
			}

			public void UpdateObjects()
			{
				int k = IRichEditOle.GetObjectCount();

				for (int i = 0; i < k; i++)
				{
					REOBJECT reoObject = new REOBJECT();

					IRichEditOle.GetObject(i, reoObject, GETOBJECTOPTIONS.REO_GETOBJ_ALL_INTERFACES);

					if (reoObject.dwUser == 1)
					{
						Point pt = _richEdit.GetPositionFromCharIndex(reoObject.cp);
						Rectangle rect = new Rectangle(pt, reoObject.sizel);

						_richEdit.Invalidate(rect, false); // repaint
					}
				}
			}
		}

		#endregion
	}
}