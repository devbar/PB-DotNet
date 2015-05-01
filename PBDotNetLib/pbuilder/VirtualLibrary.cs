using PBDotNetLib.orca;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBDotNetLib.pbuilder {
    public class VirtualLibrary : ILibrary {

        private DirectoryInfo dirInfo = null;

        public string Dir {
            get { return dirInfo.FullName; }
        }

        public string File {
            get { return dirInfo.Name; }
        }

        public orca.ILibEntry[] EntryList {
            get {
                var files = Directory
                    .GetFiles(this.Dir, "*.*")
                    .Where(f => f.ToLower().EndsWith(".psr") || f.Substring(f.Length -3, 2).ToLower() == "sr")
                    .ToList();

                var entries = new VirtualLibEntry[files.Count];

                for (int i = 0; i < files.Count; i++) {
                    entries[i] = new VirtualLibEntry(files[i]);
                }

                return entries;
            }
        }

        public VirtualLibrary(string folder) {
            this.dirInfo = new DirectoryInfo(folder);
        }
    }
}
