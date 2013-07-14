using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace PBDotNetLib.common
{
    /// <summary>
    /// projectfile like a workspace or a target
    /// </summary>
	abstract public class PBProjFile
    {

        #region private

        private string minorVersion;
        private string majorVersion;
        private string dir;
        private string file;
        private bool exists;

        #endregion

        #region properties
        public string MinorVersion
        {
            get
            {
                return minorVersion;
            }
        }

        public string MajorVersion
        {
            get
            {
                return majorVersion;
            }
        }

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

        public bool Exists
        {
            get
            {
                return exists;
            }
        }
        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="file">the file to read</param>
        public PBProjFile(string file)
        {
            string source;
            StreamReader reader = null;
            
            this.dir = file.Substring(0, file.LastIndexOf("\\"));
            this.file = file.Substring(file.LastIndexOf("\\") + 1);

            reader = new StreamReader(new FileStream(file, FileMode.Open));
           
            exists = true;

            source = reader.ReadToEnd();
            reader.Close();

            Parse(source);
        }

        /// <summary>
        /// Parse method can be extened in ancestor for reading liblist 
        /// or containing targets etc.
        /// </summary>
        /// <param name="source">source to parse</param>
        protected virtual void Parse(string source)
        {
            ParseVersion(source);
        }

        /// <summary>
        /// method to parse version from file
        /// </summary>
        /// <param name="source">source to parse</param>
        private void ParseVersion(string source)
        {
            MatchCollection matches = null;

            matches = Regex.Matches(source, @"Save Format v(?<majorversion>[0-9]*\.[0-9])\((?<minorversion>[0-9]*)\)", RegexOptions.IgnoreCase);

            if (matches.Count == 0) return;

            majorVersion = matches[0].Groups["majorversion"].Value;
            minorVersion = matches[0].Groups["minorversion"].Value;
        }
	}
}
