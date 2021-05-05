using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Management;
using System.Diagnostics;
using System.Windows.Threading;
using System.IO;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace HardWare_SoftWare_Monitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    class Drivers
    {
        public string DriverFriendlyName { get; set; }
        public string DriverDescription { get; set; }
        public string DriverVersion { get; set; }
        public string DriverLocation { get; set; }
        public string DriverDriverName { get; set; }
        public string DriverManufacturer { get; set; }
        public string DriverProvider { get; set; }
        public string DriverCompatID { get; set; }
        public string DriverDeviceID { get; set; }
        public string DriverInfName { get; set; }

        public Drivers(ManagementObject obj)
        {
            DriverFriendlyName = Convert.ToString(obj["FriendlyName"]);
            DriverDescription = Convert.ToString(obj["CompatID"]);
            DriverVersion = Convert.ToString(obj["DriverVersion"]);
            DriverLocation = Convert.ToString(obj["Location"]);
            DriverDriverName = Convert.ToString(obj["DriverName"]);
            DriverManufacturer = Convert.ToString(obj["Manufacturer"]);
            DriverProvider = Convert.ToString(obj["DriverProviderName"]);
            DriverCompatID = Convert.ToString(obj["CompatID"]);
            DriverDeviceID = Convert.ToString(obj["DeviceID"]);
            DriverInfName = Convert.ToString(obj["InfName"]);
        }
    }

    class Programs
    {
        public string ProgramName { get; set; }
        public string ProgramVersion { get; set; }

        public Programs(ManagementObject obj)
        {
            ProgramName = Convert.ToString(obj["Name"]);
            ProgramVersion = Convert.ToString(obj["Version"]);
        }
    }

    public partial class MainWindow : Window
    {
        PerformanceCounter processorUsage = new PerformanceCounter("Processzor", "A processzor kihasználtsága (%)", "_Total");
        PerformanceCounter availableMemory = new PerformanceCounter("Memória", "Rendelkezésre álló memória (megabájt)");
        PerformanceCounter uptime = new PerformanceCounter("Rendszer", "Rendszerindítás óta eltelt idő (s)");
        DispatcherTimer tick = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            tick.Interval = TimeSpan.FromSeconds(1);
            tick.Tick += Tick_Tick;
            tick.Start();

            ManagementObjectSearcher myBaseBoardObject = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            ManagementObjectSearcher myMotherboardObject = new ManagementObjectSearcher("select * from Win32_MotherboardDevice");
            ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_Processor"); 
            ManagementObjectSearcher myMemoryObject = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            ManagementObjectSearcher driverSearch = new ManagementObjectSearcher("select * from Win32_PnPSignedDriver");
            ManagementObjectSearcher programSearch = new ManagementObjectSearcher("select * from Win32_Product");
            ManagementObjectSearcher myVideoObject = new ManagementObjectSearcher("select * from Win32_VideoController");
            ManagementObjectSearcher myOperativeSystemObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            ManagementObjectSearcher myPrinterObject = new ManagementObjectSearcher("select * from Win32_Printer");
            ManagementObjectSearcher myAudioObject = new ManagementObjectSearcher("select * from Win32_SoundDevice");
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            int db = 1;

            foreach (ManagementObject obj in myBaseBoardObject.Get())
            {
                MBManufacturer.Content = obj["Manufacturer"];
                MBProduct.Content = obj["Product"];
                MBSerial.Content = obj["SerialNumber"];
            }
            foreach (ManagementObject obj in myMotherboardObject.Get())
            {
                MBSysName.Content = obj["SystemName"];
                MBbus.Content = obj["PrimaryBusType"];
            }
            foreach (ManagementObject obj in myProcessorObject.Get())
            {
                CPUName.Content = obj["Name"];
                CPUID.Content = obj["DeviceID"];
                CPUManu.Content = obj["Manufacturer"];
                CPUClock.Content = obj["CurrentClockSpeed"];
                CPUCores.Content = obj["NumberOfCores"];
                CPUArch.Content = obj["Architecture"];
                CPUFamily.Content = obj["Family"];
            }
            foreach (ManagementObject obj in myMemoryObject.Get())
            {
                TabItem tab = new TabItem();
                tab.Header = db + ". modul";
                Memory.Items.Add(tab);
                Grid grid = new Grid();
                grid.Background = new SolidColorBrush(Color.FromRgb(229, 229, 229));
                Label l1 = new Label();
                l1.Margin = new Thickness(10, 10, 0, 0);
                l1.Content = "Kapacitás: " + Math.Round(Convert.ToInt64(obj["Capacity"]) / 1073741824.0, 2) + "GB";
                Label l2 = new Label();
                l2.Margin = new Thickness(10, 40, 0, 0);
                l2.Content = "Gyártó: " + obj["Manufacturer"];
                Label l3 = new Label();
                l3.Margin = new Thickness(10, 70, 0, 0);
                l3.Content = "Formai tényező: " + obj["FormFactor"];
                Label l4 = new Label();
                l4.Margin = new Thickness(10, 100, 0, 0);
                l4.Content = "Típus: " + obj["MemoryType"];
                grid.Children.Add(l1);
                grid.Children.Add(l2);
                grid.Children.Add(l3);
                grid.Children.Add(l4);
                tab.Content = grid;
                db++;
            }
            foreach (ManagementObject service in driverSearch.Get())
            {
                DriverGrid.Items.Add(new Drivers(service));
            }
            
            foreach (ManagementObject obj in programSearch.Get())
            {
                ProgramsGrid.Items.Add(new Programs(obj));
            }
            foreach (DriveInfo d in allDrives)
            {
                TabItem tab = new TabItem();
                tab.Header = d.Name;
                Drives.Items.Add(tab);
                Grid grid = new Grid();
                grid.Background = new SolidColorBrush(Color.FromRgb(229, 229, 229));
                Label l1 = new Label();
                l1.Margin = new Thickness(10, 10, 0, 0);
                l1.Content = "Típus: " + d.DriveType;
                grid.Children.Add(l1);
                if (d.IsReady == true)
                {
                    Label l2 = new Label();
                    l2.Margin = new Thickness(10, 40, 0, 0);
                    l2.Content = "Fájlrendszer: " + d.DriveFormat;
                    Label l3 = new Label();
                    l3.Margin = new Thickness(10, 70, 0, 0);
                    l3.Content = "Szabad tárhely: " + Math.Round(d.TotalFreeSpace / 1073741824.0, 2) + "GB";
                    Label l4 = new Label();
                    l4.Margin = new Thickness(10, 100, 0, 0);
                    l4.Content = "Teljes méret: " + Math.Round(d.TotalSize / 1073741824.0, 2) + "GB";
                    grid.Children.Add(l2);
                    grid.Children.Add(l3);
                    grid.Children.Add(l4);
                }
                tab.Content = grid;
            }
            foreach (ManagementObject obj in myVideoObject.Get())
            {
                GPUName.Content = obj["Name"];
                GPUStatus.Content = obj["Status"];
                GPUID.Content = obj["DeviceID"];
                GPURAM.Content = Math.Round(Convert.ToDouble(obj["AdapterRAM"]) / 1048576.0, 2) + "MB";
                GPUDAC.Content = obj["AdapterDACType"];
                GPUDriver.Content = obj["DriverVersion"];
                GPUVideoProcessor.Content =  obj["VideoProcessor"];
                if (Convert.ToInt32(obj["VideoArchitecture"]) == 0)
                {
                    GPUArch.Content = "x86";
                }
                else if (Convert.ToInt32(obj["VideoArchitecture"]) == 1)
                {
                    GPUArch.Content = "MIPS";
                }
                else if (Convert.ToInt32(obj["VideoArchitecture"]) == 2)
                {
                    GPUArch.Content =  "Alpha";
                }
                else if (Convert.ToInt32(obj["VideoArchitecture"]) == 3)
                {
                    GPUArch.Content = "PowerPC";
                }
                else if (Convert.ToInt32(obj["VideoArchitecture"]) == 5)
                {
                    GPUArch.Content = "ARM";
                }
                else if (Convert.ToInt32(obj["VideoArchitecture"]) == 6)
                {
                    GPUArch.Content =  "ia64";
                }
                else
                {
                    GPUArch.Content = " Egyéb";
                }
                if (Convert.ToInt32(obj["VideoMemoryType"]) == 3)
                {
                    GPUMemType.Content = "VRAM";
                }
                else if (Convert.ToInt32(obj["VideoMemoryType"]) == 4)
                {
                    GPUMemType.Content = "DRAM";
                }
                else if (Convert.ToInt32(obj["VideoMemoryType"]) == 5)
                {
                    GPUMemType.Content = "SRAM";
                }
                else
                {
                    GPUMemType.Content = "Egyéb";
                }
                List<string> drivers = new List<string>();
                foreach (var item in obj["InstalledDisplayDrivers"].ToString().Split(','))
                {
                    GPUDrivers.Items.Add(item);
                }
            }
            
            foreach (ManagementObject obj in myOperativeSystemObject.Get())
            {
                OSType.Content = obj["Caption"];
                OSWinDir.Content = obj["WindowsDirectory"];
                OSSysDir.Content = obj["SystemDirectory"];
                OSSerial.Content = obj["SerialNumber"];
                OSVersion.Content = obj["Version"];
            }
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            if (nics == null || nics.Length < 1)
            {
                NetGrid.Children.Clear();
                Label l = new Label();
                l.Margin = new Thickness(20, 20, 0, 0);
                l.FontSize = 18;
                l.Content = "Nincs hálózati kapcsolat.";
                NetGrid.Children.Add(l);
            }
            else
            {
                foreach (NetworkInterface adapter in nics)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    TabItem tab = new TabItem();
                    tab.Header = adapter.Name;
                    NetworkConnections.Items.Add(tab);
                    Grid grid = new Grid();
                    grid.Background = new SolidColorBrush(Color.FromRgb(229, 229, 229));
                    Label l1 = new Label();
                    l1.Margin = new Thickness(10, 10, 0, 0);
                    l1.FontSize = 16;
                    l1.Content = adapter.Description;
                    Label l2 = new Label();
                    l2.Margin = new Thickness(10, 50, 0, 0);
                    l2.Content = "Interfész típus: " + adapter.NetworkInterfaceType;
                    Label l3 = new Label();
                    l3.Margin = new Thickness(10, 80, 0, 0);
                    l3.Content = "Fizikai cím: " + adapter.GetPhysicalAddress().ToString();
                    Label l4 = new Label();
                    l4.Margin = new Thickness(10, 110, 0, 0);
                    l4.Content = "Állapot: " + adapter.OperationalStatus;
                    grid.Children.Add(l1);
                    grid.Children.Add(l2);
                    grid.Children.Add(l3);
                    grid.Children.Add(l4);
                    tab.Content = grid;
                }
            }
            foreach (ManagementObject obj in myAudioObject.Get())
            {
                TabItem tab = new TabItem();
                tab.Header = obj["Name"];
                AudioDevices.Items.Add(tab);
                Grid grid = new Grid();
                grid.Background = new SolidColorBrush(Color.FromRgb(229, 229, 229));
                Label l1 = new Label();
                l1.Margin = new Thickness(10, 10, 0, 0);
                l1.FontSize = 16;
                l1.Content = "Termék név: " + obj["ProductName"];
                Label l2 = new Label();
                l2.Margin = new Thickness(10, 50, 0, 0);
                l2.Content = "Eszköz ID: " + obj["DeviceID"];
                Label l3 = new Label();
                l3.Margin = new Thickness(10, 80, 0, 0);
                l3.Content = "Állapot: " + obj["Status"];
                grid.Children.Add(l1);
                grid.Children.Add(l2);
                grid.Children.Add(l3);
                tab.Content = grid;
            }
            foreach (ManagementObject obj in myPrinterObject.Get())
            {
                TabItem tab = new TabItem();
                tab.Header = obj["Name"];
                Printers.Items.Add(tab);
                Grid grid = new Grid();
                grid.Background = new SolidColorBrush(Color.FromRgb(229, 229, 229));
                Label l1 = new Label();
                l1.Margin = new Thickness(10, 10, 0, 0);
                l1.Content = "Alapértelmezett nyomtató: " + obj["Default"];
                Label l2 = new Label();
                l2.Margin = new Thickness(10, 40, 0, 0);
                l2.Content = "Hálózati nyomtató: " + obj["Network"];
                Label l3 = new Label();
                l3.Margin = new Thickness(10, 70, 0, 0);
                l3.Content = "Eszköz ID: " + obj["DeviceID"];
                Label l4 = new Label();
                l4.Margin = new Thickness(10, 100, 0, 0);
                l4.Content = "Állapot:" + obj["Status"];
                grid.Children.Add(l1);
                grid.Children.Add(l2);
                grid.Children.Add(l3);
                grid.Children.Add(l4);
                tab.Content = grid;
            }
        }
        private void Tick_Tick(object sender, EventArgs e)
        {
            cpu.Content = (int)processorUsage.NextValue() + " %";
            mem.Content = (int)availableMemory.NextValue() + " MB";
            sysup.Content = (int)uptime.NextValue() / 60 / 60 + " óra";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
