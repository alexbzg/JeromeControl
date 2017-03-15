﻿using Jerome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetComm
{
    public class JeromeConnectionState
    {
        public bool watch = false;
        public bool active = false;
        public bool[] linesStates;
        public JeromeController controller = null;
        public int[] lines;

        public JeromeConnectionState()
        {
            linesStates = new bool[FNetComm.lines.Count()];
            lines = new int[FNetComm.lines.Count()];
            for (int co = 0; co < linesStates.Count(); co++)
            {
                linesStates[co] = false;
                lines[co] = co + 1;
            }

        }

        public bool connected
        {
            get
            {
                return controller != null && controller.connected;
            }
        }
    }

    public class ConnectionFormState : JeromeConnectionState
    {
        public System.Drawing.Point formLocation;
        public System.Drawing.Size formSize;
    }

    public class NetCommConfig
    {
        public JeromeConnectionParams[] connections;
        public ConnectionFormState[] states;
        public string[] buttonLabels;
        public int[] esMhzValues;
        public int[] esButtons;
        public string esHost;
        public int esPort;
        public int lastConnection;
    }
}