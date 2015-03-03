using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using PBDotNetLib.common;
using PBDotNetLib.orca;

namespace PBDotNetLib.pbuilder
{
    /// <summary>
    /// pb library
    /// </summary>
    public class Library : PBSrcFile, ILibrary
    {
        #region private

        private string dir;
        private string file;
        private Orca orca = null;
        
        #endregion

        #region properties

        public string Dir
        {
            get
            {
                return dir;

            }
        }

        public string File
        {
            get
            {
                return file;
            }
        }

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="file">path to lib</param>
        /// <param name="version">PB version</param>
        public Library(string file, Orca.Version version)
        {
            this.orca = new Orca(version);
            dir = file.Substring(0, file.LastIndexOf("\\"));
            this.file = file.Substring(file.LastIndexOf("\\") + 1);
        }

        public ILibEntry[] EntryList
        {
            get
            {
                return orca.DirLibrary(Dir + "\\" + File).ToArray();
            }
        }

    }
}
