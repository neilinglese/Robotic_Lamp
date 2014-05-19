using System;
using System.Net;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using Windows.Devices.Sensors;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Lamp_Controller.Resources;

namespace Lamp_Controller
{
    public partial class MainPage : PhoneApplicationPage
    {
        StreamSocket BTSock; 

        string BTStatus = ""; 
        string sentText = "";
        bool stateChanged = false;

        public MainPage()
        {
            InitializeComponent(); 

            if(Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)
            {
                MessageBox.Show("Sorry, Bluetooth isn't compatible in this enviornment. Please use your Phone.", "Error: Device Required", MessageBoxButton.OK);
                return;
            }
            Loaded +=MainPage_Loaded; 
       

        }

        private async void  MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            PeerFinder.Start();
            PeerFinder.AlternateIdentities["Bluetooth:Paired"] = ""; 
            var peers = await PeerFinder.FindAllPeersAsync(); 
            txtBTStatus.Text = "Finding Paired Devices..."; 

            for (int i = 0; i < peers.Count; i++)
               {
                 lstBTPaired.Items.Add(peers[i].DisplayName);
               }
           if (peers.Count <= 2)
             {
                txtBTStatus.Text = "Found " + peers.Count + " Devices";
            }

        }

        private async void BT2Arduino_Send (string WhatToSend) 
        {
            if (BTSock == null) 
            {
                txtBTStatus.Text = "No connection found. Try again!"; 
                return;
            }
            else
                if (BTSock != null) 
                {
                    var datab = GetBufferFromByteArray(UTF8Encoding.UTF8.GetBytes(WhatToSend)); 
                    await BTSock.OutputStream.WriteAsync(datab); 

                    switch (WhatToSend)
                   {
                       case "2":
                           sentText = "Arm Up";
                           txtBTStatus.Text = "Message Sent (" + sentText + ")"; 
                           break;
                       case "3":
                           sentText = "Arm Down";
                           txtBTStatus.Text = "Message Sent (" + sentText + ")"; 
                           break;
                       case "4":
                           sentText = "Head Left";
                           txtBTStatus.Text = "Message Sent (" + sentText + ")"; 
                           break;
                       case "5":
                           sentText = "Head Right";
                           txtBTStatus.Text = "Message Sent (" + sentText + ")"; 
                           break;
                       case "6":
                           sentText = "Head Center";
                           txtBTStatus.Text = "Message Sent (" + sentText + ")";
                           break;
                       case "7":
                           sentText = "Head Sweep";
                           txtBTStatus.Text = "Message Sent (" + sentText + ")";
                           break;
                       case "8":
                           sentText = "Going to Sleep";
                           txtBTStatus.Text = "Message Sent (" + sentText + ")";
                           break;
                       case "9":
                           sentText = "Waking Up";
                           txtBTStatus.Text = "Message Sent (" + sentText + ")";
                           break;
                   }
                }
        }

        private IBuffer GetBufferFromByteArray(byte[] package)
        {
            using (DataWriter dw = new DataWriter())
            {
                dw.WriteBytes(package);
                return dw.DetachBuffer();
            }
        }

        private async void lstBTPaired_Tap_1(object sender, GestureEventArgs e)
        {
            if (lstBTPaired.SelectedItem == null) 
            {
                txtBTStatus.Text = "No Device Selected! Try again..."; 
                return;
            }
            else
                if (lstBTPaired.SelectedItem != null) 
                {
                    PeerFinder.AlternateIdentities["Bluetooth:Paired"] = ""; 
                    var PF = await PeerFinder.FindAllPeersAsync(); 

                    BTSock = new StreamSocket(); 
                    await BTSock.ConnectAsync(PF[lstBTPaired.SelectedIndex].HostName, "1"); 

                    var datab = GetBufferFromByteArray(Encoding.UTF8.GetBytes("Hi")); 
                    await BTSock.OutputStream.WriteAsync(datab); 

                    btnSendCommand.IsEnabled = true; 
                    armUp.IsEnabled = true;
                    armDown.IsEnabled = true;
                    headLeft.IsEnabled = true;
                    headRight.IsEnabled = true;
                    headCenter.IsEnabled = true;
                    headSweep.IsEnabled = true;
                    ledON.IsEnabled = true;
                    ledOFF.IsEnabled = true;
                }
        }


        void PeerFinder_TriggeredConnectionStateChanged(object sender, TriggeredConnectionStateChangedEventArgs args)
        {

            if (args.State == TriggeredConnectState.Failed)
            {
                txtBTStatus.Text = "Failed to Connect... Try again!";
                BTStatus = "no"; 
                return;
            }

            if (args.State == TriggeredConnectState.Completed)
            {
                txtBTStatus.Text = "Connected!";
                BTStatus = "yes";
                MessageBox.Show("hi");
            }
        }
        
        private void btnSendCommand_Click(object sender, RoutedEventArgs e)
        {
            if (stateChanged == false)
            {
                btnSendCommand.Content = "Wake Up";
                BT2Arduino_Send("8"); 
                stateChanged = true;
            }
            else
            {
                btnSendCommand.Content = "Go to sleep";
                BT2Arduino_Send("9"); 
                stateChanged = false;
            }
        }

        private void armUp_Click(object sender, RoutedEventArgs e)
        {
            BT2Arduino_Send("2");
        }

        private void armDown_Click(object sender, RoutedEventArgs e)
        {
            BT2Arduino_Send("3");
        }

        private void headLeft_Click(object sender, RoutedEventArgs e)
        {
            BT2Arduino_Send("4");
        }

        private void headRight_Click(object sender, RoutedEventArgs e)
        {
            BT2Arduino_Send("5");
        }

        private void headCenter_Click(object sender, RoutedEventArgs e)
        {
            BT2Arduino_Send("6");
        }

        private void headSweep_Click(object sender, RoutedEventArgs e)
        {
            BT2Arduino_Send("7");
        }

        private void ledON_Click(object sender, RoutedEventArgs e)
        {
            BT2Arduino_Send("0");
        }

        private void ledOFF_Click(object sender, RoutedEventArgs e)
        {
            BT2Arduino_Send("1");
        }

    }
}