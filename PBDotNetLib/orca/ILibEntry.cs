using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBDotNetLib.orca {
    public interface ILibEntry {
        DateTime Createtime {get;}
        string Comment {get;}
        string Name {get;}
        int Size {get;}
        Objecttype Type {get;}
        string Library {get;}
        string Source {set; get;}
    }
}
