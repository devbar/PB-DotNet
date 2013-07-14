using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PBDotNetLib.pbuilder.powerscript
{
    /// <summary>
    /// method (subroutine or function) in pb
    /// </summary>
    public class Method
    {
        #region private

        private string name;
        private string returntype;
        private string source;
        private string modifier;
        private Parameter[] parameter;

        #endregion

        #region properties

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string Returntype
        {
            get
            {
                return returntype;
            }
        }

        public Parameter[] Parameter
        {
            get
            {
                return parameter;
            }
        }

        public string Source
        {
            get
            {
                return source;
            }
        }

        public string Modifier
        {
            get
            {
                return modifier;
            }
        }

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="modifier">modifier (private, protected, public, ... )</param>
        /// <param name="name">name of the method</param>
        /// <param name="returntype">return type of the method</param>
        /// <param name="parameter">parameter list of the method</param>
        /// <param name="source">source of the method</param>
        public Method(string modifier, string name, string returntype, Parameter[] parameter, string source = "")
        {
            this.name = name;
            this.parameter = parameter;
            this.returntype = returntype;
            this.source = source;
            this.modifier = modifier;
        }

    }
}
