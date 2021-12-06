using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Microsoft.DirectX.DirectInput;
using System.IO;

namespace ControllerKeyboard
{
    public partial class Form1 : Form
    {

        Device joystick;
        string[] characters = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", " " };
        int characterIndex = 0;
        int zeroIndex;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Init
            foreach (DeviceInstance di in Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly)) {
                joystick = new Device(di.InstanceGuid);
                handleJoystick(joystick);

                label1.Text = "Using gamepad: " + joystick.Properties.ProductName;
                break;
            }

            if (joystick == null) {
                // No joystick found
                MessageBox.Show("No controller connected.");
                Application.Exit();
            }

            FormClosing += Form1_FormClosing;
        }

        private void Form1_FormClosing(object sender, EventArgs e) {
            // Cleanup
            joystick.Unacquire();
        }


        void handleJoystick(Device joystick) {
            // Now we have access to the gamepad
            // https://docs.microsoft.com/en-us/previous-versions/ms837190(v=msdn.10)

            try{
                joystick.SendForceFeedbackCommand(ForceFeedbackCommand.StopAll);
            }
            catch (Exception) {
                
            }
            joystick.Acquire();
            timer1.Start();

            // Get the initial X value to use as a zeroIndex (center).
            joystick.Poll();
            zeroIndex = joystick.CurrentJoystickState.X;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Get updates on our joystick
            joystick.Poll();
            label2.Text = "Raw X: " + joystick.CurrentJoystickState.X;
            label3.Text = "X: " + (joystick.CurrentJoystickState.X - zeroIndex);

            // Higher than 1000 == make the index go up
            // Lower than 1000 == make the index go down
            if ((joystick.CurrentJoystickState.X - zeroIndex) > 1000) {
                characterIndex++;
            }
            else if ((joystick.CurrentJoystickState.X - zeroIndex) < -1000) {
                characterIndex--;
            }

            // Looping
            if (characterIndex >= characters.Length) {
                characterIndex = 0;
            }
            if (characterIndex < 0) {
                characterIndex = characters.Length - 1;
            }

            characterLabel.Text = characters[characterIndex];




            label4.Text = "Button: " + joystick.CurrentJoystickState.GetButtons()[0];

            // Send the key
            if (joystick.CurrentJoystickState.GetButtons()[0] == 128) {
                SendKeys.Send(characterLabel.Text);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            label1.Visible = checkBox1.Checked;
            label2.Visible = checkBox1.Checked;
            label3.Visible = checkBox1.Checked;
            label4.Visible = checkBox1.Checked;
        }
    }
}
