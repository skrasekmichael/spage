using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TBSGame.Controls;

namespace TBSGame
{
    public class Layout
    {
        protected void LoadControls(Panel panel)
        {
            foreach (FieldInfo info in this.GetType().GetRuntimeFields())
            {
                Attribute attr = info.GetCustomAttribute(typeof(LayoutControlAttribute));
                if (attr != null)
                {
                    string name = ((LayoutControlAttribute)attr).Name;
                    if (name == null)
                        name = info.Name;

                    Control control = panel.GetControl(name);
                    if (control == null)
                        Error.Log($"Control {name} not found!");
                    else
                        info.SetValue(this, control);
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class LayoutControlAttribute : Attribute
    {
        public string Name { get; set; } = null;

        public LayoutControlAttribute(string name = null)
        {
            Name = name;
        }
    }
}
