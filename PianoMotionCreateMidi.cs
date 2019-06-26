using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DxMath;
using MikuMikuPlugin;

namespace PianoMotionCreateMidi
{
    /// <summary>
    /// This file is MikuMikuMoving plugin.
    /// Generate fingering motion for keyboard from fingering lists and prototype poses.
    /// 
    /// programmed by SEKIWA, from 2015-03-21
    /// </summary>
    public class PianoMotionCreateMidi : ICommandPlugin
    {
        /// <summary>
        /// This plugin's GUID
        /// </summary>
        public Guid GUID
        {
            get { return new Guid("226698b5-f84b-4854-94ae-e20744bd4a0b"); }
        }

        /// <summary>
        /// Main form.
        /// Given from MikuMikuMoving.
        /// Use to show dialogs or messages.
        /// </summary>
        public IWin32Window ApplicationForm { get; set; }
        
        /// <summary>
        /// Scene object.
        /// Given from MikuMikuMoving
        /// To access MikuMikuMoving's models, accessarys, and so on.
        /// </summary>
        public Scene Scene { get; set; }
        
        /// <summary>
        /// Description for plugin, name, auther.
        /// </summary>
        public string Description
        {
            get { return "Generate fingering motion for keyboard from MIDI data v1.01"; }
        }
        
        /// <summary>
        /// Menu text (Japanese)
        /// To use "Environment.NewLine" for line break.
        /// </summary>
        public string Text
        {
            get { return "キーボード演奏モーション"; }
        }
        /// <summary>
        /// Menu text (English)
        /// </summary>
        public string EnglishText
        {
            get { return "Generate fingering motion for keyboard"; }
        }
        
        /// <summary>
        /// Button icon(32x32)
        /// If it is null, it is displayed default image.
        /// </summary>
        public Image Image
        {
            get { return Properties.Resources.PianoMotionCreateMidiIconL; }
        }
        /// <summary>
        /// Button icon for command bar(20x20)
        /// If it is null, it is displayed default image.
        /// </summary>
        public Image SmallImage
        {
            get { return Properties.Resources.PianoMotionCreateMidiIconS; }
        }

