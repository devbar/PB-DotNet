using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PBDotNetLib.common;
using PBDotNetLib.orca;

namespace PBDotNetLib.pbuilder
{
    /// <summary>
    /// pb library
    /// </summary>
    public class Library : PBSrcFile
    {
        #region private

        private string dir;
        private string file;
        private Orca orca = new Orca();
        
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
        public Library(string file)
        {
            dir = file.Substring(0, file.LastIndexOf("\\"));
            this.file = file.Substring(file.LastIndexOf("\\") + 1);
        }

        public LibEntry[] EntryList
        {
            get
            {
                return orca.DirLibrary(Dir + "\\" + File).ToArray();
            }
        }

    }
}
