using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBDotNetLib.orca {
    /// <summary>
    /// This class represents a file based Lib Entry. The behaviour 
    /// should be similar to the entry of a PBL.
    /// </summary>
    public class VirtualLibEntry : ILibEntry {

        private FileInfo fileInfo = null;
        private string source;

        public DateTime Createtime {
            get { return this.fileInfo.CreationTime; }
        }

        public string Comment {
            get { return ""; }
        }

        public string Name {
            get { return this.fileInfo.Name; }
        }

        public int Size {
            get { return (int)this.fileInfo.Length; }
        }

        public Objecttype Type {
            get {
                switch (this.fileInfo.Name.Substring(this.fileInfo.Name.Length - 3).ToLower()) {
                    case "sru":
                        return Objecttype.Userobject;
                    case "psr":
                    case "srd":
                        return Objecttype.Datawindow;
                    case "sra":
                        return Objecttype.Application;
                    case "srm":
                        return Objecttype.Menu;
                    case "srf":
                        return Objecttype.Function;
                    case "srj":
                        return Objecttype.Project;
                    case "srs":
                        return Objecttype.Structure;
                    case "srw":
                        return Objecttype.Window;
                    default:
                        return Objecttype.None;
                }
            }
        }

        public string Library {
            get { return this.fileInfo.FullName; }
        }

        public string Source {
            get {
                if (String.IsNullOrEmpty(this.source)) {
                    this.source = this.source = new StreamReader(new FileStream(this.fileInfo.FullName, FileMode.Open)).ReadToEnd();

                    if (this.fileInfo.Name.EndsWith(".psr")) {
                        this.source = util.PsrCleaner.Clean(this.source);
                    }
                }

                return this.source;
            }
            set {
                this.source = value;
            }
        }

        public VirtualLibEntry(string file) {
            this.fileInfo = new FileInfo(file);            
        }
    }
}
