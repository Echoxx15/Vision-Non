using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ControllerDllCSharp;

namespace LightControlNet
{
    public partial class Frm_CST : Form
    {
        private int controllerHandle = 0;

        public int Rs232_or_Ethernet = 1;

        public int connect_state = 0;

        public int get_data_flag = 0;

        private int getIP_flag = 0;

        private static int workStationCount = 50;

        private ClassLibControllerDll.Host_prm[] mHost_prm = new ClassLibControllerDll.Host_prm[workStationCount];

        private int connectTimeOut = 1;

        private string[] m_oldSerialPortNames;

        private static Mutex ProcessLock;

        private static bool HasLock;

        private int flag5 = 0;

        private int controller_setUI = 0;
        public Frm_CST()
        {
            InitializeComponent();
        }

        private void get_wangka()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            int AdatterCnt = 0;
            int num = 20;
            int num2 = Marshal.SizeOf(typeof(ClassLibControllerDll.Adapter_prm));
            IntPtr intPtr = Marshal.AllocHGlobal(num2 * num);
            ClassLibControllerDll.Adapter_prm[] array = new ClassLibControllerDll.Adapter_prm[num];
            if (ClassLibControllerDll.GetAdapter(ref AdatterCnt, intPtr) == 10000)
            {
                for (int i = 0; i < num; i++)
                {
                    IntPtr ptr = intPtr + i * num2;
                    array[i] = (ClassLibControllerDll.Adapter_prm)Marshal.PtrToStructure(ptr, typeof(ClassLibControllerDll.Adapter_prm));
                }
                if (AdatterCnt > 20)
                {
                    AdatterCnt = 20;
                }
                string[] array2 = new string[AdatterCnt];
                for (int j = 0; j < AdatterCnt; j++)
                {
                    array2[j] = new string(array[j].cSn);
                    comboBox1.Items.Add(array2[j]);
                }
                string[] array3 = new string[AdatterCnt];
                for (int k = 0; k < AdatterCnt; k++)
                {
                    array3[k] = new string(array[k].cIp);
                    comboBox2.Items.Add(array3[k]);
                }
                comboBox2.Text = array3[0];
                label36.Text = "成功获取网卡信息";
            }
            else
            {
                label36.Text = "获取网卡信息失败";
            }
        }

        private void get_IP()
        {
            int controllerCnt = 0;
            comboBoxSn.Items.Clear();
            textBoxIpAddress.Text = "";
            textBoxSm.Text = "";
            textBoxGw.Text = "";
            textBoxSn.Text = "";
            string adapterIP = comboBox2.Text;
            int num = Marshal.SizeOf(typeof(ClassLibControllerDll.Host_prm));
            IntPtr intPtr = Marshal.AllocHGlobal(num * workStationCount);
            if (ClassLibControllerDll.GetHost(ref controllerCnt, intPtr, adapterIP) == 10000)
            {
                for (int i = 0; i < workStationCount; i++)
                {
                    IntPtr ptr = intPtr + i * num;
                    mHost_prm[i] = (ClassLibControllerDll.Host_prm)Marshal.PtrToStructure(ptr, typeof(ClassLibControllerDll.Host_prm));
                }
                if (controllerCnt > 0)
                {
                    string[] array = new string[controllerCnt];
                    for (int j = 0; j < controllerCnt; j++)
                    {
                        array[j] = new string(mHost_prm[j].cSn);
                        comboBoxSn.Items.Add(array[j]);
                        comboBoxSn.Text = array[0];
                        textBoxSn.Text = comboBoxSn.Text;
                    }
                    string[] array2 = new string[controllerCnt];
                    for (int k = 0; k < controllerCnt; k++)
                    {
                        array2[k] = new string(mHost_prm[k].cIp);
                        textBoxIpAddress.Text = array2[0];
                    }
                    buttonSetHost.Enabled = true;
                    label36.Text = $"成功获取控制器信息，该网卡下控制器数量为：{controllerCnt}";
                }
                else
                {
                    buttonSetHost.Enabled = false;
                    label36.Text = $"该网卡下控制器数量为：{controllerCnt}";
                }
            }
            else
            {
                label36.Text = "获取控制器信息失败";
            }
            int cb = Marshal.SizeOf(typeof(ClassLibControllerDll.Controller_prm));
            IntPtr intPtr2 = Marshal.AllocHGlobal(cb);
            if (ClassLibControllerDll.GetConfigure(mHost_prm[0].cMac, intPtr2, adapterIP) == 10000)
            {
                IntPtr ptr2 = intPtr2;
                ClassLibControllerDll.Controller_prm controller_prm = (ClassLibControllerDll.Controller_prm)Marshal.PtrToStructure(ptr2, typeof(ClassLibControllerDll.Controller_prm));
                string text = new string(controller_prm.cSm);
                textBoxSm.Text = text;
                string text2 = new string(controller_prm.cGw);
                textBoxGw.Text = text2;
                string text3 = new string(controller_prm.cIp);
                textBoxIpAddress.Text = text3;
                if (controller_prm.DHCP == '\0')
                {
                    checkBox_DHCP.Checked = false;
                }
                else
                {
                    checkBox_DHCP.Checked = true;
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            get_wangka();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = comboBox1.SelectedIndex;
            comboBox2.SelectedIndex = selectedIndex;
            getIP_flag = 1;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = comboBox2.SelectedIndex;
            comboBox1.SelectedIndex = selectedIndex;
            getIP_flag = 1;
        }

        private void buttonGetHost_Click(object sender, EventArgs e)
        {
            get_wangka();
            get_IP();
        }

        private void comboBoxSn_SelectedIndexChanged(object sender, EventArgs e)
        {
            string adapterIP = comboBox2.Text;
            int cb = Marshal.SizeOf(typeof(ClassLibControllerDll.Controller_prm));
            IntPtr intPtr = Marshal.AllocHGlobal(cb);
            if (ClassLibControllerDll.GetConfigure(mHost_prm[comboBoxSn.SelectedIndex].cMac, intPtr, adapterIP) == 10000)
            {
                IntPtr ptr = intPtr;
                ClassLibControllerDll.Controller_prm controller_prm = (ClassLibControllerDll.Controller_prm)Marshal.PtrToStructure(ptr, typeof(ClassLibControllerDll.Controller_prm));
                textBoxSn.Text = comboBoxSn.Text;
                string text = new string(controller_prm.cSm);
                textBoxSm.Text = text;
                string text2 = new string(controller_prm.cGw);
                textBoxGw.Text = text2;
                string text3 = new string(controller_prm.cIp);
                textBoxIpAddress.Text = text3;
                if (controller_prm.DHCP == '\0')
                {
                    checkBox_DHCP.Checked = false;
                }
                else
                {
                    checkBox_DHCP.Checked = true;
                }
            }
        }

        private void checkBox_DHCP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_DHCP.Checked)
            {
                textBoxSn.Enabled = false;
                textBoxIpAddress.Enabled = false;
                textBoxSm.Enabled = false;
                textBoxGw.Enabled = false;
            }
            else
            {
                textBoxSn.Enabled = true;
                textBoxIpAddress.Enabled = true;
                textBoxSm.Enabled = true;
                textBoxGw.Enabled = true;
            }
        }

