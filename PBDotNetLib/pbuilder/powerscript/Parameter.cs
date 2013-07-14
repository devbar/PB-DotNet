using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PBDotNetLib.pbuilder.powerscript
{
    /// <summary>
    /// parameter in pb
    /// </summary>
    public class Parameter : Variable
    {
        private static char[] variableSeparators = new char[2] { ' ', '=' };

        /// <summary>
        /// enum to set the "call by" of a variable if it 
        /// is used for a paramater in a method or an event
        /// </summary>
        public enum CallBy{
            Reference,
            Value,
            ReadOnly
        }

        private CallBy callby = CallBy.Value;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="datatype">datatype of the parameter</param>
        /// <param name="name">name of the parameter</param>
        /// <param name="callby">whats the call by of the parameter</param>
        public Parameter(string datatype, string name, CallBy callby = CallBy.Value)
            : base(datatype, name)
        {
            this.callby = callby;
        }

        /// <summary>
        /// parse the paramater from a string 
        /// </summary>
        /// <param name="commaSepParameter"></param>
        /// <returns></returns>
        public static Parameter[] FromString(string commaSepParameter)
        {
            List<Parameter> resultParams = new List<Parameter>();

            foreach (string parameter in commaSepParameter.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] varParts = parameter.Split(variableSeparators, StringSplitOptions.RemoveEmptyEntries);
                int index = 0;
                Parameter.CallBy callby = Parameter.CallBy.Value;


                switch (varParts[0].Trim().ToLower())
                {
                    case "ref":
                        callby = Parameter.CallBy.Reference;
                        index++;
                        break;
                    case "readonly":
                        callby = Parameter.CallBy.ReadOnly;
                        index++;
                        break;
                }

                resultParams.Add(new Parameter(varParts[index + 0].Trim(), varParts[index + 1].Trim(), callby));
            }

            return resultParams.ToArray();
        }
    }
}
