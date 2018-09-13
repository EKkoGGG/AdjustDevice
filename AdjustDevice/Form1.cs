using System;
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

namespace AdjustDevice
{

    public partial class Form1 : Form
    {
        public string path = Environment.CurrentDirectory + @"\Config.ini";


        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {

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
            serialPort1.PortName = comboBox1.Text;
            serialPort1.Open();
            button2.Enabled = false;
            if (!button3.Enabled)
            {
                button3.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            button3.Enabled = false;
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


    }
}
