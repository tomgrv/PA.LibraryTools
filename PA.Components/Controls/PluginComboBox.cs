﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using PA.Plugin.Components.Interfaces;
using PA.Plugin.Components;


namespace PA.Plugin.Components.Controls
{
    public partial class PluginComboBox : ComboBox, IPluginHandler, ISupportInitialize
    {
        #region Plugin Management

        public void BuildWithType<T>() where T : IPluginOperation
        {
            this.BeginUpdate();

            this.Items.Clear();

            foreach (IPluginOperation o in this.Imports.OfType<T>())
            {
                this.Items.Add(o);
            }

            this.Enabled = this.Items.Count > 0;

            this.EndUpdate();
        }

        #endregion

        public PluginComboBox()
        {
            InitializeComponent();
        }

        #region IPluginSource Members

        [Category("Plugin Management")]
        public PluginLoader Loader { get; set; }

        [Browsable(false)]
        public IPlugin Plugin
        {
            get { return this.SelectedItem is IPlugin ? this.SelectedItem as IPlugin : null; }
        }

        [Category("Plugin Management")]
        public event EventHandler<PluginEventArgs> PluginChanged;

        #endregion

        #region IPartImportsSatisfiedNotification Members

        [ImportMany]
        protected virtual IEnumerable<IPluginOperation> Imports { get; set; }

        public virtual void OnImportsSatisfied()
        {
            this.BuildWithType<IPluginOperation>();
        }

        #endregion

        #region ISupportInitialize Members

        public virtual void BeginInit()
        {
            if (!this.DesignMode)
            {

            }
        }

        public virtual void EndInit()
        {
            if (!this.DesignMode)
            {
                if (this.Loader is PluginLoader)
                {
                    this.Loader.DelayedComposition(this);
                }
            }
        }

        #endregion

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (this.PluginChanged != null && this.SelectedItem is IPlugin)
            {
                this.PluginChanged(this, new PluginEventArgs(this.SelectedItem as IPlugin));
            }
        }
    }
}
