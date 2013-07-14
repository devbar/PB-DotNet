using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using PBDotNetLib.common;

namespace PBDotNetLib.pbuilder
{
    /// <summary>
    /// pb workspace
    /// </summary>
    public class Workspace : PBProjFile
    {
        #region private

        private List<Tuple<int, string,bool,bool>> targets = new List<Tuple<int,string,bool,bool>>();
        private string defaultTarget;
        private string defaultRemoteTarget;

        #endregion

        #region properties

        public Target[] Targets
        {
            get
            {
                List<Target> targList = new List<Target>();

                foreach(Tuple<int,string,bool,bool> targ in targets){
                    targList.Add(new Target(targ.Item1, targ.Item2, targ.Item3, targ.Item4));
                }

                return targList.ToArray();
            }
        }

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="file">path to workspace</param>
        public Workspace(string file) : base (file)
        {
            
        }

        /// <summary>
        /// parse source of workspace
        /// </summary>
        /// <param name="source">source of workspace</param>
        protected override void Parse(string source)
        {
            base.Parse(source);

            ParseDefaultTargets(source);
            ParseTargets(source);
        }

        /// <summary>
        /// find the default targets in workspace
        /// </summary>
        /// <param name="source">source of workspace</param>
        private void ParseDefaultTargets(string source)
        {
            MatchCollection matches = null;

            matches = Regex.Matches(source, "DefaultTarget \"(?<defaulttarget>.*)\";", RegexOptions.IgnoreCase);
            if (matches.Count > 0)
                defaultTarget = matches[0].Groups["defaulttarget"].Value;

            matches = Regex.Matches(source, "DefaultRemoteTarget \"(?<defaultremtarget>.*)\";", RegexOptions.IgnoreCase);
            if (matches.Count > 0)
                defaultRemoteTarget = matches[0].Groups["defaultremtarget"].Value;
        }

        /// <summary>
        /// parse the target list in workspace
        /// </summary>
        /// <param name="source"></param>
        private void ParseTargets(string source)
        {
            MatchCollection matches = null;

            matches = Regex.Matches(source, "@begin Targets\r\n(?<target>.*\r\n)*?@end", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            if (matches.Count == 0) return;

            foreach (Capture cap in matches[0].Groups["target"].Captures)
            {
                string capture, path;
                bool defTargetFlag = false, defRemTarFlag = false;
                MatchCollection matchesTarget = null;

                capture = cap.Value;
                matchesTarget = Regex.Matches(capture, "(?<order>[0-9]+).*\"(?<path>.*)\"", RegexOptions.IgnoreCase);

                if (matchesTarget.Count == 0) continue;

                path = matchesTarget[0].Groups["path"].Value;

                if (path.Equals(defaultTarget))
                    defTargetFlag = true;

                if (path.Equals(defaultRemoteTarget))
                    defRemTarFlag = true;

                if(path.IndexOf(':') < 0)
                    path = Dir + "\\" + path;

                path = path.Replace(@"\\", @"\").Trim();

                targets.Add(new Tuple<int, string, bool, bool>(Int32.Parse(matchesTarget[0].Groups["order"].Value), path, defTargetFlag, defRemTarFlag));
            }
        }
    }
}
