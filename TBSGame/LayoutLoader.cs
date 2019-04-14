using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSGame.Controls;
using System.Xml;
using Microsoft.Xna.Framework;
using System.IO;
using System.Data;

namespace TBSGame
{
    public class LayoutLoader
    {
        public int ForWidth { get; private set; }
        public int ForHeight { get; private set; }

        private XmlDocument document = new XmlDocument();
        private Graphics graphics;
        private Dictionary<string, string> variables = new Dictionary<string, string>();

        public LayoutLoader(Settings settings, Stream path)
        {
            document.Load(path);
            variables.Add("saves_path", settings.GameSaves);
        }

        public void Load(Graphics graphics, Panel parent)
        {
            this.graphics = graphics;
            load(parent, document["layout"]["childs"].ChildNodes);
        }

        private void load(Panel parent, XmlNodeList childs)
        {
            foreach (XmlNode node in childs)
            {
                if (node.Name == "define")
                {
                    Type type = Type.GetType("TBSGame.Controls." + node.Attributes["type"].Value + ", TBSGame");
                    int from = node.Attributes["from"] == null ? -1 : int.Parse(node.Attributes["from"].Value);
                    int to = node.Attributes["to"] == null ? -1 : int.Parse(node.Attributes["to"].Value);

                    foreach (XmlNode prop in node.ChildNodes)
                    {
                        string ptype = prop.Attributes["type"].Value;
                        string convert = prop.Attributes["convert"].Value;
                        string val = prop.FirstChild.Value.Trim();

                        if (convert != "exp")
                        {
                            object obj = parse(val, convert);
                            parent.Defines.Add(new ControlDefine(obj)
                            {
                                ControlType = type,
                                Property = ptype,
                                From = from,
                                To = to
                            });
                        }
                        else
                        {
                            parent.Defines.Add(new ControlDefine(val, true)
                            {
                                ControlType = type,
                                Property = ptype,
                                From = from,
                                To = to
                            });
                        }
                    }
                }
                else
                {
                    Type type = Type.GetType($"TBSGame.Controls.{node.Name}, TBSGame");

                    List<object> args = new List<object>();
                    if (node["arguments"] != null)
                    {
                        foreach (XmlNode attr in node["arguments"].ChildNodes)
                        {
                            string convert = attr.Attributes["convert"].Value;
                            string val = convert == "null" ? "null" : attr.FirstChild.Value;

                            object obj = parse(val, convert);
                            args.Add(obj);
                        }
                    }

                    Control control = (Control)type.GetConstructors()[0].Invoke(args.ToArray());
                    control.Name = node.Attributes["name"] == null ? null : node.Attributes["name"].Value;
                    control.Bounds = parse_bounds(parent, node);

                    if (node["properties"] != null)
                    {
                        foreach (XmlNode prop in node["properties"].ChildNodes)
                        {
                            string ptype = prop.Attributes["type"].Value;
                            string convert = prop.Attributes["convert"].Value;
                            string val = prop.FirstChild.Value.Trim();

                            if (convert != "exp")
                                control.SetValue(ptype.Split('.'), parse(val, convert));
                            else
                                control.SetValue(ptype.Split('.'), eval(parent, val));
                        }
                    }

                    parent.Add(control);
                    if (type == typeof(Panel) || type.IsSubclassOf(typeof(Panel)))
                    {
                        if (node["childs"] != null)
                            load((Panel)control, node["childs"].ChildNodes);
                    }
                }
            }
        }

        private int eval(Panel parent, string expression)
        {
            expression = expression.Replace("W", parent.Bounds.Width.ToString());
            expression = expression.Replace("H", parent.Bounds.Height.ToString());
            return (int)double.Parse(new DataTable().Compute(expression, "").ToString());
        }

        private Rectangle parse_bounds(Panel parent, XmlNode node)
        {
            int get_val(XmlNode xn)
            {
                if (xn == null)
                    return 0;

                return eval(parent, xn.Value);
            }

            Rectangle bounds = new Rectangle
            (
                get_val(node.Attributes["x"]),
                get_val(node.Attributes["y"]),
                get_val(node.Attributes["width"]),
                get_val(node.Attributes["height"])
            );

            return bounds;
        }

        private object parse(string data, string type)
        {
            System.Drawing.Color color;
            string[] split = data.Split(';');    

            switch (type.ToLower())
            {
                case "string":
                    if (data[0] == '#')
                        return Resources.GetString(data.Substring(1));
                    return data;
                case "int":
                    return int.Parse(data);
                case "float":
                    return float.Parse(data);
                case "var":
                    return variables[data];
                case "double":
                    return double.Parse(data);
                case "bool":
                    return bool.Parse(data);
                case "color":
                    color = System.Drawing.ColorTranslator.FromHtml(data);
                    return new Color(color.R, color.G, color.B, color.A);
                case "colorname":
                    color = System.Drawing.Color.FromName(data);
                    return new Color(color.R, color.G, color.B, color.A);
                case "rgb":
                    return new Color(byte.Parse(split[0]), byte.Parse(split[1]), byte.Parse(split[2]));
                case "rgba":
                    return new Color(byte.Parse(split[0]), byte.Parse(split[1]), byte.Parse(split[2]), float.Parse(split[3]));
                case "font":
                    return data == "normal" ? graphics.Normal : graphics.Small;
            }

            return null;
        }
    }

    public class ControlDefine
    {
        private object value;
        private bool exp;

        public Type ControlType { get; set; }
        public string Property { get; set; }
        public int From { get; set; } = -1;
        public int To { get; set; } = -1;
        public bool HasRange => !(From == -1 || To == -1);

        public ControlDefine(object val, bool exp = false)
        {
            this.value = val;
            this.exp = exp;
        }

        public object GetValue(Panel parent, int index = 0)
        {
            if (!exp)
                return value;

            string expression = (string)value;
            expression = expression.Replace("i", index.ToString());
            expression = expression.Replace("W", parent.Bounds.Width.ToString());
            expression = expression.Replace("H", parent.Bounds.Height.ToString());
            return (int)double.Parse(new DataTable().Compute(expression, "").ToString());
        }
    }
}
