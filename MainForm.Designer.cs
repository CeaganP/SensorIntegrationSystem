
namespace SenseSys
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.textBox_Input = new System.Windows.Forms.TextBox();
            this.button_Add = new System.Windows.Forms.Button();
            this.listBox_Contents = new System.Windows.Forms.ListBox();
            this.button_Delete = new System.Windows.Forms.Button();
            this.listBox_Ports = new System.Windows.Forms.ListBox();
            this.portUpdate = new System.Windows.Forms.Button();
            this.richTextBox_Data = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label_Log = new System.Windows.Forms.Label();
            this.Connect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // textBox_Input
            // 
            this.textBox_Input.Location = new System.Drawing.Point(84, 97);
            this.textBox_Input.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox_Input.Name = "textBox_Input";
            this.textBox_Input.Size = new System.Drawing.Size(148, 26);
            this.textBox_Input.TabIndex = 0;
            // 
            // button_Add
            // 
            this.button_Add.Location = new System.Drawing.Point(243, 94);
            this.button_Add.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_Add.Name = "button_Add";
            this.button_Add.Size = new System.Drawing.Size(112, 35);
            this.button_Add.TabIndex = 1;
            this.button_Add.Text = "Add";
            this.button_Add.UseVisualStyleBackColor = true;
            this.button_Add.Click += new System.EventHandler(this.button_Add_Click);
            // 
            // listBox_Contents
            // 
            this.listBox_Contents.FormattingEnabled = true;
            this.listBox_Contents.ItemHeight = 20;
            this.listBox_Contents.Location = new System.Drawing.Point(84, 138);
            this.listBox_Contents.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listBox_Contents.Name = "listBox_Contents";
            this.listBox_Contents.Size = new System.Drawing.Size(270, 144);
            this.listBox_Contents.TabIndex = 2;
            this.listBox_Contents.DoubleClick += new System.EventHandler(this.listBox_Contents_DoubleClick);
            // 
            // button_Delete
            // 
            this.button_Delete.Location = new System.Drawing.Point(84, 294);
            this.button_Delete.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_Delete.Name = "button_Delete";
            this.button_Delete.Size = new System.Drawing.Size(272, 35);
            this.button_Delete.TabIndex = 3;
            this.button_Delete.Text = "Delete";
            this.button_Delete.UseVisualStyleBackColor = true;
            this.button_Delete.Click += new System.EventHandler(this.button_Delete_Click);
            // 
            // listBox_Ports
            // 
            this.listBox_Ports.FormattingEnabled = true;
            this.listBox_Ports.ItemHeight = 20;
            this.listBox_Ports.Location = new System.Drawing.Point(550, 138);
            this.listBox_Ports.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listBox_Ports.Name = "listBox_Ports";
            this.listBox_Ports.Size = new System.Drawing.Size(178, 144);
            this.listBox_Ports.TabIndex = 5;
            this.listBox_Ports.SelectedIndexChanged += new System.EventHandler(this.listBox_Ports_SelectedIndexChanged);
            this.listBox_Ports.DoubleClick += new System.EventHandler(this.listBo_Ports_Contents_DoubleClick);
            // 
            // portUpdate
            // 
            this.portUpdate.Location = new System.Drawing.Point(550, 97);
            this.portUpdate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.portUpdate.Name = "portUpdate";
            this.portUpdate.Size = new System.Drawing.Size(96, 31);
            this.portUpdate.TabIndex = 6;
            this.portUpdate.Text = "Update";
            this.portUpdate.UseVisualStyleBackColor = true;
            this.portUpdate.Click += new System.EventHandler(this.portUpdate_Click);
            // 
            // richTextBox_Data
            // 
            this.richTextBox_Data.Location = new System.Drawing.Point(84, 340);
            this.richTextBox_Data.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBox_Data.Name = "richTextBox_Data";
            this.richTextBox_Data.Size = new System.Drawing.Size(1096, 332);
            this.richTextBox_Data.TabIndex = 7;
            this.richTextBox_Data.Text = "";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(80, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Console:";
            // 
            // label_Log
            // 
            this.label_Log.Location = new System.Drawing.Point(147, 14);
            this.label_Log.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Log.Name = "label_Log";
            this.label_Log.Size = new System.Drawing.Size(656, 20);
            this.label_Log.TabIndex = 8;
            this.label_Log.Text = "label1";
            // 
            // Connect
            // 
            this.Connect.Location = new System.Drawing.Point(828, 92);
            this.Connect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Connect.Name = "Connect";
            this.Connect.Size = new System.Drawing.Size(96, 31);
            this.Connect.TabIndex = 9;
            this.Connect.Text = "Connect";
            this.Connect.UseVisualStyleBackColor = true;
            this.Connect.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 692);
            this.Controls.Add(this.Connect);
            this.Controls.Add(this.label_Log);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBox_Data);
            this.Controls.Add(this.portUpdate);
            this.Controls.Add(this.listBox_Ports);
            this.Controls.Add(this.button_Delete);
            this.Controls.Add(this.listBox_Contents);
            this.Controls.Add(this.button_Add);
            this.Controls.Add(this.textBox_Input);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.TextBox textBox_Input;
        private System.Windows.Forms.Button button_Add;
        private System.Windows.Forms.ListBox listBox_Contents;
        private System.Windows.Forms.Button button_Delete;
        private System.Windows.Forms.ListBox listBox_Ports;
        private System.Windows.Forms.Button portUpdate;
        private System.Windows.Forms.RichTextBox richTextBox_Data;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_Log;
        private System.Windows.Forms.Button Connect;
    }
}

