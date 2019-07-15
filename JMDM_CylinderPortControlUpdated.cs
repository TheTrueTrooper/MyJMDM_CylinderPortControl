using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace MyJMDM_CylinderPortControl
{
    public class JMDM_CylinderPortControlUpdated : IDisposable
    {
        byte[] DefaultHeight = new byte[] { 0, 0, 0, 0, 0, 0 };

        private SerialPort sp_send { set; get; }

        public string send_port { private set; get; }

        public int CylinderCount { private set; get; }

        public JMDM_CylinderPortControlUpdated(string sendport, int CylinderCount = 6)
        {
            send_port = sendport;
            sp_send = new SerialPort(send_port, 9600, Parity.None, 8, StopBits.One);
            sp_send.ReadTimeout = 400;
            this.CylinderCount = CylinderCount;
        }

        public void Open_Port()
        {
            if (!sp_send.IsOpen)
            {
                sp_send.Open();
            }
        }

        //num  1-6   height  10-250    3个缸的行程控制
        //num 1-6 height 10-250 3 cylinder stroke control
        public void SetCylinderHeight(byte num, byte height)
        {
            string str = string.Format("OC(0{0},{1:D3})", num, height);
            if (sp_send.IsOpen)
            {
                sp_send.Write(str);
            }
            else
                throw new Exception("Port is not open");
        }

        public void SetAllCylinders(byte?[] Heights)
        {
            for (int i = 0; i < Heights.Length; i++)
            {
                if (sp_send.IsOpen)
                {
                    if (Heights[i] != null)
                        sp_send.Write(string.Format("OC({0},{1:D3})", i, Heights[i]));
                }
                else
                    throw new Exception("Port is not open");
            }
        }

        public void ZeroAllCylinders()
        {
            for (int i = 0; i < CylinderCount; i++)
            {
                if (sp_send.IsOpen)
                {

                    sp_send.Write(string.Format("OC({0},000)", i));
                }
                else
                    throw new Exception("Port is not open");
            }
        }

        public void Close_Port()
        {
            if (sp_send.IsOpen)
            {
                sp_send.Close();
            }
        }

        public void Dispose()
        {
            Close_Port();
            sp_send.Dispose();
        }

        ~JMDM_CylinderPortControlUpdated()
        {
            Close_Port();
            sp_send.Dispose();
        }
    }
}

