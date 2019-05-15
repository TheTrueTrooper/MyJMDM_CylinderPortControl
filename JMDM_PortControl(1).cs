using System.Collections;
using System.Collections.Generic;

using System.IO.Ports;
using System;
using System.Text;

public class JMDM_PortControl {
    private string send_port;
    private SerialPort sp_send = null;

    public void Open_Port(string sendport)
    {
        send_port = sendport;
        sp_send = new SerialPort(send_port, 9600, Parity.None, 8, StopBits.One);
        sp_send.ReadTimeout = 400;
        try
        {
            if (!sp_send.IsOpen)
            {
                sp_send.Open();
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Port_Error:" + ex.Message);
        }
    }

    //num  1-6   height  10-250    3个缸的行程控制
    public void Send_Data(int num,int height,bool limit = true)
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


    public void Reset_Zerp()
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
}
