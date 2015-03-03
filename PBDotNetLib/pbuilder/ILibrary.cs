using PBDotNetLib.orca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBDotNetLib.pbuilder {
    public interface ILibrary {
        string Dir { get; }
        string File { get; }
        ILibEntry[] EntryList { get; }
    }
}