        private void buttonSetHost_Click(object sender, EventArgs e)
        {
            ClassLibControllerDll.Controller_prm mConPrm = default(ClassLibControllerDll.Controller_prm);
            string text = "000000000000000000000";
            mConPrm.cSn = text.ToCharArray();
            text = "0000000000000000";
            mConPrm.cIp = text.ToCharArray();
            mConPrm.cGw = text.ToCharArray();
            mConPrm.cSm = text.ToCharArray();
            for (uint num = 0u; num < 21; num++)
            {
                mConPrm.cSn[num] = '\0';
            }
            for (uint num2 = 0u; num2 < 16; num2++)
            {
                mConPrm.cIp[num2] = '\0';
            }
            for (uint num3 = 0u; num3 < 16; num3++)
            {
                mConPrm.cGw[num3] = '\0';
            }
            for (uint num4 = 0u; num4 < 16; num4++)
            {
                mConPrm.cSm[num4] = '\0';
            }
            textBoxIpAddress.Text.ToCharArray().CopyTo(mConPrm.cIp, 0);
            textBoxGw.Text.ToCharArray().CopyTo(mConPrm.cGw, 0);
            textBoxSm.Text.ToCharArray().CopyTo(mConPrm.cSm, 0);
            textBoxSn.Text.ToCharArray().CopyTo(mConPrm.cSn, 0);
            if (checkBox_DHCP.Checked)
            {
                mConPrm.DHCP = '\u0001';
            }
            else
            {
                mConPrm.DHCP = '\0';
            }
            string adapterIP = comboBox2.Text;
            if (ClassLibControllerDll.SetConfigure(mHost_prm[comboBoxSn.SelectedIndex].cMac, ref mConPrm, adapterIP) == 10000)
            {
                label36.Text = "成功设置控制器信息";
            }
            else
            {
                label36.Text = "设置IP失败,请检查地址的合法性";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "IP连接")
            {
                string ipAddress = textBoxIpAddress.Text.ToString();
                int num = ClassLibControllerDll.ConnectIP(ipAddress, connectTimeOut, ref controllerHandle);
                if (num == 10000)
                {
                    button1.Text = "断开连接";
                    textBoxMessageShow.Text = "IP连接控制器成功";
                    button2.Enabled = false;
                    Rs232_or_Ethernet = 0;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBoxSn.Enabled = false;
                    buttonGetHost.Enabled = false;
                    get_data_flag = 99;
                    connect_state = 1;
                    label36.Text = "网口已连接";
                }
                else
                {
                    label36.Text = "IP连接控制器失败";
                }
            }
            else if (button1.Text == "断开连接")
            {
                int num2 = ClassLibControllerDll.DestroyIpConnection(controllerHandle);
                if (num2 == 10000)
                {
                    button1.Text = "IP连接";
                    textBoxMessageShow.Text = "IP断开控制器成功";
                    button2.Enabled = true;
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    comboBoxSn.Enabled = true;
                    buttonGetHost.Enabled = true;
                    connect_state = 0;
                    label36.Text = "请连接串口或网口...";
                }
                else
                {
                    label36.Text = "IP断开控制器失败";
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "打开串口")
            {
                int result = 0;
                if (comboBox_serial_port_name.Text.Length == 0)
                {
                    MessageBox.Show("端口不存在！");
                    return;
                }
                if (int.TryParse(new string(comboBox_serial_port_name.Text.Where((char a) => char.IsDigit(a)).ToArray()), out result))
                {
                }
                int num = ClassLibControllerDll.CreateSerialPort(result, ref controllerHandle);
                if (num == 10000)
                {
                    button2.Text = "断开串口";
                    textBoxMessageShow.Text = "成功打开串口";
                    button1.Enabled = false;
                    Rs232_or_Ethernet = 1;
                    comboBox_serial_port_name.Enabled = false;
                    connect_state = 1;
                    get_data_flag = 99;
                    label36.Text = "串口已连接";
                }
                else
                {
                    label36.Text = "打开串口失败";
                }
            }
            else if (button2.Text == "断开串口")
            {
                int num2 = ClassLibControllerDll.ReleaseSerialPort(controllerHandle);
                if (num2 == 10000)
                {
                    button2.Text = "打开串口";
                    textBoxMessageShow.Text = "成功释放串口";
                    button1.Enabled = true;
                    comboBox_serial_port_name.Enabled = true;
                    connect_state = 0;
                    label36.Text = "请连接串口或网口...";
                }
                else
                {
                    label36.Text = "释放串口失败";
                }
            }
        }

        private void button32_Click(object sender, EventArgs e)
        {
            int num = 100;
            int num2 = 1;
            if (ClassLibControllerDll.SetDigitalValue(1, num2, num, controllerHandle) == 10000)
            {
                textBoxMessageShow.Text = $"成功设置{num2}通道亮度值为：{num}";
            }
            else
            {
                textBoxMessageShow.Text = $"设置{num2}通道亮度值失败";
            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
            ClassLibControllerDll.MulIntensityValue[] array = new ClassLibControllerDll.MulIntensityValue[2];
            array[0].channelIndex = 1;
            array[0].intensity = 100;
            array[1].channelIndex = 2;
            array[1].intensity = 200;
            if (ClassLibControllerDll.SetMulDigitalValue(1, array, array.Length, controllerHandle) == 10000)
            {
                textBoxMessageShow.Text = "成功设置多通道亮度值";
            }
            else
            {
                textBoxMessageShow.Text = "设置多通道亮度值失败";
            }
        }

        private void InitSerialPort()
        {
            string[] portNames = SerialPort.GetPortNames();
            if (portNames.Length != 0)
            {
                m_oldSerialPortNames = portNames;
                ComboBox.ObjectCollection items = comboBox_serial_port_name.Items;
                object[] items2 = portNames;
                items.AddRange(items2);
                comboBox_serial_port_name.SelectedIndex = 0;
            }
        }

        public static void GetProcessLock()
        {
            ProcessLock = new Mutex(initiallyOwned: false, "Global\\LuYao.Toolkit[" + GetUid() + "]", out HasLock);
            if (!HasLock)
            {
                ActiveWindow();
                Environment.Exit(0);
            }
        }

        private static string GetUid()
        {
            byte[] array = Encoding.UTF8.GetBytes(Assembly.GetExecutingAssembly().Location);
            using (MD5 mD = MD5.Create())
            {
                array = mD.ComputeHash(array);
            }
            return BitConverter.ToString(array);
        }

        public static void ActiveWindow()
        {
            using Process process = Process.GetCurrentProcess();
            string processName = process.ProcessName;
            Process[] processesByName = Process.GetProcessesByName(processName);
            Process[] array = processesByName;
            foreach (Process process2 in array)
            {
                if (process2.MainModule.FileName == process.MainModule.FileName)
                {
                    IntPtr mainWindowHandle = process2.MainWindowHandle;
                    SwitchToThisWindow(mainWindowHandle, fAltTab: true);
                    break;
                }
            }
        }

        public static void ReleaseLock()
        {
            if (ProcessLock != null && HasLock)
            {
                ProcessLock.Dispose();
                HasLock = false;
            }
        }

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        private void Form1_Load(object sender, EventArgs e)
        {
            InitSerialPort();
            panel_tongxin.Visible = true;
            panel3.Visible = false;
            panel_pinshan4.Visible = false;
            panel_pinshan8.Visible = false;
            panel_shuzi2.Visible = false;
            panel_shuzi4.Visible = false;
            panel_shuzi8.Visible = false;
            panel_tongxin.Dock = DockStyle.Fill;
            buttonUI_5.Visible = false;
            buttonUI_6.Visible = false;
            buttonUI_7.Visible = false;
            panel7.Location = new Point(19, 157);
            panel8.Location = new Point(19, 207);
            buttonUI_8.Visible = false;
            panel6.Visible = false;
        }

        private void checking_serial_port()
        {
            string[] portNames = SerialPort.GetPortNames();
            if (portNames.Length == 0)
            {
                return;
            }
            int num = 0;
            if (m_oldSerialPortNames != null && portNames.Length == m_oldSerialPortNames.Length)
            {
                for (int i = 0; i < portNames.Length; i++)
                {
                    if (portNames[i] == m_oldSerialPortNames[i])
                    {
                        num++;
                    }
                }
                if (num == portNames.Length)
                {
                    return;
                }
                m_oldSerialPortNames = portNames;
            }
            else
            {
                m_oldSerialPortNames = portNames;
            }
            comboBox_serial_port_name.Items.Clear();
            ComboBox.ObjectCollection items = comboBox_serial_port_name.Items;
            object[] items2 = portNames;
            items.AddRange(items2);
            comboBox_serial_port_name.SelectedIndex = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            checking_serial_port();
            if (getIP_flag == 1)
            {
                getIP_flag = 0;
                get_IP();
            }
            if (get_data_flag == 99)
            {
                int channelIndex = 1;
                int intensity = 0;
                if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex, ref intensity, controllerHandle) != 10000 && ClassLibControllerDll.GetTriggerModeValue(Rs232_or_Ethernet, ref intensity, controllerHandle) != 10000)
                {
                    label36.Text = "串口(网口)下未挂载控制器";
                }
                get_data_flag = 0;
            }
            if (get_data_flag == 14)
            {
                int LightDelay = 0;
                if (ClassLibControllerDll.GetLightDelayValue(Rs232_or_Ethernet, 1, ref LightDelay, controllerHandle) == 10000)
                {
                    ClassLibControllerDll.GetPulseWidthValue(Rs232_or_Ethernet, 1, ref LightDelay, controllerHandle);
                    int pulseWidth = 51;
                    int channelIndex2 = 1;
                    if (ClassLibControllerDll.SetPulseWidthValue(Rs232_or_Ethernet, channelIndex2, pulseWidth, controllerHandle) == 10000)
                    {
                        pinshan4_MK1.Maximum = 999m;
                        pinshan4_MK2.Maximum = 999m;
                        pinshan4_MK3.Maximum = 999m;
                        pinshan4_MK4.Maximum = 999m;
                        pinshan4_MKtrackBar1.Maximum = 999;
                        pinshan4_MKtrackBar2.Maximum = 999;
                        pinshan4_MKtrackBar3.Maximum = 999;
                        pinshan4_MKtrackBar4.Maximum = 999;
                    }
                    else
                    {
                        pinshan4_MK1.Maximum = 50m;
                        pinshan4_MK2.Maximum = 50m;
                        pinshan4_MK3.Maximum = 50m;
                        pinshan4_MK4.Maximum = 50m;
                        pinshan4_MKtrackBar1.Maximum = 50;
                        pinshan4_MKtrackBar2.Maximum = 50;
                        pinshan4_MKtrackBar3.Maximum = 50;
                        pinshan4_MKtrackBar4.Maximum = 50;
                    }
                    ClassLibControllerDll.SetPulseWidthValue(Rs232_or_Ethernet, channelIndex2, LightDelay, controllerHandle);
                    get_CH1();
                    get_CH2();
                    get_CH3();
                    get_CH4();
                    get_other();
                }
                get_data_flag = 0;
            }
            if (get_data_flag == 18)
            {
                int LightDelay2 = 0;
                if (ClassLibControllerDll.GetLightDelayValue(Rs232_or_Ethernet, 1, ref LightDelay2, controllerHandle) == 10000)
                {
                    get_8CH1();
                    get_8CH2();
                    get_8CH3();
                    get_8CH4();
                    get_8CH5();
                    get_8CH6();
                    get_8CH7();
                    get_8CH8();
                    get_8other();
                    get_8othershuzi();
                }
                get_data_flag = 0;
            }
            if (get_data_flag == 22)
            {
                int channelIndex3 = 1;
                int intensity2 = 0;
                int intensity3 = 0;
                if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex3, ref intensity3, controllerHandle) == 10000)
                {
                    if (intensity3 > 255)
                    {
                        shuzi2_numericUpDown1.Maximum = 999m;
                        shuzi2_trackBar1.Maximum = 999;
                    }
                    else if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, channelIndex3, 999, controllerHandle) == 10000)
                    {
                        shuzi2_numericUpDown1.Maximum = 999m;
                        shuzi2_trackBar1.Maximum = 999;
                        ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, channelIndex3, intensity3, controllerHandle);
                    }
                    else
                    {
                        shuzi2_numericUpDown1.Maximum = 255m;
                        shuzi2_trackBar1.Maximum = 255;
                    }
                    shuzi2_numericUpDown1.Value = intensity3;
                    channelIndex3 = 2;
                    if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex3, ref intensity2, controllerHandle) == 10000)
                    {
                        label9.Visible = true;
                        shuzi2_trackBar2.Visible = true;
                        shuzi2_numericUpDown2.Visible = true;
                        shuzi2_numericUpDown2.Value = intensity2;
                    }
                    else
                    {
                        label9.Visible = false;
                        shuzi2_trackBar2.Visible = false;
                        shuzi2_numericUpDown2.Visible = false;
                    }
                    int TriggerMode = 0;
                    if (ClassLibControllerDll.GetLightModeValue(Rs232_or_Ethernet, ref TriggerMode, controllerHandle) == 10000)
                    {
                        if (TriggerMode == 1)
                        {
                            changliang.Checked = true;
                        }
                        else
                        {
                            changmie.Checked = true;
                        }
                    }
                }
                get_data_flag = 0;
            }
            if (get_data_flag == 24)
            {
                int channelIndex4 = 1;
                int intensity4 = 0;
                if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex4, ref intensity4, controllerHandle) == 10000)
                {
                    shuzi4_numericUpDown1.Value = intensity4;
                    channelIndex4 = 2;
                    if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex4, ref intensity4, controllerHandle) == 10000)
                    {
                        shuzi4_numericUpDown2.Value = intensity4;
                    }
                    channelIndex4 = 3;
                    if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex4, ref intensity4, controllerHandle) == 10000)
                    {
                        shuzi4_numericUpDown3.Value = intensity4;
                    }
                    channelIndex4 = 4;
                    if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex4, ref intensity4, controllerHandle) == 10000)
                    {
                        shuzi4_numericUpDown4.Value = intensity4;
                    }
                    int TriggerMode2 = 0;
                    if (ClassLibControllerDll.GetLightModeValue(Rs232_or_Ethernet, ref TriggerMode2, controllerHandle) == 10000)
                    {
                        if (TriggerMode2 == 1)
                        {
                            radioButton2.Checked = true;
                        }
                        else
                        {
                            radioButton1.Checked = true;
                        }
                    }
                }
                get_data_flag = 0;
            }
            if (get_data_flag != 28)
            {
                return;
            }
            int channelIndex5 = 1;
            int intensity5 = 0;
            if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex5, ref intensity5, controllerHandle) == 10000)
            {
                shuzi8_numericUpDown1.Value = intensity5;
                channelIndex5 = 2;
                if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex5, ref intensity5, controllerHandle) == 10000)
                {
                    shuzi8_numericUpDown2.Value = intensity5;
                }
                channelIndex5 = 3;
                if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex5, ref intensity5, controllerHandle) == 10000)
                {
                    shuzi8_numericUpDown3.Value = intensity5;
                }
                channelIndex5 = 4;
                if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex5, ref intensity5, controllerHandle) == 10000)
                {
                    shuzi8_numericUpDown4.Value = intensity5;
                }
                channelIndex5 = 5;
                if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex5, ref intensity5, controllerHandle) == 10000)
                {
                    shuzi8_numericUpDown5.Value = intensity5;
                }
                channelIndex5 = 6;
                if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex5, ref intensity5, controllerHandle) == 10000)
                {
                    shuzi8_numericUpDown6.Value = intensity5;
                }
                channelIndex5 = 7;
                if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex5, ref intensity5, controllerHandle) == 10000 && intensity5 < 256)
                {
                    shuzi8_numericUpDown7.Value = intensity5;
                }
                channelIndex5 = 8;
                if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex5, ref intensity5, controllerHandle) == 10000 && intensity5 < 256)
                {
                    shuzi8_numericUpDown8.Value = intensity5;
                }
                int TriggerMode3 = 0;
                if (ClassLibControllerDll.GetLightModeValue(Rs232_or_Ethernet, ref TriggerMode3, controllerHandle) == 10000)
                {
                    if (TriggerMode3 == 1)
                    {
                        radioButton4.Checked = true;
                    }
                    else
                    {
                        radioButton3.Checked = true;
                    }
                }
            }
            get_data_flag = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel4.Location = new Point(0, 57);
            label55.Text = "通信设置";
            panel_tongxin.Visible = true;
            panel3.Visible = false;
            panel_pinshan4.Visible = false;
            panel_pinshan8.Visible = false;
            panel_shuzi2.Visible = false;
            panel_shuzi4.Visible = false;
            panel_shuzi8.Visible = false;
            panel_tongxin.Dock = DockStyle.Fill;
            button3.BackColor = SystemColors.GradientInactiveCaption;
            button4.BackColor = SystemColors.InactiveBorder;
            button5.BackColor = SystemColors.InactiveBorder;
            button7.BackColor = SystemColors.InactiveBorder;
            buttonUI_5.Visible = false;
            buttonUI_6.Visible = false;
            buttonUI_7.Visible = false;
            panel7.Location = new Point(19, 157);
            panel8.Location = new Point(19, 207);
            buttonUI_8.Visible = false;
            panel6.Visible = false;
            set_UI_color();
            controller_setUI = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel4.Location = new Point(0, 107);
            if (controller_setUI == 0)
            {
                label55.Text = "频闪控制器";
            }
            button3.BackColor = SystemColors.InactiveBorder;
            button4.BackColor = SystemColors.GradientInactiveCaption;
            button5.BackColor = SystemColors.InactiveBorder;
            button7.BackColor = SystemColors.InactiveBorder;
            buttonUI_5.Visible = false;
            buttonUI_6.Visible = false;
            buttonUI_7.Visible = false;
            panel7.Location = new Point(19, 217);
            panel8.Location = new Point(19, 267);
            buttonUI_8.Visible = false;
            panel6.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (controller_setUI == 0)
            {
                label55.Text = "数字控制器";
            }
            panel_tongxin.Visible = false;
            panel3.Visible = true;
            panel4.Location = new Point(0, 157);
            panel6.Visible = false;
            button3.BackColor = SystemColors.InactiveBorder;
            button4.BackColor = SystemColors.InactiveBorder;
            button5.BackColor = SystemColors.GradientInactiveCaption;
            button7.BackColor = SystemColors.InactiveBorder;
            panel7.Location = new Point(19, 157);
            buttonUI_5.Visible = true;
            buttonUI_6.Visible = true;
            buttonUI_7.Visible = true;
            panel8.Location = new Point(19, 297);
            buttonUI_8.Visible = false;
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            if (controller_setUI == 0)
            {
                label55.Text = "恒流控制器";
            }
            panel_tongxin.Visible = false;
            panel3.Visible = true;
            panel4.Location = new Point(0, 207);
            panel6.Visible = false;
            button3.BackColor = SystemColors.InactiveBorder;
            button4.BackColor = SystemColors.InactiveBorder;
            button5.BackColor = SystemColors.InactiveBorder;
            button7.BackColor = SystemColors.GradientInactiveCaption;
            panel7.Location = new Point(19, 157);
            buttonUI_5.Visible = false;
            buttonUI_6.Visible = false;
            buttonUI_7.Visible = false;
            panel8.Location = new Point(19, 207);
            buttonUI_8.Visible = true;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "*.csv";
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "csv files|*.csv";
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.FileName = "AnalysisData";
            if (saveFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.FileName != null)
            {
                string fileName = saveFileDialog.FileName;
                StreamWriter streamWriter = new StreamWriter(fileName, append: false, Encoding.Default);
                StringBuilder stringBuilder = new StringBuilder();
                streamWriter.WriteLine(stringBuilder.ToString());
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            openFileDialog.ShowDialog();
            string fileName = openFileDialog.FileName;
            if (!(fileName == ""))
            {
                FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                StreamReader streamReader = new StreamReader(stream, Encoding.Default);
                string text = null;
                string[] array = null;
                int num = 1;
                while ((text = streamReader.ReadLine()) != null)
                {
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void set_UI_color()
        {
            switch (controller_setUI)
            {
                case 1:
                    buttonUI_1.BackColor = SystemColors.InactiveBorder;
                    break;
                case 2:
                    buttonUI_2.BackColor = SystemColors.InactiveBorder;
                    break;
                case 3:
                    buttonUI_3.BackColor = SystemColors.InactiveBorder;
                    break;
                case 4:
                    buttonUI_4.BackColor = SystemColors.InactiveBorder;
                    break;
                case 5:
                    buttonUI_5.BackColor = SystemColors.InactiveBorder;
                    break;
                case 6:
                    buttonUI_6.BackColor = SystemColors.InactiveBorder;
                    break;
                case 7:
                    buttonUI_7.BackColor = SystemColors.InactiveBorder;
                    break;
                case 8:
                    buttonUI_8.BackColor = SystemColors.InactiveBorder;
                    break;
            }
        }

        private void get_CH1()
        {
            int PulseWidth = 0;
            int channelIndex = 1;
            if (ClassLibControllerDll.GetPulseWidthValue(Rs232_or_Ethernet, channelIndex, ref PulseWidth, controllerHandle) == 10000)
            {
                pinshan4_MK1.Value = PulseWidth;
            }
            int LightDelay = 0;
            if (ClassLibControllerDll.GetLightDelayValue(Rs232_or_Ethernet, channelIndex, ref LightDelay, controllerHandle) == 10000)
            {
                pinshan4_LIGHT1.Value = LightDelay;
            }
            int CameraDelay = 0;
            if (ClassLibControllerDll.GetCameraDelayValue(Rs232_or_Ethernet, channelIndex, ref CameraDelay, controllerHandle) == 10000)
            {
                pinshan4_Camera1.Value = CameraDelay;
            }
        }

        private void get_CH2()
        {
            int PulseWidth = 0;
            int channelIndex = 2;
            if (ClassLibControllerDll.GetPulseWidthValue(Rs232_or_Ethernet, channelIndex, ref PulseWidth, controllerHandle) == 10000)
            {
                pinshan4_MK2.Value = PulseWidth;
            }
            int LightDelay = 0;
            if (ClassLibControllerDll.GetLightDelayValue(Rs232_or_Ethernet, channelIndex, ref LightDelay, controllerHandle) == 10000)
            {
                pinshan4_LIGHT2.Value = LightDelay;
            }
            int CameraDelay = 0;
            if (ClassLibControllerDll.GetCameraDelayValue(Rs232_or_Ethernet, channelIndex, ref CameraDelay, controllerHandle) == 10000)
            {
                pinshan4_Camera2.Value = CameraDelay;
            }
        }

        private void get_CH3()
        {
            int PulseWidth = 0;
            int channelIndex = 3;
            if (ClassLibControllerDll.GetPulseWidthValue(Rs232_or_Ethernet, channelIndex, ref PulseWidth, controllerHandle) == 10000)
            {
                pinshan4_MK3.Value = PulseWidth;
            }
            int LightDelay = 0;
            if (ClassLibControllerDll.GetLightDelayValue(Rs232_or_Ethernet, channelIndex, ref LightDelay, controllerHandle) == 10000)
            {
                pinshan4_LIGHT3.Value = LightDelay;
            }
            int CameraDelay = 0;
            if (ClassLibControllerDll.GetCameraDelayValue(Rs232_or_Ethernet, channelIndex, ref CameraDelay, controllerHandle) == 10000)
            {
                pinshan4_Camera3.Value = CameraDelay;
            }
        }

        private void get_CH4()
        {
            int PulseWidth = 0;
            int channelIndex = 4;
            if (ClassLibControllerDll.GetPulseWidthValue(Rs232_or_Ethernet, channelIndex, ref PulseWidth, controllerHandle) == 10000)
            {
                pinshan4_MK4.Value = PulseWidth;
            }
            int LightDelay = 0;
            if (ClassLibControllerDll.GetLightDelayValue(Rs232_or_Ethernet, channelIndex, ref LightDelay, controllerHandle) == 10000)
            {
                pinshan4_LIGHT4.Value = LightDelay;
            }
            int CameraDelay = 0;
            if (ClassLibControllerDll.GetCameraDelayValue(Rs232_or_Ethernet, channelIndex, ref CameraDelay, controllerHandle) == 10000)
            {
                pinshan4_Camera4.Value = CameraDelay;
            }
        }

        private void get_other()
        {
            int TriggerMode = 0;
            if (ClassLibControllerDll.GetTriggerModeValue(Rs232_or_Ethernet, ref TriggerMode, controllerHandle) == 10000)
            {
                if (TriggerMode == 1)
                {
                    in_chufa.Checked = true;
                }
                else
                {
                    out_chufa.Checked = true;
                }
            }
            int TriggerCycle = 0;
            if (ClassLibControllerDll.GetTriggerCycleValue(Rs232_or_Ethernet, ref TriggerCycle, controllerHandle) == 10000 && TriggerCycle >= 15 && TriggerCycle <= 999)
            {
                pinshan4_TriggerCycle.Value = TriggerCycle;
            }
            int CameraSignal = 0;
            if (ClassLibControllerDll.GetCameraSignalValue(Rs232_or_Ethernet, ref CameraSignal, controllerHandle) == 10000)
            {
                if (CameraSignal == 1)
                {
                    shangshengyan.Checked = true;
                }
                else
                {
                    xiajiangyan.Checked = true;
                }
            }
        }

        private void get_8CH1()
        {
            int PulseWidth = 0;
            int channelIndex = 1;
            if (ClassLibControllerDll.GetPulseWidthValue(Rs232_or_Ethernet, channelIndex, ref PulseWidth, controllerHandle) == 10000)
            {
                pinshan8_MK1.Value = PulseWidth;
            }
            int LightDelay = 0;
            if (ClassLibControllerDll.GetLightDelayValue(Rs232_or_Ethernet, channelIndex, ref LightDelay, controllerHandle) == 10000)
            {
                pinshan8_LIGHT1.Value = LightDelay;
            }
            int CameraDelay = 0;
            if (ClassLibControllerDll.GetCameraDelayValue(Rs232_or_Ethernet, channelIndex, ref CameraDelay, controllerHandle) == 10000)
            {
                pinshan8_Camera1.Value = CameraDelay;
            }
        }

        private void get_8CH2()
        {
            int PulseWidth = 0;
            int channelIndex = 2;
            if (ClassLibControllerDll.GetPulseWidthValue(Rs232_or_Ethernet, channelIndex, ref PulseWidth, controllerHandle) == 10000)
            {
                pinshan8_MK2.Value = PulseWidth;
            }
            int LightDelay = 0;
            if (ClassLibControllerDll.GetLightDelayValue(Rs232_or_Ethernet, channelIndex, ref LightDelay, controllerHandle) == 10000)
            {
                pinshan8_LIGHT2.Value = LightDelay;
            }
            int CameraDelay = 0;
            if (ClassLibControllerDll.GetCameraDelayValue(Rs232_or_Ethernet, channelIndex, ref CameraDelay, controllerHandle) == 10000)
            {
                pinshan8_Camera2.Value = CameraDelay;
            }
        }

        private void get_8CH3()
        {
            int PulseWidth = 0;
            int channelIndex = 3;
            if (ClassLibControllerDll.GetPulseWidthValue(Rs232_or_Ethernet, channelIndex, ref PulseWidth, controllerHandle) == 10000)
            {
                pinshan8_MK3.Value = PulseWidth;
            }
            int LightDelay = 0;
            if (ClassLibControllerDll.GetLightDelayValue(Rs232_or_Ethernet, channelIndex, ref LightDelay, controllerHandle) == 10000)
            {
                pinshan8_LIGHT3.Value = LightDelay;
            }
            int CameraDelay = 0;
            if (ClassLibControllerDll.GetCameraDelayValue(Rs232_or_Ethernet, channelIndex, ref CameraDelay, controllerHandle) == 10000)
            {
                pinshan8_Camera3.Value = CameraDelay;
            }
        }

        private void get_8CH4()
        {
            int PulseWidth = 0;
            int channelIndex = 4;
            if (ClassLibControllerDll.GetPulseWidthValue(Rs232_or_Ethernet, channelIndex, ref PulseWidth, controllerHandle) == 10000)
            {
                pinshan8_MK4.Value = PulseWidth;
            }
            int LightDelay = 0;
            if (ClassLibControllerDll.GetLightDelayValue(Rs232_or_Ethernet, channelIndex, ref LightDelay, controllerHandle) == 10000)
            {
                pinshan8_LIGHT4.Value = LightDelay;
            }
            int CameraDelay = 0;
            if (ClassLibControllerDll.GetCameraDelayValue(Rs232_or_Ethernet, channelIndex, ref CameraDelay, controllerHandle) == 10000)
            {
                pinshan8_Camera4.Value = CameraDelay;
            }
        }

        private void get_8CH5()
        {
            int PulseWidth = 0;
            int channelIndex = 5;
            if (ClassLibControllerDll.GetPulseWidthValue(Rs232_or_Ethernet, channelIndex, ref PulseWidth, controllerHandle) == 10000)
            {
                pinshan8_MK5.Value = PulseWidth;
            }
            int LightDelay = 0;
            if (ClassLibControllerDll.GetLightDelayValue(Rs232_or_Ethernet, channelIndex, ref LightDelay, controllerHandle) == 10000)
            {
                pinshan8_LIGHT5.Value = LightDelay;
            }
            int CameraDelay = 0;
            if (ClassLibControllerDll.GetCameraDelayValue(Rs232_or_Ethernet, channelIndex, ref CameraDelay, controllerHandle) == 10000)
            {
                pinshan8_Camera5.Value = CameraDelay;
            }
        }

        private void get_8CH6()
        {
            int PulseWidth = 0;
            int channelIndex = 6;
            if (ClassLibControllerDll.GetPulseWidthValue(Rs232_or_Ethernet, channelIndex, ref PulseWidth, controllerHandle) == 10000)
            {
                pinshan8_MK6.Value = PulseWidth;
            }
            int LightDelay = 0;
            if (ClassLibControllerDll.GetLightDelayValue(Rs232_or_Ethernet, channelIndex, ref LightDelay, controllerHandle) == 10000)
            {
                pinshan8_LIGHT6.Value = LightDelay;
            }
            int CameraDelay = 0;
            if (ClassLibControllerDll.GetCameraDelayValue(Rs232_or_Ethernet, channelIndex, ref CameraDelay, controllerHandle) == 10000)
            {
                pinshan8_Camera6.Value = CameraDelay;
            }
        }

        private void get_8CH7()
        {
            int PulseWidth = 0;
            int channelIndex = 7;
            if (ClassLibControllerDll.GetPulseWidthValue(Rs232_or_Ethernet, channelIndex, ref PulseWidth, controllerHandle) == 10000)
            {
                pinshan8_MK7.Value = PulseWidth;
            }
            int LightDelay = 0;
            if (ClassLibControllerDll.GetLightDelayValue(Rs232_or_Ethernet, channelIndex, ref LightDelay, controllerHandle) == 10000)
            {
                pinshan8_LIGHT7.Value = LightDelay;
            }
            int CameraDelay = 0;
            if (ClassLibControllerDll.GetCameraDelayValue(Rs232_or_Ethernet, channelIndex, ref CameraDelay, controllerHandle) == 10000)
            {
                pinshan8_Camera7.Value = CameraDelay;
            }
        }

        private void get_8CH8()
        {
            int PulseWidth = 0;
            int channelIndex = 8;
            if (ClassLibControllerDll.GetPulseWidthValue(Rs232_or_Ethernet, channelIndex, ref PulseWidth, controllerHandle) == 10000)
            {
                pinshan8_MK8.Value = PulseWidth;
            }
            int LightDelay = 0;
            if (ClassLibControllerDll.GetLightDelayValue(Rs232_or_Ethernet, channelIndex, ref LightDelay, controllerHandle) == 10000)
            {
                pinshan8_LIGHT8.Value = LightDelay;
            }
            int CameraDelay = 0;
            if (ClassLibControllerDll.GetCameraDelayValue(Rs232_or_Ethernet, channelIndex, ref CameraDelay, controllerHandle) == 10000)
            {
                pinshan8_Camera8.Value = CameraDelay;
            }
        }

        private void get_8other()
        {
            int TriggerMode = 0;
            if (ClassLibControllerDll.GetTriggerModeValue(Rs232_or_Ethernet, ref TriggerMode, controllerHandle) == 10000)
            {
                if (TriggerMode == 1)
                {
                    pinsan8_neichufa.Checked = true;
                }
                else
                {
                    pinsan8_waichufa.Checked = true;
                }
            }
            int TriggerCycle = 0;
            if (ClassLibControllerDll.GetTriggerCycleValue(Rs232_or_Ethernet, ref TriggerCycle, controllerHandle) == 10000 && TriggerCycle >= 15 && TriggerCycle <= 999)
            {
                pinshan8_TriggerCycle.Value = TriggerCycle;
            }
            int CameraSignal = 0;
            if (ClassLibControllerDll.GetCameraSignalValue(Rs232_or_Ethernet, ref CameraSignal, controllerHandle) == 10000)
            {
                if (CameraSignal == 1)
                {
                    pinsan8_shangshengyan.Checked = true;
                }
                else
                {
                    pinsan8_xiajiangyan.Checked = true;
                }
            }
        }

        private void get_8othershuzi()
        {
            int channelIndex = 1;
            int intensity = 0;
            if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex, ref intensity, controllerHandle) != 10000)
            {
                return;
            }
            pinsan8_numericUpDown1.Value = intensity;
            channelIndex = 2;
            intensity = 0;
            if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex, ref intensity, controllerHandle) == 10000)
            {
                pinsan8_numericUpDown2.Value = intensity;
            }
            channelIndex = 3;
            intensity = 0;
            if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex, ref intensity, controllerHandle) == 10000)
            {
                pinsan8_numericUpDown3.Value = intensity;
            }
            channelIndex = 4;
            intensity = 0;
            if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex, ref intensity, controllerHandle) == 10000)
            {
                pinsan8_numericUpDown4.Value = intensity;
            }
            channelIndex = 5;
            intensity = 0;
            if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex, ref intensity, controllerHandle) == 10000)
            {
                pinsan8_numericUpDown5.Value = intensity;
            }
            channelIndex = 6;
            intensity = 0;
            if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex, ref intensity, controllerHandle) == 10000)
            {
                pinsan8_numericUpDown6.Value = intensity;
            }
            channelIndex = 7;
            intensity = 0;
            if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex, ref intensity, controllerHandle) == 10000)
            {
                pinsan8_numericUpDown7.Value = intensity;
            }
            channelIndex = 8;
            intensity = 0;
            if (ClassLibControllerDll.GetDigitalValue(Rs232_or_Ethernet, channelIndex, ref intensity, controllerHandle) == 10000)
            {
                pinsan8_numericUpDown8.Value = intensity;
            }
            int TriggerMode = 0;
            if (ClassLibControllerDll.GetLightModeValue(Rs232_or_Ethernet, ref TriggerMode, controllerHandle) == 10000)
            {
                if (TriggerMode == 1)
                {
                    pinsan8_changliang.Checked = true;
                }
                else
                {
                    pinsan8_changmie.Checked = true;
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            set_UI_color();
            buttonUI_1.BackColor = SystemColors.ActiveCaption;
            label55.Text = buttonUI_1.Text;
            controller_setUI = 1;
            panel_pinshan4.Visible = true;
            panel_pinshan4.Dock = DockStyle.Fill;
            panel_tongxin.Visible = false;
            panel_pinshan8.Visible = false;
            panel_shuzi2.Visible = false;
            panel_shuzi4.Visible = false;
            panel_shuzi8.Visible = false;
            panel3.Visible = true;
            if (connect_state == 1)
            {
                get_data_flag = 14;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            set_UI_color();
            buttonUI_2.BackColor = SystemColors.ActiveCaption;
            label55.Text = buttonUI_2.Text;
            controller_setUI = 2;
            panel_pinshan8.Visible = true;
            panel_pinshan8.Dock = DockStyle.Fill;
            panel_tongxin.Visible = false;
            panel_pinshan4.Visible = false;
            panel_shuzi2.Visible = false;
            panel_shuzi4.Visible = false;
            panel_shuzi8.Visible = false;
            panel3.Visible = true;
            if (connect_state == 1)
            {
                get_data_flag = 18;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            set_UI_color();
            buttonUI_3.BackColor = SystemColors.ActiveCaption;
            label55.Text = buttonUI_3.Text;
            controller_setUI = 3;
            panel_tongxin.Visible = false;
            panel_pinshan8.Visible = false;
            panel_shuzi2.Visible = false;
            panel_shuzi4.Visible = false;
            panel_shuzi8.Visible = false;
            panel3.Visible = true;
            panel_pinshan4.Visible = true;
            panel_pinshan4.Dock = DockStyle.Fill;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            set_UI_color();
            buttonUI_4.BackColor = SystemColors.ActiveCaption;
            label55.Text = buttonUI_4.Text;
            controller_setUI = 4;
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            set_UI_color();
            buttonUI_5.BackColor = SystemColors.ActiveCaption;
            label55.Text = buttonUI_5.Text;
            controller_setUI = 5;
            panel_tongxin.Visible = false;
            panel_pinshan4.Visible = false;
            panel_pinshan8.Visible = false;
            panel_shuzi4.Visible = false;
            panel_shuzi8.Visible = false;
            panel3.Visible = true;
            panel_shuzi2.Visible = true;
            panel_shuzi2.Dock = DockStyle.Fill;
            if (connect_state == 1)
            {
                get_data_flag = 22;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            set_UI_color();
            buttonUI_6.BackColor = SystemColors.ActiveCaption;
            label55.Text = buttonUI_6.Text;
            controller_setUI = 6;
            panel_tongxin.Visible = false;
            panel_pinshan4.Visible = false;
            panel_pinshan8.Visible = false;
            panel_shuzi2.Visible = false;
            panel_shuzi8.Visible = false;
            panel3.Visible = true;
            panel_shuzi4.Visible = true;
            panel_shuzi4.Dock = DockStyle.Fill;
            if (connect_state == 1)
            {
                get_data_flag = 24;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            set_UI_color();
            buttonUI_7.BackColor = SystemColors.ActiveCaption;
            label55.Text = buttonUI_7.Text;
            controller_setUI = 7;
            panel_tongxin.Visible = false;
            panel_pinshan4.Visible = false;
            panel_pinshan8.Visible = false;
            panel_shuzi2.Visible = false;
            panel_shuzi4.Visible = false;
            panel3.Visible = true;
            panel_shuzi8.Visible = true;
            panel_shuzi8.Dock = DockStyle.Fill;
            if (connect_state == 1)
            {
                get_data_flag = 28;
            }
        }

        private void buttonUI_8_Click(object sender, EventArgs e)
        {
            set_UI_color();
            buttonUI_8.BackColor = SystemColors.ActiveCaption;
            label55.Text = buttonUI_8.Text;
            controller_setUI = 8;
            panel_tongxin.Visible = false;
            panel_pinshan4.Visible = false;
            panel_pinshan8.Visible = false;
            panel_shuzi4.Visible = false;
            panel_shuzi8.Visible = false;
            panel3.Visible = true;
            panel_shuzi2.Visible = true;
            panel_shuzi2.Dock = DockStyle.Fill;
            if (connect_state == 1)
            {
                get_data_flag = 22;
            }
        }

        private int check_connet()
        {
            if (connect_state == 0)
            {
                MessageBox.Show("通信未连接！");
                return 1;
            }
            return 0;
        }

        private void pinshan4_MKtrackBar1_Scroll(object sender, EventArgs e)
        {
            pinshan4_MK1.Value = pinshan4_MKtrackBar1.Value;
        }

        private void pinshan4_MKtrackBar2_Scroll(object sender, EventArgs e)
        {
            pinshan4_MK2.Value = pinshan4_MKtrackBar2.Value;
        }

        private void pinshan4_MKtrackBar3_Scroll(object sender, EventArgs e)
        {
            pinshan4_MK3.Value = pinshan4_MKtrackBar3.Value;
        }

        private void pinshan4_MKtrackBar4_Scroll(object sender, EventArgs e)
        {
            pinshan4_MK4.Value = pinshan4_MKtrackBar4.Value;
        }

        private void pinshan4_LIGHTtrackBar1_Scroll(object sender, EventArgs e)
        {
            pinshan4_LIGHT1.Value = pinshan4_LIGHTtrackBar1.Value;
        }

        private void pinshan4_LIGHTtrackBar2_Scroll(object sender, EventArgs e)
        {
            pinshan4_LIGHT2.Value = pinshan4_LIGHTtrackBar2.Value;
        }

        private void pinshan4_LIGHTtrackBar3_Scroll(object sender, EventArgs e)
        {
            pinshan4_LIGHT3.Value = pinshan4_LIGHTtrackBar3.Value;
        }

        private void pinshan4_LIGHTtrackBar4_Scroll(object sender, EventArgs e)
        {
            pinshan4_LIGHT4.Value = pinshan4_LIGHTtrackBar4.Value;
        }

        private void pinshan4_CameraTrackBar1_Scroll(object sender, EventArgs e)
        {
            pinshan4_Camera1.Value = pinshan4_CameraTrackBar1.Value;
        }

        private void pinshan4_CameraTrackBar2_Scroll(object sender, EventArgs e)
        {
            pinshan4_Camera2.Value = pinshan4_CameraTrackBar2.Value;
        }

        private void pinshan4_CameraTrackBar3_Scroll(object sender, EventArgs e)
        {
            pinshan4_Camera3.Value = pinshan4_CameraTrackBar3.Value;
        }

        private void pinshan4_CameraTrackBar4_Scroll(object sender, EventArgs e)
        {
            pinshan4_Camera4.Value = pinshan4_CameraTrackBar4.Value;
        }

        private void pinshan4_TriggerCycleTrackBar_Scroll(object sender, EventArgs e)
        {
            pinshan4_TriggerCycle.Value = pinshan4_TriggerCycleTrackBar.Value;
        }

        private void pinshan4_MK1_ValueChanged(object sender, EventArgs e)
        {
            pinshan4_MKtrackBar1.Value = (int)pinshan4_MK1.Value;
            if (get_data_flag != 14 && check_connet() != 1)
            {
                int num = (int)pinshan4_MK1.Value;
                int num2 = 1;
                if (ClassLibControllerDll.SetPulseWidthValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道脉宽值为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道脉宽值失败";
                }
            }
        }

        private void pinshan4_MK2_ValueChanged(object sender, EventArgs e)
        {
            pinshan4_MKtrackBar2.Value = (int)pinshan4_MK2.Value;
            if (get_data_flag != 14 && check_connet() != 1)
            {
                int num = (int)pinshan4_MK2.Value;
                int num2 = 2;
                if (ClassLibControllerDll.SetPulseWidthValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道脉宽值为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道脉宽值失败";
                }
            }
        }

        private void pinshan4_MK3_ValueChanged(object sender, EventArgs e)
        {
            pinshan4_MKtrackBar3.Value = (int)pinshan4_MK3.Value;
            if (get_data_flag != 14 && check_connet() != 1)
            {
                int num = (int)pinshan4_MK3.Value;
                int num2 = 3;
                if (ClassLibControllerDll.SetPulseWidthValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道脉宽值为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道脉宽值失败";
                }
            }
        }

        private void pinshan4_MK4_ValueChanged(object sender, EventArgs e)
        {
            pinshan4_MKtrackBar4.Value = (int)pinshan4_MK4.Value;
            if (get_data_flag != 14 && check_connet() != 1)
            {
                int num = (int)pinshan4_MK4.Value;
                int num2 = 4;
                if (ClassLibControllerDll.SetPulseWidthValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道脉宽值为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道脉宽值失败";
                }
            }
        }

        private void pinshan4_LIGHT1_ValueChanged(object sender, EventArgs e)
        {
            pinshan4_LIGHTtrackBar1.Value = (int)pinshan4_LIGHT1.Value;
            if (get_data_flag != 14 && check_connet() != 1)
            {
                int num = (int)pinshan4_LIGHT1.Value;
                int num2 = 1;
                if (ClassLibControllerDll.SetLightDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道光源延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道光源延时失败";
                }
            }
        }

        private void pinshan4_LIGHT2_ValueChanged(object sender, EventArgs e)
        {
            pinshan4_LIGHTtrackBar2.Value = (int)pinshan4_LIGHT2.Value;
            if (get_data_flag != 14 && check_connet() != 1)
            {
                int num = (int)pinshan4_LIGHT2.Value;
                int num2 = 2;
                if (ClassLibControllerDll.SetLightDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道光源延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道光源延时失败";
                }
            }
        }

        private void pinshan4_LIGHT3_ValueChanged(object sender, EventArgs e)
        {
            pinshan4_LIGHTtrackBar3.Value = (int)pinshan4_LIGHT3.Value;
            if (get_data_flag != 14 && check_connet() != 1)
            {
                int num = (int)pinshan4_LIGHT3.Value;
                int num2 = 3;
                if (ClassLibControllerDll.SetLightDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道光源延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道光源延时失败";
                }
            }
        }

        private void pinshan4_LIGHT4_ValueChanged(object sender, EventArgs e)
        {
            pinshan4_LIGHTtrackBar4.Value = (int)pinshan4_LIGHT4.Value;
            if (get_data_flag != 14 && check_connet() != 1)
            {
                int num = (int)pinshan4_LIGHT4.Value;
                int num2 = 4;
                if (ClassLibControllerDll.SetLightDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道光源延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道光源延时失败";
                }
            }
        }

        private void pinshan4_Camera1_ValueChanged(object sender, EventArgs e)
        {
            pinshan4_CameraTrackBar1.Value = (int)pinshan4_Camera1.Value;
            if (get_data_flag != 14 && check_connet() != 1)
            {
                int value = pinshan4_CameraTrackBar1.Value;
                int num = 1;
                if (ClassLibControllerDll.SetCameraDelayValue(Rs232_or_Ethernet, num, value, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num}通道相机延时为：{value}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num}通道相机延时失败";
                }
            }
        }

        private void pinshan4_Camera2_ValueChanged(object sender, EventArgs e)
        {
            pinshan4_CameraTrackBar2.Value = (int)pinshan4_Camera2.Value;
            if (get_data_flag != 14 && check_connet() != 1)
            {
                int value = pinshan4_CameraTrackBar2.Value;
                int num = 2;
                if (ClassLibControllerDll.SetCameraDelayValue(Rs232_or_Ethernet, num, value, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num}通道相机延时为：{value}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num}通道相机延时失败";
                }
            }
        }

        private void pinshan4_Camera3_ValueChanged(object sender, EventArgs e)
        {
            pinshan4_CameraTrackBar3.Value = (int)pinshan4_Camera3.Value;
            if (get_data_flag != 14 && check_connet() != 1)
            {
                int value = pinshan4_CameraTrackBar3.Value;
                int num = 3;
                if (ClassLibControllerDll.SetCameraDelayValue(Rs232_or_Ethernet, num, value, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num}通道相机延时为：{value}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num}通道相机延时失败";
                }
            }
        }

        private void pinshan4_Camera4_ValueChanged(object sender, EventArgs e)
        {
            pinshan4_CameraTrackBar4.Value = (int)pinshan4_Camera4.Value;
            if (get_data_flag != 14 && check_connet() != 1)
            {
                int value = pinshan4_CameraTrackBar4.Value;
                int num = 4;
                if (ClassLibControllerDll.SetCameraDelayValue(Rs232_or_Ethernet, num, value, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num}通道相机延时为：{value}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num}通道相机延时失败";
                }
            }
        }

        private void pinshan4_TriggerCycle_ValueChanged(object sender, EventArgs e)
        {
            pinshan4_TriggerCycleTrackBar.Value = (int)pinshan4_TriggerCycle.Value;
            if (get_data_flag != 14 && check_connet() != 1)
            {
                int num = (int)pinshan4_TriggerCycle.Value;
                if (ClassLibControllerDll.SetTriggerCycleValue(Rs232_or_Ethernet, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置内触发周期为：{num}ms";
                }
                else
                {
                    textBox1.Text = "设置内触发周期失败";
                }
            }
        }

        private void shangshengyan_CheckedChanged(object sender, EventArgs e)
        {
            if (get_data_flag == 14 || check_connet() == 1)
            {
                return;
            }
            int num = 0;
            num = (shangshengyan.Checked ? 1 : 0);
            if (ClassLibControllerDll.SetCameraSignalValue(Rs232_or_Ethernet, num, controllerHandle) == 10000)
            {
                if (num == 1)
                {
                    textBox1.Text = "成功设置上升沿";
                }
                else
                {
                    textBox1.Text = "成功设置下升沿";
                }
            }
            else if (num == 1)
            {
                textBox1.Text = "设置上升沿失败";
            }
            else
            {
                textBox1.Text = "设置下升沿失败";
            }
        }

        private void out_chufa_CheckedChanged(object sender, EventArgs e)
        {
            if (get_data_flag == 14 || check_connet() == 1)
            {
                return;
            }
            int num = 0;
            num = (in_chufa.Checked ? 1 : 0);
            if (ClassLibControllerDll.SetTriggerModeValue(Rs232_or_Ethernet, num, controllerHandle) == 10000)
            {
                if (num == 1)
                {
                    textBox1.Text = "成功设置内触发模式";
                }
                else
                {
                    textBox1.Text = "成功设置外触发模式";
                }
            }
            else if (num == 1)
            {
                textBox1.Text = "设置内触发模式失败";
            }
            else
            {
                textBox1.Text = "设置外触发模式失败";
            }
        }

        private void shuzi2_numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            shuzi2_trackBar1.Value = (int)shuzi2_numericUpDown1.Value;
            if (get_data_flag != 22 && check_connet() != 1)
            {
                int num = (int)shuzi2_numericUpDown1.Value;
                int num2 = 1;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void shuzi2_numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            shuzi2_trackBar2.Value = (int)shuzi2_numericUpDown2.Value;
            if (get_data_flag != 22 && check_connet() != 1)
            {
                int num = (int)shuzi2_numericUpDown2.Value;
                int num2 = 2;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void changliang_CheckedChanged(object sender, EventArgs e)
        {
            if (get_data_flag == 22 || check_connet() == 1)
            {
                return;
            }
            int num = 0;
            num = (changliang.Checked ? 1 : 0);
            if (ClassLibControllerDll.SetLightModeValue(Rs232_or_Ethernet, num, controllerHandle) == 10000)
            {
                if (num == 1)
                {
                    textBox1.Text = "成功设置常亮";
                }
                else
                {
                    textBox1.Text = "成功设置常灭";
                }
            }
            else if (num == 1)
            {
                textBox1.Text = "设置常亮失败";
            }
            else
            {
                textBox1.Text = "设置常灭失败";
            }
        }

        private void shuzi2_trackBar1_Scroll(object sender, EventArgs e)
        {
            shuzi2_numericUpDown1.Value = shuzi2_trackBar1.Value;
        }

        private void shuzi2_trackBar2_Scroll(object sender, EventArgs e)
        {
            shuzi2_numericUpDown2.Value = shuzi2_trackBar2.Value;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (get_data_flag == 24 || check_connet() == 1)
            {
                return;
            }
            int num = 0;
            num = (radioButton2.Checked ? 1 : 0);
            if (ClassLibControllerDll.SetLightModeValue(Rs232_or_Ethernet, num, controllerHandle) == 10000)
            {
                if (num == 1)
                {
                    textBox1.Text = "成功设置常亮";
                }
                else
                {
                    textBox1.Text = "成功设置常灭";
                }
            }
            else if (num == 1)
            {
                textBox1.Text = "设置常亮失败";
            }
            else
            {
                textBox1.Text = "设置常灭失败";
            }
        }

        private void shuzi4_trackBar1_Scroll(object sender, EventArgs e)
        {
            shuzi4_numericUpDown1.Value = shuzi4_trackBar1.Value;
        }

        private void shuzi4_trackBar2_Scroll(object sender, EventArgs e)
        {
            shuzi4_numericUpDown2.Value = shuzi4_trackBar2.Value;
        }

        private void shuzi4_trackBar3_Scroll(object sender, EventArgs e)
        {
            shuzi4_numericUpDown3.Value = shuzi4_trackBar3.Value;
        }

        private void shuzi4_trackBar4_Scroll(object sender, EventArgs e)
        {
            shuzi4_numericUpDown4.Value = shuzi4_trackBar4.Value;
        }

        private void shuzi4_numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            shuzi4_trackBar1.Value = (int)shuzi4_numericUpDown1.Value;
            if (get_data_flag != 24 && check_connet() != 1)
            {
                int num = (int)shuzi4_numericUpDown1.Value;
                int num2 = 1;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void shuzi4_numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            shuzi4_trackBar2.Value = (int)shuzi4_numericUpDown2.Value;
            if (get_data_flag != 24 && check_connet() != 1)
            {
                int num = (int)shuzi4_numericUpDown2.Value;
                int num2 = 2;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void shuzi4_numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            shuzi4_trackBar3.Value = (int)shuzi4_numericUpDown3.Value;
            if (get_data_flag != 24 && check_connet() != 1)
            {
                int num = (int)shuzi4_numericUpDown3.Value;
                int num2 = 3;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void shuzi4_numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            shuzi4_trackBar4.Value = (int)shuzi4_numericUpDown4.Value;
            if (get_data_flag != 24 && check_connet() != 1)
            {
                int num = (int)shuzi4_numericUpDown4.Value;
                int num2 = 4;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void shuzi8_trackBar1_Scroll(object sender, EventArgs e)
        {
            shuzi8_numericUpDown1.Value = shuzi8_trackBar1.Value;
        }

        private void shuzi8_trackBar2_Scroll(object sender, EventArgs e)
        {
            shuzi8_numericUpDown2.Value = shuzi8_trackBar2.Value;
        }

        private void shuzi8_trackBar3_Scroll(object sender, EventArgs e)
        {
            shuzi8_numericUpDown3.Value = shuzi8_trackBar3.Value;
        }

        private void shuzi8_trackBar4_Scroll(object sender, EventArgs e)
        {
            shuzi8_numericUpDown4.Value = shuzi8_trackBar4.Value;
        }

        private void shuzi8_trackBar5_Scroll(object sender, EventArgs e)
        {
            shuzi8_numericUpDown5.Value = shuzi8_trackBar5.Value;
        }

        private void shuzi8_trackBar6_Scroll(object sender, EventArgs e)
        {
            shuzi8_numericUpDown6.Value = shuzi8_trackBar6.Value;
        }

        private void shuzi8_trackBar7_Scroll(object sender, EventArgs e)
        {
            shuzi8_numericUpDown7.Value = shuzi8_trackBar7.Value;
        }

        private void shuzi8_trackBar8_Scroll(object sender, EventArgs e)
        {
            shuzi8_numericUpDown8.Value = shuzi8_trackBar8.Value;
        }

        private void shuzi8_numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            shuzi8_trackBar1.Value = (int)shuzi8_numericUpDown1.Value;
            if (get_data_flag != 28 && check_connet() != 1)
            {
                int num = (int)shuzi8_numericUpDown1.Value;
                int num2 = 1;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void shuzi8_numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            shuzi8_trackBar2.Value = (int)shuzi8_numericUpDown2.Value;
            if (get_data_flag != 28 && check_connet() != 1)
            {
                int num = (int)shuzi8_numericUpDown2.Value;
                int num2 = 2;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void shuzi8_numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            shuzi8_trackBar3.Value = (int)shuzi8_numericUpDown3.Value;
            if (get_data_flag != 28 && check_connet() != 1)
            {
                int num = (int)shuzi8_numericUpDown3.Value;
                int num2 = 3;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void shuzi8_numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            shuzi8_trackBar4.Value = (int)shuzi8_numericUpDown4.Value;
            if (get_data_flag != 28 && check_connet() != 1)
            {
                int num = (int)shuzi8_numericUpDown4.Value;
                int num2 = 4;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void shuzi8_numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            shuzi8_trackBar5.Value = (int)shuzi8_numericUpDown5.Value;
            if (get_data_flag != 28 && check_connet() != 1)
            {
                int num = (int)shuzi8_numericUpDown5.Value;
                int num2 = 5;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void shuzi8_numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            shuzi8_trackBar6.Value = (int)shuzi8_numericUpDown6.Value;
            if (get_data_flag != 28 && check_connet() != 1)
            {
                int num = (int)shuzi8_numericUpDown6.Value;
                int num2 = 6;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void shuzi8_numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            shuzi8_trackBar7.Value = (int)shuzi8_numericUpDown7.Value;
            if (get_data_flag != 28 && check_connet() != 1)
            {
                int num = (int)shuzi8_numericUpDown7.Value;
                int num2 = 7;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void shuzi8_numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            shuzi8_trackBar8.Value = (int)shuzi8_numericUpDown8.Value;
            if (get_data_flag != 28 && check_connet() != 1)
            {
                int num = (int)shuzi8_numericUpDown8.Value;
                int num2 = 8;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (get_data_flag == 28 || check_connet() == 1)
            {
                return;
            }
            int num = 0;
            num = (radioButton4.Checked ? 1 : 0);
            if (ClassLibControllerDll.SetLightModeValue(Rs232_or_Ethernet, num, controllerHandle) == 10000)
            {
                if (num == 1)
                {
                    textBox1.Text = "成功设置常亮";
                }
                else
                {
                    textBox1.Text = "成功设置常灭";
                }
            }
            else if (num == 1)
            {
                textBox1.Text = "设置常亮失败";
            }
            else
            {
                textBox1.Text = "设置常灭失败";
            }
        }

        private void button6_Click_2(object sender, EventArgs e)
        {
            int[] array = new int[80];
            char[] array2 = new char[80];
            int revLength = 0;
            string text = Convert.ToString(textBox2.Text);
            for (int i = 0; i < text.Length; i++)
            {
                array2[i] = text[i];
            }
            if (ClassLibControllerDll.GetDataValue(Rs232_or_Ethernet, array2, text.Length, array, ref revLength, controllerHandle) == 10000)
            {
                byte[] array3 = new byte[revLength];
                for (int j = 0; j < revLength; j++)
                {
                    array3[j] = (byte)array[j];
                }
                textBox1.Text = "";
                for (int k = 0; k < revLength; k++)
                {
                    ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
                    char[] chars = aSCIIEncoding.GetChars(array3);
                    textBox1.Text += chars[k];
                }
            }
            else
            {
                textBox1.Text = "发送的命令有错误";
            }
        }

        private void pinshan8_MK1_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_MK1.Value;
                int num2 = 1;
                if (ClassLibControllerDll.SetPulseWidthValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道脉宽值为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道脉宽值失败";
                }
            }
        }

        private void pinshan8_MK2_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_MK2.Value;
                int num2 = 2;
                if (ClassLibControllerDll.SetPulseWidthValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道脉宽值为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道脉宽值失败";
                }
            }
        }

        private void pinshan8_MK3_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_MK3.Value;
                int num2 = 3;
                if (ClassLibControllerDll.SetPulseWidthValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道脉宽值为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道脉宽值失败";
                }
            }
        }

        private void pinshan8_MK4_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_MK4.Value;
                int num2 = 4;
                if (ClassLibControllerDll.SetPulseWidthValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道脉宽值为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道脉宽值失败";
                }
            }
        }

        private void pinshan8_MK5_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_MK5.Value;
                int num2 = 5;
                if (ClassLibControllerDll.SetPulseWidthValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道脉宽值为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道脉宽值失败";
                }
            }
        }

        private void pinshan8_MK6_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_MK6.Value;
                int num2 = 6;
                if (ClassLibControllerDll.SetPulseWidthValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道脉宽值为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道脉宽值失败";
                }
            }
        }

        private void pinshan8_MK7_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_MK7.Value;
                int num2 = 7;
                if (ClassLibControllerDll.SetPulseWidthValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道脉宽值为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道脉宽值失败";
                }
            }
        }

        private void pinshan8_MK8_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_MK8.Value;
                int num2 = 8;
                if (ClassLibControllerDll.SetPulseWidthValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道脉宽值为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道脉宽值失败";
                }
            }
        }

        private void pinshan8_LIGHT1_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_LIGHT1.Value;
                int num2 = 1;
                if (ClassLibControllerDll.SetLightDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道光源延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道光源延时失败";
                }
            }
        }

        private void pinshan8_LIGHT2_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_LIGHT2.Value;
                int num2 = 2;
                if (ClassLibControllerDll.SetLightDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道光源延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道光源延时失败";
                }
            }
        }

        private void pinshan8_LIGHT3_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_LIGHT3.Value;
                int num2 = 3;
                if (ClassLibControllerDll.SetLightDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道光源延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道光源延时失败";
                }
            }
        }

        private void pinshan8_LIGHT4_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_LIGHT4.Value;
                int num2 = 4;
                if (ClassLibControllerDll.SetLightDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道光源延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道光源延时失败";
                }
            }
        }

        private void pinshan8_LIGHT5_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_LIGHT5.Value;
                int num2 = 5;
                if (ClassLibControllerDll.SetLightDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道光源延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道光源延时失败";
                }
            }
        }

        private void pinshan8_LIGHT6_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_LIGHT6.Value;
                int num2 = 6;
                if (ClassLibControllerDll.SetLightDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道光源延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道光源延时失败";
                }
            }
        }

        private void pinshan8_LIGHT7_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_LIGHT7.Value;
                int num2 = 7;
                if (ClassLibControllerDll.SetLightDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道光源延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道光源延时失败";
                }
            }
        }

        private void pinshan8_LIGHT8_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_LIGHT8.Value;
                int num2 = 8;
                if (ClassLibControllerDll.SetLightDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道光源延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道光源延时失败";
                }
            }
        }

        private void pinshan8_Camera1_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_Camera1.Value;
                int num2 = 1;
                if (ClassLibControllerDll.SetCameraDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道相机延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道相机延时失败";
                }
            }
        }

        private void pinshan8_Camera2_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_Camera2.Value;
                int num2 = 2;
                if (ClassLibControllerDll.SetCameraDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道相机延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道相机延时失败";
                }
            }
        }

        private void pinshan8_Camera3_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_Camera3.Value;
                int num2 = 3;
                if (ClassLibControllerDll.SetCameraDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道相机延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道相机延时失败";
                }
            }
        }

        private void pinshan8_Camera4_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_Camera4.Value;
                int num2 = 4;
                if (ClassLibControllerDll.SetCameraDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道相机延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道相机延时失败";
                }
            }
        }

        private void pinshan8_Camera5_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_Camera5.Value;
                int num2 = 5;
                if (ClassLibControllerDll.SetCameraDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道相机延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道相机延时失败";
                }
            }
        }

        private void pinshan8_Camera6_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_Camera6.Value;
                int num2 = 6;
                if (ClassLibControllerDll.SetCameraDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道相机延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道相机延时失败";
                }
            }
        }

        private void pinshan8_Camera7_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_Camera7.Value;
                int num2 = 7;
                if (ClassLibControllerDll.SetCameraDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道相机延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道相机延时失败";
                }
            }
        }

        private void pinshan8_Camera8_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_Camera8.Value;
                int num2 = 8;
                if (ClassLibControllerDll.SetCameraDelayValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道相机延时为：{num}us";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道相机延时失败";
                }
            }
        }

        private void pinsan8_waichufa_CheckedChanged(object sender, EventArgs e)
        {
            if (get_data_flag == 18 || check_connet() == 1)
            {
                return;
            }
            int num = 0;
            num = (pinsan8_neichufa.Checked ? 1 : 0);
            if (ClassLibControllerDll.SetTriggerModeValue(Rs232_or_Ethernet, num, controllerHandle) == 10000)
            {
                if (num == 1)
                {
                    textBox1.Text = "成功设置内触发模式";
                }
                else
                {
                    textBox1.Text = "成功设置外触发模式";
                }
            }
            else if (num == 1)
            {
                textBox1.Text = "设置内触发模式失败";
            }
            else
            {
                textBox1.Text = "设置外触发模式失败";
            }
        }

        private void pinsan8_shangshengyan_CheckedChanged(object sender, EventArgs e)
        {
            if (get_data_flag == 18 || check_connet() == 1)
            {
                return;
            }
            int num = 0;
            num = (pinsan8_shangshengyan.Checked ? 1 : 0);
            if (ClassLibControllerDll.SetCameraSignalValue(Rs232_or_Ethernet, num, controllerHandle) == 10000)
            {
                if (num == 1)
                {
                    textBox1.Text = "成功设置上升沿";
                }
                else
                {
                    textBox1.Text = "成功设置下升沿";
                }
            }
            else if (num == 1)
            {
                textBox1.Text = "设置上升沿失败";
            }
            else
            {
                textBox1.Text = "设置下升沿失败";
            }
        }

        private void pinshan8_TriggerCycle_ValueChanged(object sender, EventArgs e)
        {
            pinshan8_TriggerCycleTrackBar.Value = (int)pinshan8_TriggerCycle.Value;
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinshan8_TriggerCycle.Value;
                if (ClassLibControllerDll.SetTriggerCycleValue(Rs232_or_Ethernet, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置内触发周期为：{num}ms";
                }
                else
                {
                    textBox1.Text = "设置内触发周期失败";
                }
            }
        }

        private void pinshan8_TriggerCycleTrackBar_Scroll(object sender, EventArgs e)
        {
            pinshan8_TriggerCycle.Value = pinshan8_TriggerCycleTrackBar.Value;
        }

        private void pinsan8_numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinsan8_numericUpDown1.Value;
                int num2 = 1;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void pinsan8_numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinsan8_numericUpDown2.Value;
                int num2 = 2;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void pinsan8_numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinsan8_numericUpDown3.Value;
                int num2 = 3;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void pinsan8_numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinsan8_numericUpDown4.Value;
                int num2 = 4;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void pinsan8_numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinsan8_numericUpDown5.Value;
                int num2 = 5;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void pinsan8_numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinsan8_numericUpDown6.Value;
                int num2 = 6;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void pinsan8_numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinsan8_numericUpDown7.Value;
                int num2 = 7;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void pinsan8_numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            if (get_data_flag != 18 && check_connet() != 1)
            {
                int num = (int)pinsan8_numericUpDown8.Value;
                int num2 = 8;
                if (ClassLibControllerDll.SetDigitalValue(Rs232_or_Ethernet, num2, num, controllerHandle) == 10000)
                {
                    textBox1.Text = $"成功设置CH{num2}通道亮度为：{num}";
                }
                else
                {
                    textBox1.Text = $"设置CH{num2}通道亮度失败";
                }
            }
        }

        private void pinsan8_changmie_CheckedChanged(object sender, EventArgs e)
        {
            if (get_data_flag == 18 || check_connet() == 1)
            {
                return;
            }
            int num = 0;
            num = (pinsan8_changliang.Checked ? 1 : 0);
            if (ClassLibControllerDll.SetLightModeValue(Rs232_or_Ethernet, num, controllerHandle) == 10000)
            {
                if (num == 1)
                {
                    textBox1.Text = "成功设置常亮";
                }
                else
                {
                    textBox1.Text = "成功设置常灭";
                }
            }
            else if (num == 1)
            {
                textBox1.Text = "设置常亮失败";
            }
            else
            {
                textBox1.Text = "设置常灭失败";
            }
        }
    }
}
