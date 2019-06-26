using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PianoMotionCreateMidi
{
    public partial class PianoMotionCreateForm : Form
    {
        // member
        List<string> listAllBones;

        // getter/setter
        public string MidiFileName
        {
            get { return this.textBoxFile.Text; }
            set { this.textBoxFile.Text = value; }
        }
        public int TrackNumberR
        {
            get { return (int)this.numericTrackR.Value; }
            set { this.numericTrackR.Value = value; }
        }
        public int TrackNumberL
        {
            get { return (int)this.numericTrackL.Value; }
            set { this.numericTrackL.Value = value; }
        }
        public int PasteStartPos
        {
            get { return (int)this.numericStartFrame.Value; }
            set { this.numericStartFrame.Value = value; }
        }
        public bool CsvOverwrite
        {
            get { return this.checkFingering.Checked; }
            set { this.checkFingering.Checked = value; }
        }
        public int EnableDiffFrames
        {
            get { return (int)this.numericDiffFrame.Value; }
            set { this.numericDiffFrame.Value = value; }
        }
        public bool CheckPedal
        {
            get { return this.checkPedal.Checked; }
            set { this.checkPedal.Checked = value; }
        }
        public bool CheckUpperbody
        {
            get { return this.checkUpperBody.Checked; }
            set { this.checkUpperBody.Checked = value; }
        }
        public int UpperBodySuppressInterval
        {
            get { return (int)this.numericSuppressInt.Value; }
            set { this.numericSuppressInt.Value = value; }
        }

        // Initialize
        public PianoMotionCreateForm(bool bJapanese=true)
        {
            InitializeComponent();
            this.buttonTrack.Enabled = false;
            if (bJapanese)
            {
                this.label7.Text = "左手";
                this.label8.Text = "右手";
                this.label9.Text = "手首水平移動";
                this.label10.Text = "手首持上げ";
                this.label11.Text = "手首ブレ";
                this.label12.Text = "手首持上げ有効化フレーム数";
                this.label13.Text = "抑制間隔[frame]";
                this.checkPedal.Text = "ペダル操作";
                this.checkUpperBody.Text = "上半身モーション作成";
            }
        }

        /// <summary>
        /// Set wrist names
        /// </summary>
        /// <param name="lstBones">model's all bones</param>
        /// <param name="lstTargets">target bones(Left wrist, wrist upper, wrist sub, and Right so on)</param>
        public void SetAllBones(List<string> lstBones, List<string> lstTargets)
        {
            listAllBones = new List<string>(lstBones);
            this.comboLHorizon.BeginUpdate();
            this.comboLVertical.BeginUpdate();
            this.comboLShake.BeginUpdate();
            this.comboRHorizon.BeginUpdate();
            this.comboRVertical.BeginUpdate();
            this.comboRShake.BeginUpdate();
            foreach (var item in lstBones)
            {
                this.comboLHorizon.Items.Add(item);
                this.comboLVertical.Items.Add(item);
                this.comboLShake.Items.Add(item);
                this.comboRHorizon.Items.Add(item);
                this.comboRVertical.Items.Add(item);
                this.comboRShake.Items.Add(item);
            }
            this.comboLHorizon.EndUpdate();
            this.comboLVertical.EndUpdate();
            this.comboLShake.EndUpdate();
            this.comboRHorizon.EndUpdate();
            this.comboRVertical.EndUpdate();
            this.comboRShake.EndUpdate();

            for (int i = 0; i < this.comboLHorizon.Items.Count; i++)
            {
                if (this.comboLHorizon.Items[i].ToString() == lstTargets[0])
                {
                    this.comboLHorizon.SelectedIndex = i;
                    break;
                }
            }
            for (int i = 0; i < this.comboLVertical.Items.Count; i++)
            {
                if (this.comboLVertical.Items[i].ToString() == lstTargets[1])
                {
                    this.comboLVertical.SelectedIndex = i;
                    break;
                }
            }
            for (int i = 0; i < this.comboLShake.Items.Count; i++)
            {
                if (this.comboLShake.Items[i].ToString() == lstTargets[2])
                {
                    this.comboLShake.SelectedIndex = i;
                    break;
                }
            }
            for (int i = 0; i < this.comboRHorizon.Items.Count; i++)
            {
                if (this.comboRHorizon.Items[i].ToString() == lstTargets[3])
                {
                    this.comboRHorizon.SelectedIndex = i;
                    break;
                }
            }
            for (int i = 0; i < this.comboRVertical.Items.Count; i++)
            {
                if (this.comboRVertical.Items[i].ToString() == lstTargets[4])
                {
                    this.comboRVertical.SelectedIndex = i;
                    break;
                }
            }
            for (int i = 0; i < this.comboRShake.Items.Count; i++)
            {
                if (this.comboRShake.Items[i].ToString() == lstTargets[5])
                {
                    this.comboRShake.SelectedIndex = i;
                    break;
                }
            }
        }

        public string GetTargetBoneName(int iIndex)
        {
            if (iIndex == 0)
                return listAllBones[this.comboLHorizon.SelectedIndex];
            else if (iIndex == 1)
                return listAllBones[this.comboLVertical.SelectedIndex];
            else if (iIndex == 2)
                return listAllBones[this.comboLShake.SelectedIndex];
            else if (iIndex == 3)
                return listAllBones[this.comboRHorizon.SelectedIndex];
            else if (iIndex == 4)
                return listAllBones[this.comboRVertical.SelectedIndex];
            else if (iIndex == 5)
                return listAllBones[this.comboRShake.SelectedIndex];
            else
                return null;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Hide();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "MIDI file(*.mid;*.smf)|*.mid;*.smf|all(*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.Title = "Select MIDI file";
            //ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.textBoxFile.Text = ofd.FileName;
                this.buttonTrack.Enabled = true;
            }
        }

        private void buttonTrack_Click(object sender, EventArgs e)
        {
            // track statictics
            if (File.Exists(this.textBoxFile.Text))
            {
                string strHtml = "";
                MidiDataHolder.ShowTracksInfoHtml(this.textBoxFile.Text, ref strHtml, (int)this.numericTrackR.Value);
                InfoWebForm dlg = new InfoWebForm(strHtml);
                dlg.ShowDialog();
            }
        }

        private void checkUpperBody_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkUpperBody.Checked)
            {
                this.numericSuppressInt.Enabled = true;
            }
            else
            {
                this.numericSuppressInt.Enabled = false;
            }
        }
    }
}
