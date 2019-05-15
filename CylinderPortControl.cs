using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace MyJMDM_CylinderPortControl
{
    public class CylinderPortControl : IDisposable
    {
        public static Func<string, object, int> OnErrorAction = null;

        public string send_port { private set; get; }
        private SerialPort sp_send = null;


        public CylinderPortControl(string sendport)
        {
            send_port = sendport;
            sp_send = new SerialPort(send_port, 9600, Parity.None, 8, StopBits.One);
            sp_send.ReadTimeout = 400;
        }

        public void Open_Port()
        {
            try
            {
                if (!sp_send.IsOpen)
                {
                    sp_send.Open();
                }
            }
            catch (Exception ex)
            {
                OnErrorAction.Invoke(ex.Message, this);
            }
        }

        //num  1-6   height  10-250    3个缸的行程控制
        //num 1-6 height 10-250 3 cylinder stroke control
        public void Send_Data(int num, int height, bool limit = true)
        {
            if (num < 1 || num > 4)
            {
                return;
            }
            if (limit)
            {
                if (height < 10)
                {
                    height = 10;
                }
                else if (height > 250)
                {
                    height = 250;
                }
            }

            string str = string.Format("OC(0{0},{1:D3})", num, height);
            if (sp_send.IsOpen)
            {
                sp_send.Write(str);
            }
        }

        //direction  10-250  旋转控制  127是停止  与127的相差分别向左或向右的速度
        //direction 10-250 rotation control 127 is the speed difference between 127 and left, respectively.
        public void Rotate_Control(int direction)
        {
            if (direction < 10)
                direction = 10;
            else if (direction > 250)
                direction = 250;
            string str = string.Format("OC(04,{0:D3})", direction);
            if (sp_send.IsOpen)
            {
                sp_send.Write(str);
            }

        }


        public void Reset_Zero()
        {
            string str = string.Format("OC(01,000)");
            if (sp_send.IsOpen)
            {
                sp_send.Write(str);
            }
            str = string.Format("OC(02,000)");
            if (sp_send.IsOpen)
            {
                sp_send.Write(str);
            }
            str = string.Format("OC(03,000)");
            if (sp_send.IsOpen)
            {
                sp_send.Write(str);
            }
            str = string.Format("OC(04,000)");
            if (sp_send.IsOpen)
            {
                sp_send.Write(str);
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
            sp_send.Dispose();
        }

        ~CylinderPortControl()
        {
            sp_send.Dispose();
        }
    }
}

