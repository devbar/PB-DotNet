using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PBDotNetLib.pbuilder.powerscript
{
    public class Datawindow
    {
        private string release;
        private Element obj;

        public string Release
        {
            get
            {
                return release;
            }
            set
            {
                release = value;
            }
        }

        public Element Object
        {
            get
            {
                return obj;
            }
            set
            {
                obj = value;
            }
        }      


        public static Datawindow GetDatawindowFromSource(string source)
        {
            Datawindow datawindow = new Datawindow();

            try {
                datawindow.Release = GetRelease(ref source);
                datawindow.Object = Parse(ref source);
            } catch (Exception exp) {                
                //MessageBox.Show("Error parsing DW");
            }

            return datawindow;
        }

        private static string GetRelease(ref string source)
        {
            Match matchRelease = Regex.Match(source, "release (?<release>.*)", RegexOptions.IgnoreCase);

            if (matchRelease.Groups["release"].Success)
                return matchRelease.Groups["release"].Value.Replace(";","");
            else
                return null;
        }

        private static Element Parse(ref string source)
        {
            int start = source.IndexOf(";") +1;
            bool quote = false;
            string attrib = null;
            StringBuilder sb = new StringBuilder();
            Element root = new Element();
            Element current = null;

            current = root;
            
            for(int i = start; i < source.Length; i++){
                char c = source[i];

                if (c == '"')
                {
                    if (source[i - 1] != '~')
                    {
                        quote = !quote;
                    }
                }
                    
                
                if (!quote)
                {
                    if (attrib == null)
                    {
                        switch (c)
                        {
                            case '(':
                                Element child = new Element();
                                string name = sb.ToString().Trim();

                                if (name.Length > 0)
                                {
                                    if (name.EndsWith("="))
                                        name = name.Substring(0, name.Length - 1);
                                }

                                child.Name = name;
                                current.AddChild(child);
                                current = child;
                                sb = new StringBuilder();
                                break;
                            case ')':
                                string val;

                                val = sb.ToString().Trim();
                                if (val.Length > 0)
                                    current.Value = val;

                                current = current.Parent;
                                sb = new StringBuilder();
                                break;
                            case '=':
                                if (source[i + 1] == '(') goto default;

                                attrib = sb.ToString().Trim();
                                sb = new StringBuilder();
                                break;
                            default:
                                sb.Append(c);
                                break;
                        }
                    }
                    else
                    {
                        // if space or closing quote or closing quote without opened: value to attribute
                        if (c == ' ' || ( c == '"' && !quote ) || ( c == ')' && !sb.ToString().Contains("(") ) )
                        {
                            if (sb.Length > 0)
                            {
                                current.AddAttribute(attrib, sb.ToString());
                                attrib = null;
                                sb = new StringBuilder();
                            }                            
                        }else{
                            sb.Append(c);
                        }
                    }
                }else{
                    sb.Append(c);
                }

                
            }

            return root;
        }

        public class Attribute
        {
            private string name;
            private object val;

            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }

            public object Value
            {
                get
                {
                    return val;
                }
                set
                {
                    val = value;
                }
            }

            public Attribute(string name, object value)
            {
                this.name = name;
                this.val = value;
            }
        }

        public class Element
        {
            private string val;
            private string name;
            private List<Element> childs = new List<Element>();
            private List<Attribute> attributes = new List<Attribute>();
            private Element parent = null;            
            
            public string Value
            {
                get
                {
                    return val;
                }
                set
                {
                    val = value;
                }
            }

            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }

            public List<Element> Childs
            {
                get
                {
                    return childs;
                }
            }

            public List<Attribute> Attributes
            {
                get
                {
                    return attributes;
                }
                set
                {
                    attributes = value;
                }
            }

            public Element Parent
            {
                get
                {
                    return parent;
                }
                set
                {
                    parent = value;
                }
            }

            public void AddChild(Element child)
            {
                child.Parent = this;
                childs.Add(child);
            }

            public void AddAttribute(string name, object value)
            {
                attributes.Add(new Attribute(name, value));
            }
        }
    }

}
