﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace AdjustDevice
{

    public partial class Form1 : Form
    {
        public string path = Environment.CurrentDirectory + @"\Config.ini";
        //static string getData;
        //public string datatemp;
        public string[] command =
            { "01044001000175CA", "01044002000185CA","010440030001D40A","01044004000165CB",
              "010440050001340B","010440060001C40B","01044007000195CB","010440080001A5CB","010440090001F40B"
        };
        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SearchAndAddSerialToComboBox(serialPort1, comboBox1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SearchAndAddSerialToComboBox(serialPort1, comboBox1);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Rows.Add(9);
            IniFile ini = new IniFile(path);
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells[0].Value = row.Index + 1;
            }

            if (ini.ExistINIFile(path))
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Cells[1].Value = ini.IniReadValue("name", "name" + row.Index.ToString());
                    row.Cells[3].Value = ini.IniReadValue("Magnification", "Mag" + row.Index.ToString());
                    row.Cells[4].Value = ini.IniReadValue("unit", "unit" + row.Index.ToString());
                    row.Cells[5].Value = ini.IniReadValue("symbol", "symbol" + row.Index.ToString());
                    row.Cells[6].Value = ini.IniReadValue("Remark", "Remark" + row.Index.ToString());
                }
            }

            serialPort1.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            comboBox3.Text = "9600";
        }

        double temp1;
        string InputStr;
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //byte data;
            //data = (byte)serialPort1.ReadByte();
            //string str = Convert.ToString(data, 16).ToUpper();
            //datatemp += (str.Length == 1 ? "0" + str : str);

            //byte data;
            //data = (byte)serialPort1.ReadByte();
            //string str = Convert.ToString(data, 16).ToUpper();
            //datatemp += (str.Length == 1 ? "0" + str : str);
            //textBox1.AppendText((str.Length == 1 ? "0" + str : str));

            byte[] readBuffer = new byte[10];

            serialPort1.Read(readBuffer, 0, 10);

            InputStr = BitConverter.ToString(readBuffer);
            //MessageBox.Show(InputStr);
            string str1 = InputStr.Substring(9, 5);
            string str2 = str1.Remove(2, 1);
            temp = Convert.ToInt32(str2, 16);
            temp1 = (double)temp;
            //MessageBox.Show(str2);

            //Thread.Sleep(500);
        }

        private void SearchAndAddSerialToComboBox(SerialPort MyPort, ComboBox MyBox)
        {                                                               //将可用端口号添加到ComboBox  
            string Buffer;                                              //缓存  
            MyBox.Items.Clear();                                        //清空ComboBox内容  
            for (int i = 1; i < 20; i++)                                //循环这里只扫描1-19  
            {
                try                                                     //核心原理是依靠try和catch完成遍历  
                {
                    Buffer = "COM" + i.ToString();
                    MyPort.PortName = Buffer;
                    MyPort.Open();                                      //如果失败，后面的代码不会执行                                                                          
                    MyBox.Items.Add(Buffer);                            //打开成功，添加至下俩列表  
                    MyPort.Close();                                     //关闭  
                    MyBox.Text = Buffer;
                }
                catch//出错了什么也不做继续循环  
                {
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            label4.ForeColor = Color.Green;
            serialPort1.BaudRate = Convert.ToInt32(comboBox3.Text);
            if (!button3.Enabled)
            {
                button3.Enabled = true;
            }
            try
            {
                serialPort1.PortName = comboBox1.Text;
                serialPort1.Open();

                for (int i = 0; i < 9; i++)
                {
                    SendData(i);
                    Thread.Sleep(50);
                    string Magnification = (string)dataGridView1.Rows[i].Cells[3].Value;
                    double Magnification1 = Convert.ToDouble(Magnification);
                    dataGridView1.Rows[i].Cells[2].Value = temp1 * Magnification1;
                }
                timer1.Enabled = true;
                //SendData(3);
                //Thread.Sleep(1000);
                //MessageBox.Show(temp.ToString());

                //Thread.Sleep(1000);
                //SendData(1);
                //timer1.Enabled = true;
                //for (int i = 0; i < 9; i++)
                //{
                //    SendData(i);
                //    Thread.Sleep(20);
                //    SendData(i);
                //    Thread.Sleep(20);
                //    SendData(i);
                //    Thread.Sleep(20);
                //    timer1.Enabled = true;
                //    Thread.Sleep(100);
                //    dataGridView1.Rows[i].Cells[2].Value = temp/1000;

                //}
                //SendData(0);
                //Thread.Sleep(20);
                //SendData(0);
                //Thread.Sleep(20);
                //SendData(0);
                //Thread.Sleep(20);
                //timer1.Enabled = true;

            }
            catch (Exception)
            {
            }

        }

        public void SendData(int i)
        {
            //byte[] Data = new byte[1];
            //string command1 = command[i];
            //for (int a = 0; a < command1.Length / 2; a++)
            //{
            //    //每次取两位字符组成一个16进制
            //    Data[0] = Convert.ToByte(command1.Substring(a * 2, 2), 16);
            //    serialPort1.Write(Data, 0, 1);//循环发送（如果输入字符为0A0BB,则只发送0A,0B）
            //}

            byte[] Data = new byte[8];
            string command1 = command[i];
            for (int a = 0; a < command1.Length / 2; a++)
            {
                //每次取两位字符组成一个16进制
                Data[a] = Convert.ToByte(command1.Substring(a * 2, 2), 16);
                //serialPort1.Write(Data, 0, 8);//循环发送（如果输入字符为0A0BB,则只发送0A,0B）
            }
            serialPort1.Write(Data, 0, 8);//循环发送（如果输入字符为0A0BB,则只发送0A,0B）

        }

        private void button3_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            timer1.Enabled = false;
            button3.Enabled = false;
            label4.ForeColor = Color.Red;
            if (button2.Enabled)
            {
                button3.Enabled = false;
            }
            else
            {
                button2.Enabled = true;
            }



        }
        private void button4_Click(object sender, EventArgs e)
        {
            IniFile ini = new IniFile(path);
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                ini.IniWriteValue("name", "name" + row.Index.ToString(), row.Cells[1].Value.ToString());
                ini.IniWriteValue("Magnification", "Mag" + row.Index.ToString(), row.Cells[3].Value.ToString());
                ini.IniWriteValue("unit", "unit" + row.Index.ToString(), row.Cells[4].Value.ToString());
                ini.IniWriteValue("symbol", "symbol" + row.Index.ToString(), row.Cells[5].Value.ToString());
                ini.IniWriteValue("Remark", "Remark" + row.Index.ToString(), row.Cells[6].Value.ToString());
            }
            MessageBox.Show("保存配置成功");
            FileInfo info = new FileInfo(path);
            if (info.Exists)
            {
                info.Attributes = FileAttributes.Hidden;
            }
        }


        public class IniFile
        {
            public string Path;

            public IniFile(string path)
            {
                this.Path = path;
            }

            #region 声明读写INI文件的API函数

            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);

            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section, string key, string defVal, Byte[] retVal, int size, string filePath);

            #endregion

            /// <summary>
            /// 写INI文件
            /// </summary>
            /// <param name="section">段落</param>
            /// <param name="key">键</param>
            /// <param name="iValue">值</param>
            public void IniWriteValue(string section, string key, string iValue)
            {
                WritePrivateProfileString(section, key, iValue, this.Path);
            }

            /// <summary>
            /// 读取INI文件
            /// </summary>
            /// <param name="section">段落</param>
            /// <param name="key">键</param>
            /// <returns>返回的键值</returns>
            public string IniReadValue(string section, string key)
            {
                StringBuilder temp = new StringBuilder(255);

                int i = GetPrivateProfileString(section, key, "", temp, 255, this.Path);
                return temp.ToString();
            }

            /// <summary>
            /// 读取INI文件
            /// </summary>
            /// <param name="Section">段，格式[]</param>
            /// <param name="Key">键</param>
            /// <returns>返回byte类型的section组或键值组</returns>
            public byte[] IniReadValues(string section, string key)
            {
                byte[] temp = new byte[255];

                int i = GetPrivateProfileString(section, key, "", temp, 255, this.Path);
                return temp;
            }

            public bool ExistINIFile(string path)
            {
                return File.Exists(path);
            }
        }

        int temp;

        private void timer1_Tick(object sender, EventArgs e)
        {
            //timer1.Enabled = false;
            try
            {
                //serialPort1.Close();
                //serialPort1.PortName = comboBox1.Text;
                //serialPort1.Open();
                //serialPort1.BaudRate = Convert.ToInt32(comboBox3.Text);
                for (int i = 0; i < 9; i++)
                {
                    SendData(i);
                    Thread.Sleep(50);
                    string Magnification = (string)dataGridView1.Rows[i].Cells[3].Value;
                    double Magnification1 = Convert.ToDouble(Magnification);
                    dataGridView1.Rows[i].Cells[2].Value = temp1 * Magnification1;
                }
            }
            catch
            {
            }
            
        }



        public int CRC16_Check(byte[] Pushdata, int length)
        {
            int Reg_CRC = 0xffff;
            int temp;
            int i, j;

            for (i = 0; i < length; i++)
            {
                temp = Pushdata[i];
                if (temp < 0) temp += 256;
                temp &= 0xff;
                Reg_CRC ^= temp;

                for (j = 0; j < 8; j++)
                {
                    if ((Reg_CRC & 0x0001) == 0x0001)
                        Reg_CRC = (Reg_CRC >> 1) ^ 0xA001;
                    else
                        Reg_CRC >>= 1;
                }
            }
            return (Reg_CRC & 0xffff);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                SendCorrect();
                Thread.Sleep(200);
                textBox2.Text = InputStr;
                string send = BitConverter.ToString(data);
                //MessageBox.Show(send);
                //string crcResult1 = string.Join("-", Regex.Matches(send, @"..").Cast<Match>().ToList());
                //MessageBox.Show(crcResult1);
                if (textBox2.Text == send)
                {
                    MessageBox.Show("校正成功！！");
                }
                else
                {
                    MessageBox.Show("校正失败！！ 请检查配置或报文！！");
                }
            }
            catch
            {
                MessageBox.Show("校正失败，请检查是否已经打开串口并且填写相应的信息");
            }
        }
        string crcResult;
        byte[] data = new byte[10];
        public void SendCorrect()
        {
            string adress = comboBox2.Text;
            string select;
            int i = adress.IndexOf("x");
            int j = adress.IndexOf(")");
            adress = (adress.Substring(i + 1)).Substring(0, j - i - 1);
            //MessageBox.Show(adress);
            if (radioButton1.Checked)
            {
                select = "06";
            }
            else
            {
                select = "10";
            }
            string current = textBox1.Text;
            float num = float.Parse(current);
            //MessageBox.Show(num.ToString());
            var num1 = BitConverter.GetBytes(num);
            current = BitConverter.ToString(num1.Reverse().ToArray()).Replace("-", "");
            //MessageBox.Show(current);
            string unCrc = "01" + select + adress + current;
            //MessageBox.Show(unCrc);
            byte[] Data = new byte[8];
            string crcTemp;
            string crc;
            for (int a = 0; a < unCrc.Length / 2; a++)
            {
                //每次取两位字符组成一个16进制
                Data[a] = Convert.ToByte(unCrc.Substring(a * 2, 2), 16);
            }
            int result = CRC16_Check(Data, 8);
            //MessageBox.Show(result.ToString());
            crcTemp = result.ToString("x").ToUpper().PadLeft(4, '0');
            //MessageBox.Show(crcTemp);
            crc = crcTemp.Substring(2, 2) + crcTemp.Substring(0, 2);
            crcResult = unCrc + crc;
            //MessageBox.Show(crcResult);
            //byte[] data = new byte[10];
            for (int b = 0; b < crcResult.Length / 2; b++)
            {
                data[b] = Convert.ToByte(crcResult.Substring(b * 2, 2), 16);
            }
            serialPort1.Write(data, 0, 10);
            //MessageBox.Show("校正成功！！");
        }






    }
}
