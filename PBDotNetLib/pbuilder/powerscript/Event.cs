using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PBDotNetLib.pbuilder.powerscript
{
    /// <summary>
    /// event in pb
    /// </summary>
    public class Event
    {
        #region private

        private string name;
        private string returntype;
        private string source;
        private Parameter[] parameter;
        private bool extended = false;

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

        public bool Extended
        {
            get
            {
                return extended;
            }
            set
            {
                extended = value;
            }
        }

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">event name</param>
        /// <param name="returntype">return type of the event</param>
        /// <param name="parameter">parameter list of the event</param>
        /// <param name="source">source of the event</param>
        /// <param name="extended">flag if the event is extended</param>
        public Event(string name, string returntype, Parameter[] parameter, string source = "", bool extended = false)
        {
            this.name = name;
            this.parameter = parameter;
            this.returntype = returntype;
            this.source = source;
            this.extended = extended;
        }
    }
}
