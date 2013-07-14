using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PBDotNetLib.common;
using System.Text.RegularExpressions;
using System.IO;

namespace PBDotNetLib.pbuilder
{
    /// <summary>
    /// pb target 
    /// </summary>
    public class Target : PBProjFile
    {

        #region private

        private int order;
        private bool defaultTarget;
        private bool defaultRemoteTarget;
        private List<string> libs = new List<string>();

        #endregion

        #region properties

        public Library[] Libraries
        {
            get
            {
                List<Library> libList = new List<Library>();

                foreach (string lib in libs)
                {
                    libList.Add(new Library(lib));
                }

                return libList.ToArray();
            }
        }

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="order">order number in worpspace</param>
        /// <param name="file">target file path</param>
        /// <param name="defaultTarget">fag if it is default target</param>
        /// <param name="defaultRemoteTarget">flag if it is default remote target</param>
        public Target(int order, string file, bool defaultTarget, bool defaultRemoteTarget)
            : base(file)
        {
            this.order = order;
            this.defaultTarget = defaultTarget;
            this.defaultRemoteTarget = defaultRemoteTarget;
        }

        /// <summary>
        /// parse source
        /// </summary>
        /// <param name="source">source of pbt</param>
        protected override void Parse(string source)
        {
            base.Parse(source);

            ParseLibList(source);
        }


        /// <summary>
        /// parse liblist from source
        /// </summary>
        /// <param name="source">source of pbt</param>
        private void ParseLibList(string source)
        {
            string liblist;
            MatchCollection matches = null;

            matches = Regex.Matches(source, "LibList \"(?<liblist>[^\"]*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (matches.Count == 0) return;

            liblist = matches[0].Groups["liblist"].Value;
            libs = new List<string>();

            libs = CompletePath(liblist.Split(new char[] { ';' }));
        }

        /// <summary>
        /// complete path to get a absolute path of pbls in pbt
        /// </summary>
        /// <param name="list">list of pbls (relativ)</param>
        /// <returns>list of pbls (absolute)</returns>
        private List<string> CompletePath(string[] list)
        {
            int parentCount = 0;
            List<string> resList = new List<string>();

            foreach (string lib in list)
            {
                parentCount = Regex.Matches(lib, @"\.\.\\\\", RegexOptions.IgnoreCase | RegexOptions.Singleline ).Count;
                if (parentCount > 0)
                {
                    string parentDir = Dir;

                    for (int i = 0; i < parentCount; i++)
                    {
                        parentDir = Directory.GetParent(parentDir).FullName;
                    }

                    resList.Add(parentDir + "\\" + Regex.Replace(lib, @"\.\.\\\\", ""));
                }
                else
                {
                    resList.Add(Dir + "\\" + lib);
                }
            }


            return resList;
        }
    }
}
