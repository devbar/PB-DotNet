using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PBDotNetLib.pbuilder.powerscript
{
    /// <summary>
    /// In type in pb can be a lot of thigs i.g. function, userobject, window,
    /// structure etc. Every type can have "within" types. 
    /// </summary>
    public class Type
    {
        #region private

        private static char[] variableSeparators = new char[2] { ' ', '=' };
        private string name;
        private string ancestor;
        private string parent;
        private bool global;
        private string instanceVariables;
        private string sharedVariables;
        private string globalVariables;
        private string externalFunctions;
        private Variable[] properties;
        private Event[] events;
        private Method[] methods;

        #endregion

        #region properties

        public Variable[] Properties
        {
            get
            {
                return properties;
            }
            set
            {
                properties = value;
            }
        }

        public Event[] Events
        {
            get
            {
                return events;
            }
            set
            {
                events = value;
            }
        }

        public Method[] Methods
        {
            get
            {
                return methods;
            }
            set
            {
                methods = value;
            }
        }

        public string InstanceVariables
        {
            get
            {
                return instanceVariables;
            }
            set
            {
                instanceVariables = value;
            }
        }

        public string SharedVariables
        {
            get
            {
                return sharedVariables;
            }
            set
            {
                sharedVariables = value;
            }
        }

        public string GlobalVariables
        {
            get
            {
                return globalVariables;
            }
            set
            {
                globalVariables = value;
            }
        }

        public string ExternalFunctions
        {
            get
            {
                return externalFunctions;
            }
            set
            {
                externalFunctions = value;
            }
        }

        public string Declaration
        {
            get
            {
                return (global ? "global " : "") + "type " + name + " from " + ancestor + (parent == null ? "" : " within " + parent);
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            
        }

        public string Ancestor
        {
            get
            {
                return ancestor;
            }
        }

        public string Parent
        {
            get
            {
                return parent;
            }
        }

        public bool Global
        {
            get
            {
                return global;
            }
        }

        #endregion

        /// <summary>
        /// parses global block from the source
        /// </summary>
        /// <param name="source">complete source</param>
        /// <param name="globalSource">global source comes back</param>
        /// <param name="objSource">rest of the source</param>
        private static void ParseGlobal(string source, out string globalSource, out string objSource)
        {
            globalSource = null;
            objSource = null;

            ParseBlock(source, "global", ref globalSource, ref objSource);
        }

        /// <summary>
        /// parses forward block from the source
        /// </summary>
        /// <param name="source">complete source</param>
        /// <param name="fwdSource">forward source comes bacl</param>
        /// <param name="objSource">rest of the source comes back</param>
        private static void ParseForward(string source, out string fwdSource, out string objSource)
        {
            fwdSource = null;
            objSource = null;

            ParseBlock(source, "forward", ref fwdSource, ref objSource);
        }

        /// <summary>
        /// parses a block of the specified tag
        /// </summary>
        /// <param name="source">complete source</param>
        /// <param name="tag">tag of the block</param>
        /// <param name="blockSource">block source comes back</param>
        /// <param name="restSource">rest source comes back</param>
        private static void ParseBlock(string source, string tag, ref string blockSource, ref string restSource)
        {
            MatchCollection matches = null;
            string endTag, beginTag;

            endTag = "end " + tag;
            beginTag = tag;

            matches = Regex.Matches(source, "^" + beginTag + "\r\n(.*\r\n)*^" + endTag, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (matches.Count == 0)
            {
                restSource = source;
                return;
            }

            foreach (Capture cap in matches[0].Captures)
            {
                blockSource = cap.Value + "\r\n";
            }

            restSource = source.Substring(source.IndexOf(endTag) + endTag.Length); 
        }

        /// <summary>
        /// parses external functions
        /// </summary>
        /// <param name="source">complete source</param>
        /// <param name="externalSource">source of external functions</param>
        /// <param name="objSource">rest source</param>
        private static void ParseExternalFunctions(string source, out string externalSource, out string objSource)
        {
            Match match = null;

            externalSource = "";
            objSource = null;

            match = Regex.Match(source, "^type prototypes\r\n(?<variables>.*\r\n)*?^end prototypes", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (match.Groups["variables"].Success)
            {
                foreach (Capture cap in match.Groups["variables"].Captures)
                    externalSource += cap.Value;

                objSource = source.Substring(source.IndexOf("end prototypes") + 14);
            }
            else
            {
                objSource = source;
            }


        }

        /// <summary>
        /// parses global variables from source
        /// </summary>
        /// <param name="source">complete source</param>
        /// <param name="globalSource">source of global variables</param>
        /// <param name="objSource">rest source</param>
        private static void ParseGlobalVariables(string source, out string globalSource, out string objSource)
        {
            Match match = null;

            globalSource = "";
            objSource = null;

            match = Regex.Match(source, "^global variables\r\n(?<variables>.*\r\n)*?^end variables", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (match.Groups["variables"].Success)
            {
                foreach (Capture cap in match.Groups["variables"].Captures)
                    globalSource += cap.Value;

                objSource = source.Substring(source.IndexOf("end variables") + 13);
            }
            else
            {
                objSource = source;
            }


        }

        /// <summary>
        /// parses shared variables from source
        /// </summary>
        /// <param name="source">complete source</param>
        /// <param name="shrdSource">shared source</param>
        /// <param name="objSource">rest source</param>
        private static void ParseSharedVariables(string source, out string shrdSource, out string objSource)
        {
            Match match = null;

            shrdSource = "";
            objSource = null;

            match = Regex.Match(source, "^shared variables\r\n(?<variables>.*\r\n)*?^end variables", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (match.Groups["variables"].Success)
            {
                foreach (Capture cap in match.Groups["variables"].Captures)
                    shrdSource += cap.Value;

                objSource = source.Substring(source.IndexOf("end variables") + 13); 
            }
            else
            {
                objSource = source;
            }

            
        }

        private static Type[] ParseForwardTypes(string source)
        {
            List<Type> types = new List<Type>();
            MatchCollection matches = null;

            matches = Regex.Matches(source, "(?<global>global )?type (?<name>[^\x20]*) from (?<ancestor>[^\x20|\t]*)( within (?<parent>.*))?\r\n(.*\r\n)*?end type*");

            if (matches.Count == 0)
                return null;

            foreach (Match match in matches)
            {
                string name, ancestor, parent;
                bool global;

                global = match.Groups["global"].Success;
                name = match.Groups["name"].Value;
                ancestor = match.Groups["ancestor"].Value;

                // Strukturen haben kein "within", komisch, is aber so
                if (match.Groups["parent"].Success && ancestor.ToLower() != "structure" ) 
                    parent = match.Groups["parent"].Value;
                else
                    parent = null;

                types.Add(new Type(name, ancestor, parent, global));
            }

            return types.ToArray();
        }

        private static Variable[] ParsePropertiesFromCreate(string source, string typeName )
        {
            MatchCollection matches = null;
            List<Variable> props = new List<Variable>();

            matches = Regex.Matches(source, "^on " + typeName + ".create\r\n(?<properties>.*\r\n)*?^end on", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (matches.Count == 0)
                return props.ToArray();

            // parsing properties
            foreach (Capture cap in matches[0].Groups["properties"].Captures)
            {
                if (cap.Value.IndexOf("this") != 0 || cap.Value.IndexOf("[") >= 0)
                    continue;

                string[] varParts = cap.Value.Split(variableSeparators, StringSplitOptions.RemoveEmptyEntries);

                if (varParts.Length == 2)
                    props.Add(new Variable("any", varParts[0].Trim().Replace("this.", ""), varParts[1].Trim()));
                else
                    props.Add(new Variable("any", varParts[0].Trim().Replace("this.", "")));
                
            }

            return props.ToArray();
        }

        private static void ParsePropertiesAndEvents(string source, Type type, out Variable[] properties, out Event[] events)
        {
            MatchCollection matches = null;
            List<Variable> props = new List<Variable>();
            List<Event> evts = new List<Event>();
            Match eventSourceEnd = null;
            Match variables = null;
            Match structVariables = null;
            string eventSource = null;
            string eventAndVariableSource = null;
            string propLine = "";

            properties = null;
            events = null;

            matches = Regex.Matches(source, type.Declaration + "( descriptor (?<descriptor>.*))?\r\n(?<properties>.*\r\n)*?end type");
            if (matches.Count == 0)
                return;

            // parsing properties
            foreach (Capture cap in matches[0].Groups["properties"].Captures)
            {
                string propType, propName, propValue;
                int indexName = 0;
                
                if (type.ancestor.ToLower() == "structure")
                {
                    structVariables = Regex.Match(cap.Value, "(?<datatype>[^\\t]+)\\t+(?<name>[^\\t]+)\\t*(descriptor (?<descriptor>.+))?");
                    props.Add(new Variable( structVariables.Groups["datatype"].Value.Trim(),
                                            structVariables.Groups["name"].Value.Trim(),
                                            "",
                                            structVariables.Groups["descriptor"].Success ? structVariables.Groups["descriptor"].Value.Trim() : ""));
                }
                else
                {
                    propLine += cap.Value;
                    if (propLine.EndsWith("&\r\n"))
                    {
                        propLine = propLine.Substring(0, propLine.Length - 3);
                        continue;
                    }

                    propType = propLine.Substring(0, propLine.IndexOf(' '));
                    indexName = propLine.IndexOf('=', propType.Length +1);
                    if (indexName > 0)
                    {
                        propName = propLine.Substring(propType.Length + 1, indexName - (propType.Length + 1));
                        propValue = propLine.Substring(indexName + 1);
                    }
                    else
                    {
                        propName = propLine.Substring(propType.Length + 1);
                        propValue = "";
                    }

                    if (propType == "event")
                    {
                        //evts.Add(ParseEvents(cap.Value)[0]);
                    }
                    else
                    {
                        if (propValue.Length > 0)
                            props.Add(new Variable(propType.Trim(), propName.Trim(), propValue.Trim()));
                        else
                            props.Add(new Variable(propType.Trim(), propName.Trim()));
                    }

                    propLine = "";
                }
            }

            // parsing events
            eventAndVariableSource = source.Substring(source.IndexOf("end type\r\n", matches[0].Index) + 9);

            variables = Regex.Match(eventAndVariableSource, "^type variables\r\n(?<variables>.*\r\n)*^end variables", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (variables.Groups["variables"].Success)
            {
                foreach(Capture cap in variables.Groups["variables"].Captures)
                {
                    type.InstanceVariables += cap.Value;
                }

                eventSource = eventAndVariableSource.Substring(variables.Index + variables.Length);
            }
            else
            {
                eventSource = eventAndVariableSource;
            }

            eventSourceEnd = Regex.Match(eventSource, "^type ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (eventSourceEnd.Success)
            {
                eventSource = eventSource.Substring(0, eventSourceEnd.Index - 1).Trim();
            }
            else
            {
                eventSource = eventSource.Trim();
            }

            if (!String.IsNullOrEmpty(eventSource))
            {
                evts.AddRange(ParseEvents(eventSource));
            }

            props.AddRange(ParsePropertiesFromCreate(source, type.name));
                        
            properties = props.ToArray();
            events = evts.ToArray();
        }

        private static Event[] ParseEvents(string source){
            MatchCollection matches = Regex.Matches(source,"^event (type (?<returntype>[^\\x20]*))?(?<name>.*)(\\((?<parameter>.*)\\))?;(?<source>.*\r\n)*?^end event", RegexOptions.IgnoreCase | RegexOptions.Multiline); 
            Parameter[] evtParamater = null;
            List<Event> evts = new List<Event>();
            string evtName = null, evtReturntype = null, evtSource = null;
            bool extended = false;
            
            foreach (Match match in matches)
            {
                evtName = match.Groups["name"].Value.Trim();
                evtSource = "";
                extended = false;

                string[] parts = evtName.Split(new char[] {';'});
                
                if(parts.Length > 1){
                    int indexParts = 1;
                    
                    evtName = parts[0];
                    if (parts[1].Equals("call super::" + evtName))
                    {
                        extended = true;
                        indexParts++;
                    }

                    for (int i = indexParts; i < parts.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(evtSource))
                        {
                            evtSource = ";";
                        }
                        evtSource += parts[i];
                    }
                }

                if (match.Groups["returntype"].Success)
                    evtReturntype = match.Groups["returntype"].Value.Trim();

                if (match.Groups["paramater"].Success)
                {
                   evtParamater = Parameter.FromString(match.Groups["paramater"].Value);
                }
                                
                if(match.Groups["source"].Success)
                {
                    foreach ( Capture cap in match.Groups["source"].Captures){
                        evtSource += cap.Value;
                    }                    
                }
                

                evts.Add(new Event(evtName, evtReturntype, evtParamater, evtSource, extended));
            }

            return evts.ToArray();
        }

        public static Method[] ParseMethods(string source)
        {
            string methName, methReturnType, methSource, methMod;
            Parameter[] methParameter = null;
            List<Method> methods = new List<Method>();
            MatchCollection matches = Regex.Matches(source, "^forward prototypes\\r\\n(?<prototypes>.*\\r\\n)*^end prototypes", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Match matchMethod = null;

            foreach (Match match in matches)
            {
                if (match.Groups["prototypes"].Success)
                {
                    foreach (Capture cap in match.Groups["prototypes"].Captures)
                    {
                        matchMethod = Regex.Match(cap.Value, "^(?<modifier>public|private|protected|global) (function (?<returntype>[^\\x20]*)|subroutine)\\x20?(?<name>[^\\x20|\\(]*)\\x20\\(?(?<parameter>[^\\)]*)", RegexOptions.IgnoreCase );

                        methMod = matchMethod.Groups["modifier"].Value;
                        methName = matchMethod.Groups["name"].Value;

                        if (matchMethod.Groups["returntype"].Success){
                            methReturnType = matchMethod.Groups["returntype"].Value;
                        }else{
                            methReturnType = String.Empty;
                        }

                        if(matchMethod.Groups["paramater"].Success){
                            methParameter = Parameter.FromString ( matchMethod.Groups["parameter"].Value );
                        }

                        methSource = ParseMethodSource(source, cap.Value, String.IsNullOrEmpty(methReturnType), cap.Index);

                        methods.Add(new Method(methMod, methName, methReturnType, methParameter, methSource));
                    }
                }
            }

            return methods.ToArray();
        }

        private static string ParseMethodSource(string source, string signature, bool subroutine, int start)
        {
            int startPos, endPos;

            startPos = source.IndexOf(signature.Trim(), start + signature.Length);
            if (startPos < 0) return String.Empty;

            startPos += signature.Length - 1;

            endPos = source.IndexOf ( "end " + (subroutine ? "subroutine" : "function"), startPos );

            return source.Substring(startPos, endPos - startPos);
        }

        public static Type[] GetTypesFromSource(string source)
        {
            string fwdSource, objSource, shrdSource, globalSource, externalSource;
            Type[] types = null;

            ParseForward(source, out fwdSource, out objSource);
            if (fwdSource != null)
                types = ParseForwardTypes(fwdSource);
            else
                types = ParseForwardTypes(source);

            if (types == null)
                return null;

            foreach (Type type in types)
            {
                Variable[] properties;
                Event[] events;

                ParsePropertiesAndEvents(objSource, type, out properties, out events);

                type.Properties = properties;
                type.Events = events;
            }

            // methods are always relevant to the main type
            if (types.Length > 0)
            {
                ParseSharedVariables(source, out shrdSource, out objSource);
                ParseGlobalVariables(source, out globalSource, out objSource);
                ParseExternalFunctions(source, out externalSource, out objSource);

                types[0].ExternalFunctions = externalSource;
                types[0].SharedVariables = shrdSource;
                types[0].GlobalVariables = globalSource;
                types[0].Methods = ParseMethods(source);

            }
            

            return types;
        }

        public Type(string name, string ancestor, string parent, bool global)
        {
            this.name = name;
            this.ancestor = ancestor;
            this.parent = parent;
            this.global = global;
        }
    }
}