        /// <summary>
        /// Execute
        /// </summary>
        public void Run(CommandArgs e)
        {
            //MessageBox.Show(ApplicationForm, Scene.ApplicationVersion, "MikuMikuMoving Version");
            Log.DEBUG_LEVEL = Log.PRINT_OFF;
#if DEBUG
            Log.DEBUG_LEVEL = Log.PRINT_DEBUG;
            Log.Debug("START PianoMotionCreateMidi -- keyboard motion auto generation, debug mode");
#endif

            // Examine active model
            if (Scene.ActiveModel == null)
            {
                if (Scene.Language == "ja")
                    MessageBox.Show(ApplicationForm, "モデルを選択してください", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else
                    MessageBox.Show(ApplicationForm, "Please select a model.", "Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                // Disable change flag to set true in cancel.
                e.Cancel = true;
                return;
            }
            // model's bone
            long nSelectedLayers = 1;
            string R_THUMB_FINGER_POS = "右親指先";
            string R_THUMB_FINGER_ROT = "右親指２";
            string R_INDEX_FINGER_POS = "右人指先";
            string R_INDEX_FINGER_ROT = "右人指３";
            string R_MIDDLE_FINGER_POS = "右中指先";
            string R_MIDDLE_FINGER_ROT = "右中指３";
            string R_RING_FINGER_POS = "右薬指先";
            string R_RING_FINGER_ROT = "右薬指３";
            string R_LITTLE_FINGER_POS = "右小指先";
            string R_LITTLE_FINGER_ROT = "右小指３";
            string L_THUMB_FINGER_POS = "左親指先";
            string L_THUMB_FINGER_ROT  = "左親指２";
            string L_INDEX_FINGER_POS  = "左人指先";
            string L_INDEX_FINGER_ROT  = "左人指３";
            string L_MIDDLE_FINGER_POS = "左中指先";
            string L_MIDDLE_FINGER_ROT = "左中指３";
            string L_RING_FINGER_POS   = "左薬指先";
            string L_RING_FINGER_ROT   = "左薬指３";
            string L_LITTLE_FINGER_POS = "左小指先";
            string L_LITTLE_FINGER_ROT = "左小指３";
            string R_WRIST = "右手首";
            string L_WRIST = "左手首";
            string R_WRIST_SUB = "右手首F";
            string L_WRIST_SUB = "左手首F";
            string R_WRIST_UPPER = "右手首FP";
            string L_WRIST_UPPER = "左手首FP";
            if (Scene.ActiveModel.Bones[R_WRIST] == null) { nSelectedLayers = 0; }
            if (Scene.ActiveModel.Bones[R_THUMB_FINGER_POS] == null) { nSelectedLayers = 0; }
            if (Scene.ActiveModel.Bones[R_THUMB_FINGER_ROT] == null) { R_THUMB_FINGER_ROT = "右親指2"; if (Scene.ActiveModel.Bones[R_THUMB_FINGER_ROT] == null) { nSelectedLayers = 0; } }
            if (Scene.ActiveModel.Bones[R_INDEX_FINGER_POS] == null) { R_INDEX_FINGER_POS = "右人差指先"; if (Scene.ActiveModel.Bones[R_INDEX_FINGER_POS] == null) { nSelectedLayers = 0; } }
            if (Scene.ActiveModel.Bones[R_INDEX_FINGER_ROT] == null)
            {
                R_INDEX_FINGER_ROT = "右人指3";
                if (Scene.ActiveModel.Bones[R_INDEX_FINGER_ROT] == null)
                {
                    R_INDEX_FINGER_ROT = "右人差指３";
                    if (Scene.ActiveModel.Bones[R_INDEX_FINGER_ROT] == null)
                    {
                        R_INDEX_FINGER_ROT = "右人差指3";
                        if (Scene.ActiveModel.Bones[R_INDEX_FINGER_ROT] == null)
                        {
                            nSelectedLayers = 0;
                        }
                    }
                }
            }
            if (Scene.ActiveModel.Bones[R_MIDDLE_FINGER_POS] == null) { nSelectedLayers = 0; }
            if (Scene.ActiveModel.Bones[R_MIDDLE_FINGER_ROT] == null) { R_MIDDLE_FINGER_ROT = "右中指3"; if (Scene.ActiveModel.Bones[R_MIDDLE_FINGER_ROT] == null) { nSelectedLayers = 0; } }
            if (Scene.ActiveModel.Bones[R_RING_FINGER_POS] == null) { nSelectedLayers = 0; }
            if (Scene.ActiveModel.Bones[R_RING_FINGER_ROT] == null) { R_RING_FINGER_ROT = "右薬指3"; if (Scene.ActiveModel.Bones[R_RING_FINGER_ROT] == null) { nSelectedLayers = 0; } }
            if (Scene.ActiveModel.Bones[R_LITTLE_FINGER_POS] == null) { nSelectedLayers = 0; }
            if (Scene.ActiveModel.Bones[R_LITTLE_FINGER_ROT] == null) { R_LITTLE_FINGER_ROT = "右小指3"; if (Scene.ActiveModel.Bones[R_LITTLE_FINGER_ROT] == null) { nSelectedLayers = 0; } }
            if (Scene.ActiveModel.Bones[L_WRIST] == null) { nSelectedLayers = 0; }
            if (Scene.ActiveModel.Bones[L_THUMB_FINGER_POS] == null) { nSelectedLayers = 0; }
            if (Scene.ActiveModel.Bones[L_THUMB_FINGER_ROT] == null) { L_THUMB_FINGER_ROT  = "左親指2"; if (Scene.ActiveModel.Bones[L_THUMB_FINGER_ROT] == null){ nSelectedLayers = 0; } }
            if (Scene.ActiveModel.Bones[L_INDEX_FINGER_POS] == null) { L_INDEX_FINGER_POS  = "左人差指先"; if (Scene.ActiveModel.Bones[L_INDEX_FINGER_POS] == null) { nSelectedLayers = 0; } }
            if (Scene.ActiveModel.Bones[L_INDEX_FINGER_ROT] == null)
            {
                L_INDEX_FINGER_ROT  = "左人指3";
                if (Scene.ActiveModel.Bones[L_INDEX_FINGER_ROT] == null)
                {
                    L_INDEX_FINGER_ROT  = "左人差指３";
                    if (Scene.ActiveModel.Bones[L_INDEX_FINGER_ROT] == null)
                    {
                        L_INDEX_FINGER_ROT  = "左人差指3";
                        if (Scene.ActiveModel.Bones[L_INDEX_FINGER_ROT] == null)
                        {
                            nSelectedLayers = 0;
                        }
                    }
                }
            }
            if (Scene.ActiveModel.Bones[L_MIDDLE_FINGER_POS] == null) { nSelectedLayers = 0; }
            if (Scene.ActiveModel.Bones[L_MIDDLE_FINGER_ROT] == null) { L_MIDDLE_FINGER_ROT = "右中指3"; if (Scene.ActiveModel.Bones[L_MIDDLE_FINGER_ROT] == null) { nSelectedLayers = 0; } }
            if (Scene.ActiveModel.Bones[L_RING_FINGER_POS] == null)   { nSelectedLayers = 0; }
            if (Scene.ActiveModel.Bones[L_RING_FINGER_ROT] == null)   { L_RING_FINGER_ROT   = "右薬指3"; if (Scene.ActiveModel.Bones[L_RING_FINGER_ROT] == null) { nSelectedLayers = 0; } }
            if (Scene.ActiveModel.Bones[L_LITTLE_FINGER_POS] == null) { nSelectedLayers = 0; }
            if (Scene.ActiveModel.Bones[L_LITTLE_FINGER_ROT] == null) { L_LITTLE_FINGER_ROT = "右小指3"; if (Scene.ActiveModel.Bones[L_LITTLE_FINGER_ROT] == null) { nSelectedLayers = 0; } }
            if (nSelectedLayers == 0)
            {
                if (Scene.Language == "ja")
                    MessageBox.Show(ApplicationForm, "必要なボーンが足りません", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else
                    MessageBox.Show(ApplicationForm, "Can't find required bones.", "Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
                return;
            }
            // generate pedal motion
            bool bGenPedalMotion = true;
            // pedal target bones
            string R_LEG_IK = "右足ＩＫ";
            string L_LEG_IK = "左足ＩＫ";
            if (Scene.ActiveModel.Bones[R_LEG_IK] == null) { R_LEG_IK = "右足IK"; }
            if (Scene.ActiveModel.Bones[L_LEG_IK] == null) { L_LEG_IK = "左足IK"; }

            // bone names
            List<string> listAllBones = new List<string>();
            foreach (Bone bone in Scene.ActiveModel.Bones)
            {
                listAllBones.Add(bone.Name);
            }
            // wrist target bone names
            List<string> listTargetBones = new List<string>();
            listTargetBones.Add(L_WRIST);
            listTargetBones.Add(L_WRIST_UPPER);
            listTargetBones.Add(L_WRIST_SUB);
            listTargetBones.Add(R_WRIST);
            listTargetBones.Add(R_WRIST_UPPER);
            listTargetBones.Add(R_WRIST_SUB);
            
            // Language
            bool bJapanese = false;
            if (Scene.Language == "ja")
                bJapanese = true;

            // midi file
            string strMidiFile = "";
            // note track
            int[] iTracks = { 0, 0 };

            // MMD midi (audio) start frame
            long nMMDPasteStartPos = 30;
            // MMD preliminary action frame (To paste a keyframe previous nPreAction to set MIDI gate-on.)
            long nPreAction = 0;
            // To write csv file as analysed MIDI information.
            bool bCsvOverwrite = true;
            // Wrists upper motion threshold. Keyframe intervals between a previous note to the current note.
            long nEnableDiffFrame = 8;
            // Trill decision window keyframes, suppress moving wrist (2016-05-01 added)
            long nTrillRangeFrame = 4;

            // fingering list.
            List<List<long>> listMidiFingering = null;
            List<List<long>> listMidiFingeringR = new List<List<long>>();
            List<List<long>> listMidiFingeringL = new List<List<long>>();

            // The center position of right wrist(=middle finger): nRightWristCenterPos = 64
            const long nRightWristCenterPos = 64;
            // The center position of left wrist(=middle finger): nLeftWristCenterPos = 52
            const long nLeftWristCenterPos = 52;
            // check exist fingering csv file 
            bool bSaveFingeringFile = false;

            // generate upper body motion
            bool bGenUpperBody = true;
            long nUpperBodyMinMaxQueueSize = 30; // [frames] 30frames = 1sec
            long nUpperBodySuppressInterval = 10;// [frames]
            // upper body target bones
            string BONE_CENTER_W = "センターW";
            string BONE_UPPERBODY1_W = "上半身W";
            string BONE_UPPERBODY2_W = "上半身2W";
            string BONE_NECK = "首";
            string BONE_LINESIGHT_IK = "視線IK";
            if (Scene.ActiveModel.Bones[BONE_UPPERBODY2_W] == null) { BONE_UPPERBODY2_W = "上半身２W"; }
            if (Scene.ActiveModel.Bones[BONE_LINESIGHT_IK] == null) { BONE_LINESIGHT_IK = "視線ＩＫ"; }
            Vector3 vecBoneCenterWMove = new Vector3(0.0f, 0.0f, 0.0f);

#if DEBUG
            strMidiFile = "C:\\Users\\sekiwa\\Documents\\Visual Studio 2013\\Projects\\MMMPlugInSekiwa\\PianoMotionCreateMidi\\bin\\Release\\";
            strMidiFile += "_bach_846.mid";
            //strMidiFile += "_test03.mid";
            iTracks[0] = 2;
            iTracks[1] = 3;
            nMMDPasteStartPos = 30;
            Debug.Print(strMidiFile);
#endif
            using (PianoMotionCreateForm myDialog = new PianoMotionCreateForm(bJapanese))
            {
                myDialog.PasteStartPos = (int)nMMDPasteStartPos;
                myDialog.MidiFileName = strMidiFile;
                myDialog.CsvOverwrite = bCsvOverwrite;
                myDialog.EnableDiffFrames = (int)nEnableDiffFrame;
                myDialog.SetAllBones(listAllBones, listTargetBones);
                myDialog.CheckPedal = bGenPedalMotion;
                myDialog.CheckUpperbody = bGenUpperBody;
                myDialog.UpperBodySuppressInterval = (int)nUpperBodySuppressInterval;
                if (myDialog.ShowDialog(this.ApplicationForm) == DialogResult.OK)
                {
                    strMidiFile = myDialog.MidiFileName;
                    iTracks[0] = myDialog.TrackNumberR;
                    iTracks[1] = myDialog.TrackNumberL;
                    nMMDPasteStartPos = myDialog.PasteStartPos;
                    bCsvOverwrite = myDialog.CsvOverwrite;
                    nEnableDiffFrame = myDialog.EnableDiffFrames;
                    if (nEnableDiffFrame < 3)
                    {
                        nEnableDiffFrame = 3;
                    }
                    L_WRIST = myDialog.GetTargetBoneName(0);
                    L_WRIST_UPPER = myDialog.GetTargetBoneName(1);
                    L_WRIST_SUB = myDialog.GetTargetBoneName(2);
                    R_WRIST = myDialog.GetTargetBoneName(3);
                    R_WRIST_UPPER = myDialog.GetTargetBoneName(4);
                    R_WRIST_SUB = myDialog.GetTargetBoneName(5);
                    bGenPedalMotion = myDialog.CheckPedal;
                    bGenUpperBody = myDialog.CheckUpperbody;
                    nUpperBodySuppressInterval = myDialog.UpperBodySuppressInterval;
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }
            if (!File.Exists(strMidiFile))
            {
                if (Scene.Language == "ja")
                    MessageBox.Show(this.ApplicationForm, "MIDIファイルが見つかりません", "確認", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(this.ApplicationForm, "MIDI file does not exist", "Note", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }

            // show modeless dialog
            ProgressForm progressForm = new ProgressForm();
            progressForm.Show(ApplicationForm);
            progressForm.UpdateProgressBar(0, "reading tempo ...");

            // get tempo information
            AutoGenPolyphonicPianoFingerings myPianoFingerings = new AutoGenPolyphonicPianoFingerings(strMidiFile, nMMDPasteStartPos, nPreAction);

            // Select hands to right and left hands
            List<string> listRL = new List<string>() { "R", "L" };
            for (int iRL = 0; iRL < listRL.Count; iRL++)
            {
                progressForm.UpdateProgressBar(0, "analyzing fingering ... (" + listRL[iRL] + ")");
                
                string strExt = Path.GetExtension(strMidiFile);
                string strMidiCsvFile = strMidiFile.Substring(0, strMidiFile.Length - strExt.Length);
                strMidiCsvFile = strMidiCsvFile + listRL[iRL] + ".csv";

                if (iRL == 0) // Rright hand
                {
                    listMidiFingering = listMidiFingeringR;
                }
                else // Left hand
                {
                    listMidiFingering = listMidiFingeringL;
                }

                // Generate fingering and fingering csv files, if there is no csv files or no overwrite flag.
                // 同名のCSVファイルがない、または、上書き指定があれば、運指ファイルを生成
                if (!File.Exists(strMidiCsvFile) || bCsvOverwrite)
                {
                    int iRet = myPianoFingerings.GenFingerings(iRL, iTracks, Scene.KeyFramePerSec, ref listMidiFingering, strMidiCsvFile);
                    if (iRet == 0)
                    {
                        bSaveFingeringFile = true;
                    }
                    else
                    {
                        if (iRet == 1)
                        {
                            if (Scene.Language == "ja")
                                MessageBox.Show(ApplicationForm, "MIDIトラックの選択が誤っています", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            else
                                MessageBox.Show(ApplicationForm, "Please select effective track.", "Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else
                        {
                            if (Scene.Language == "ja")
                                MessageBox.Show(ApplicationForm, "運指の作成に失敗しました", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            else
                                MessageBox.Show(ApplicationForm, "Can't generate fingerings.", "Note", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        e.Cancel = true;
                        return;
                    }
                }
                else
                {
                    // read CSV file
                    if (!File.Exists(strMidiCsvFile))
                    {
                        if (Scene.Language == "ja")
                            MessageBox.Show(this.ApplicationForm, "CSVファイルが見つかりません\r\n" + Path.GetFileName(strMidiCsvFile), "確認", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            MessageBox.Show(this.ApplicationForm, "CSV file does not exist", "Note", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                        return;
                    }
                    StreamReader sr = new StreamReader(strMidiCsvFile);
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        if (line.Substring(0, 1) == "#")
                        {
                            continue;
                        }
                        List<long> lst = new List<long>();
                        var values = line.Split(',');
                        foreach (var value in values)
                        {
                            lst.Add(Convert.ToInt64(value.ToString()));
                        }
                        listMidiFingering.Add(lst);
                    }
                }
            }

            // pedal motion
            if (bGenPedalMotion)
            {
                progressForm.UpdateProgressBar(0, "Pedal motion generating...");
                List<List<long>> listPedalHold = new List<List<long>>();
                List<List<long>> listPedalSoft = new List<List<long>>();

                // - Reed MIDI file
                MidiDataHolder.GetMidiDataInfo(strMidiFile, -1, Scene.KeyFramePerSec, nMMDPasteStartPos, nPreAction, ref listPedalHold, MidiDataHolder.CONTROL_EVENT_HOLD);
                MidiDataHolder.GetMidiDataInfo(strMidiFile, -1, Scene.KeyFramePerSec, nMMDPasteStartPos, nPreAction, ref listPedalSoft, MidiDataHolder.CONTROL_EVENT_SOFT);

                // paste key frames
                if (Scene.ActiveModel.Bones[R_LEG_IK] != null)
                {
                    foreach (MotionLayer layer in Scene.ActiveModel.Bones[R_LEG_IK].Layers)
                    {
                        int iOnCnt = 0;
                        MotionFrameData data0 = layer.Frames.GetFrame(0);
                        MotionFrameData data1 = layer.Frames.GetFrame(1);
                        foreach (List<long> items in listPedalHold)
                        {
                            if (items[CommonConstants.CSV_COL_VEL] == 127)
                            {
                                data0.FrameNumber = items[CommonConstants.CSV_COL_ON_FRAME] - 1;
                                data1.FrameNumber = items[CommonConstants.CSV_COL_ON_FRAME];
                                layer.Frames.AddKeyFrame(data0);
                                layer.Frames.AddKeyFrame(data1);
                                iOnCnt++;
                            }
                            else
                            {
                                data0.FrameNumber = items[CommonConstants.CSV_COL_ON_FRAME];
                                data1.FrameNumber = items[CommonConstants.CSV_COL_ON_FRAME] - 1;
                                layer.Frames.AddKeyFrame(data0);
                                if (iOnCnt > 0)
                                {
                                    layer.Frames.AddKeyFrame(data1);
                                    iOnCnt = 0;
                                }
                            }
                        }
                        break;
                    }
                }
                if (Scene.ActiveModel.Bones[L_LEG_IK] != null)
                {
                    foreach (MotionLayer layer in Scene.ActiveModel.Bones[L_LEG_IK].Layers)
                    {
                        int iOnCnt = 0;
                        MotionFrameData data0 = layer.Frames.GetFrame(0);
                        MotionFrameData data1 = layer.Frames.GetFrame(1);
                        foreach (List<long> items in listPedalSoft)
                        {
                            if (items[CommonConstants.CSV_COL_VEL] == 127)
                            {
                                data0.FrameNumber = items[CommonConstants.CSV_COL_ON_FRAME] - 1;
                                data1.FrameNumber = items[CommonConstants.CSV_COL_ON_FRAME];
                                layer.Frames.AddKeyFrame(data0);
                                layer.Frames.AddKeyFrame(data1);
                                iOnCnt++;
                            }
                            else
                            {
                                data0.FrameNumber = items[CommonConstants.CSV_COL_ON_FRAME];
                                data1.FrameNumber = items[CommonConstants.CSV_COL_ON_FRAME] - 1;
                                layer.Frames.AddKeyFrame(data0);
                                if (iOnCnt > 0)
                                    layer.Frames.AddKeyFrame(data1);
                            }
                        }
                        break;
                    }
                }
            }

            // copy and paste key frames in MMM
            const int KEYFRAME_OFF = 0;                     // wrists height A
            const int KEYFRAME_ON_WHITE_NARROW = 1;         // 
            const int KEYFRAME_ON_WHITE_WIDE = 2;           // 
            const int KEYFRAME_OFF_WHITE_DRIVE_THUMB = 3;   // 
            const int KEYFRAME_ON_WHITE_DRIVE_THUMB = 4;    // 
            const int KEYFRAME_ON_WHITE_DRIVE_MIDDLE = 5;   // 
            const int KEYFRAME_OFF_MIDDLE_BLACK = 6;        // wrists height B
            const int KEYFRAME_ON_MIDDLE_BLACK_NARROW = 7;  // 
            const int KEYFRAME_ON_MIDDLE_BLACK_WIDE = 8;    // 
            const int KEYFRAME_OFF_BLACK = 10;              // wrists height C
            const int KEYFRAME_ON_BLACK_NARROW = 11;        // 
            const int KEYFRAME_ON_BLACK_WIDE = 12;          // 
            const int KEYFRAME_ON_BLACK_WHITE_NARROW = 13;  // 
            const int KEYFRAME_ON_BLACK_WHITE_WIDE = 14;    // 
            const int KEYFRAME_WIDE_WRIST = 16;
            const int KEYFRAME_UPPER_WRIST = 18;

            // right hand to left hand
            string[] listWristsName = new string[] { R_WRIST, L_WRIST };
            float[] listWristTemplateOctave = new float[] { 3.0f, -2.0f };
            long[] listWristCenterPos = new long[] { nRightWristCenterPos, nLeftWristCenterPos };
            List<List<string>> listFingersPosRL = new List<List<string>>();
            listFingersPosRL.Add(new List<string>() { "dummy", R_THUMB_FINGER_POS, R_INDEX_FINGER_POS, R_MIDDLE_FINGER_POS, R_RING_FINGER_POS, R_LITTLE_FINGER_POS });
            listFingersPosRL.Add(new List<string>() { "dummy", L_THUMB_FINGER_POS, L_INDEX_FINGER_POS, L_MIDDLE_FINGER_POS, L_RING_FINGER_POS, L_LITTLE_FINGER_POS });
            List<List<string>> listFingersRotRL = new List<List<string>>();
            listFingersRotRL.Add(new List<string>() { "dummy", R_THUMB_FINGER_ROT, R_INDEX_FINGER_ROT, R_MIDDLE_FINGER_ROT, R_RING_FINGER_ROT, R_LITTLE_FINGER_ROT });
            listFingersRotRL.Add(new List<string>() { "dummy", L_THUMB_FINGER_ROT, L_INDEX_FINGER_ROT, L_MIDDLE_FINGER_ROT, L_RING_FINGER_ROT, L_LITTLE_FINGER_ROT });
            string[] listWristsSubName = new string[] { R_WRIST_SUB, L_WRIST_SUB };
            string[] listWristsUpperName = new string[] { R_WRIST_UPPER, L_WRIST_UPPER };
            for (int iRL = 0; iRL < listRL.Count; iRL++)
            {
                progressForm.UpdateProgressBar(0, "analysing finger movement width ... (" + listRL[iRL] + ")");
                
                if (iRL == 0) // right-handed
                {
                    listMidiFingering = listMidiFingeringR;
                }
                else // left-handed
                {
                    listMidiFingering = listMidiFingeringL;
                }
                if (listMidiFingering.Count == 0)
                {
                    continue;
                }

                // calculate the amount of movement, to shift the wrist to treble side of white key.
                // 手首を白鍵1つ分高音側にずらすときの移動量を求める
                Vector3 deltaWristXw = new Vector3(0.0f, 0.0f, 0.0f);
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listWristsName[iRL]].Layers)
                {
                    // calculate the amount of movement for 1 white key from 3 octave (right hand), 2 octave (left hand).
                    deltaWristXw = layer.Frames.GetFrame(KEYFRAME_WIDE_WRIST).Position - layer.Frames.GetFrame(KEYFRAME_OFF).Position;
                    deltaWristXw /= (7.0f * listWristTemplateOctave[iRL]); // 7 x 3 oct(right) or 7 x 2 oct(left)
                    Log.Debug("{0} [00]:{1}", listWristsName[iRL], layer.Frames.GetFrame(KEYFRAME_OFF).Position.ToString());
                    Log.Debug("{0} [16]:{1}", listWristsName[iRL], layer.Frames.GetFrame(KEYFRAME_WIDE_WRIST).Position.ToString());
                    Log.Debug("{0} deltaXw:{1}", listWristsName[iRL], deltaWristXw.ToString());

                    // center bone motion (15-07-06)
                    vecBoneCenterWMove += deltaWristXw;
                    break;
                }
                vecBoneCenterWMove = vecBoneCenterWMove / 2;
                // wrist overshoot (手首のブレ用)
                Vector3 deltaWristSubY = new Vector3(0.0f, 0.0f, 0.0f);
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listWristsSubName[iRL]].Layers)
                {
                    if (layer != null)
                    {
                        deltaWristSubY = layer.Frames.GetFrame(1).Position - layer.Frames.GetFrame(KEYFRAME_OFF).Position;
                        Log.Debug("{0} deltaWristSubY:{1}", listWristsSubName[iRL], deltaWristSubY.ToString());
                    }
                    break;
                }
                // wrist upper (手首の持ち上げ用)
                Vector3 deltaWristUpper = new Vector3(0.0f, 0.0f, 0.0f);
                Quaternion deltaWristUpperQ = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
                MotionFrameData frameDataWristPre = new MotionFrameData();
                int iFrameDataWristFlag = 0;
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listWristsName[iRL]].Layers)
                {
                    deltaWristUpper = layer.Frames.GetFrame(KEYFRAME_UPPER_WRIST).Position - layer.Frames.GetFrame(KEYFRAME_OFF).Position;
                    //deltaWristUpperQ = layer.Frames.GetFrame(KEYFRAME_UPPER_WRIST).Quaternion - layer.Frames.GetFrame(KEYFRAME_OFF).Quaternion;
                    deltaWristUpperQ = layer.Frames.GetFrame(KEYFRAME_UPPER_WRIST).Quaternion;
                }
                // 白鍵時(KEYFRAME_ON_WHITE_NARROW)：各指を白鍵1つ分高音側にずらすときの移動量を求める
                List<Vector3> listDeltaXOnWhite = new List<Vector3>();
                listDeltaXOnWhite.Add(new Vector3(0.0f, 0.0f, 0.0f));// dummy
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][1]].Layers)// 親指
                {
                    Vector3 _delta = layer.Frames.GetFrame(KEYFRAME_ON_WHITE_NARROW).Position - layer.Frames.GetFrame(KEYFRAME_ON_WHITE_WIDE).Position;
                    if (iRL == 1)// Left
                        _delta *= -1.0f;
                    listDeltaXOnWhite.Add(_delta);
                    Log.Debug("{0} 白鍵 deltaXw:{1}", listFingersPosRL[iRL][1], _delta.ToString());
                    break;
                }
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][2]].Layers)// 人差し指
                {
                    Vector3 _delta = layer.Frames.GetFrame(KEYFRAME_ON_WHITE_NARROW).Position - layer.Frames.GetFrame(KEYFRAME_ON_WHITE_WIDE).Position;
                    if (iRL == 1)// Left
                        _delta *= -1.0f;
                    listDeltaXOnWhite.Add(_delta);
                    Log.Debug("{0} 白鍵 deltaXw:{1}", listFingersPosRL[iRL][2], _delta.ToString());
                    break;
                }
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][3]].Layers)// 中指
                {
                    Vector3 _delta = layer.Frames.GetFrame(KEYFRAME_ON_WHITE_WIDE).Position - layer.Frames.GetFrame(KEYFRAME_ON_WHITE_NARROW).Position;
                    if (iRL == 1)// Left
                        _delta *= -1.0f;
                    listDeltaXOnWhite.Add(_delta);
                    Log.Debug("{0} 白鍵 deltaXw:{1}", listFingersPosRL[iRL][3], _delta.ToString());
                    break;
                }
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][4]].Layers)// 薬指
                {
                    Vector3 _delta = layer.Frames.GetFrame(KEYFRAME_ON_WHITE_WIDE).Position - layer.Frames.GetFrame(KEYFRAME_ON_WHITE_NARROW).Position;
                    if (iRL == 1)// Left
                        _delta *= -1.0f;
                    Log.Debug("{0} 白鍵 deltaXw:{1}", listFingersPosRL[iRL][4], _delta.ToString());
                    listDeltaXOnWhite.Add(_delta);
                    break;
                }
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][5]].Layers)// 小指
                {
                    Vector3 _delta = layer.Frames.GetFrame(KEYFRAME_ON_WHITE_WIDE).Position - layer.Frames.GetFrame(KEYFRAME_ON_WHITE_NARROW).Position;
                    if (iRL == 1)// Left
                        _delta *= -1.0f;
                    listDeltaXOnWhite.Add(_delta);
                    Log.Debug("{0} 白鍵 deltaXw:{1}", listFingersPosRL[iRL][5], _delta.ToString());
                    break;
                }
                // 黒鍵時(KEYFRAME_ON_BLACK_NARROW)：各指を黒鍵1つ分高音側にずらすときの移動量を求める
                List<Vector3> listDeltaXOnBlack = new List<Vector3>();
                listDeltaXOnBlack.Add(new Vector3(0.0f, 0.0f, 0.0f));// dummy
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][1]].Layers)
                {
                    Vector3 _delta = layer.Frames.GetFrame(KEYFRAME_ON_BLACK_NARROW).Position - layer.Frames.GetFrame(KEYFRAME_ON_BLACK_WIDE).Position;
                    if (iRL == 0)//Right
                        _delta /= 2.0f;
                    if (iRL == 1)//Left
                        _delta *= -1.0f;
                    listDeltaXOnBlack.Add(_delta);// 親指
                    Log.Debug("{0} 黒鍵 deltaXw:{1}", listFingersPosRL[iRL][1], _delta.ToString());
                    break;
                }
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][2]].Layers)
                {
                    Vector3 _delta = layer.Frames.GetFrame(KEYFRAME_ON_BLACK_NARROW).Position - layer.Frames.GetFrame(KEYFRAME_ON_BLACK_WIDE).Position;
                    if (iRL == 1)//Left
                        _delta *= -1.0f;
                    listDeltaXOnBlack.Add(_delta);// 人差し指
                    Log.Debug("{0} 黒鍵 deltaXw:{1}", listFingersPosRL[iRL][2], _delta.ToString());
                    break;
                }
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][3]].Layers)
                {
                    Vector3 _delta = layer.Frames.GetFrame(KEYFRAME_ON_BLACK_WIDE).Position - layer.Frames.GetFrame(KEYFRAME_ON_BLACK_NARROW).Position;
                    if (iRL == 1)//Left
                        _delta *= -1.0f;
                    listDeltaXOnBlack.Add(_delta);// 中指
                    Log.Debug("{0} 黒鍵 deltaXw:{1}", listFingersPosRL[iRL][3], _delta.ToString());
                    break;
                }
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][4]].Layers)
                {
                    Vector3 _delta = layer.Frames.GetFrame(KEYFRAME_ON_BLACK_WIDE).Position - layer.Frames.GetFrame(KEYFRAME_ON_BLACK_NARROW).Position;
                    if (iRL == 1)//Left
                        _delta *= -1.0f;
                    listDeltaXOnBlack.Add(_delta);// 薬指
                    Log.Debug("{0} 黒鍵 deltaXw:{1}", listFingersPosRL[iRL][4], _delta.ToString());
                    break;
                }
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][5]].Layers)
                {
                    Vector3 _delta = layer.Frames.GetFrame(KEYFRAME_ON_BLACK_WIDE).Position - layer.Frames.GetFrame(KEYFRAME_ON_BLACK_NARROW).Position;
                    if (iRL == 1)//Left
                        _delta /= -2.0f;
                    listDeltaXOnBlack.Add(_delta);// 小指
                    Log.Debug("{0} 黒鍵 deltaXw:{1}", listFingersPosRL[iRL][5], _delta.ToString());
                    break;
                }
                // 白鍵+黒鍵時(KEYFRAME_ON_MIDDLE_BLACK_NARROW)：人差指～薬指を黒鍵1つ分高音側にずらすときの移動量を求める
                // 右手系・左手系共通
                List<Vector3> listDeltaXOnWBlack = new List<Vector3>();
                listDeltaXOnWBlack.Add(new Vector3(0.0f, 0.0f, 0.0f));// dummy
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][1]].Layers)
                {
                    listDeltaXOnWBlack.Add(new Vector3(0.0f, 0.0f, 0.0f));// dummy
                    break;
                }
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][2]].Layers)
                {
                    Vector3 _delta = layer.Frames.GetFrame(KEYFRAME_ON_MIDDLE_BLACK_WIDE).Position - layer.Frames.GetFrame(KEYFRAME_ON_MIDDLE_BLACK_NARROW).Position;
                    listDeltaXOnWBlack.Add(_delta);// 人差し指
                    break;
                }
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][3]].Layers)
                {
                    Vector3 _delta = layer.Frames.GetFrame(KEYFRAME_ON_MIDDLE_BLACK_WIDE).Position - layer.Frames.GetFrame(KEYFRAME_ON_MIDDLE_BLACK_NARROW).Position;
                    listDeltaXOnWBlack.Add(_delta / 2.0f);// 中指
                    break;
                }
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][4]].Layers)
                {
                    Vector3 _delta = layer.Frames.GetFrame(KEYFRAME_ON_MIDDLE_BLACK_WIDE).Position - layer.Frames.GetFrame(KEYFRAME_ON_MIDDLE_BLACK_NARROW).Position;
                    listDeltaXOnWBlack.Add(_delta);// 薬指
                    break;
                }
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][5]].Layers)
                {
                    listDeltaXOnWBlack.Add(new Vector3(0.0f, 0.0f, 0.0f));// dummy
                    break;
                }
                // 黒鍵+白鍵時(KEYFRAME_ON_BLACK_WHITE_NARROW)：人差指～薬指を白鍵1つ分高音側にずらすときの移動量を求める
                // 右手系・左手系共通
                List<Vector3> listDeltaXOnBWhite = new List<Vector3>();
                listDeltaXOnBWhite.Add(new Vector3(0.0f, 0.0f, 0.0f));// dummy
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][1]].Layers)
                {
                    listDeltaXOnBWhite.Add(new Vector3(0.0f, 0.0f, 0.0f));// dummy
                    break;
                }
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][2]].Layers)
                {
                    Vector3 _delta = layer.Frames.GetFrame(KEYFRAME_ON_BLACK_WHITE_NARROW).Position - layer.Frames.GetFrame(KEYFRAME_ON_BLACK_WHITE_WIDE).Position;
                    listDeltaXOnBWhite.Add(_delta);// 人差し指
                    break;
                }
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][3]].Layers)
                {
                    Vector3 _delta = layer.Frames.GetFrame(KEYFRAME_ON_BLACK_WHITE_NARROW).Position - layer.Frames.GetFrame(KEYFRAME_ON_BLACK_WHITE_WIDE).Position;
                    listDeltaXOnBWhite.Add(_delta);// 中指
                    break;
                }
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][4]].Layers)
                {
                    Vector3 _delta = layer.Frames.GetFrame(KEYFRAME_ON_BLACK_WHITE_NARROW).Position - layer.Frames.GetFrame(KEYFRAME_ON_BLACK_WHITE_WIDE).Position;
                    listDeltaXOnBWhite.Add(_delta);// 薬指
                    break;
                }
                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listFingersPosRL[iRL][5]].Layers)
                {
                    listDeltaXOnBWhite.Add(new Vector3(0.0f, 0.0f, 0.0f));// dummy
                    break;
                }

                // 手首と指の貼り付け
                string strProgressMsg = "paste fingering ... " + listRL[iRL];
                int nProgressMsg = 0;
                progressForm.UpdateProgressBar(nProgressMsg, strProgressMsg);
                int iCount = 0;
                long nFingerNumberOld = -1;
                long nNoteOld = -1;
                long nOldWristFrameCopyFrom = -1;
                //                                      E->E  E->F  E->F# E->G  E->G# E->A  E->A# E->B  E->C  E->C# E->D  E->D#
                float[] fWristMoveSteps = new float[] { 0.0f, 1.0f, 1.5f, 2.0f, 2.5f, 3.0f, 3.5f, 4.0f, 5.0f, 5.5f, 6.0f, 6.5f };
                foreach (List<long> fingerRef in listMidiFingering)
                {
                    List<long> finger = new List<long>(fingerRef);
                    // fingering number 0 is passed
                    if (finger[CommonConstants.CSV_COL_FINGER] == 0)
                    {
                        iCount++;
                        continue;
                    }
                    if (finger[CommonConstants.CSV_COL_FINGER] > 5)
                    {
                        iCount++;
                        continue;
                    }
#if DEBUG
                    //Log.Print("#### 手首と指の貼り付け OFF:{0}", DebugListToString(finger));
#endif
                    // コピー元のフレームを決める
                    long nWristFrameCopyFrom = KEYFRAME_ON_WHITE_NARROW;// 1:白鍵, 4:白鍵+指潜り, 7:白鍵+黒鍵, 11:黒鍵, 13:黒鍵+白鍵
                    long nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_NARROW;
                    // single tone 単音のとき
                    if (finger[CommonConstants.CSV_COL_POLYPHONIC] == 0)
                    {
                        // decide white or black key 白鍵、黒鍵の判定
                        nWristFrameCopyFrom = KEYFRAME_ON_WHITE_NARROW;// 1:白鍵, 4:白鍵+指潜り, 7:白鍵+黒鍵, 11:黒鍵, 13:黒鍵+白鍵
                        nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_NARROW;
                        if (!AutoGenPolyphonicPianoFingerings.IsWhiteKey(finger[CommonConstants.CSV_COL_NOTE])) // 黒鍵(7:白鍵+黒鍵, 11:黒鍵, 13:黒鍵+白鍵)
                        {
                            // 手首位置の補正
                            if (finger[CommonConstants.CSV_COL_FINGER] == 1 || finger[CommonConstants.CSV_COL_FINGER] == 5)// 親指または小指
                            {
                                nWristFrameCopyFrom = KEYFRAME_ON_BLACK_NARROW;
                                nFingerFrameCopyFrom = KEYFRAME_ON_BLACK_NARROW;
                            }
                            else
                            {
                                // white key + black key 白鍵+黒鍵
                                nWristFrameCopyFrom = KEYFRAME_ON_MIDDLE_BLACK_NARROW;
                                nFingerFrameCopyFrom = KEYFRAME_ON_MIDDLE_BLACK_NARROW;
                                // 1つ前の音が黒鍵、かつ、親指または小指
                                //if (!AutoGenPolyphonicPianoFingerings.IsWhiteKey(nNoteOld) && (nFingerNumberOld == 1 || nFingerNumberOld == 5))
                                //{
                                //    nWristFrameCopyFrom = KEYFRAME_ON_BLACK_NARROW;
                                //    nFingerFrameCopyFrom = KEYFRAME_ON_BLACK_NARROW;
                                //}
                                // 1つ前の手首も黒鍵
                                if (nOldWristFrameCopyFrom == KEYFRAME_ON_BLACK_NARROW)
                                {
                                    nWristFrameCopyFrom = KEYFRAME_ON_BLACK_NARROW;
                                    nFingerFrameCopyFrom = KEYFRAME_ON_BLACK_NARROW;
                                }
                                // 1つ後の手首も黒鍵
                                if ((iCount < listMidiFingering.Count - 1) && (!AutoGenPolyphonicPianoFingerings.IsWhiteKey(listMidiFingering[iCount + 1][CommonConstants.CSV_COL_NOTE])))
                                {
                                    nWristFrameCopyFrom = KEYFRAME_ON_BLACK_NARROW;
                                    nFingerFrameCopyFrom = KEYFRAME_ON_BLACK_NARROW;
                                }
                            }
                        }
                        else // 白鍵(1:白鍵, 4:白鍵+指潜り, 7:白鍵+黒鍵, 13:黒鍵+白鍵)
                        {
                            // Correct wrist position
                            // from GenarateCost method
                            // Right
                            // fingerA ＜ fingerB, distance ≧ 0 → order(1,2,3...)
                            // fingerA ＜ fingerB, distance ＜ 0 → stride, fingerAが親指以外は高コスト(1)
                            // fingerA ＞ fingerB, distance ＞ 0 → stride, fingerBが親指以外は高コスト(2)
                            // fingerA ＞ fingerB, distance ≦ 0 → order
                            // Left
                            // fingerA ＜ fingerB, distance ≦ 0 → order
                            // fingerA ＜ fingerB, distance ＞ 0 → stride, fingerAが親指以外は高コスト(3)
                            // fingerA ＞ fingerB, distance ＜ 0 → stride, fingerBが親指以外は高コスト(4)
                            // fingerA ＞ fingerB, distance ≧ 0 → order
                            if (nFingerNumberOld == 1)// finger stride 指潜りの判定
                            {
                                if (finger[CommonConstants.CSV_COL_FINGER] > 1)
                                {
                                    if (iRL == 0)// Right
                                    {
                                        if (nNoteOld >= finger[CommonConstants.CSV_COL_NOTE])// (1)
                                        {
                                            nWristFrameCopyFrom = KEYFRAME_ON_WHITE_DRIVE_THUMB;
                                            nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_DRIVE_THUMB;
                                            // 手首位置の補正, 白鍵で2つ分高音側へ
                                            finger[CommonConstants.CSV_COL_WRIST] = ShiftWhiteKey(finger[CommonConstants.CSV_COL_WRIST], 2);
                                        }
                                    }
                                    else if (iRL == 1)// Left
                                    {
                                        if (nNoteOld <= finger[CommonConstants.CSV_COL_NOTE])// (3)
                                        {
                                            nWristFrameCopyFrom = KEYFRAME_ON_WHITE_DRIVE_THUMB;
                                            nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_DRIVE_THUMB;
                                            finger[CommonConstants.CSV_COL_WRIST] = ShiftWhiteKey(finger[CommonConstants.CSV_COL_WRIST], -2);
                                        }
                                    }
                                }
                            }
                            if (nFingerNumberOld > 1)// 指潜りの判定
                            {
                                if (finger[CommonConstants.CSV_COL_FINGER] == 1)
                                {
                                    if (iRL == 0)// Right
                                    {
                                        if (nNoteOld <= finger[CommonConstants.CSV_COL_NOTE])// (2)
                                        {
                                            nWristFrameCopyFrom = KEYFRAME_ON_WHITE_DRIVE_THUMB;
                                            nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_DRIVE_THUMB;
                                            // 手首位置の補正, 白鍵で1つ分低音側へ
                                            finger[CommonConstants.CSV_COL_WRIST] = ShiftWhiteKey(finger[CommonConstants.CSV_COL_WRIST], -1);
                                        }
                                    }
                                    else if (iRL == 1)// Left
                                    {
                                        if (nNoteOld >= finger[CommonConstants.CSV_COL_NOTE])// (4)
                                        {
                                            nWristFrameCopyFrom = KEYFRAME_ON_WHITE_DRIVE_THUMB;
                                            nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_DRIVE_THUMB;
                                            finger[CommonConstants.CSV_COL_WRIST] = ShiftWhiteKey(finger[CommonConstants.CSV_COL_WRIST], 1);
                                        }
                                    }
                                }
                            }
                            // 1つ前の音が黒鍵、かつ、親指または小指
                            //if (!AutoGenPolyphonicPianoFingerings.IsWhiteKey(nNoteOld) && (nFingerNumberOld == 1 || nFingerNumberOld == 5))
                            //{
                            //    if (finger[CommonConstants.CSV_COL_FINGER] >= 2 && finger[CommonConstants.CSV_COL_FINGER] <= 4)
                            //    {
                            //        nWristFrameCopyFrom = 13;
                            //        nFingerFrameCopyFrom = 13;
                            //    }
                            //}
                            // 1つ前の手首が黒鍵, ただし親指、小指は除く
                            if (nOldWristFrameCopyFrom == KEYFRAME_ON_BLACK_NARROW)
                            {
                                if (finger[CommonConstants.CSV_COL_FINGER] >= 2 && finger[CommonConstants.CSV_COL_FINGER] <= 4)
                                {
                                    nWristFrameCopyFrom = KEYFRAME_ON_BLACK_WHITE_NARROW;
                                    nFingerFrameCopyFrom = KEYFRAME_ON_BLACK_WHITE_NARROW;
                                }
                            }
                        }
                        // trill decision
                        // In window frames (nTrillRangeFrame), the same note repeated, decide trill.
                        if (0 < iCount && iCount < listMidiFingering.Count - 1)
                        {
                            if (listMidiFingering[iCount - 1][CommonConstants.CSV_COL_POLYPHONIC] == 0 &&
                                listMidiFingering[iCount + 1][CommonConstants.CSV_COL_POLYPHONIC] == 0)
                            {
                                if (listMidiFingering[iCount - 1][CommonConstants.CSV_COL_ON_FRAME] > finger[CommonConstants.CSV_COL_ON_FRAME] - nTrillRangeFrame &&
                                    listMidiFingering[iCount + 1][CommonConstants.CSV_COL_ON_FRAME] < finger[CommonConstants.CSV_COL_ON_FRAME] + nTrillRangeFrame)
                                {
                                    // sample C-D-C, C-C#-C, C#-D-C#, C#-D#-C#
                                    if (listMidiFingering[iCount - 1][CommonConstants.CSV_COL_NOTE] == listMidiFingering[iCount + 1][CommonConstants.CSV_COL_NOTE])
                                    {
                                        finger[CommonConstants.CSV_COL_WRIST] = (listMidiFingering[iCount][CommonConstants.CSV_COL_WRIST] + listMidiFingering[iCount - 1][CommonConstants.CSV_COL_WRIST]) / 2;
                                        // priority to black keys than white keys (sample C-C#-C, C#-D-C#)
                                        bool b1 = AutoGenPolyphonicPianoFingerings.IsWhiteKey(finger[CommonConstants.CSV_COL_NOTE]);
                                        bool b2 = AutoGenPolyphonicPianoFingerings.IsWhiteKey(listMidiFingering[iCount - 1][CommonConstants.CSV_COL_NOTE]);
                                        if (!b1 || !b2)
                                        {
                                            nWristFrameCopyFrom = KEYFRAME_ON_BLACK_WHITE_NARROW;// 1:白鍵, 4:白鍵+指潜り, 7:白鍵+黒鍵, 11:黒鍵, 13:黒鍵+白鍵
                                            nFingerFrameCopyFrom = KEYFRAME_ON_BLACK_WHITE_NARROW;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // 和音(polyphonic): node単位で計算すればよいので冗長….
                    else
                    {
                        // 手首位置を先に決める
                        // 親指、小指が黒鍵なら KEYFRAME_ON_BLACK_NARROW.
                        int iBlack = 0;
                        int iWhiteBlack = 0;
                        int iWhite = 0;
                        if (!AutoGenPolyphonicPianoFingerings.IsWhiteKey(listMidiFingering[iCount][CommonConstants.CSV_COL_NOTE]))// 黒鍵
                        {
                            if ((listMidiFingering[iCount][CommonConstants.CSV_COL_FINGER] == 1) || (listMidiFingering[iCount][CommonConstants.CSV_COL_FINGER] == 5))
                            {
                                iBlack++;
                            }
                            else
                            {
                                iWhiteBlack++;
                            }
                        }
                        else
                        {
                            iWhite++;
                        }
                        for (int i = iCount - 1; i >= 0; i--)
                        {
                            if (listMidiFingering[iCount][CommonConstants.CSV_COL_POLYPHONIC] == listMidiFingering[i][CommonConstants.CSV_COL_POLYPHONIC])// 同一ノード
                            {
                                if (!AutoGenPolyphonicPianoFingerings.IsWhiteKey(listMidiFingering[i][CommonConstants.CSV_COL_NOTE]))
                                {
                                    if ((listMidiFingering[i][CommonConstants.CSV_COL_FINGER] == 1) || (listMidiFingering[i][CommonConstants.CSV_COL_FINGER] == 5))
                                    {
                                        iBlack++;
                                    }
                                    else
                                    {
                                        iWhiteBlack++;
                                    }
                                }
                                else
                                {
                                    iWhite++;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        for (int i = iCount + 1; i < listMidiFingering.Count; i++)
                        {
                            if (listMidiFingering[iCount][CommonConstants.CSV_COL_POLYPHONIC] == listMidiFingering[i][CommonConstants.CSV_COL_POLYPHONIC])// 同一ノード
                            {
                                if (!AutoGenPolyphonicPianoFingerings.IsWhiteKey(listMidiFingering[i][CommonConstants.CSV_COL_NOTE]))
                                {
                                    if ((listMidiFingering[i][CommonConstants.CSV_COL_FINGER] == 1) || (listMidiFingering[i][CommonConstants.CSV_COL_FINGER] == 5))
                                    {
                                        iBlack++;
                                    }
                                    else
                                    {
                                        iWhiteBlack++;
                                    }
                                }
                                else
                                { 
                                    iWhite++;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (iBlack > 0)// 1:白鍵, 4:白鍵+指潜り, 7:白鍵+黒鍵, 11:黒鍵, 13:黒鍵+白鍵
                        {
                            nWristFrameCopyFrom = KEYFRAME_ON_BLACK_NARROW;
                            nFingerFrameCopyFrom = KEYFRAME_ON_BLACK_NARROW;
                        }
                        else if (iWhiteBlack > 0)
                        {
                            // 白鍵+黒鍵
                            nWristFrameCopyFrom = KEYFRAME_ON_MIDDLE_BLACK_NARROW;
                            nFingerFrameCopyFrom = KEYFRAME_ON_MIDDLE_BLACK_NARROW;
                        }
                        else
                        {
                            nWristFrameCopyFrom = KEYFRAME_ON_WHITE_NARROW;
                            nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_NARROW;
                        }
                        //Log.Debug("和音判定 手首コピー元フレーム: " + nWristFrameCopyFrom.ToString());
                    }

                    // 手首の位置の座標換変のための倍数の計算
                    // listWristCenterPos[iRL] 0:64, 1:52
                    float fQW = ((finger[CommonConstants.CSV_COL_WRIST] - listWristCenterPos[iRL]) / 12) * 7;// 整数計算であることが重要
                    long nWristMod = finger[CommonConstants.CSV_COL_WRIST] - listWristCenterPos[iRL];
                    if (finger[CommonConstants.CSV_COL_WRIST] < listWristCenterPos[iRL])
                    {
                        fQW = ((finger[CommonConstants.CSV_COL_WRIST] - 11 - listWristCenterPos[iRL]) / 12) * 7;
                        nWristMod = 12 - (listWristCenterPos[iRL] - finger[CommonConstants.CSV_COL_WRIST]) % 12;
                    }
                    fQW += fWristMoveSteps[nWristMod % 12];

                    // Check wrist: (1) not harmony, (2) the first note on the node.
                    // 一つ前のノードの CommonConstants.CSV_COL_OFF と 現在のノードの CommonConstants.CSV_COL_ON でnEnableDiffFrameフレーム以上あれば持ち上げる
                    long nPreviousOffFrame = 0;
                    long nCurrentOnFrame = 0;
                    int iCurrentVel = (int)finger[CommonConstants.CSV_COL_VEL];
                    bool bSetWrist = false;
                    bool bSetWristUp = false;
                    if (finger[CommonConstants.CSV_COL_POLYPHONIC] == 0)
                    {
                        nCurrentOnFrame = finger[CommonConstants.CSV_COL_ON_FRAME];
                        for (int i = iCount - 1; i >= Math.Max(iCount - 10, 0); i--)
                        {
                            if (nPreviousOffFrame < listMidiFingering[i][CommonConstants.CSV_COL_OFF_FRAME])
                                nPreviousOffFrame = listMidiFingering[i][CommonConstants.CSV_COL_OFF_FRAME];
                        }
                        bSetWrist = true;
                        bSetWristUp = true;
                    }
                    else
                    {
                        int iCheckTieCur = 0;// nodeがタイならば手首を持ち上げない
                        int iCheckTieNext = 0;
                        long nMinPosition = finger[CommonConstants.CSV_COL_POSITION];// nodeの最初以外は手首を持ち上げない
                        long nCurrentPolyId = finger[CommonConstants.CSV_COL_POLYPHONIC];
                        if (finger[CommonConstants.CSV_COL_DIVIDE_TIE] == CommonConstants.LIST_POS_TIE)// 自身がタイであるかどうかは関係ないため、コメントアウト
                            iCheckTieCur++;
                        for (int i = iCount - 1; i >= 0; i--)
                        {
                            if (listMidiFingering[i][CommonConstants.CSV_COL_POLYPHONIC] == nCurrentPolyId)
                            {
                                nMinPosition = listMidiFingering[i][CommonConstants.CSV_COL_POSITION];
                                if (listMidiFingering[i][CommonConstants.CSV_COL_DIVIDE_TIE] == CommonConstants.LIST_POS_TIE)
                                    iCheckTieCur++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        for (int i = iCount + 1; i < listMidiFingering.Count; i++)
                        {
                            
                            if (listMidiFingering[i][CommonConstants.CSV_COL_POLYPHONIC] == nCurrentPolyId)
                            {
                                if (listMidiFingering[i][CommonConstants.CSV_COL_DIVIDE_TIE] == CommonConstants.LIST_POS_TIE)
                                    iCheckTieCur++;
                            }
                            else if (finger[CommonConstants.CSV_COL_POLYPHONIC] == nCurrentPolyId + 1)
                            {
                                if (listMidiFingering[i][CommonConstants.CSV_COL_DIVIDE_TIE] == CommonConstants.LIST_POS_TIE)
                                    iCheckTieNext++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        // nodeの最初の音のときのみ手首を貼り付ける
                        if (nMinPosition == finger[CommonConstants.CSV_COL_POSITION])
                        {
                            bSetWrist = true;

                            // かつ, nodeがタイでないときのみ手首を持ち上げるモーションを貼り付ける
                            if (iCheckTieCur == 0)
                            {
                                nCurrentOnFrame = finger[CommonConstants.CSV_COL_ON_FRAME];
                                for (int i = iCount - 1; i >= Math.Max(iCount - 10, 0); i--)
                                {
                                    if (nPreviousOffFrame < listMidiFingering[i][CommonConstants.CSV_COL_OFF_FRAME])
                                    {
                                        nPreviousOffFrame = listMidiFingering[i][CommonConstants.CSV_COL_OFF_FRAME];
                                    }
                                }
                                bSetWristUp = true;
                            }
                        }
                    }
                    // Add wrist pose
                    if (bSetWrist)
                    {
                        MotionFrameData frameDataWristCur1 = new MotionFrameData();
                        MotionFrameData frameDataWristCur2 = new MotionFrameData();
                        foreach (MotionLayer layer in Scene.ActiveModel.Bones[listWristsName[iRL]].Layers)
                        {
                            MotionFrameData data1 = layer.Frames.GetFrame(nWristFrameCopyFrom);
                            // for (int i = 1; i <= layer.Frames.Count; i++) // 2015-06-24 comment out
                            for (int i = 1; i < layer.Frames.Count; i++)
                            {
                                if (layer.Frames[i].FrameNumber == nWristFrameCopyFrom)
                                {
                                    data1 = layer.Frames.GetKeyFrames()[i];
                                    break;
                                }
                            }
                            data1.Position += deltaWristXw * fQW;
                            data1.FrameNumber = finger[CommonConstants.CSV_COL_ON_FRAME];
                            layer.Frames.AddKeyFrame(data1);

                            // for rotation, from current position to next position
                            frameDataWristCur1 = data1;

                            data1.FrameNumber = finger[CommonConstants.CSV_COL_OFF_FRAME] - 1;
                            layer.Frames.AddKeyFrame(data1);

                            // for rotation, from current position to next position
                            frameDataWristCur2 = data1;
                            break;
                        }
                        // Log.Debug("手首位置補正 fQW:{0}, wrist:{1}, nWristFrameCopyFrom:{2}", fQW, finger[CommonConstants.CSV_COL_WRIST], nWristFrameCopyFrom);

                        // 音符と音符に間隔があるときは手首を持ち上げてみる
                        if (bSetWristUp && (nPreviousOffFrame + nEnableDiffFrame < nCurrentOnFrame))
                        {
                            // add wrist upper (up)
                            float fAmount = 1.0f;
                            if (Scene.ActiveModel.Bones[listWristsUpperName[iRL]] != null)
                            {
                                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listWristsUpperName[iRL]].Layers)
                                {
                                    // Log.Debug("手首持ち上げ off:{0}, on:{1}", nPreviousOffFrame, AutoGenPolyphonicPianoFingerings.DebugListToString(finger));

                                    // 補完曲線は素早く持ち上げてどすんと落とす
                                    MotionFrameData data1 = new MotionFrameData();
                                    data1 = layer.Frames.GetFrame(KEYFRAME_OFF);
                                    data1.FrameNumber = nPreviousOffFrame;
                                    InterpolatePoint pt1 = new InterpolatePoint(0, 64);
                                    InterpolatePoint pt2 = new InterpolatePoint(64, 127);
                                    data1.InterpolXA = pt1;
                                    data1.InterpolXB = pt2;
                                    data1.InterpolYA = pt1;
                                    data1.InterpolYB = pt2;
                                    data1.InterpolZA = pt1;
                                    data1.InterpolZB = pt2;
                                    layer.Frames.AddKeyFrame(data1);

                                    data1.FrameNumber = nCurrentOnFrame;
                                    pt1.X = 43;
                                    pt1.Y = 0;
                                    pt2.X = 127;
                                    pt2.Y = 28;
                                    data1.InterpolXA = pt1;
                                    data1.InterpolXB = pt2;
                                    data1.InterpolYA = pt1;
                                    data1.InterpolYB = pt2;
                                    data1.InterpolZA = pt1;
                                    data1.InterpolZB = pt2;
                                    layer.Frames.AddKeyFrame(data1);

                                    Vector3 _delta = new Vector3(0.0f, 0.0f, 0.0f);
                                    _delta = deltaWristUpper;
                                    // velや間隔で持ち上げる量(_delta)の倍率を変える
                                    if (iCurrentVel < 64)
                                        fAmount = 0.5f;
                                    else
                                        fAmount = iCurrentVel / 127.0f; // 0.5<->1.0
                                    _delta *= fAmount;
                                    data1.Position += _delta;
                                    // deltaWristUpperQは手首FPではなく, 手首に設定するべき. でないと変なことに….
                                    //data1.Quaternion = deltaWristUpperQ; // Not good. Shold be used R_WRIST or L_WRIST.
                                    data1.FrameNumber = (nPreviousOffFrame + nCurrentOnFrame) / 2;
                                    pt1.X = 0;
                                    pt1.Y = 64;
                                    pt2.X = 64;
                                    pt2.Y = 127;
                                    data1.InterpolXA = pt1;
                                    data1.InterpolXB = pt2;
                                    data1.InterpolYA = pt1;
                                    data1.InterpolYB = pt2;
                                    data1.InterpolZA = pt1;
                                    data1.InterpolZB = pt2;
                                    layer.Frames.AddKeyFrame(data1);
                                    break;
                                }
                            }
                            // add wrist shake upper
                            if (Scene.ActiveModel.Bones[listWristsSubName[iRL]] != null)
                            {
                                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listWristsSubName[iRL]].Layers)
                                {
                                    Vector3 _delta = new Vector3(0.0f, 0.0f, 0.0f);
                                    _delta = deltaWristSubY;
                                    MotionFrameData data1 = layer.Frames.GetFrame(KEYFRAME_OFF);
                                    if (layer.Frames[KEYFRAME_OFF].FrameNumber == 0)
                                    {
                                        data1 = layer.Frames.GetKeyFrames()[KEYFRAME_OFF];
                                    }
                                    data1.FrameNumber = nPreviousOffFrame;
                                    layer.Frames.AddKeyFrame(data1);
                                    data1.FrameNumber = nCurrentOnFrame;
                                    layer.Frames.AddKeyFrame(data1);
                                    data1.Position -= _delta / 4;
                                    data1.FrameNumber = (nPreviousOffFrame + nCurrentOnFrame) / 2;
                                    layer.Frames.AddKeyFrame(data1);
                                    break;
                                }
                            }
                            // add wrist upper (rotate) 2016-07-16 in test
                            if (Scene.ActiveModel.Bones[listWristsName[iRL]] != null)
                            {
                                foreach (MotionLayer layer in Scene.ActiveModel.Bones[listWristsName[iRL]].Layers)
                                {
                                    //Log.Debug("1:{0},{1},{2},{3} <-> 2:{4},{5},{6},{7}", frameDataWristPre.FrameNumber, frameDataWristPre.Position.X, frameDataWristPre.Position.Y, frameDataWristPre.Position.Z, frameDataWristCur1.FrameNumber, frameDataWristCur1.Position.X, frameDataWristCur1.Position.Y, frameDataWristCur1.Position.Z);
                                    //Log.Debug(" {0}+{1}/2={2}<->{3}+{4}/2={5}", frameDataWristPre.FrameNumber, frameDataWristCur1.FrameNumber, (frameDataWristPre.FrameNumber + frameDataWristCur1.FrameNumber) / 2, nPreviousOffFrame, nCurrentOnFrame, (nPreviousOffFrame + nCurrentOnFrame) / 2);

                                    // center position from previous position to next position
                                    MotionFrameData data1 = new MotionFrameData();
                                    //data1.FrameNumber = (frameDataWristPre.FrameNumber + frameDataWristCur1.FrameNumber) / 2;
                                    data1.FrameNumber = (nPreviousOffFrame + nCurrentOnFrame) / 2;
                                    Vector3 _delta = new Vector3(
                                        (frameDataWristPre.Position.X + frameDataWristCur1.Position.X) / 2.0f,
                                        (frameDataWristPre.Position.Y + frameDataWristCur1.Position.Y) / 2.0f,
                                        (frameDataWristPre.Position.Z + frameDataWristCur1.Position.Z) / 2.0f);
                                    data1.Position = _delta;
                                    Quaternion _deltaQ = Quaternion.Slerp(frameDataWristPre.Quaternion, deltaWristUpperQ, fAmount);
                                    data1.Quaternion = _deltaQ;
                                    InterpolatePoint pt1 = new InterpolatePoint(32, 0);
                                    InterpolatePoint pt2 = new InterpolatePoint(95, 95);
                                    data1.InterpolXA = pt1;
                                    data1.InterpolXB = pt2;
                                    data1.InterpolYA = pt1;
                                    data1.InterpolYB = pt2;
                                    data1.InterpolZA = pt1;
                                    data1.InterpolZB = pt2;
                                    data1.InterpolRA = new InterpolatePoint(0, 32);
                                    data1.InterpolRB = new InterpolatePoint(95, 127);
                                    layer.Frames.AddKeyFrame(data1);
                                    // re-entry
                                    frameDataWristCur1.FrameNumber = finger[CommonConstants.CSV_COL_ON_FRAME];
                                    pt1 = new InterpolatePoint(32, 32);
                                    pt2 = new InterpolatePoint(95, 127);
                                    frameDataWristCur1.InterpolXA = pt1;
                                    frameDataWristCur1.InterpolXB = pt2;
                                    frameDataWristCur1.InterpolYA = pt1;
                                    frameDataWristCur1.InterpolYB = pt2;
                                    frameDataWristCur1.InterpolZA = pt1;
                                    frameDataWristCur1.InterpolZB = pt2;
                                    frameDataWristCur1.InterpolRA = new InterpolatePoint(48, 0);
                                    frameDataWristCur1.InterpolRB = new InterpolatePoint(127, 95);
                                    layer.Frames.AddKeyFrame(frameDataWristCur1);
                                    break;
                                }
                            }
                        }
                        frameDataWristPre = frameDataWristCur2;
                    }

                    // Add wrist shake
                    if (finger[CommonConstants.CSV_COL_DIVIDE_TIE] != CommonConstants.LIST_POS_TIE)
                    {
                        // Shold be rewrite: not single note, only one calculate. (this code calculates each times.)
                        // iOneFive=1: "1" generates wrist shake to all fingers, this is very annoying. "0" generates wrist shake thumb or little fingers.
                        int iOneFive = 0;
                        if (finger[CommonConstants.CSV_COL_POLYPHONIC] == 0)
                        {
                            // exclusion thumb finger stride (2016-04-20)
                            if (nWristFrameCopyFrom != KEYFRAME_ON_WHITE_DRIVE_THUMB)
                            {
                                if (finger[CommonConstants.CSV_COL_FINGER] == 1 || finger[CommonConstants.CSV_COL_FINGER] == 5)
                                    iOneFive++;
                            }
                        }
                        else
                        {
                            // for (int i = iCount - 1; i > 0; i--) 2015-06-25 TBD: calculate current value?
                            for (int i = iCount; i > 0; i--)
                            {
                                if (finger[CommonConstants.CSV_COL_POLYPHONIC] == listMidiFingering[i][CommonConstants.CSV_COL_POLYPHONIC])
                                {
                                    if (listMidiFingering[i][CommonConstants.CSV_COL_FINGER] == 1 || listMidiFingering[i][CommonConstants.CSV_COL_FINGER] == 5)
                                        iOneFive++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            for (int i = iCount + 1; i < listMidiFingering.Count; i++)
                            {
                                if (finger[CommonConstants.CSV_COL_POLYPHONIC] == listMidiFingering[i][CommonConstants.CSV_COL_POLYPHONIC])
                                {
                                    if (listMidiFingering[i][CommonConstants.CSV_COL_FINGER] == 1 || listMidiFingering[i][CommonConstants.CSV_COL_FINGER] == 5)
                                        iOneFive++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        int iWristShakeOnOffInterval = (int)(2.0f * Scene.KeyFramePerSec / 30.0f);
                        int iWristShakeOnOffPre = iWristShakeOnOffInterval / 2;
                        int iWristShakeOff = iWristShakeOnOffInterval * 2 - iWristShakeOnOffPre;
                        int iWristShakeOn = iWristShakeOnOffInterval - iWristShakeOnOffPre;
                        if ((Scene.ActiveModel.Bones[listWristsSubName[iRL]] != null) && (iOneFive > 0))
                        {
                            foreach (MotionLayer layer in Scene.ActiveModel.Bones[listWristsSubName[iRL]].Layers)
                            {
                                Vector3 _delta = new Vector3(0.0f, 0.0f, 0.0f);
                                _delta = deltaWristSubY;
                                MotionFrameData data1 = layer.Frames.GetFrame(KEYFRAME_OFF);
                                if (layer.Frames[KEYFRAME_OFF].FrameNumber == 0)
                                {
                                    data1 = layer.Frames.GetKeyFrames()[KEYFRAME_OFF];
                                }
                                data1.FrameNumber = finger[CommonConstants.CSV_COL_ON_FRAME] - iWristShakeOnOffPre;
                                layer.Frames.AddKeyFrame(data1);
                                data1.FrameNumber = finger[CommonConstants.CSV_COL_ON_FRAME] + iWristShakeOff;
                                layer.Frames.AddKeyFrame(data1);
                                data1.Position += _delta;
                                data1.FrameNumber = finger[CommonConstants.CSV_COL_ON_FRAME] + iWristShakeOn;
                                layer.Frames.AddKeyFrame(data1);
                                break;
                            }
                        }
                    }

                    // Correct finger position
                    // difference from default wrist position: listWristCenterPos[0]:64, [1]:52 and current wrist position
                    Vector3 deltaFingerX = new Vector3(0.0f, 0.0f, 0.0f);
                    float fQF = 0.0f;
                    long nRelativeNote = (finger[CommonConstants.CSV_COL_WRIST] - listWristCenterPos[iRL]) % 12;
                    if (nRelativeNote < 0) { nRelativeNote += 12; }
                    if (nFingerFrameCopyFrom == KEYFRAME_ON_MIDDLE_BLACK_NARROW) // 白鍵(1,2指)+黒鍵(2,3,4指)
                    {
                        List<Vector3> lstDeltaX = listDeltaXOnWBlack;
                        int Afinger = (int)finger[CommonConstants.CSV_COL_FINGER];
                        if (iRL == 1) // Left
                        {
                            if (Afinger == 1)
                                Afinger = 5;
                            else if (Afinger == 2)
                                Afinger = 4;
                            else if (Afinger == 4)
                                Afinger = 2;
                            else if (Afinger == 5)
                                Afinger = 1;
                        }
                        if (Afinger == 2) // 人差指
                        {
                            fQF = GetFingerCorrection(-3, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                        }
                        else if (Afinger == 3) // 中指
                        {
                            fQF = GetFingerCorrection(-1, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                        }
                        else if (Afinger == 4) // 薬指
                        {
                            fQF = GetFingerCorrection(3, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                        }
                        else if (Afinger == 5) // 小指
                        {
                            nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_NARROW;// 親指, 小指は1フレーム目をコピーする
                            fQF = GetFingerCorrection(4, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                            lstDeltaX = listDeltaXOnWhite;
                        }
                        else
                        {
                            nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_NARROW;// 親指, 小指は1フレーム目をコピーする
                            fQF = GetFingerCorrection(-4, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                            lstDeltaX = listDeltaXOnWhite;
                        }
                        if (!AutoGenPolyphonicPianoFingerings.IsWhiteKey(finger[CommonConstants.CSV_COL_WRIST]))
                        {
                            fQF -= 0.5f;
                        }
                        deltaFingerX = lstDeltaX[(int)finger[CommonConstants.CSV_COL_FINGER]] * fQF;
                    }
                    else if (nFingerFrameCopyFrom == KEYFRAME_ON_WHITE_DRIVE_THUMB) // 指潜り(白鍵のみ)
                    {
                        int Afinger = (int)finger[CommonConstants.CSV_COL_FINGER];
                        if (iRL == 0) // Right
                        {
                            if (Afinger == 2) // 人差指(B->)
                            {
                                nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_NARROW;// 親指以外は白鍵用のモーションをコピーする
                                fQF = GetFingerCorrection(-6, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                            }
                            else if (Afinger == 3) // 中指(C->)
                            {
                                nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_NARROW;// 親指以外は白鍵用のモーションをコピーする
                                fQF = GetFingerCorrection(-4, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                            }
                            else if (Afinger == 4) // 薬指(D->)
                            {
                                nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_NARROW;// 親指以外は白鍵用のモーションをコピーする
                                fQF = GetFingerCorrection(-2, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                            }
                            else if (Afinger == 5) // 小指(E->)
                            {
                                nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_NARROW;// 親指以外は白鍵用のモーションをコピーする
                                fQF = GetFingerCorrection(0, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                            }
                            else
                            {
                                nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_DRIVE_THUMB;
                                fQF = GetFingerCorrection(-2, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                            }
                        }
                        else // Left
                        {
                            if (Afinger == 2) // 人差指(A->)
                            {
                                nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_NARROW;
                                fQF = GetFingerCorrection(6, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                            }
                            else if (Afinger == 3) // 中指(G->)
                            {
                                nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_NARROW;
                                fQF = GetFingerCorrection(4, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                            }
                            else if (Afinger == 4) // 薬指(F->)
                            {
                                nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_NARROW;
                                fQF = GetFingerCorrection(2, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                            }
                            else if (Afinger == 5) // 小指(E->)
                            {
                                nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_NARROW;
                                fQF = GetFingerCorrection(0, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                            }
                            else
                            {
                                nFingerFrameCopyFrom = KEYFRAME_ON_WHITE_DRIVE_THUMB;
                                fQF = GetFingerCorrection(2, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                            }
                        }
                        if (!AutoGenPolyphonicPianoFingerings.IsWhiteKey(finger[CommonConstants.CSV_COL_WRIST]))
                        {
                            fQF -= 0.5f;
                        }
                        deltaFingerX = listDeltaXOnWhite[(int)finger[CommonConstants.CSV_COL_FINGER]] * fQF;
                    }
                    else if (nFingerFrameCopyFrom == KEYFRAME_ON_BLACK_NARROW) // 黒鍵のみ(1,2,3,4,5指)
                    {
                        int Afinger = (int)finger[CommonConstants.CSV_COL_FINGER];
                        if (iRL == 1)
                        {
                            if (Afinger == 1)
                                Afinger = 5;
                            else if (Afinger == 2)
                                Afinger = 4;
                            else if (Afinger == 4)
                                Afinger = 2;
                            else if (Afinger == 5)
                                Afinger = 1;
                        }
                        if (Afinger == 2) // 人差指
                        {
                            fQF = GetFingerCorrection(-1, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                        }
                        else if (Afinger == 3) // 中指
                        {
                            fQF = GetFingerCorrection(1, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                        }
                        else if (Afinger == 4) // 薬指
                        {
                            fQF = GetFingerCorrection(3, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                        }
                        else if (Afinger == 5) // 小指
                        {
                            fQF = GetFingerCorrection(5, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                        }
                        else
                        {
                            fQF = GetFingerCorrection(-3, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                        }
                        if (!AutoGenPolyphonicPianoFingerings.IsWhiteKey(finger[CommonConstants.CSV_COL_WRIST]))
                        {
                            fQF -= 0.5f;
                        }
                        deltaFingerX = listDeltaXOnBlack[(int)finger[CommonConstants.CSV_COL_FINGER]] * fQF;
                    }
                    else if (nFingerFrameCopyFrom == KEYFRAME_ON_BLACK_WHITE_NARROW) // 白鍵のみ(2,3,4指)
                    {
                        int Afinger = (int)finger[CommonConstants.CSV_COL_FINGER];
                        if (iRL == 1) // Left
                        {
                            if (Afinger == 2)
                                Afinger = 4;
                            else if (Afinger == 4)
                                Afinger = 2;
                        }
                        if (Afinger == 2) // 人差指
                        {
                            fQF = GetFingerCorrection(0, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                        }
                        else if (Afinger == 3) // 中指
                        {
                            fQF = GetFingerCorrection(2, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                        }
                        else if (Afinger == 4) // 薬指
                        {
                            fQF = GetFingerCorrection(4, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                        }
                        if (!AutoGenPolyphonicPianoFingerings.IsWhiteKey(finger[CommonConstants.CSV_COL_WRIST]))
                        {
                            fQF -= 0.5f;
                        }
                        deltaFingerX = listDeltaXOnBWhite[(int)finger[CommonConstants.CSV_COL_FINGER]] * fQF;
                    }
                    else // 白鍵のみ(1,2,3,4,5指)
                    {
                        int Afinger = (int)finger[CommonConstants.CSV_COL_FINGER];
                        if (iRL == 1) // Left
                        {
                            if (Afinger == 1)
                                Afinger = 5;
                            else if (Afinger == 2)
                                Afinger = 4;
                            else if (Afinger == 4)
                                Afinger = 2;
                            else if (Afinger == 5)
                                Afinger = 1;
                        }
                        if (Afinger == 2)
                        {
                            fQF = GetFingerCorrection(-2, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                        }
                        else if (Afinger == 3)
                        {
                            fQF = GetFingerCorrection(0, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                        }
                        else if (Afinger == 4)
                        {
                            fQF = GetFingerCorrection(2, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                        }
                        else if (Afinger == 5)
                        {
                            fQF = GetFingerCorrection(4, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                        }
                        else
                        {
                            fQF = GetFingerCorrection(-4, (int)finger[CommonConstants.CSV_COL_WRIST], (int)finger[CommonConstants.CSV_COL_NOTE]);
                        }
                        if (!AutoGenPolyphonicPianoFingerings.IsWhiteKey(finger[CommonConstants.CSV_COL_WRIST]))
                        {
                            fQF -= 0.5f;
                        }
                        deltaFingerX = listDeltaXOnWhite[(int)finger[CommonConstants.CSV_COL_FINGER]] * fQF;
                    }
                    // Log.Debug("nFingerFrameCopyFrom:{0}, finger[CommonConstants.CSV_COL_FINGER]:{1}, nRelativeNote={2}", nFingerFrameCopyFrom, finger[CommonConstants.CSV_COL_FINGER], nRelativeNote);
                    // Log.Debug("finger[CommonConstants.CSV_COL_NOTE]:{0}, fQF:{1}", finger[CommonConstants.CSV_COL_NOTE], fQF);

                    // Paste finger pose
                    List<Bone> listTwoBone = new List<Bone>();
                    listTwoBone.Add(Scene.ActiveModel.Bones[listFingersPosRL[iRL][(int)finger[CommonConstants.CSV_COL_FINGER]]]);// POS
                    listTwoBone.Add(Scene.ActiveModel.Bones[listFingersRotRL[iRL][(int)finger[CommonConstants.CSV_COL_FINGER]]]);// ROT
                    foreach (Bone bone in listTwoBone)
                    {
                        foreach (MotionLayer layer in bone.Layers)
                        {
                            // OFF
                            // Not paste off keyframe before on keyframe which note is the same for previous note.
                            long nCheckSamePrev = 0;
                            long nCheckSamePost = 0;
                            for (int nBackForward = layer.Frames.Count - 1; nBackForward > 0; nBackForward--)
                            {
                                if (layer.Frames[nBackForward].FrameNumber == finger[CommonConstants.CSV_COL_ON_FRAME])
                                {
                                    nCheckSamePrev++;
                                    break;
                                }
                                if (layer.Frames[nBackForward].FrameNumber < finger[CommonConstants.CSV_COL_ON_FRAME])
                                {
                                    break;
                                }
                            }
                            // Not paste off keyframe before on keyframe which note is the tie and the same finger.
                            if (finger[CommonConstants.CSV_COL_DIVIDE_TIE] == CommonConstants.LIST_POS_TIE)//tie
                            {
                                for (int i = iCount - 1; i >= Math.Max(iCount - 10, 0); i--)
                                {
                                    if (listMidiFingering[i][CommonConstants.CSV_COL_FINGER] == finger[CommonConstants.CSV_COL_FINGER])
                                    {
                                        if (listMidiFingering[i][CommonConstants.CSV_COL_NOTE] == finger[CommonConstants.CSV_COL_NOTE])
                                        {
                                            if (listMidiFingering[i][CommonConstants.CSV_COL_OFF_FRAME] + 1 >= finger[CommonConstants.CSV_COL_ON_FRAME])
                                            {
                                                nCheckSamePrev++;
                                                // Remove previous off keyframe
                                                layer.Frames.RemoveKeyFrame(listMidiFingering[i][CommonConstants.CSV_COL_OFF_FRAME]);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            //// Not paste off keyframe before on keyframe which note is polyphonic and the same code. (2016-05-02 added) ←アイディア倒れのためコメントアウト. 型に指を広げた状態でOFFが必要なため.
                            //if (finger[CommonConstants.CSV_COL_POLYPHONIC] > 0)
                            //{
                            //    // previous node is the same code: nCheckSamePrev++;
                            //    for (int i = iCount - 1; i >= Math.Max(iCount - 10, 0); i--)
                            //    {
                            //        if (listMidiFingering[i][CommonConstants.CSV_COL_POLYPHONIC] == finger[CommonConstants.CSV_COL_POLYPHONIC])
                            //        {
                            //            continue;
                            //        }
                            //        else
                            //        {
                            //            if (listMidiFingering[i][CommonConstants.CSV_COL_POLYPHONIC] > 0)
                            //            {
                            //                if (listMidiFingering[i][CommonConstants.CSV_COL_POLYPHONIC] == finger[CommonConstants.CSV_COL_POLYPHONIC] - 1)
                            //                {
                            //                    if (listMidiFingering[i][CommonConstants.CSV_COL_NOTE] == finger[CommonConstants.CSV_COL_NOTE])
                            //                    {
                            //                        if (listMidiFingering[i][CommonConstants.CSV_COL_OFF_FRAME] + 10 >= finger[CommonConstants.CSV_COL_ON_FRAME])// under 10 frames, not off
                            //                        {
                            //                            nCheckSamePrev++;
                            //                            break;
                            //                        }
                            //                    }
                            //                }
                            //                else
                            //                {
                            //                    break;
                            //                }
                            //            }
                            //            else
                            //            {
                            //                break;
                            //            }
                            //        }
                            //    }
                            //    // next node is the same code: nCheckSamePost++;
                            //    for (int i = iCount + 1; i < Math.Min(iCount + 10, listMidiFingering.Count); i++)
                            //    {
                            //        if (listMidiFingering[i][CommonConstants.CSV_COL_POLYPHONIC] == finger[CommonConstants.CSV_COL_POLYPHONIC])
                            //        {
                            //            continue;
                            //        }
                            //        else
                            //        {
                            //            if (listMidiFingering[i][CommonConstants.CSV_COL_POLYPHONIC] > 0)
                            //            {
                            //                if (listMidiFingering[i][CommonConstants.CSV_COL_POLYPHONIC] == finger[CommonConstants.CSV_COL_POLYPHONIC] + 1)
                            //                {
                            //                    if (listMidiFingering[i][CommonConstants.CSV_COL_NOTE] == finger[CommonConstants.CSV_COL_NOTE])
                            //                    {
                            //                        if (listMidiFingering[i][CommonConstants.CSV_COL_ON_FRAME] - 10 <= finger[CommonConstants.CSV_COL_OFF_FRAME])// under 10 frames, not off
                            //                        {
                            //                            nCheckSamePost++;
                            //                            break;
                            //                        }
                            //                    }
                            //                }
                            //                else
                            //                {
                            //                    break;
                            //                }
                            //            }
                            //            else
                            //            {
                            //                break;
                            //            }
                            //        }
                            //    }
                            //}

                            // Get 0 keyframe
                            MotionFrameData data0 = layer.Frames.GetFrame(0);
                            if (layer.Frames[0].FrameNumber == 0)
                            {
                                data0 = (MotionFrameData)layer.Frames.GetKeyFrames()[0];
                            }
                            if (nCheckSamePrev == 0)
                            {
                                data0.FrameNumber = finger[CommonConstants.CSV_COL_ON_FRAME] - 1;
                                layer.Frames.AddKeyFrame(data0);
                            }
                            if (nCheckSamePost == 0)
                            {
                                data0.FrameNumber = finger[CommonConstants.CSV_COL_OFF_FRAME];
                                layer.Frames.AddKeyFrame(data0);
                            }
                            
                            // Change ON finger position
                            MotionFrameData data1 = layer.Frames.GetFrame(nFingerFrameCopyFrom);
                            for (int i = 1; i < layer.Frames.Count; i++)
                            {
                                if (layer.Frames[i].FrameNumber == nFingerFrameCopyFrom)
                                {
                                    data1 = layer.Frames.GetKeyFrames()[i];
                                    break;
                                }
                            }
                            data1.Position += deltaFingerX;
                            data1.FrameNumber = finger[CommonConstants.CSV_COL_ON_FRAME];
                            layer.Frames.AddKeyFrame(data1);
                            if (finger[CommonConstants.CSV_COL_ON_FRAME] < finger[CommonConstants.CSV_COL_OFF_FRAME])
                            {
                                data1.FrameNumber = finger[CommonConstants.CSV_COL_OFF_FRAME] - 1;
                                layer.Frames.AddKeyFrame(data1);
                            }
                            break;
                        }
                    }
                    if (iCount % 5 == 0 || iCount == listMidiFingering.Count - 1)
                    {
                        progressForm.UpdateProgressBar(nProgressMsg + (iCount * 100) / listMidiFingering.Count, strProgressMsg);
                    }

                    // 次回ループ用に値を代入
                    nNoteOld = finger[CommonConstants.CSV_COL_NOTE];
                    nFingerNumberOld = finger[CommonConstants.CSV_COL_FINGER];
                    nOldWristFrameCopyFrom = nWristFrameCopyFrom;
                    iCount++;
                }
                nProgressMsg = 0;
            }

            // Target bone: center, upperbody, neck, eye_ik motion 
            // 運指生成時に手首位置を修正しているので, 運指後に実施.
            // memo: 中心位置, 変化点, 移動判定閾値, 移動量
            long nBodyCenterPos = 64;
            const int POS_CSV_BODY_ONFRAME = 0;
            const int POS_CSV_BODY_BOTHHANDS = 1;
            const int POS_CSV_BODY_R = 2;
            const int POS_CSV_BODY_L = 3;
            if (bGenUpperBody && ((listMidiFingeringR.Count > 0) || (listMidiFingeringL.Count > 0)))
            {
                progressForm.UpdateProgressBar(100, "set body key frames...");
                
                // X方向を左右手首の中心位置を計算 0:frame, 1:center-pos(MIDI note), 2:right, 3:left
                List<List<long>> listCenter = new List<List<long>>();
                int nR = 0;
                int nL = 0;
                if (listMidiFingeringR.Count > 0)
                {
                    long nMaxFrame = Math.Max(listMidiFingeringR.Last()[CommonConstants.CSV_COL_ON_FRAME], listMidiFingeringL.Last()[CommonConstants.CSV_COL_ON_FRAME]);
                    for (int i = 0; i < listMidiFingeringR.Count; i++)
                    {
                        if (listMidiFingeringR[i][CommonConstants.CSV_COL_ON_FRAME] < nMMDPasteStartPos)
                        {
                            nR = i;
                        }
                        else
                        {
                            if ((i == 0) || (listCenter.Count == 0) || (listMidiFingeringR[i - 1][CommonConstants.CSV_COL_ON_FRAME] < listMidiFingeringR[i][CommonConstants.CSV_COL_ON_FRAME]))// not add the same frame
                            {
                                listCenter.Add(new List<long>() { listMidiFingeringR[i][CommonConstants.CSV_COL_ON_FRAME], 0, listMidiFingeringR[i][CommonConstants.CSV_COL_WRIST], 0 });
                            }
                        }
                    }
                }
                if (listMidiFingeringL.Count > 0)
                {
                    for (int i = 0; i < listMidiFingeringL.Count; i++)
                    {
                        if (listMidiFingeringL[i][CommonConstants.CSV_COL_ON_FRAME] < nMMDPasteStartPos)
                        {
                            nL = i;
                        }
                        else
                        {
                            if ((i == 0) || (listCenter.Count == 0) || (listMidiFingeringL[i - 1][CommonConstants.CSV_COL_ON_FRAME] < listMidiFingeringL[i][CommonConstants.CSV_COL_ON_FRAME]))// not add the same frame
                            {
                                listCenter.Add(new List<long>() { listMidiFingeringL[i][CommonConstants.CSV_COL_ON_FRAME], 0, 0, listMidiFingeringL[i][CommonConstants.CSV_COL_WRIST] });
                            }
                        }
                    }
                }
                for (int i = 0; i < listCenter.Count - 1; i++) // bubble sort..., Should be rewrite LINQ
                {
                    for (int j = i + 1; j < listCenter.Count; j++)
                    {
                        if (listCenter[i][POS_CSV_BODY_ONFRAME] > listCenter[j][POS_CSV_BODY_ONFRAME])
                        {
                            List<long> tmpLst = new List<long>(listCenter[i]);
                            listCenter[i] = listCenter[j];
                            listCenter[j] = tmpLst;
                        }
                    }
                }
                long nOldR = 0;
                long nOldL = 0;
                for (int i = 0; i < listCenter.Count; i++)
                {
                    if (listCenter[i][POS_CSV_BODY_R] != 0)
                        nOldR = listCenter[i][POS_CSV_BODY_R];
                    if (listCenter[i][POS_CSV_BODY_L] != 0)
                        nOldL = listCenter[i][POS_CSV_BODY_L];
                    if ((nOldR == 0) || (nOldL == 0))
                    {
                        listCenter[i][POS_CSV_BODY_BOTHHANDS] = nBodyCenterPos;
                    }
                    else
                    {
                        listCenter[i][POS_CSV_BODY_BOTHHANDS] = (nOldR + nOldL) / 2;
                        listCenter[i][POS_CSV_BODY_R] = nOldR;
                        listCenter[i][POS_CSV_BODY_L] = nOldL;
                    }
                }
                int iOvCheckCnt = 1;
                while (true) // Remove the same keyframe
                {
                    if (listCenter[iOvCheckCnt - 1][POS_CSV_BODY_ONFRAME] == listCenter[iOvCheckCnt][POS_CSV_BODY_ONFRAME])
                    {
                        if (listCenter[iOvCheckCnt - 1][POS_CSV_BODY_R] < listCenter[iOvCheckCnt][POS_CSV_BODY_R])
                            listCenter[iOvCheckCnt - 1][POS_CSV_BODY_R] = listCenter[iOvCheckCnt][POS_CSV_BODY_R];
                        if (listCenter[iOvCheckCnt - 1][POS_CSV_BODY_L] < listCenter[iOvCheckCnt][POS_CSV_BODY_L])
                            listCenter[iOvCheckCnt - 1][POS_CSV_BODY_L] = listCenter[iOvCheckCnt][POS_CSV_BODY_L];
                        listCenter.RemoveAt(iOvCheckCnt);
                    }
                    else
                    {
                        iOvCheckCnt++;
                    }
                    if (iOvCheckCnt >= listCenter.Count)
                        break;
                }
                // feature point extraction
                // type1: threshold variant, time interval, measure head, ...
                int iLimitFrames = (int)(Scene.KeyFramePerSec / 3.0f * 2.0f);// [frame]
                //float fThAlwaysGenCenterW = 1.0f;
                //float fThGenCenterW = 0.5f;
                //List<List<long>> listCenterLmtd = new List<List<long>>();
                //listCenterLmtd.Add(listCenter[POS_CSV_BODY_ONFRAME]);
                //for (int i = 1; i < listCenter.Count; i++)
                //{
                //    float fMean = 0.0f;
                //    for (int j = i - 1; j < Math.Min(i + 3, listCenter.Count); j++)
                //        fMean += listCenter[j][POS_CSV_BODY_BOTHHANDS];
                //    fMean /= 4.0f;
                //    float fVar = 0.0f;
                //    for (int j = i - 1; j < Math.Min(i + 3, listCenter.Count); j++)
                //    {
                //        float fTmp = listCenter[j][POS_CSV_BODY_BOTHHANDS] - fMean;
                //        fVar += fTmp * fTmp;
                //    }
                //    fVar = (float)Math.Pow(fVar / 4.0f, 0.5f);
                //    if (fVar > fThAlwaysGenCenterW)
                //    {
                //        listCenterLmtd.Add(listCenter[i]);
                //    }
                //    else
                //    {
                //        if ((fVar > fThGenCenterW) && (listCenter[Math.Min(i + 2, listCenter.Count - 1)][0] - listCenter[i - 1][0] > iLimitFrames))
                //            listCenterLmtd.Add(listCenter[i]);
                //    }
                //}

                // type2. Hold min-max
                List<List<long>> listCenterInterporate = new List<List<long>>();
                long nTstCur = 0;
                long nPreCenter = 0;
                foreach (List<long> item in listCenter)
                {
                    if (listCenterInterporate.Count == 0)
                    {
                        nTstCur = item[0];
                        nPreCenter = item[POS_CSV_BODY_BOTHHANDS];
                        listCenterInterporate.Add(new List<long> { nTstCur, nPreCenter });
                    }
                    else
                    {
                        while (nTstCur < item[0])
                        {
                            nTstCur++;
                            if (nTstCur < item[0])
                                listCenterInterporate.Add(new List<long> { nTstCur, nPreCenter });
                        }
                        nTstCur = item[0];
                        nPreCenter = item[POS_CSV_BODY_BOTHHANDS];
                        listCenterInterporate.Add(new List<long> { nTstCur, nPreCenter });
                    }
                }
                // select min-max
                List<long> listCenterMinMax = new List<long>();// loop buffer, nUpperBodyMinMaxQueueSize size
                List<List<long>> listCenterLmtd = new List<List<long>>();
                long nCenterCnt = listCenterInterporate[0][0];
                for (int iCC = 0; iCC < (int)Math.Min(nUpperBodyMinMaxQueueSize, listCenterInterporate.Count); iCC++)
                {
                    listCenterMinMax.Add(listCenterInterporate[iCC][POS_CSV_BODY_BOTHHANDS]);
                }
                for (int iCC = 0; iCC < (int)listCenterInterporate.Count; iCC++)
                {
                    if (listCenterInterporate[iCC][POS_CSV_BODY_BOTHHANDS] >= 64)
                    {
                        listCenterLmtd.Add(new List<long> { listCenterInterporate[iCC][0], listCenterMinMax.Max(), 0 });
                    }
                    else
                    {
                        listCenterLmtd.Add(new List<long> { listCenterInterporate[iCC][0], listCenterMinMax.Min(), 0 });
                    }
                    if (nUpperBodyMinMaxQueueSize / 2 <= iCC && iCC < listCenterInterporate.Count - nUpperBodyMinMaxQueueSize)
                    {
                        listCenterMinMax.Add(listCenterInterporate[iCC + (int)(nUpperBodyMinMaxQueueSize / 2)][POS_CSV_BODY_BOTHHANDS]);
                        listCenterMinMax.RemoveAt(0);
                    }
                }
                // Reduce the same keyframes
                iOvCheckCnt = 1;
                while (true) // Remove the same keyframe
                {
                    if (listCenterLmtd[iOvCheckCnt - 1][POS_CSV_BODY_BOTHHANDS] == listCenterLmtd[iOvCheckCnt][POS_CSV_BODY_BOTHHANDS])
                    {
                        listCenterLmtd.RemoveAt(iOvCheckCnt);
                    }
                    else
                    {
                        iOvCheckCnt++;
                    }
                    if (iOvCheckCnt >= listCenterLmtd.Count)
                        break;
                }
                // Reduce continuous keyframe in min keyframe rate
                iOvCheckCnt = 1;
                while (true)
                {
                    if ((listCenterLmtd[iOvCheckCnt][0] - listCenterLmtd[iOvCheckCnt - 1][0] < nUpperBodySuppressInterval) &&
                        (listCenterLmtd[iOvCheckCnt + 1][0] - listCenterLmtd[iOvCheckCnt][0] < nUpperBodySuppressInterval))
                    {
                        listCenterLmtd.RemoveAt(iOvCheckCnt);
                    }
                    else
                    {
                        iOvCheckCnt++;
                    }
                    if (iOvCheckCnt >= listCenterLmtd.Count - 1)
                        break;
                }
                for (iOvCheckCnt = 1; iOvCheckCnt < listCenterLmtd.Count - 1; iOvCheckCnt++)
                {
                    if (listCenterLmtd[iOvCheckCnt][POS_CSV_BODY_BOTHHANDS] - listCenterLmtd[iOvCheckCnt - 1][POS_CSV_BODY_BOTHHANDS] > 0)
                    {
                        if (listCenterLmtd[iOvCheckCnt + 1][POS_CSV_BODY_BOTHHANDS] - listCenterLmtd[iOvCheckCnt][POS_CSV_BODY_BOTHHANDS] > 0)
                            listCenterLmtd[iOvCheckCnt][2] = 1;//slope
                    }
                    else
                    {
                        if (listCenterLmtd[iOvCheckCnt + 1][POS_CSV_BODY_BOTHHANDS] - listCenterLmtd[iOvCheckCnt][POS_CSV_BODY_BOTHHANDS] < 0)
                            listCenterLmtd[iOvCheckCnt][2] = -1;//slope
                    }
                }
                iOvCheckCnt = 1;
                while (true) // Add middle point when interval has "nUpperBodySuppressInterval * 2" key frames.
                {
                    if (iOvCheckCnt >= listCenterLmtd.Count)// check before insert method.
                        break;
                    if (listCenterLmtd[iOvCheckCnt][0] - listCenterLmtd[iOvCheckCnt - 1][0] > nUpperBodySuppressInterval * 2)
                    {
                        listCenterLmtd.Insert(iOvCheckCnt, new List<long>() { (listCenterLmtd[iOvCheckCnt - 1][0] + listCenterLmtd[iOvCheckCnt][0]) / 2, listCenterLmtd[iOvCheckCnt - 1][1], listCenterLmtd[iOvCheckCnt - 1][2] * 2 });
                        iOvCheckCnt += 2;
                    }
                    else
                    {
                        iOvCheckCnt += 1;
                    }
                }
                //Encoding enc = Encoding.GetEncoding("Shift_JIS");
                //System.IO.StreamWriter writer = new System.IO.StreamWriter("C:/Users/sekiwa/Documents/Visual Studio 2013/Projects/MMMPlugInSekiwa/PianoMotionCreateMidi/_wrist.csv", true, enc);
                //writer.WriteLine("\nonframe,center,R,L\n");
                //writer.WriteLine(AutoGenPolyphonicPianoFingerings.DebugListToString(listCenterLmtd));
                //writer.Close();

                // 閾値以上でキーフレーム登録, 変化点でキーフレーム登録
                // モデルによって要調整. とりあえずのテスト値
                long nMaxMoveCenter = 12 * 2;// 1オクターブ * 2
                long nMinMoveCenter = 6;
                float fMaxRadCenterWx = 0.06f / 2.0f;// 3.48[deg] / 2
                float fMaxRadCenterWz = 0.20f / 1.0f;// 11.4[deg] / 1
                float fMaxRadUpperWx = 0.10f;// 5.73[deg]
                float fMaxRadUpperWy = 0.10f;
                float fMaxRadUpperWz = -0.01f;
                float fMaxRadNecky = -0.10f;
                Vector3 vecC0 = new Vector3(0.0f, 0.0f, 0.0f);
                Quaternion quatC0 = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
                Quaternion quatUB10 = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
                Quaternion quatUB20 = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
                Quaternion quatN0 = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
                if (Scene.ActiveModel.Bones[BONE_CENTER_W] != null)
                {
                    foreach (MotionLayer layer in Scene.ActiveModel.Bones[BONE_CENTER_W].Layers)
                    {
                        MotionFrameData data0 = layer.Frames.GetFrame(0);
                        vecC0 = data0.Position;
                        quatC0 = data0.Quaternion;
                        break;
                    }
                }
                if (Scene.ActiveModel.Bones[BONE_UPPERBODY1_W] != null)
                {
                    foreach (MotionLayer layer in Scene.ActiveModel.Bones[BONE_UPPERBODY1_W].Layers)
                    {
                        MotionFrameData data0 = layer.Frames.GetFrame(0);
                        quatUB10 = data0.Quaternion;
                        break;
                    }
                }
                if (Scene.ActiveModel.Bones[BONE_UPPERBODY2_W] != null)
                {
                    foreach (MotionLayer layer in Scene.ActiveModel.Bones[BONE_UPPERBODY2_W].Layers)
                    {
                        MotionFrameData data0 = layer.Frames.GetFrame(0);
                        quatUB20 = data0.Quaternion;
                        break;
                    }
                }
                if (Scene.ActiveModel.Bones[BONE_NECK] != null)
                {
                    foreach (MotionLayer layer in Scene.ActiveModel.Bones[BONE_NECK].Layers)
                    {
                        MotionFrameData data0 = layer.Frames.GetFrame(0);
                        quatN0 = data0.Quaternion;
                        break;
                    }
                }
                float fMoveCenterX = vecBoneCenterWMove.X * vecBoneCenterWMove.X + vecBoneCenterWMove.Y * vecBoneCenterWMove.Y + vecBoneCenterWMove.Z * vecBoneCenterWMove.Z;
                fMoveCenterX = (float)Math.Pow(fMoveCenterX, 0.5f) * 1.0f;//白鍵1つ分
                foreach (List<long> item in listCenterLmtd)
                {
                    Vector3 vecC1 = new Vector3(0.0f, 0.0f, 0.0f);
                    Quaternion quatC1 = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
                    Quaternion quatUB11 = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
                    Quaternion quatUB21 = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
                    Quaternion quatN1 = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
                    vecC1 = vecC0;
                    if (item[1] < nBodyCenterPos - nMinMoveCenter)//低音側に移動 X+,Rx-,Ry-,Rz-
                    {
                        float fRate = ((nBodyCenterPos - nMinMoveCenter) - item[1] * 1.0f) / (nMaxMoveCenter - nMinMoveCenter);
                        if (item[1] < nBodyCenterPos - nMaxMoveCenter)
                            fRate = 1.0f;
                        vecC1.X += fMoveCenterX * fRate * 1.0f;
                        Vector3 vecS1 = new Vector3(1.0f, 0.0f, 0.0f);
                        Vector3 vecS2 = new Vector3(0.0f, 0.0f, 1.0f);
                        Quaternion quatCS1 = Quaternion.RotationAxis(vecS1, fMaxRadCenterWx * -1.0f * fRate);
                        Quaternion quatCS2 = Quaternion.RotationAxis(vecS2, fMaxRadCenterWz * -1.0f * fRate);
                        Quaternion quatCS3 = Quaternion.Multiply(quatCS1, quatCS2);
                        quatC1 = Quaternion.Multiply(quatC0, quatCS3);
                        
                        Vector3 vecU1 = new Vector3(1.0f, 0.0f, 0.0f);
                        Vector3 vecU2 = new Vector3(0.0f, 1.0f, 0.0f);
                        Vector3 vecU3 = new Vector3(0.0f, 0.0f, 1.0f);
                        Quaternion quatNU1 = Quaternion.RotationAxis(vecU1, fMaxRadUpperWx * -0.5f * fRate);
                        Quaternion quatNU2 = Quaternion.RotationAxis(vecU2, fMaxRadUpperWy * -0.5f * fRate);
                        Quaternion quatNU3 = Quaternion.RotationAxis(vecU3, fMaxRadUpperWz * -1.0f * fRate);
                        Quaternion quatNU4 = Quaternion.Multiply(quatNU1, quatNU2);
                        Quaternion quatNU5 = Quaternion.Multiply(quatNU4, quatNU3);
                        quatUB11 = Quaternion.Multiply(quatUB10, quatNU5);

                        Quaternion quatNU6 = Quaternion.RotationAxis(vecU3, fMaxRadUpperWz * 0.5f * fRate);
                        Quaternion quatNU7 = Quaternion.Multiply(quatNU1, quatNU2);
                        Quaternion quatNU8 = Quaternion.Multiply(quatNU7, quatNU6);
                        quatUB21 = Quaternion.Multiply(quatUB20, quatNU8);

                        Vector3 _vecN1 = new Vector3(0.0f, 1.0f, 0.0f);
                        Quaternion quatNN1 = Quaternion.RotationAxis(_vecN1, fMaxRadNecky * fRate * 1.0f);
                        quatN1 = Quaternion.Multiply(quatN0, quatNN1);
#if DEBUG
                        //Log.Debug("Quaternion {0}|{1}|{2}", _quatUB11.ToString(), _quatUB11.ToString(), _quatUB21.ToString());
#endif
                    }
                    else if (item[1] > nBodyCenterPos + nMinMoveCenter)//高音側に移動 X+,Rx-,Ry+,Rz+
                    {
                        float fRate = (item[1] * 1.0f - (nBodyCenterPos + nMinMoveCenter)) / (nMaxMoveCenter - nMinMoveCenter);
                        if (item[1] > nBodyCenterPos + nMaxMoveCenter)
                            fRate = 1.0f;
                        vecC1.X -= fMoveCenterX * fRate * 1.0f;
                        Vector3 vecS1 = new Vector3(1.0f, 0.0f, 0.0f);
                        Vector3 vecS2 = new Vector3(0.0f, 0.0f, 1.0f);
                        Quaternion quatCS1 = Quaternion.RotationAxis(vecS1, fMaxRadCenterWx * -1.0f * fRate);
                        Quaternion quatCS2 = Quaternion.RotationAxis(vecS2, fMaxRadCenterWz * fRate);
                        Quaternion quatCS3 = Quaternion.Multiply(quatCS1, quatCS2);
                        quatC1 = Quaternion.Multiply(quatC0, quatCS3);

                        Vector3 vecU1 = new Vector3(1.0f, 0.0f, 0.0f);
                        Vector3 vecU2 = new Vector3(0.0f, 1.0f, 0.0f);
                        Vector3 vecU3 = new Vector3(0.0f, 0.0f, 1.0f);
                        Quaternion quatNU1 = Quaternion.RotationAxis(vecU1, fMaxRadUpperWx * -0.5f * fRate);
                        Quaternion quatNU2 = Quaternion.RotationAxis(vecU2, fMaxRadUpperWy * 0.5f * fRate);
                        Quaternion quatNU3 = Quaternion.RotationAxis(vecU3, fMaxRadUpperWz * 1.0f * fRate);
                        Quaternion quatNU4 = Quaternion.Multiply(quatNU1, quatNU2);
                        Quaternion quatNU5 = Quaternion.Multiply(quatNU4, quatNU3);
                        quatUB11 = Quaternion.Multiply(quatUB10, quatNU5);
                        Quaternion quatNU6 = Quaternion.RotationAxis(vecU3, fMaxRadUpperWz * -0.5f * fRate);
                        Quaternion quatNU7 = Quaternion.Multiply(quatNU1, quatNU2);
                        Quaternion quatNU8 = Quaternion.Multiply(quatNU7, quatNU6);
                        quatUB21 = Quaternion.Multiply(quatUB20, quatNU8);

                        Vector3 _vecN1 = new Vector3(0.0f, 1.0f, 0.0f);
                        Quaternion quatNN1 = Quaternion.RotationAxis(_vecN1, fMaxRadNecky * -1.0f * fRate);
                        quatN1 = Quaternion.Multiply(quatN0, quatNN1);
                        //quatN1 = Quaternion.Add(quatN1, Quaternion.RotationYawPitchRoll(fMaxRadNecky * fRate, 0.0f, 0.0f));
                    }
                    else
                    {
                        vecC1 = vecC0;
                        quatC1 = quatC0;
                        quatUB11 = quatUB10;
                        quatUB21 = quatUB20;
                        quatN1 = quatN0;
                    }
#if DEBUG
                    //Log.Debug(", {0}{1},{2},{3},{4}", DebugListToString(item), vecC1.ToString(), quatC1.ToString(), quatUB11.ToString(), quatUB21.ToString());
#endif
                    if (Scene.ActiveModel.Bones[BONE_CENTER_W] != null)
                    {
                        foreach (MotionLayer layer in Scene.ActiveModel.Bones[BONE_CENTER_W].Layers)
                        {
                            MotionFrameData data0 = layer.Frames.GetFrame(0);
                            data0.FrameNumber = item[0] - 1;
                            data0.Position = vecC1;
                            data0.Quaternion = quatC1;
                            InterpolatePoint pt1 = new InterpolatePoint(0, 32);
                            InterpolatePoint pt2 = new InterpolatePoint(96, 127);
                            if (Math.Abs(item[2]) == 1)
                            {
                                pt1.X = 32;
                                pt1.Y = 0;
                            }
                            data0.InterpolRA = data0.InterpolXA = data0.InterpolYA = data0.InterpolZA = pt1;
                            data0.InterpolRB = data0.InterpolXB = data0.InterpolYB = data0.InterpolZB = pt2;
                            layer.Frames.AddKeyFrame(data0);
                            break;
                        }
                    }
                    if (Scene.ActiveModel.Bones[BONE_UPPERBODY1_W] != null)
                    {
                        foreach (MotionLayer layer in Scene.ActiveModel.Bones[BONE_UPPERBODY1_W].Layers)
                        {
                            MotionFrameData data0 = layer.Frames.GetFrame(0);
                            data0.FrameNumber = item[0] - 2;
                            data0.Quaternion = quatUB11;
                            InterpolatePoint pt1 = new InterpolatePoint(0, 32);
                            InterpolatePoint pt2 = new InterpolatePoint(96, 127);
                            if (Math.Abs(item[2]) == 1)
                            {
                                pt1.X = 32;
                                pt1.Y = 0;
                            }
                            data0.InterpolRA = pt1;
                            data0.InterpolRB = pt2;
                            layer.Frames.AddKeyFrame(data0);
                            break;
                        }
                    }
                    if (Scene.ActiveModel.Bones[BONE_UPPERBODY2_W] != null)
                    {
                        foreach (MotionLayer layer in Scene.ActiveModel.Bones[BONE_UPPERBODY2_W].Layers)
                        {
                            MotionFrameData data0 = layer.Frames.GetFrame(0);
                            data0.FrameNumber = item[0] - 1;
                            data0.Quaternion = quatUB21;
                            InterpolatePoint pt1 = new InterpolatePoint(0, 32);
                            InterpolatePoint pt2 = new InterpolatePoint(96, 127);
                            if (Math.Abs(item[2]) == 1)
                            {
                                pt1.X = 32;
                                pt1.Y = 0;
                            }
                            data0.InterpolRA = pt1;
                            data0.InterpolRB = pt2;
                            layer.Frames.AddKeyFrame(data0);
                            break;
                        }
                    }
                    if (Scene.ActiveModel.Bones[BONE_NECK] != null)
                    {
                        foreach (MotionLayer layer in Scene.ActiveModel.Bones[BONE_NECK].Layers)
                        {
                            MotionFrameData data0 = layer.Frames.GetFrame(0);
                            data0.FrameNumber = item[0];// -2;
                            data0.Quaternion = quatN1;
                            InterpolatePoint pt1 = new InterpolatePoint(0, 32);
                            InterpolatePoint pt2 = new InterpolatePoint(96, 127);
                            if (Math.Abs(item[2]) == 1)
                            {
                                pt1.X = 32;
                                pt1.Y = 0;
                            }
                            data0.InterpolRA = pt1;
                            data0.InterpolRB = pt2;
                            layer.Frames.AddKeyFrame(data0);
                            break;
                        }
                    }
                }
                // 視線IK対応
                // 左右で分散の大きいほうに目線を送る, 閾値以下ならデフォルト位置に設定する
                // variantの大きい位置, 一定以上の時間が開いた, 小節の先頭(実装難しい)
                float fThAlwaysLineSightVariant = 1.6f;
                float fThLineSightVariant = 0.8f;
                List<List<long>> listCenterLineSight = new List<List<long>>();
                for (int i = 1; i < listCenter.Count; i++)
                {
                    if (listCenter[i][POS_CSV_BODY_R] == 0 || listCenter[i][POS_CSV_BODY_L] == 0)
                        continue;
                    float fMean = 0.0f;
                    for (int j = i - 1; j < Math.Min(i + 3, listCenter.Count); j++)
                        fMean += listCenter[j][POS_CSV_BODY_R];
                    fMean /= 4.0f;
                    float fVarR = 0.0f;
                    for (int j = i - 1; j < Math.Min(i + 3, listCenter.Count); j++)
                    {
                        float fTmp = listCenter[j][POS_CSV_BODY_R] - fMean;
                        fVarR += fTmp * fTmp;
                    }
                    fVarR = (float)Math.Pow(fVarR / 4.0f, 0.5f);
                    fMean = 0.0f;
                    for (int j = i - 1; j < Math.Min(i + 3, listCenter.Count); j++)
                        fMean += listCenter[j][POS_CSV_BODY_L];
                    fMean /= 4.0f;
                    float fVarL = 0.0f;
                    for (int j = i - 1; j < Math.Min(i + 3, listCenter.Count); j++)
                    {
                        float fTmp = listCenter[j][POS_CSV_BODY_L] - fMean;
                        fVarL += fTmp * fTmp;
                    }
                    fVarL = (float)Math.Pow(fVarL / 4.0f, 0.5f);
                    int iIkRL = POS_CSV_BODY_R;
                    if (fVarR < fVarL)
                        iIkRL = POS_CSV_BODY_L;
                    float fMaxRL = Math.Max(fVarR, fVarL);
                    if (fMaxRL > fThAlwaysLineSightVariant)
                    {
                        listCenterLineSight.Add(new List<long>() { listCenter[i][0], listCenter[i][iIkRL] });
#if DEBUG
                        //Log.Debug(", {0}{1}, {2}", DebugListToString(listCenterLineSight.Last()), fVarR, fVarL);
#endif
                    }
                    else
                    {
                        if ((fMaxRL > fThLineSightVariant) &&
                            (listCenter[Math.Min(i + 2, listCenter.Count - 1)][0] - listCenter[i - 1][0] > iLimitFrames))
                        {
                            listCenterLineSight.Add(new List<long>() { listCenter[i][0], listCenter[i][iIkRL] });
#if DEBUG
                            //Log.Debug(", {0}{1}, {2}", DebugListToString(listCenterLineSight.Last()), fVarR, fVarL);
#endif
                        }
                    }
                }
                // 視線IKを貼り付け
                Vector3 vecLineSight0 = new Vector3(0.0f, 0.0f, 0.0f);
                float fLineSightCorrection = 0.8f;
                if (Scene.ActiveModel.Bones[BONE_LINESIGHT_IK] != null)
                {
                    foreach (MotionLayer layer in Scene.ActiveModel.Bones[BONE_LINESIGHT_IK].Layers)
                    {
                        MotionFrameData data0 = layer.Frames.GetFrame(0);
                        vecLineSight0 = data0.Position;
                        Vector3 vecPrevLineSight = new Vector3(0.0f, 0.0f, 0.0f);
                        vecPrevLineSight = data0.Position;

                        fMoveCenterX = vecBoneCenterWMove.X * vecBoneCenterWMove.X + vecBoneCenterWMove.Y * vecBoneCenterWMove.Y + vecBoneCenterWMove.Z * vecBoneCenterWMove.Z;
                        fMoveCenterX = (float)Math.Pow(fMoveCenterX, 0.5f);
                        foreach (List<long> item in listCenterLineSight)
                        {
                            if (item[0] - 4 < 0)
                                continue;

                            Vector3 vecLineSight1 = new Vector3(0.0f, 0.0f, 0.0f);
                            vecLineSight1 = vecLineSight0;
                            long nMagLineSight = item[1] - nBodyCenterPos;
                            //vecLineSight1.X -= fMoveCenterX * nMagLineSight;
                            vecLineSight1.X += vecBoneCenterWMove.X * nMagLineSight * fLineSightCorrection;
                            vecLineSight1.Y += vecBoneCenterWMove.Y * nMagLineSight * fLineSightCorrection;
                            vecLineSight1.Z += vecBoneCenterWMove.Z * nMagLineSight * fLineSightCorrection;

                            data0.FrameNumber = item[0] - 2;
                            data0.Position = vecLineSight1;
                            InterpolatePoint pt1 = new InterpolatePoint(32, 0);//(0, 48);
                            InterpolatePoint pt2 = new InterpolatePoint(96, 127);//(127, 80);
                            data0.InterpolRA = data0.InterpolXA = data0.InterpolYA = data0.InterpolZA = pt1;
                            data0.InterpolRB = data0.InterpolXB = data0.InterpolYB = data0.InterpolZB = pt2;
                            layer.Frames.AddKeyFrame(data0);

                            data0.FrameNumber = item[0] - 4;
                            data0.Position = vecPrevLineSight;
                            layer.Frames.AddKeyFrame(data0);

                            vecPrevLineSight = vecLineSight1;
                        }
                        break;
                    }
                }
            }

            progressForm.UpdateProgressBar(100, "");
            progressForm.Dispose();

            string strFingeringFile = Path.GetFileName(strMidiFile);
            string strFingeringExt = Path.GetExtension(strFingeringFile);
            string strFingeringCsv = strFingeringFile.Substring(0, strFingeringFile.Length - strFingeringExt.Length);
            strFingeringCsv = strFingeringCsv + listRL[0] + ".csv";

            if (Scene.Language == "ja")
            {
                string strMsg = "モーション生成が完了しました.\r\n";
                if (bSaveFingeringFile)
                    strMsg += "運指データは " + strFingeringCsv + listRL[0] + ".csv, " + strFingeringCsv + listRL[1] + ".csv に保存しました。";
                MessageBox.Show(ApplicationForm, strMsg, "登録完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(ApplicationForm, "Complete", "Create piano motion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// プラグイン破棄処理
        /// もし解放しないといけないオブジェクトがある場合は、ここで解放してください。
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// 与えられた基準距離(0～, 整数)と、手首のnoteと指のnoteの距離との差分(白鍵間＝1.0f)を取得する
        /// </summary>
        /// <param name="iFingerDistance">基準距離(0～, 整数)</param>
        /// <param name="iWristeNote">手首のnote</param>
        /// <param name="iFingerNote">指のnote</param>
        /// <returns>距離(白鍵間＝1.0f)</returns>
        public static float GetFingerCorrection(int iFingerDistance, int iWristeNote, int iFingerNote)
        {
            float fFullTwice = 0.0f;// 完全2度=1.0f, 1 octave=7.0f
            int iDist = AutoGenPolyphonicPianoFingerings.GenerateDistance(iWristeNote, iFingerNote);
            fFullTwice = (iDist - iFingerDistance) / 2.0f;
            return fFullTwice;
        }

        /// <summary>
        /// 白鍵で左右に1単位で移動したときのMIDI note値を取得する
        /// </summary>
        /// <param name="nWhiteNote">white key's MIDI note number</param>
        /// <param name="iWhiteShift">right/left move steps in white keys</param>
        /// <returns>moved MIDI note number</returns>
        public static long ShiftWhiteKey(long nWhiteNote, int iWhiteShift)
        {
            long nLocalNote = nWhiteNote;
            int iWMod = iWhiteShift % 7;
            int iWDiv = iWhiteShift / 7;
            if (iWhiteShift < 0)
            {
                iWMod = (7 - (iWhiteShift * -1) % 7) % 7;
                iWDiv = iWhiteShift / 7;
                nLocalNote = nLocalNote - 12;
            }

            int[] arrayPos = null;
            long nMode = nLocalNote % 12;
            if (nMode == 0) { arrayPos = new int[] { 0, 2, 4, 5, 7, 9, 11 }; }// C
            else if (nMode == 1) { arrayPos = new int[] { 0, 2, 4, 5, 7, 9, 11 }; }// dummy
            else if (nMode == 2) { arrayPos = new int[] { 0, 2, 3, 5, 7, 9, 10 }; }// D
            else if (nMode == 3) { arrayPos = new int[] { 0, 2, 3, 5, 7, 9, 10 }; }// dummy
            else if (nMode == 4) { arrayPos = new int[] { 0, 1, 3, 5, 7, 8, 10 }; }// E
            else if (nMode == 5) { arrayPos = new int[] { 0, 2, 4, 6, 7, 9, 11 }; }// F
            else if (nMode == 6) { arrayPos = new int[] { 0, 2, 4, 6, 7, 9, 11 }; }// dummy
            else if (nMode == 7) { arrayPos = new int[] { 0, 2, 4, 5, 7, 9, 10 }; }// G
            else if (nMode == 8) { arrayPos = new int[] { 0, 2, 4, 5, 7, 9, 10 }; }// dummy
            else if (nMode == 9) { arrayPos = new int[] { 0, 2, 3, 5, 7, 8, 10 }; }// A
            else if (nMode == 10) { arrayPos = new int[] { 0, 2, 3, 5, 7, 8, 10 }; }// dummy
            else if (nMode == 11) { arrayPos = new int[] { 0, 1, 3, 5, 6, 8, 10 }; }// B

            return nLocalNote + arrayPos[iWMod] + iWDiv * 12;
        }
    }
}
