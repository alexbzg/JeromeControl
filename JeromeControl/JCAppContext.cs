using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using ExpertSync;
using AntennaeRotator;
using NetComm;
using WX0B;

/*
 * ==============================================================
 * @ID       $Id: MainForm.cs 971 2010-09-30 16:09:30Z ww $
 * @created  2008-07-31
 * ==============================================================
 *
 * The official license for this file is shown next.
 * Unofficially, consider this e-postcardware as well:
 * if you find this module useful, let us know via e-mail, along with
 * where in the world you are and (if applicable) your website address.
 */

/* ***** BEGIN LICENSE BLOCK *****
 * Version: MIT License
 *
 * Copyright (c) 2010 Michael Sorens http://www.simple-talk.com/author/michael-sorens/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *
 * ***** END LICENSE BLOCK *****
 */

namespace JeromeControl
{

    /// <summary>
    /// Framework for running application as a tray app.
    /// </summary>
    /// <remarks>
    /// Tray app code adapted from "Creating Applications with NotifyIcon in Windows Forms", Jessica Fosler,
    /// http://windowsclient.net/articles/notifyiconapplications.aspx
    /// </remarks>
    public class JCAppContext : ApplicationContext
    {
        // Icon graphic from http://prothemedesign.com/circular-icons/
        private static readonly string IconFileName = "icon_ant1.ico";
        private static readonly string DefaultTooltip = "Управление антеннами";

        public JCConfig config;

        private ExpertSyncConnector esConnector;
        private ToolStripMenuItem miExpertSync = new ToolStripMenuItem("ExpertSync");
        private ToolStripMenuItem miWX0B = new ToolStripMenuItem("WX0B");

        private FRotator fRotator;
        private FNetComm fNetComm;
        volatile bool exiting;
        /// <summary>
		/// This class should be created and passed into Application.Run( ... )
		/// </summary>
		public JCAppContext()
        {
            InitializeContext();

        }

        public void writeConfig()
        {
            config.write();
        }

        public void showNotification( string title, string txt, ToolTipIcon icon)
        {
            notifyIcon.ShowBalloonTip(60 * 1000, title, txt, icon);
        }

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            miExpertSync.Text = "ExpertSync";
            miExpertSync.Image = null;
            if (config.esHost != null && config.esPort != 0)
            {
                miExpertSync.Text += " " + config.esHost + ":" + config.esPort.ToString();
                miExpertSync.Image = esConnector.connected ? Properties.Resources.signal_green : Properties.Resources.signal_red;
            }
        }

        private void esMessage( object Sender, MessageEventArgs e )
        {
            int mhz = ((int)e.vfoa) / 1000000;
            System.Diagnostics.Debug.WriteLine("TRX " + (e.trx ? "ON" : "OFF"));
            if (fRotator != null)
                fRotator.esMessage(mhz);
            if (fNetComm != null)
                fNetComm.esMessage(mhz, e.trx);
        }

        # region the child forms

        private void ShowIntroForm()
        {
        }

        private void ShowDetailsForm()
        {
            /*
            if (detailsForm == null)
            {
                detailsForm = new DetailsForm { HostManager = hostManager };
                detailsForm.Closed += detailsForm_Closed; // avoid reshowing a disposed form
                detailsForm.Show();
            }
            else { detailsForm.Activate(); }*/
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e) { ShowIntroForm(); }

        // From http://stackoverflow.com/questions/2208690/invoke-notifyicons-context-menu
        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon, null);
            }
        }


        // attach to context menu items
        private void showHelpItem_Click(object sender, EventArgs e) { ShowIntroForm(); }
        private void showDetailsItem_Click(object sender, EventArgs e) { ShowDetailsForm(); }

        // null out the forms so we know to create a new one.
