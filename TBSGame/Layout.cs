using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSGame.Controls;
using System.Xml;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.IO;

namespace TBSGame
{
    public class Layout
    {
        public int ForWidth { get; private set; }
        public int ForHeight { get; private set; }

        private XmlDocument document = new XmlDocument();
        private Graphics graphics;
        private Dictionary<string, string> variables = new Dictionary<string, string>();

        public Layout(Settings settings, Stream path)
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
                Type type = Type.GetType("TBSGame.Controls." + node.Name + ", TBSGame");

                List<object> args = new List<object>();
                foreach (XmlNode attr in node["arguments"].ChildNodes)
                {
                    string convert = attr.Attributes["convert"].Value;
                    string val = attr.FirstChild.Value;

                    object obj = parse(val, convert);
                    args.Add(obj);
                }

                Control control = (Control)type.GetConstructors()[0].Invoke(args.ToArray());
                control.Name = node.Attributes["name"].Value;
                control.Bounds = new Rectangle
                (
                    node.Attributes["x"] != null ? int.Parse(node.Attributes["x"].Value) : 0,
                    node.Attributes["y"] != null ? int.Parse(node.Attributes["y"].Value) : 0,
                    node.Attributes["width"] != null ? int.Parse(node.Attributes["width"].Value) : 0,
                    node.Attributes["height"] != null ? int.Parse(node.Attributes["height"].Value) : 0
                );

                if (node["properties"] != null)
                {
                    foreach (XmlNode prop in node["properties"].ChildNodes)
                    {
                        string ptype = prop.Attributes["type"].Value;
                        string convert = prop.Attributes["convert"].Value;
                        string val = prop.FirstChild.Value.Trim();

                        object obj = parse(val, convert);
                        set_value(control, ptype.Split('.'), obj);
                       // type.GetProperty(ptype).SetValue(control, obj);
                    }
                }

                parent.Add(control);
                if (control.GetType() == typeof(Panel))
                {
                    if (node["childs"] != null)
                        load((Panel)control, node["childs"].ChildNodes);
                }
            }
        }
        
        private object set_value(object obj, string[] name, object val, int index = 0)
        {
            PropertyInfo info = obj.GetType().GetProperty(name[index]);
            if (index == name.Length - 1)
            {
                info.SetValue(obj, val);
                return obj;
            }
            else
            {
                object no = info.GetValue(obj);
                object ret = set_value(no, name, val, index + 1);
                info.SetValue(obj, ret);
                return obj;
            }
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
}
