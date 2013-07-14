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

        #region extern and unsafe

        [DllImport("pborc125.dll", EntryPoint = "PBORCA_SessionOpen", CharSet = CharSet.Unicode, SetLastError = true)]
        private static unsafe extern int PBORCA_SessionOpen();

        [DllImport("pborc125.dll", EntryPoint = "PBORCA_SessionClose", CharSet = CharSet.Unicode, SetLastError = true)]
        private static unsafe extern void PBORCA_SessionClose(int hORCASession);

        [DllImport("pborc125.dll", EntryPoint = "PBORCA_LibraryCreate", CharSet = CharSet.Unicode, SetLastError = true)]
        private static unsafe extern int PBORCA_LibraryCreate(int hORCASession, [MarshalAs(UnmanagedType.LPTStr)] string lpszLibName, [MarshalAs(UnmanagedType.LPTStr)] string lpszLibComment);

        [DllImport("pborc125.dll", CharSet = CharSet.Auto)]
        private static extern int PBORCA_LibraryEntryExport(
            int hORCASession,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszLibraryName,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszEntryName,
            PBORCA_ENTRY_TYPE otEntryType,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszExportBuffer,
            System.Int32 lExportBufferSize);

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

        [DllImport("pborc125.dll", EntryPoint = "PBORCA_LibraryDirectory", CharSet = CharSet.Unicode, SetLastError = true)]      
        private static extern int PBORCA_LibraryDirectory(
            int hORCASession,
            [MarshalAs(UnmanagedType.LPTStr)] string lpszLibName,
            [MarshalAs(UnmanagedType.LPTStr)] string lpszLibComments,
            int iCmntsBufflen,
            PBORCA_LIBDIRCALLBACK pListProc,
            IntPtr pUserData
        );

        private void PBORCA_LibDirCallback(IntPtr pDirEntry, IntPtr lpUserData)
        {
            PBORCA_DIRENTRY myDirEntry = (PBORCA_DIRENTRY)Marshal.PtrToStructure(pDirEntry,typeof(PBORCA_DIRENTRY));
            DateTime myDateTime = new DateTime(1970, 01, 01, 00, 00, 00).AddSeconds((double) myDirEntry.lCreateTime);

            libEntries.Add(new LibEntry(myDirEntry.lpszEntryName, GetObjecttype(myDirEntry.otEntryType), myDateTime, myDirEntry.lEntrySize, currentLibrary, myDirEntry.szComments));
        }
        #endregion


        public Orca()
        {
            if (session == 0)
                SessionOpen();
        }

        /// <summary>
        /// converts the LibEntry.Objecttype to PBORCA_ENTRY_TYPE
        /// </summary>
        /// <param name="type">LibEntry.Objecttype</param>
        /// <returns></returns>
        private PBORCA_ENTRY_TYPE GetEntryType(LibEntry.Objecttype type)
        {
            PBORCA_ENTRY_TYPE entryType = PBORCA_ENTRY_TYPE.PBORCA_BINARY;

            switch (type)
            {
                case LibEntry.Objecttype.Application:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_APPLICATION;
                    break;
                case LibEntry.Objecttype.Binary:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_BINARY;
                    break;
                case LibEntry.Objecttype.Datawindow:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_DATAWINDOW;
                    break;
                case LibEntry.Objecttype.Function:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_FUNCTION;
                    break;
                case LibEntry.Objecttype.Menu:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_MENU;
                    break;
                case LibEntry.Objecttype.Pipeline:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_PIPELINE;
                    break;
                case LibEntry.Objecttype.Project:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_PROJECT;
                    break;
                case LibEntry.Objecttype.Proxyobject:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_PROXYOBJECT;
                    break;
                case LibEntry.Objecttype.Query:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_QUERY;
                    break;
                case LibEntry.Objecttype.Structure:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_STRUCTURE;
                    break;
                case LibEntry.Objecttype.Userobject:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_USEROBJECT;
                    break;
                case LibEntry.Objecttype.Window:
                    entryType = PBORCA_ENTRY_TYPE.PBORCA_WINDOW;
                    break;
            }

            return entryType;
        }

        /// <summary>
        /// converts the PBORCA_ENTRY_TYPE to LibEntry.Objecttype
        /// </summary>
        /// <param name="entryType"></param>
        /// <returns></returns>
        private LibEntry.Objecttype GetObjecttype(PBORCA_ENTRY_TYPE entryType)
        {
            LibEntry.Objecttype type = LibEntry.Objecttype.None;

            switch (entryType)
            {
                case PBORCA_ENTRY_TYPE.PBORCA_APPLICATION:
                    type = LibEntry.Objecttype.Application;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_BINARY:
                    type = LibEntry.Objecttype.Binary;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_DATAWINDOW:
                    type = LibEntry.Objecttype.Datawindow;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_FUNCTION:
                    type = LibEntry.Objecttype.Function;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_MENU:
                    type = LibEntry.Objecttype.Menu;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_PIPELINE:
                    type = LibEntry.Objecttype.Pipeline;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_PROJECT:
                    type = LibEntry.Objecttype.Project;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_PROXYOBJECT:
                    type = LibEntry.Objecttype.Proxyobject;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_QUERY:
                    type = LibEntry.Objecttype.Query;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_STRUCTURE:
                    type = LibEntry.Objecttype.Structure;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_USEROBJECT:
                    type = LibEntry.Objecttype.Userobject;
                    break;
                case PBORCA_ENTRY_TYPE.PBORCA_WINDOW:
                    type = LibEntry.Objecttype.Window;
                    break;
            }

            return type;
        }

        /// <summary>
        /// creates a new pbl
        /// </summary>
        /// <param name="file">path to new pbl</param>
        /// <param name="comment">comment for thew lib</param>
        public void CreateLibrary(string file, string comment = "")
        {
            int orcaSession;

            if (session == 0)
                orcaSession = PBORCA_SessionOpen();
            else
                orcaSession = session;
            
            PBORCA_LibraryCreate(orcaSession, file, comment);
            
            if(session == 0 )
                PBORCA_SessionClose(orcaSession);
        }

        /// <summary>
        /// lists a library
        /// </summary>
        /// <param name="file">path to librarys</param>
        /// <returns>list of entries</returns>
        public List<LibEntry> DirLibrary(string file)
        {
            int orcaSession;
            PBORCA_LIBDIRCALLBACK PBORCA_LibraryDirectoryCallback = new PBORCA_LIBDIRCALLBACK(PBORCA_LibDirCallback);
            IntPtr dummy = new IntPtr();

            currentLibrary = file;
            libEntries = new List<LibEntry>();

            if (session == 0)
                orcaSession = PBORCA_SessionOpen();
            else
                orcaSession = session;

            PBORCA_LibraryDirectory(orcaSession, file, "", 0, PBORCA_LibraryDirectoryCallback, dummy);
            
            if(session == 0)
                PBORCA_SessionClose(orcaSession);

            return libEntries;
        }

        /// <summary>
        /// reads the source of an object
        /// </summary>
        /// <param name="libEntry">library entry to export</param>
        public void FillCode(LibEntry libEntry)
        {
            int orcaSession;
            StringBuilder sbSource = new StringBuilder(5242880); // 5 MB

            if (session == 0)
                orcaSession = PBORCA_SessionOpen();
            else
                orcaSession = session;

            PBORCA_LibraryEntryExport(orcaSession, libEntry.Library, libEntry.Name, GetEntryType(libEntry.Type), sbSource, sbSource.Capacity);

            libEntry.Source = sbSource.ToString();

            if (session == 0)
                PBORCA_SessionClose(orcaSession);
        }

        public void SessionOpen()
        {
            session = PBORCA_SessionOpen();
        }

        public void SessionClose()
        {
            PBORCA_SessionClose(session);
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