//        private void detailsForm_Closed(object sender, EventArgs e) { detailsForm = null; }
//        private void mainForm_Closed(object sender, EventArgs e) { introForm = null; }

        # endregion the child forms

        # region generic code framework

        private System.ComponentModel.IContainer components;	// a list of components to dispose when the context is disposed
        private NotifyIcon notifyIcon;				            // the icon that sits in the system tray

        private void InitializeContext()
        {
            components = new System.ComponentModel.Container();
            notifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = new Icon( IconFileName ),
                Text = DefaultTooltip,
                Visible = true
            };
            notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            notifyIcon.MouseUp += notifyIcon_MouseUp;
            config = JCConfig.read();
            esConnect();

            ToolStripMenuItem miRotator = new ToolStripMenuItem("AntennaNetRotator");
            miRotator.Click += miRotator_Click;
            notifyIcon.ContextMenuStrip.Items.Add(miRotator);

            ToolStripMenuItem miNetComm = new ToolStripMenuItem("NetComm");
            miNetComm.Click += miNetComm_Click;
            notifyIcon.ContextMenuStrip.Items.Add(miNetComm);


            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

            miWX0B.Click += miWX0B_Click;
            notifyIcon.ContextMenuStrip.Items.Add(miWX0B);

            miExpertSync.Click += esItem_Click;
            notifyIcon.ContextMenuStrip.Items.Add(miExpertSync);

            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem miExit = new ToolStripMenuItem("Выход");
            miExit.Click += exitItem_Click;
            notifyIcon.ContextMenuStrip.Items.Add(miExit);

            if (config.antennaeRotatorActive)
                createRotatorForm();

            if (config.netCommActive)
                createNetCommForm();
        }

        private void miWX0B_Click( object sender, EventArgs e)
        {
            FWX0B fWX0Bc = new FWX0B();
            if (fWX0Bc.ShowDialog() == DialogResult.OK &&
                (config.WX0BHost != fWX0Bc.host || config.WX0BPort != fWX0Bc.port))
            {
                config.WX0BHost = fWX0Bc.host;
                config.WX0BPort = fWX0Bc.port;
            }

        }

        private void miNetComm_Click(object sender, EventArgs e)
        {
            createNetCommForm();
            fNetComm.Focus();
            config.netCommActive = true;
            writeConfig();
        }

        private void esConnect()
        {
            if (esConnector != null && esConnector.connected)
                esConnector.disconnect();
            if (config.esHost != null && config.esPort != 0)
            {
                writeConfig();
                esConnector = new ExpertSyncConnector(config.esHost, config.esPort);
                esConnector.reconnect = true;
                esConnector.connect();
                esConnector.onMessage += esMessage;
            }
        }

        /// <summary>
		/// When the application context is disposed, dispose things like the notify icon.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) { components.Dispose(); }
        }

        /// <summary>
        /// When the exit menu item is clicked, make a call to terminate the ApplicationContext.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitItem_Click(object sender, EventArgs e)
        {
            ExitThread();
        }

        private void esItem_Click( object sender, EventArgs e )
        {
            FESConnection fesc = new FESConnection();
            if ( fesc.ShowDialog() == DialogResult.OK && 
                ( config.esHost != fesc.host || config.esPort != fesc.port ) )
            {
                config.esHost = fesc.host;
                config.esPort = fesc.port;
                esConnect();
            }
        }

        private void miRotator_Click( object sender, EventArgs e )
        {
            createRotatorForm();
            fRotator.Focus();
            config.antennaeRotatorActive = true;
            writeConfig();
        }

        private void createNetCommForm()
        {
            if (fNetComm == null)
            {
                fNetComm = new FNetComm(this);
                fNetComm.Closed += formClosed;
            }
            fNetComm.Show();
        }



        private void createRotatorForm()
        {
            if (fRotator == null)
            {
                fRotator = new FRotator(this);
                fRotator.Closed += formClosed;
            }
            fRotator.Show();
        }

        private void formClosed( object sender, EventArgs e )
        {
            if (sender == fRotator)
            {
                fRotator = null;
                if (!exiting)
                {
                    config.antennaeRotatorActive = false;
                    writeConfig();
                }
            } else if (sender == fNetComm)
            {
                fNetComm = null;
                if (!exiting)
                {
                    config.netCommActive = false;
                    writeConfig();
                }
            }
        }

        /// <summary>
        /// If we are presently showing a form, clean it up.
        /// </summary>
        protected override void ExitThreadCore()
        {
            exiting = true;
            if (fRotator != null)
                fRotator.Close();
            if (fNetComm != null)
                fNetComm.Close();

            notifyIcon.Visible = false; // should remove lingering tray icon
            base.ExitThreadCore();
        }

        # endregion generic code framework

        # region support methods


        # endregion support methods


    }
}
