namespace PianoMotionCreateMidi
{
    partial class PianoMotionCreateForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFile = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.numericStartFrame = new System.Windows.Forms.NumericUpDown();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkFingering = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericTrackR = new System.Windows.Forms.NumericUpDown();
            this.buttonTrack = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericTrackL = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.comboLHorizon = new System.Windows.Forms.ComboBox();
            this.comboRHorizon = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.comboLVertical = new System.Windows.Forms.ComboBox();
            this.comboRVertical = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.comboLShake = new System.Windows.Forms.ComboBox();
            this.comboRShake = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.numericDiffFrame = new System.Windows.Forms.NumericUpDown();
            this.checkPedal = new System.Windows.Forms.CheckBox();
            this.checkUpperBody = new System.Windows.Forms.CheckBox();
            this.numericSuppressInt = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericStartFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTrackR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTrackL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericDiffFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSuppressInt)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "MIDI file";
            // 
            // textBoxFile
            // 
            this.textBoxFile.Location = new System.Drawing.Point(15, 28);
            this.textBoxFile.Name = "textBoxFile";
            this.textBoxFile.Size = new System.Drawing.Size(184, 19);
            this.textBoxFile.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(205, 26);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(48, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "file";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "Paste start frame";
            // 
            // numericStartFrame
            // 
            this.numericStartFrame.Location = new System.Drawing.Point(15, 140);
            this.numericStartFrame.Name = "numericStartFrame";
            this.numericStartFrame.Size = new System.Drawing.Size(84, 19);
            this.numericStartFrame.TabIndex = 10;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(53, 380);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 28;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(191, 380);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 29;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // checkFingering
            // 
            this.checkFingering.AutoSize = true;
            this.checkFingering.Location = new System.Drawing.Point(169, 141);
            this.checkFingering.Name = "checkFingering";
            this.checkFingering.Size = new System.Drawing.Size(106, 16);
            this.checkFingering.TabIndex = 11;
            this.checkFingering.Text = "Create fingering";
            this.checkFingering.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Track number";
            // 
            // numericTrackR
            // 
            this.numericTrackR.Location = new System.Drawing.Point(44, 65);
            this.numericTrackR.Name = "numericTrackR";
            this.numericTrackR.Size = new System.Drawing.Size(84, 19);
            this.numericTrackR.TabIndex = 5;
            // 
            // buttonTrack
            // 
            this.buttonTrack.Location = new System.Drawing.Point(134, 62);
            this.buttonTrack.Name = "buttonTrack";
            this.buttonTrack.Size = new System.Drawing.Size(48, 23);
            this.buttonTrack.TabIndex = 6;
            this.buttonTrack.Text = "check";
            this.buttonTrack.UseVisualStyleBackColor = true;
            this.buttonTrack.Click += new System.EventHandler(this.buttonTrack_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "R";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 92);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "L";
            // 
            // numericTrackL
            // 
            this.numericTrackL.Location = new System.Drawing.Point(44, 90);
            this.numericTrackL.Name = "numericTrackL";
            this.numericTrackL.Size = new System.Drawing.Size(84, 19);
            this.numericTrackL.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 187);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "Target bones";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(102, 198);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 12);
            this.label7.TabIndex = 13;
            this.label7.Text = "Left";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(208, 198);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 12);
            this.label8.TabIndex = 14;
            this.label8.Text = "Right";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(25, 216);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 15;
            this.label9.Text = "horizon";
            // 
            // comboLHorizon
            // 
            this.comboLHorizon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLHorizon.FormattingEnabled = true;
            this.comboLHorizon.Location = new System.Drawing.Point(104, 214);
            this.comboLHorizon.Name = "comboLHorizon";
            this.comboLHorizon.Size = new System.Drawing.Size(100, 20);
            this.comboLHorizon.TabIndex = 16;
            // 
            // comboRHorizon
            // 
            this.comboRHorizon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboRHorizon.FormattingEnabled = true;
            this.comboRHorizon.Location = new System.Drawing.Point(210, 214);
            this.comboRHorizon.Name = "comboRHorizon";
            this.comboRHorizon.Size = new System.Drawing.Size(100, 20);
            this.comboRHorizon.TabIndex = 17;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(25, 245);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 12);
            this.label10.TabIndex = 18;
            this.label10.Text = "vertical (sub)";
            // 
            // comboLVertical
            // 
            this.comboLVertical.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLVertical.FormattingEnabled = true;
            this.comboLVertical.Location = new System.Drawing.Point(104, 241);
            this.comboLVertical.Name = "comboLVertical";
            this.comboLVertical.Size = new System.Drawing.Size(100, 20);
            this.comboLVertical.TabIndex = 19;
            // 
            // comboRVertical
            // 
            this.comboRVertical.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboRVertical.FormattingEnabled = true;
            this.comboRVertical.Location = new System.Drawing.Point(210, 241);
            this.comboRVertical.Name = "comboRVertical";
            this.comboRVertical.Size = new System.Drawing.Size(100, 20);
            this.comboRVertical.TabIndex = 20;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(25, 273);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 12);
            this.label11.TabIndex = 21;
            this.label11.Text = "shake (sub)";
            // 
            // comboLShake
            // 
            this.comboLShake.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLShake.FormattingEnabled = true;
            this.comboLShake.Location = new System.Drawing.Point(104, 268);
            this.comboLShake.Name = "comboLShake";
            this.comboLShake.Size = new System.Drawing.Size(100, 20);
            this.comboLShake.TabIndex = 22;
            // 
            // comboRShake
            // 
            this.comboRShake.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboRShake.FormattingEnabled = true;
            this.comboRShake.Location = new System.Drawing.Point(210, 268);
            this.comboRShake.Name = "comboRShake";
            this.comboRShake.Size = new System.Drawing.Size(100, 20);
            this.comboRShake.TabIndex = 23;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(27, 303);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(179, 12);
            this.label12.TabIndex = 24;
            this.label12.Text = "Interval frames for enable vertical";
            // 
            // numericDiffFrame
            // 
            this.numericDiffFrame.Location = new System.Drawing.Point(210, 300);
            this.numericDiffFrame.Name = "numericDiffFrame";
            this.numericDiffFrame.Size = new System.Drawing.Size(84, 19);
            this.numericDiffFrame.TabIndex = 25;
            // 
            // checkPedal
            // 
            this.checkPedal.AutoSize = true;
            this.checkPedal.Location = new System.Drawing.Point(27, 330);
            this.checkPedal.Name = "checkPedal";
            this.checkPedal.Size = new System.Drawing.Size(106, 16);
            this.checkPedal.TabIndex = 26;
            this.checkPedal.Text = "Pedal (right leg)";
            this.checkPedal.UseVisualStyleBackColor = true;
            // 
            // checkUpperBody
            // 
            this.checkUpperBody.AutoSize = true;
            this.checkUpperBody.Location = new System.Drawing.Point(27, 353);
            this.checkUpperBody.Name = "checkUpperBody";
            this.checkUpperBody.Size = new System.Drawing.Size(135, 16);
            this.checkUpperBody.TabIndex = 27;
            this.checkUpperBody.Text = "Set upper body bones";
            this.checkUpperBody.UseVisualStyleBackColor = true;
            this.checkUpperBody.CheckedChanged += new System.EventHandler(this.checkUpperBody_CheckedChanged);
            // 
            // numericSuppressInt
            // 
            this.numericSuppressInt.Location = new System.Drawing.Point(210, 352);
            this.numericSuppressInt.Name = "numericSuppressInt";
            this.numericSuppressInt.Size = new System.Drawing.Size(84, 19);
            this.numericSuppressInt.TabIndex = 30;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(210, 338);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(93, 12);
            this.label13.TabIndex = 31;
            this.label13.Text = "suppress interval";
            // 
            // PianoMotionCreateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 420);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.numericSuppressInt);
            this.Controls.Add(this.checkUpperBody);
            this.Controls.Add(this.checkPedal);
            this.Controls.Add(this.numericDiffFrame);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.comboRShake);
            this.Controls.Add(this.comboLShake);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.comboRVertical);
            this.Controls.Add(this.comboLVertical);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.comboRHorizon);
            this.Controls.Add(this.comboLHorizon);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numericTrackL);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkFingering);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.numericStartFrame);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonTrack);
            this.Controls.Add(this.numericTrackR);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxFile);
            this.Controls.Add(this.label1);
            this.Name = "PianoMotionCreateForm";
            this.Text = "Form";
            ((System.ComponentModel.ISupportInitialize)(this.numericStartFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTrackR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTrackL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericDiffFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSuppressInt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFile;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericStartFrame;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkFingering;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericTrackR;
        private System.Windows.Forms.Button buttonTrack;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericTrackL;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboLHorizon;
        private System.Windows.Forms.ComboBox comboRHorizon;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox comboLVertical;
        private System.Windows.Forms.ComboBox comboRVertical;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboLShake;
        private System.Windows.Forms.ComboBox comboRShake;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown numericDiffFrame;
        private System.Windows.Forms.CheckBox checkPedal;
        private System.Windows.Forms.CheckBox checkUpperBody;
        private System.Windows.Forms.NumericUpDown numericSuppressInt;
        private System.Windows.Forms.Label label13;
    }
}