using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PBDotNetLib.pbuilder.powerscript
{
    /// <summary>
    /// variable in pb
    /// </summary>
    public class Variable
    {
        #region private

        private string name;
        private string datatype;
        private string value;
        private string descriptor;

        #endregion

        #region properties

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string Datatype
        {
            get
            {
                return datatype;
            }
        }

        public string Value
        {
            get
            {
                return value;
            }
        }

        public string Descriptor
        {
            get
            {
                return descriptor;
            }
        }

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="datatype">datatype of the variable</param>
        /// <param name="name">name of the variable</param>
        /// <param name="value">value of the variable (optional)</param>
        /// <param name="descriptor">descriptor of the variable (optional)</param>
        public Variable(string datatype, string name, string value = "", string descriptor = "")
        {
            this.name = name;
            this.datatype = datatype;
            this.value = value;
            this.descriptor = descriptor;
        }
    }
}
