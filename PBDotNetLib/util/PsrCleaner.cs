using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBDotNetLib.util {
    public class PsrCleaner {

        public static string Clean(string source) {
            StringBuilder cleanSource = new StringBuilder();            
            char[] chars = source.ToCharArray();


            

            for (int i = 0; i < source.Length; i++) {
                if (chars[i] != (char)0x00) {
                    cleanSource.Append(chars[i]);
                }
            }

            
            int start = cleanSource.ToString().IndexOf("release");

            cleanSource.Remove(0, start);

            int end = cleanSource.ToString().IndexOf((char)0x02 + "" + (char)0x0E);
            if (end > 0) {
                cleanSource.Remove(end, cleanSource.Length - end);
            }

            return cleanSource.ToString();
        }
    }
}
