﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace PA.Plugin.Components.ParameterForm
{
    public partial class PluginParametersPanel : FlowLayoutPanel
    {
        public PluginParametersPanel()
        {
            InitializeComponent();
        }


        [Browsable(false)]
        public IPlugin Plugin { get; private set; }

        [Browsable(false)]
        public int Count { get; private set; }

        public void Refresh<T>(T p)
            where T:IPlugin
        {
            this.Plugin = p;

            if (this.Plugin is T)
            {
                lock (this.Plugin)
                {
                    this.Controls.Clear();

                    foreach (PropertyInfo pi in this.Plugin
                        .GetType()
                        .GetProperties()
                        .Where(pda => pda.CanRead && pda.CanWrite))
                    {
                        PluginDescriptionAttribute pda = pi.GetCustomAttributes(typeof(PluginDescriptionAttribute), true)
                             .OfType<PluginDescriptionAttribute>()
                             .FirstOrDefault();

                        if (pda is PluginDescriptionAttribute && (pda.TargetType == Type.EmptyTypes || pda.TargetType.Contains(typeof(T))))
                        {
                            if (pi.PropertyType.Equals(typeof(bool)))
                            {
                                CheckBox cb = new CheckBox();
                                cb.AutoSize = true;
                                cb.Text = pda.Description + (pda.Name is string ? " (" + pda.Name + ")" : "");
                                cb.Checked = (bool)pi.GetValue(this.Plugin, null);
                                cb.Tag = pi;
                                this.Controls.Add(cb);
                            }

                            if (pi.PropertyType.Equals(typeof(string)))
                            {
                                PluginParametersTextBox cb = new PluginParametersTextBox();
                                cb.AutoSize = true;
                                cb.label.Text = pda.Description + (pda.Name is string  ? " (" + pda.Name + ")" : "");
                                cb.textBox.Text = (string)pi.GetValue(this.Plugin, null);
                                cb.Tag = pi;
                                this.Controls.Add(cb);
                            }

                            if (pi.PropertyType.IsEnum)
                            {
                                ComboBox cb = new ComboBox();
                                cb.AutoSize = true;
                                cb.Tag = pi;
                                cb.Items.AddRange(Enum.GetNames(pi.PropertyType));
                                cb.SelectedIndex = 0;
                                this.Controls.Add(cb);
                            }

                        }
                    }

                    this.Count = this.Controls.Count;
                    this.Refresh();
                }
            }

            base.Refresh();
        }

        public void Save()
        {
            if (this.Plugin is IPlugin)
            {
                lock (this.Plugin)
                {
                    foreach (Control c in this.Controls)
                    {
                        PropertyInfo pi = c.Tag as PropertyInfo;

                        if (pi is PropertyInfo)
                        {
                            if (pi.PropertyType.Equals(typeof(bool)))
                            {
                                pi.SetValue(this.Plugin, (c as CheckBox).Checked, null);
                            }

                            if (pi.PropertyType.Equals(typeof(string)))
                            {
                                pi.SetValue(this.Plugin, (c as PluginParametersTextBox).textBox.Text, null);
                            }

                            if (pi.PropertyType.IsEnum && (c as ComboBox).SelectedItem != null && Enum.IsDefined(pi.PropertyType, (c as ComboBox).SelectedItem))
                            {
                                pi.SetValue(this.Plugin, Enum.Parse(pi.PropertyType,(c as ComboBox).SelectedItem.ToString(), true), null);
                            }
                        }
                    }
                }
            }
        }
    }
}
