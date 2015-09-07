using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace PBDotNetLib.orca
{
    /// <summary>
    /// wrapper to export objects from pbl with orca
    /// </summary>
    public class Orca
    {
        public enum Version {
            PB115,
            PB125,
            PB126
        }

        private Version currentVersion;

        #region private
        private List<LibEntry> libEntries;
        private string currentLibrary = null;
        private static int session = 0;
        #endregion

        /// <summary>
        /// object types in orca/pb lib
        /// </summary>
        private enum PBORCA_ENTRY_TYPE
        {
            PBORCA_APPLICATION,
            PBORCA_DATAWINDOW,
            PBORCA_FUNCTION,
            PBORCA_MENU,
            PBORCA_QUERY,
            PBORCA_STRUCTURE,
            PBORCA_USEROBJECT,
            PBORCA_WINDOW,
            PBORCA_PIPELINE,
            PBORCA_PROJECT,
            PBORCA_PROXYOBJECT,
            PBORCA_BINARY
        }

        #region PB12.5

        [DllImport("pborc125.dll", EntryPoint = "PBORCA_SessionOpen", CharSet = CharSet.Unicode, SetLastError = true)]
        private static unsafe extern int PBORCA_SessionOpen125();

        [DllImport("pborc125.dll", EntryPoint = "PBORCA_SessionClose", CharSet = CharSet.Unicode, SetLastError = true)]
        private static unsafe extern void PBORCA_SessionClose125(int hORCASession);

        [DllImport("pborc125.dll", EntryPoint = "PBORCA_LibraryCreate", CharSet = CharSet.Unicode, SetLastError = true)]
        private static unsafe extern int PBORCA_LibraryCreate125(int hORCASession, [MarshalAs(UnmanagedType.LPTStr)] string lpszLibName, [MarshalAs(UnmanagedType.LPTStr)] string lpszLibComment);

        [DllImport("pborc125.dll", CharSet = CharSet.Auto)]
        private static extern int PBORCA_LibraryEntryExport125(
            int hORCASession,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszLibraryName,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszEntryName,
            PBORCA_ENTRY_TYPE otEntryType,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszExportBuffer,
            System.Int32 lExportBufferSize);

        [DllImport("pborc125.dll", EntryPoint = "PBORCA_LibraryDirectory", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int PBORCA_LibraryDirectory125(
            int hORCASession,
            [MarshalAs(UnmanagedType.LPTStr)] string lpszLibName,
            [MarshalAs(UnmanagedType.LPTStr)] string lpszLibComments,
            int iCmntsBufflen,
            PBORCA_LIBDIRCALLBACK pListProc,
            IntPtr pUserData
        );

        [DllImport("pborc125.dll", EntryPoint = "PBORCA_DynamicLibraryCreate", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int PBORCA_DynamicLibraryCreate125(
            int hORCASession,
            [MarshalAs(UnmanagedType.LPTStr)] string lpszLibName,
            [MarshalAs(UnmanagedType.LPTStr)] string lpszPbrName,
            IntPtr lFlags);

        #endregion PB12.5

        #region PB12.6

        [DllImport("pborc126.dll", EntryPoint = "PBORCA_SessionOpen", CharSet = CharSet.Unicode, SetLastError = true)]
        private static unsafe extern int PBORCA_SessionOpen126();

        [DllImport("pborc126.dll", EntryPoint = "PBORCA_SessionClose", CharSet = CharSet.Unicode, SetLastError = true)]
        private static unsafe extern void PBORCA_SessionClose126(int hORCASession);

        [DllImport("pborc126.dll", EntryPoint = "PBORCA_LibraryCreate", CharSet = CharSet.Unicode, SetLastError = true)]
        private static unsafe extern int PBORCA_LibraryCreate126(int hORCASession, [MarshalAs(UnmanagedType.LPTStr)] string lpszLibName, [MarshalAs(UnmanagedType.LPTStr)] string lpszLibComment);

        [DllImport("pborc126.dll", CharSet = CharSet.Auto, EntryPoint = "PBORCA_LibraryEntryExport")]
        private static extern int PBORCA_LibraryEntryExport126(
            int hORCASession,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszLibraryName,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszEntryName,
            PBORCA_ENTRY_TYPE otEntryType,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszExportBuffer,
            System.Int32 lExportBufferSize);

        [DllImport("pborc126.dll", EntryPoint = "PBORCA_LibraryDirectory", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int PBORCA_LibraryDirectory126(
            int hORCASession,
            [MarshalAs(UnmanagedType.LPTStr)] string lpszLibName,
            [MarshalAs(UnmanagedType.LPTStr)] string lpszLibComments,
            int iCmntsBufflen,
            PBORCA_LIBDIRCALLBACK pListProc,
            IntPtr pUserData
        );

        [DllImport("pborc126.dll", EntryPoint = "PBORCA_DynamicLibraryCreate", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int PBORCA_DynamicLibraryCreate126(
            int hORCASession,
            [MarshalAs(UnmanagedType.LPTStr)] string lpszLibName,
            [MarshalAs(UnmanagedType.LPTStr)] string lpszPbrName,
            IntPtr lFlags);

        #endregion PB12.6

        #region PB11.5

        [DllImport("pborc115.dll", EntryPoint = "PBORCA_SessionOpen", CharSet = CharSet.Unicode, SetLastError = true)]
        private static unsafe extern int PBORCA_SessionOpen115();

        [DllImport("pborc115.dll", EntryPoint = "PBORCA_SessionClose", CharSet = CharSet.Unicode, SetLastError = true)]
        private static unsafe extern void PBORCA_SessionClose115(int hORCASession);

        [DllImport("pborc115.dll", EntryPoint = "PBORCA_LibraryCreate", CharSet = CharSet.Unicode, SetLastError = true)]
        private static unsafe extern int PBORCA_LibraryCreate115(int hORCASession, [MarshalAs(UnmanagedType.LPTStr)] string lpszLibName, [MarshalAs(UnmanagedType.LPTStr)] string lpszLibComment);

        [DllImport("pborc115.dll", EntryPoint = "PBORCA_LibraryEntryExport", CharSet = CharSet.Auto)]
        private static extern int PBORCA_LibraryEntryExport115(
            int hORCASession,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszLibraryName,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszEntryName,
            PBORCA_ENTRY_TYPE otEntryType,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszExportBuffer,
            System.Int32 lExportBufferSize);

        [DllImport("pborc115.dll", EntryPoint = "PBORCA_LibraryDirectory", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int PBORCA_LibraryDirectory115(
            int hORCASession,
            [MarshalAs(UnmanagedType.LPTStr)] string lpszLibName,
            [MarshalAs(UnmanagedType.LPTStr)] string lpszLibComments,
            int iCmntsBufflen,
            PBORCA_LIBDIRCALLBACK pListProc,
            IntPtr pUserData
        );

        [DllImport("pborc115.dll", EntryPoint = "PBORCA_DynamicLibraryCreate", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int PBORCA_DynamicLibraryCreate115(
            int hORCASession,
            [MarshalAs(UnmanagedType.LPTStr)] string lpszLibName,
            [MarshalAs(UnmanagedType.LPTStr)] string lpszPbrName,
            IntPtr lFlags);

        #endregion PB11.5

        #region extern and unsafe

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
        private struct PBORCA_DIRENTRY
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szComments;
            public int lCreateTime;
            public int lEntrySize;
            public string lpszEntryName;
            public PBORCA_ENTRY_TYPE otEntryType;
        }

        private delegate void PBORCA_LIBDIRCALLBACK(IntPtr pDirEntry,IntPtr lpUserData);

        private void PBORCA_LibDirCallback(IntPtr pDirEntry, IntPtr lpUserData)
        {
            PBORCA_DIRENTRY myDirEntry = (PBORCA_DIRENTRY)Marshal.PtrToStructure(pDirEntry,typeof(PBORCA_DIRENTRY));
            DateTime myDateTime = new DateTime(1970, 01, 01, 00, 00, 00).AddSeconds((double) myDirEntry.lCreateTime);

            libEntries.Add(new LibEntry(myDirEntry.lpszEntryName, GetObjecttype(myDirEntry.otEntryType), myDateTime, myDirEntry.lEntrySize, currentLibrary, this.currentVersion, myDirEntry.szComments));
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="version">PB Version to use</param>
        public Orca(Version version)
        {
            this.currentVersion = version;

            if (session == 0)
                SessionOpen();
        }

        /// <summary>
        /// converts the Objecttype to PBORCA_ENTRY_TYPE
        /// </summary>
        /// <param name="type">Objecttype</param>
        /// <returns></returns>
        private PBORCA_ENTRY_TYPE GetEntryType(Objecttype type)
        {
            PBORCA_ENTRY_TYPE entryType = PBORCA_ENTRY_TYPE.PBORCA_BINARY;

            switch (type)
            {
                case Objecttype.Application:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_APPLICATION;
                    break;
                case Objecttype.Binary:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_BINARY;
                    break;
                case Objecttype.Datawindow:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_DATAWINDOW;
                    break;
                case Objecttype.Function:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_FUNCTION;
                    break;
                case Objecttype.Menu:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_MENU;
                    break;
                case Objecttype.Pipeline:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_PIPELINE;
                    break;
                case Objecttype.Project:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_PROJECT;
                    break;
                case Objecttype.Proxyobject:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_PROXYOBJECT;
                    break;
                case Objecttype.Query:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_QUERY;
                    break;
                case Objecttype.Structure:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_STRUCTURE;
                    break;
                case Objecttype.Userobject:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_USEROBJECT;
                    break;
                case Objecttype.Window:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_WINDOW;
                    break;
            }

            return entryType;
        }

        /// <summary>
        /// converts the PBORCA_ENTRY_TYPE to Objecttype
        /// </summary>
        /// <param name="entryType"></param>
        /// <returns></returns>
        private Objecttype GetObjecttype(PBORCA_ENTRY_TYPE entryType)
        {
            Objecttype type = Objecttype.None;

            switch (entryType)
            {
                case PBORCA_ENTRY_TYPE.PBORCA_APPLICATION:
                    type = Objecttype.Application;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_BINARY:
                    type = Objecttype.Binary;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_DATAWINDOW:
                    type = Objecttype.Datawindow;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_FUNCTION:
                    type = Objecttype.Function;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_MENU:
                    type = Objecttype.Menu;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_PIPELINE:
                    type = Objecttype.Pipeline;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_PROJECT:
                    type = Objecttype.Project;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_PROXYOBJECT:
                    type = Objecttype.Proxyobject;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_QUERY:
                    type = Objecttype.Query;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_STRUCTURE:
                    type = Objecttype.Structure;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_USEROBJECT:
                    type = Objecttype.Userobject;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_WINDOW:
                    type = Objecttype.Window;
                    break;
            }

            return type;
        }

        public void CreateDynamicLibrary(string file, string pbrFile)
        {
            int orcaSession = 0;

            if (orcaSession == 0) {
                switch (this.currentVersion) {
                    case Version.PB115:
                        orcaSession = PBORCA_SessionOpen115();
                        break;
                    case Version.PB125:
                        orcaSession = PBORCA_SessionOpen125();
                        break;
                    case Version.PB126:
                        orcaSession = PBORCA_SessionOpen126();
                        break;
                }
            } else {
                orcaSession = session;
            }

            switch (this.currentVersion) {
                case Version.PB115:
                    PBORCA_DynamicLibraryCreate115(orcaSession, file, pbrFile, IntPtr.Zero);
                    break;
                case Version.PB125:
                    PBORCA_DynamicLibraryCreate125(orcaSession, file, pbrFile, IntPtr.Zero);
                    break;
                case Version.PB126:
                    PBORCA_DynamicLibraryCreate126(orcaSession, file, pbrFile, IntPtr.Zero);
                    break;
            }

            if (session == 0) {
                switch (this.currentVersion) {
                    case Version.PB115:
                        PBORCA_SessionClose115(orcaSession);
                        break;
                    case Version.PB125:
                        PBORCA_SessionClose125(orcaSession);
                        break;
                    case Version.PB126:
                        PBORCA_SessionClose126(orcaSession);
                        break;
                }
            }
        }

        /// <summary>
        /// creates a new pbl
        /// </summary>
        /// <param name="file">path to new pbl</param>
        /// <param name="comment">comment for thew lib</param>
        public void CreateLibrary(string file, string comment = "")
        {
            int orcaSession = 0;

            if (session == 0){
                switch(this.currentVersion){
                    case Version.PB115:
                        orcaSession = PBORCA_SessionOpen115();
                        break;
                    case Version.PB125:
                        orcaSession = PBORCA_SessionOpen125();
                        break;
                    case Version.PB126:
                        orcaSession = PBORCA_SessionOpen126();
                        break;
                }
            }                
            else
                orcaSession = session;

            switch (this.currentVersion) {
                case Version.PB115:
                    PBORCA_LibraryCreate115(orcaSession, file, comment);
                    break;
                case Version.PB125:
                    PBORCA_LibraryCreate125(orcaSession, file, comment);
                    break;
                case Version.PB126:
                    PBORCA_LibraryCreate126(orcaSession, file, comment);
                    break;
            }


            if (session == 0) {
                switch (this.currentVersion) {
                    case Version.PB115:
                        PBORCA_SessionClose115(orcaSession);
                        break;
                    case Version.PB125:
                        PBORCA_SessionClose125(orcaSession);
                        break;
                    case Version.PB126:
                        PBORCA_SessionClose126(orcaSession);
                        break;
                }                
            }
        }

        /// <summary>
        /// lists a library
        /// </summary>
        /// <param name="file">path to librarys</param>
        /// <returns>list of entries</returns>
        public List<LibEntry> DirLibrary(string file)
        {
            int orcaSession = 0;
            PBORCA_LIBDIRCALLBACK PBORCA_LibraryDirectoryCallback = new PBORCA_LIBDIRCALLBACK(PBORCA_LibDirCallback);
            IntPtr dummy = new IntPtr();

            currentLibrary = file;
            libEntries = new List<LibEntry>();

            if (session == 0) {
                switch (this.currentVersion) {
                    case Version.PB115:
                        orcaSession = PBORCA_SessionOpen115();
                        break;
                    case Version.PB125:
                        orcaSession = PBORCA_SessionOpen125();
                        break;
                    case Version.PB126:
                        orcaSession = PBORCA_SessionOpen126();
                        break;
                }
            } else
                orcaSession = session;

            switch (this.currentVersion) { 
                case Version.PB115:
                    PBORCA_LibraryDirectory115(orcaSession, file, "", 0, PBORCA_LibraryDirectoryCallback, dummy);
                    break;
                case Version.PB125:
                    PBORCA_LibraryDirectory125(orcaSession, file, "", 0, PBORCA_LibraryDirectoryCallback, dummy);
                    break;
                case Version.PB126:
                    PBORCA_LibraryDirectory126(orcaSession, file, "", 0, PBORCA_LibraryDirectoryCallback, dummy);
                    break;
            }

            if (session == 0) {
                switch (this.currentVersion) {
                    case Version.PB115:
                        PBORCA_SessionClose115(orcaSession);
                        break;
                    case Version.PB125:
                        PBORCA_SessionClose125(orcaSession);
                        break;
                    case Version.PB126:
                        PBORCA_SessionClose126(orcaSession);
                        break;
                }
            }

            return libEntries;
        }

        /// <summary>
        /// reads the source of an object
        /// </summary>
        /// <param name="libEntry">library entry to export</param>
        public void FillCode(LibEntry libEntry)
        {
            int orcaSession = 0;
            StringBuilder sbSource = new StringBuilder(5242880); // 5 MB

            if (session == 0) {
                switch (this.currentVersion) {
                    case Version.PB115:
                        orcaSession = PBORCA_SessionOpen115();
                        break;
                    case Version.PB125:
                        orcaSession = PBORCA_SessionOpen125();
                        break;
                    case Version.PB126:
                        orcaSession = PBORCA_SessionOpen126();
                        break;
                }
            } else
                orcaSession = session;

            switch (this.currentVersion) {
                case Version.PB115:
                    PBORCA_LibraryEntryExport115(orcaSession, libEntry.Library, libEntry.Name, GetEntryType(libEntry.Type), sbSource, sbSource.Capacity);
                    break;
                case Version.PB125:
                    PBORCA_LibraryEntryExport125(orcaSession, libEntry.Library, libEntry.Name, GetEntryType(libEntry.Type), sbSource, sbSource.Capacity);
                    break;
                case Version.PB126:
                    PBORCA_LibraryEntryExport126(orcaSession, libEntry.Library, libEntry.Name, GetEntryType(libEntry.Type), sbSource, sbSource.Capacity);
                    break;
            }
            

            libEntry.Source = sbSource.ToString();

            if (session == 0) {
                switch (this.currentVersion) {
                    case Version.PB115:
                        PBORCA_SessionClose115(orcaSession);
                        break;
                    case Version.PB125:
                        PBORCA_SessionClose125(orcaSession);
                        break;
                    case Version.PB126:
                        PBORCA_SessionClose126(orcaSession);
                        break;
                }
            }
        }

        public void SessionOpen()
        {
            switch (this.currentVersion) {
                case Version.PB115:
                    session = PBORCA_SessionOpen115();
                    break;
                case Version.PB125:
                    session = PBORCA_SessionOpen125();
                    break;
                case Version.PB126:
                    session = PBORCA_SessionOpen126();
                    break;
            }
            
        }

        public void SessionClose()
        {
            switch (this.currentVersion) {
                case Version.PB115:
                    PBORCA_SessionClose115(session);
                    break;
                case Version.PB125:
                    PBORCA_SessionClose125(session);
                    break;
                case Version.PB126:
                    PBORCA_SessionClose126(session);
                    break;
            }
            
        }

        /*
         * 
         *   Code  Description
       ----  -----------------------------------
          0  Operation successful
         -1  Invalid parameter list
         -2  Duplicate operation
         -3  Object not found
         -4  Bad library name
         -5  Library list not set
         -6  Library not in library list
         -7  Library I/O error
         -8  Object exists
         -9  Invalid name
        -10  Buffer size is too small
        -11  Compile error
        -12  Link error
        -13  Current application not set
        -14  Object has no ancestors
        -15  Object has no references
        -16  Invalid # of PBDs
        -17  PBD create error
        -18  Source Management error
        */

    }
}

