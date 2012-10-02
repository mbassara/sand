using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Sand
{
    class MainWindow : Form
    {
        private PictureBox pictureBox1;
        private Button button1;
        private Graphics img;

        private bool MouseIsDown = false;
        private Label label1;
        private Label label2;
        private ComboBox sizeComboBox;
        private ComboBox speedComboBox;
        private SandBox sandBox;

        private int threadCount = 0;
        public MainWindow()
        {
            InitializeComponent();
            MyInitializeComponent();

            Debug.Listeners.Add(new TextWriterTraceListener(System.Console.Out));
            img = Graphics.FromHwnd(pictureBox1.Handle);


            sandBox = SandBox.GetInstance(img, Math.Min(pictureBox1.ClientSize.Height, pictureBox1.ClientSize.Width), 1, 30);

            Thread thread = new Thread(new ThreadStart(delegate()
            {
                sandBox.Run(1000000);
            }));
            thread.Name = "Sand Thread " + threadCount++;
            thread.Start();

            FormClosing += delegate(object o, FormClosingEventArgs e) { sandBox.IsRunning = false; };

            pictureBox1.MouseDown += delegate(object o, MouseEventArgs evt)
            {
                MouseIsDown = true;
                sandBox.FallingPoint = evt.Location;
                sandBox.IsPaused = false;
            };
            pictureBox1.MouseUp += delegate(object o, MouseEventArgs evt)
            {
                MouseIsDown = false;
                sandBox.FallingPoint = evt.Location;
                sandBox.IsPaused = true;
            };
            pictureBox1.MouseMove += delegate(object o, MouseEventArgs evt)
            {
                if(MouseIsDown)
                    sandBox.FallingPoint = evt.Location;
            };

        }

        private void MyInitializeComponent()
        {
            this.sizeComboBox.Items.AddRange(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            this.sizeComboBox.SelectedIndex = 0;
            this.sizeComboBox.SelectedValueChanged += size_changed;

            this.speedComboBox.Items.AddRange(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            this.speedComboBox.SelectedIndex = 7;
            this.speedComboBox.SelectedValueChanged += speed_changed;
        }

        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.sizeComboBox = new System.Windows.Forms.ComboBox();
            this.speedComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Location = new System.Drawing.Point(13, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(338, 338);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(410, 205);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "RESET";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(407, 120);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Grain size:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(393, 148);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Game speed:";
            // 
            // sizeComboBox
            // 
            this.sizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sizeComboBox.FormattingEnabled = true;
            this.sizeComboBox.Location = new System.Drawing.Point(466, 117);
            this.sizeComboBox.Name = "sizeComboBox";
            this.sizeComboBox.Size = new System.Drawing.Size(52, 21);
            this.sizeComboBox.TabIndex = 4;
            // 
            // speedComboBox
            // 
            this.speedComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.speedComboBox.FormattingEnabled = true;
            this.speedComboBox.Location = new System.Drawing.Point(466, 145);
            this.speedComboBox.Name = "speedComboBox";
            this.speedComboBox.Size = new System.Drawing.Size(51, 21);
            this.speedComboBox.TabIndex = 5;
            // 
            // MainWindow
            // 
            this.ClientSize = new System.Drawing.Size(554, 363);
            this.Controls.Add(this.speedComboBox);
            this.Controls.Add(this.sizeComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "MainWindow";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public static void Main()
        {
            Application.Run(new MainWindow());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sandBox.Reset();
            Thread thread = new Thread(new ThreadStart(delegate()
            {
                sandBox.Run(1000000);
            }));
            thread.Name = "Sand Thread " + threadCount++;
            thread.Start();
        }

        private void size_changed(object sender, EventArgs e)
        {
            int delay = 10 - speedComboBox.SelectedIndex - 1;
            int pixelSize = sizeComboBox.SelectedIndex + 1;
            sandBox.Reset(Math.Min(pictureBox1.ClientSize.Height, pictureBox1.ClientSize.Width), pixelSize, delay);
            
            Thread thread = new Thread(new ThreadStart(delegate()
            {
                sandBox.Run(1000000);
            }));
            thread.Name = "Sand Thread " + threadCount++;
            thread.Start();
        }

        private void speed_changed(object sender, EventArgs e)
        {
            int delay = 10 - speedComboBox.SelectedIndex - 1;
            sandBox.Delay = delay * 10;
        }

    }
}
