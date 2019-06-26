using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using NextMidi.Data.Domain;
using NextMidi.Data.Track;
using NextMidi.DataElement;
using NextMidi.DataElement.MetaData;
using NextMidi.Filing.Midi;
using NextMidi.MidiPort.Input.Core;
using NextMidi.MidiPort.Output;
using NextMidi.MidiPort.Output.Core;
using NextMidi.Time;

namespace PianoMotionCreateMidi
{
    public static class CommonConstants
    {
        /// <summary>
        /// MMD frame-paste-to1
        /// </summary>
        public const int CSV_COL_ON_FRAME = 0;
        /// <summary>
        /// MMD frame-paste-to2
        /// </summary>
        public const int CSV_COL_OFF_FRAME = 1;
        /// <summary>
        /// midi note
        /// </summary>
        public const int CSV_COL_NOTE = 2;
        /// <summary>
        /// midi velocity
        /// </summary>
        public const int CSV_COL_VEL = 3;
        /// <summary>
        /// finger number
        /// </summary>
        public const int CSV_COL_FINGER = 4;
        /// <summary>
        /// wrist center pos(note)
        /// </summary>
        public const int CSV_COL_WRIST = 5;
        /// <summary>
        /// self position in the list
        /// </summary>
        public const int CSV_COL_POSITION = 6;
        /// <summary>
        /// polyphonic id in the list. 0:single tone, more than 1:set of poly tones (node).
        /// </summary>
        public const int CSV_COL_POLYPHONIC = 7;
        /// <summary>
        /// tie flag
        /// </summary>
        public const int CSV_COL_DIVIDE_TIE = 8;
        /// <summary>
        /// a value of tie flag. This show not to assign a fingering.
        /// </summary>
        public const int LIST_POS_TIE = -1;
    }

    public static class MidiDataHolder
    {
        public const int NO_CONTROL_EVENT = 0;
        public const int CONTROL_EVENT_HOLD = 1;
        public const int CONTROL_EVENT_SUSTAIN = 2;
        public const int CONTROL_EVENT_SOFT = 3;
        public static int GetMidiDataInfo(string strMidiFile, int iTrack, float keyFramePerSec, long nMMDPasteStartPos, long nPreAction, ref List<List<long>> listMidiFingering, int iReadType = 0)
        {
            if (!File.Exists(strMidiFile))
                return 3;

            // get tempo information
            tempoHolder myTempo = new tempoHolder(strMidiFile);
            if (myTempo == null)
                return 2;

            // - Reed MIDI file
            var midiData = MidiReader.ReadFrom(strMidiFile, Encoding.GetEncoding("shift-jis"));
            if (iReadType == 0)
            {
                if (iTrack >= 0)
                {
                    if (iTrack >= midiData.Tracks.Count)
                    {
                        return 1;
                    }
                    //double dMillisecToFrame = Scene.KeyFramePerSec / 1000.0;
                    double dMillisecToFrame = keyFramePerSec / 1000.0;
                    var track = midiData.Tracks[iTrack];
                    int iMidiFingeringCnt = 0;
                    // long nPreviousNoteTickCheck = 99999;
                    foreach (var note in track.GetData<NoteEvent>())
                    {
                        // - 0:frame-paste-to1, 1:frame-paste-to2, 2:note, 3:vel, 4:finger, 5:wrist center pos, 6:self array ID, 7: Polyphonic_ID
                        long nMillisec = myTempo.GetGateOnStartTimeMilliSec(note.Tick);
                        long nFramePaste = (long)(nMillisec * dMillisecToFrame) + nMMDPasteStartPos - nPreAction;
                        long nMillsecHold = myTempo.GetGateOnStartTimeMilliSec(note.Tick + note.Gate);
                        long nFramePasteHold = (long)(nMillsecHold * dMillisecToFrame) + nMMDPasteStartPos + nPreAction;
                        // To correct fog sound (keyframe of on and offs).
                        nFramePasteHold -= 1;

                        listMidiFingering.Add(new List<long>() { nFramePaste, nFramePasteHold, note.Note, note.Velocity, 0, 0, iMidiFingeringCnt, 0, 0 });

                        iMidiFingeringCnt++;
                    }
                }
            }
            else
            {
                byte midiControlNumber = 0x00;
                if (iReadType == CONTROL_EVENT_HOLD)
                    midiControlNumber = 0x40;
                if (iReadType == CONTROL_EVENT_SUSTAIN)
                    midiControlNumber = 0x42;
                if (iReadType == CONTROL_EVENT_SOFT)
                    midiControlNumber = 0x43;
                // if iTrack is less than 0, get all tracks.
                for (int i = 0; i < midiData.Tracks.Count; i++)
                {
                    if ((iTrack >= 0) && (iTrack != i))
                    {
                            continue;
                    }
                    double dMillisecToFrame = keyFramePerSec / 1000.0;
                    var track = midiData.Tracks[i];
                    int iMidiFingeringCnt = 0;
                    foreach (var note in track.GetData<ControlEvent>())
                    {
                        if (note.Number == midiControlNumber)
                        {
                            long nMillisec = myTempo.GetGateOnStartTimeMilliSec(note.Tick);
                            long nFramePaste = (long)(nMillisec * dMillisecToFrame) + nMMDPasteStartPos - nPreAction;
                            if (listMidiFingering.Count > 0 && listMidiFingering.Last()[CommonConstants.CSV_COL_ON_FRAME] == nFramePaste)
                                nFramePaste++;
                            long nFramePasteHold = nFramePaste + 1;
                            //listMidiFingering.Add(new List<long>() { 0, nFramePaste, nFramePasteHold, note.Value });
                            listMidiFingering.Add(new List<long>() { nFramePaste, nFramePasteHold, 0, note.Value, 0, 0, iMidiFingeringCnt, 0, 0 });

                            iMidiFingeringCnt++;
                        }
                    }
                }
            }
            // hold frame modification
            // the same key off and on, change off key one step before
            for (int iMidiCnt = 0; iMidiCnt < listMidiFingering.Count - 1; iMidiCnt++)
            {
                long note = listMidiFingering[iMidiCnt][CommonConstants.CSV_COL_NOTE];
                long onframe = listMidiFingering[iMidiCnt][CommonConstants.CSV_COL_ON_FRAME];
                long offframe = listMidiFingering[iMidiCnt][CommonConstants.CSV_COL_OFF_FRAME];
                for (int iNext = iMidiCnt + 1; iNext < Math.Min(iMidiCnt + 10, listMidiFingering.Count); iNext++)
                {
                    if (listMidiFingering[iNext][CommonConstants.CSV_COL_ON_FRAME] > offframe + 1)
                        continue;
                    if (listMidiFingering[iNext][CommonConstants.CSV_COL_NOTE] == note)
                    {
                        if ((offframe + 2) >= listMidiFingering[iNext][CommonConstants.CSV_COL_ON_FRAME])
                        {
                            if (onframe + 2 < offframe)
                            {
                                listMidiFingering[iMidiCnt][CommonConstants.CSV_COL_OFF_FRAME] = offframe - 2;
                            }
                            else
                            {
                                listMidiFingering[iNext][CommonConstants.CSV_COL_ON_FRAME] = onframe;
                                listMidiFingering[iMidiCnt][CommonConstants.CSV_COL_OFF_FRAME] = listMidiFingering[iNext][CommonConstants.CSV_COL_OFF_FRAME];
                            }
                        }
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Show tracks info
        /// </summary>
        public static void ShowTracksInfoHtml(string fname, ref string strHtml, int iTrack = -1)
        {
            // MIDI ファイルを読み込む
            var midiData = MidiReader.ReadFrom(fname, Encoding.GetEncoding("shift-jis"));

            // MIDI イベント数、ノート数、プログラムチェンジの数、コントロールチェンジの数、メタイベント数、エクスクルーシブイベント数
            strHtml = "<!DOCTYPE html PUBLIC \" -//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\"><html><head><style type=\"text/css\">" +
                "body{font-size: 9pt;text-align: center; padding: 0; margin: 0;}" +
                "table{width:100%;border-collapse: collapse;border: solid 1px;}" +
                "th{text-align: center;color: #444;background-color: #ccc;border-right: 1px solid #ccc;border-bottom: 1px solid #ccc;}" +
                "td{background-color: #fafafa;border-right: 1px solid #ccc;border-bottom: 1px solid #ccc;}" +
                "td.sel{background-color: #faaa8a;border-right: 1px solid #ccc;border-bottom: 1px solid #ccc;}" +
                "</style></head><body>";
            strHtml += "<table><tr><th>Track</th><th>Event</th><th>Note</th><th>Prog</th><th>Control</th><th>Meta</th><th>Exclusive</th><th>Title</th></tr>";

            for (int i = 0; i < midiData.Tracks.Count; i++)
            {
                string strTd = "<td>";
                if (i == iTrack)
                    strTd = "<td class=\"sel\">";
                var track = midiData.Tracks[i];
                strHtml += "<tr>" + strTd +
                    i.ToString() + "</td>" + strTd +
                    track.GetData<MidiEvent>().Count.ToString() + "</td>" + strTd +
                    track.GetData<NoteEvent>().Count.ToString() + "</td>" + strTd +
                    track.GetData<ProgramEvent>().Count.ToString() + "</td>" + strTd +
                    track.GetData<ControlEvent>().Count.ToString() + "</td>" + strTd +
                    track.GetData<MetaEvent>().Count.ToString() + "</td>" + strTd +
                    track.GetData<ExclusiveEvent>().Count.ToString() + "</td>" + strTd +
                    track.GetTitle(i == 0) + "</td></tr>";
            }
            strHtml += "</table></body></html>";
        }

        /// <summary>
        /// Note name to note number (not YAMAHA)
        /// </summary>
        /// <param name="strNote">note name</param>
        /// <returns>note number (integer)</returns>
        public static int Ascii2NoteNumber(string strNote)
        {
            int iNote = -1;
            if (strNote.Length < 1)
            {
                return iNote;
            }
            string name1 = strNote.Substring(0, 1);
            if (name1 == "C")
            {
                if (strNote == "C0") { iNote = 12; }
                else if (strNote == "C1") { iNote = 24; }
                else if (strNote == "C2") { iNote = 36; }
                else if (strNote == "C3") { iNote = 48; }
                else if (strNote == "C4") { iNote = 60; }
                else if (strNote == "C5") { iNote = 72; }
                else if (strNote == "C6") { iNote = 84; }
                else if (strNote == "C7") { iNote = 96; }
                else if (strNote == "C8") { iNote = 108; }
                else if (strNote == "C9") { iNote = 120; }
                else if (strNote == "C0#") { iNote = 13; }
                else if (strNote == "C1#") { iNote = 25; }
                else if (strNote == "C2#") { iNote = 37; }
                else if (strNote == "C3#") { iNote = 49; }
                else if (strNote == "C4#") { iNote = 61; }
                else if (strNote == "C5#") { iNote = 73; }
                else if (strNote == "C6#") { iNote = 85; }
                else if (strNote == "C7#") { iNote = 97; }
                else if (strNote == "C8#") { iNote = 109; }
                else if (strNote == "C0#") { iNote = 13; }
                else if (strNote == "C#1") { iNote = 25; }
                else if (strNote == "C#2") { iNote = 37; }
                else if (strNote == "C#3") { iNote = 49; }
                else if (strNote == "C#4") { iNote = 61; }
                else if (strNote == "C#5") { iNote = 73; }
                else if (strNote == "C#6") { iNote = 85; }
                else if (strNote == "C#7") { iNote = 97; }
                else if (strNote == "C#8") { iNote = 109; }
            }
            else if (name1 == "D")
            {
                if (strNote == "D0") { iNote = 14; }
                else if (strNote == "D1") { iNote = 26; }
                else if (strNote == "D2") { iNote = 38; }
                else if (strNote == "D3") { iNote = 50; }
                else if (strNote == "D4") { iNote = 62; }
                else if (strNote == "D5") { iNote = 74; }
                else if (strNote == "D6") { iNote = 86; }
                else if (strNote == "D7") { iNote = 98; }
                else if (strNote == "D8") { iNote = 110; }
                else if (strNote == "D0#") { iNote = 15; }
                else if (strNote == "D1#") { iNote = 27; }
                else if (strNote == "D2#") { iNote = 39; }
                else if (strNote == "D3#") { iNote = 51; }
                else if (strNote == "D4#") { iNote = 63; }
                else if (strNote == "D5#") { iNote = 75; }
                else if (strNote == "D6#") { iNote = 87; }
                else if (strNote == "D7#") { iNote = 99; }
                else if (strNote == "D8#") { iNote = 111; }
                else if (strNote == "D0#") { iNote = 15; }
                else if (strNote == "D#1") { iNote = 27; }
                else if (strNote == "D#2") { iNote = 39; }
                else if (strNote == "D#3") { iNote = 51; }
                else if (strNote == "D#4") { iNote = 63; }
                else if (strNote == "D#5") { iNote = 75; }
                else if (strNote == "D#6") { iNote = 87; }
                else if (strNote == "D#7") { iNote = 99; }
                else if (strNote == "D#8") { iNote = 111; }
            }
            else if (name1 == "E")
            {
                if (strNote == "E0") { iNote = 16; }
                else if (strNote == "E1") { iNote = 28; }
                else if (strNote == "E2") { iNote = 40; }
                else if (strNote == "E3") { iNote = 52; }
                else if (strNote == "E4") { iNote = 64; }
                else if (strNote == "E5") { iNote = 76; }
                else if (strNote == "E6") { iNote = 88; }
                else if (strNote == "E7") { iNote = 100; }
                else if (strNote == "E8") { iNote = 112; }
            }
            else if (name1 == "F")
            {
                if (strNote == "F0") { iNote = 17; }
                else if (strNote == "F1") { iNote = 29; }
                else if (strNote == "F2") { iNote = 41; }
                else if (strNote == "F3") { iNote = 53; }
                else if (strNote == "F4") { iNote = 65; }
                else if (strNote == "F5") { iNote = 77; }
                else if (strNote == "F6") { iNote = 89; }
                else if (strNote == "F7") { iNote = 101; }
                else if (strNote == "F8") { iNote = 113; }
                else if (strNote == "F0#") { iNote = 18; }
                else if (strNote == "F1#") { iNote = 30; }
                else if (strNote == "F2#") { iNote = 42; }
                else if (strNote == "F3#") { iNote = 54; }
                else if (strNote == "F4#") { iNote = 66; }
                else if (strNote == "F5#") { iNote = 78; }
                else if (strNote == "F6#") { iNote = 90; }
                else if (strNote == "F7#") { iNote = 102; }
                else if (strNote == "F8#") { iNote = 114; }
                else if (strNote == "F0#") { iNote = 18; }
                else if (strNote == "F#1") { iNote = 30; }
                else if (strNote == "F#2") { iNote = 42; }
                else if (strNote == "F#3") { iNote = 54; }
                else if (strNote == "F#4") { iNote = 66; }
                else if (strNote == "F#5") { iNote = 78; }
                else if (strNote == "F#6") { iNote = 90; }
                else if (strNote == "F#7") { iNote = 102; }
                else if (strNote == "F#8") { iNote = 114; }
            }
            else if (name1 == "G")
            {
                if (strNote == "G0") { iNote = 19; }
                else if (strNote == "G1") { iNote = 31; }
                else if (strNote == "G2") { iNote = 43; }
                else if (strNote == "G3") { iNote = 55; }
                else if (strNote == "G4") { iNote = 67; }
                else if (strNote == "G5") { iNote = 79; }
                else if (strNote == "G6") { iNote = 91; }
                else if (strNote == "G7") { iNote = 103; }
                else if (strNote == "G8") { iNote = 115; }
                else if (strNote == "G0#") { iNote = 20; }
                else if (strNote == "G1#") { iNote = 32; }
                else if (strNote == "G2#") { iNote = 44; }
                else if (strNote == "G3#") { iNote = 56; }
                else if (strNote == "G4#") { iNote = 68; }
                else if (strNote == "G5#") { iNote = 80; }
                else if (strNote == "G6#") { iNote = 92; }
                else if (strNote == "G7#") { iNote = 104; }
                else if (strNote == "G8#") { iNote = 116; }
                else if (strNote == "G0#") { iNote = 20; }
                else if (strNote == "G#1") { iNote = 32; }
                else if (strNote == "G#2") { iNote = 44; }
                else if (strNote == "G#3") { iNote = 56; }
                else if (strNote == "G#4") { iNote = 68; }
                else if (strNote == "G#5") { iNote = 80; }
                else if (strNote == "G#6") { iNote = 92; }
                else if (strNote == "G#7") { iNote = 104; }
                else if (strNote == "G#8") { iNote = 116; }
            }
            else if (name1 == "A")
            {
                if (strNote == "A0") { iNote = 21; }
                else if (strNote == "A1") { iNote = 33; }
                else if (strNote == "A2") { iNote = 45; }
                else if (strNote == "A3") { iNote = 57; }
                else if (strNote == "A4") { iNote = 69; }
                else if (strNote == "A5") { iNote = 81; }
                else if (strNote == "A6") { iNote = 93; }
                else if (strNote == "A7") { iNote = 105; }
                else if (strNote == "A8") { iNote = 117; }
                else if (strNote == "A0#") { iNote = 22; }
                else if (strNote == "A1#") { iNote = 34; }
                else if (strNote == "A2#") { iNote = 46; }
                else if (strNote == "A3#") { iNote = 58; }
                else if (strNote == "A4#") { iNote = 70; }
                else if (strNote == "A5#") { iNote = 82; }
                else if (strNote == "A6#") { iNote = 94; }
                else if (strNote == "A7#") { iNote = 106; }
                else if (strNote == "A8#") { iNote = 118; }
                else if (strNote == "A0#") { iNote = 22; }
                else if (strNote == "A#1") { iNote = 34; }
                else if (strNote == "A#2") { iNote = 46; }
                else if (strNote == "A#3") { iNote = 58; }
                else if (strNote == "A#4") { iNote = 70; }
                else if (strNote == "A#5") { iNote = 82; }
                else if (strNote == "A#6") { iNote = 94; }
                else if (strNote == "A#7") { iNote = 106; }
                else if (strNote == "A#8") { iNote = 118; }
            }
            else if (name1 == "B")
            {
                if (strNote == "B0") { iNote = 23; }
                else if (strNote == "B1") { iNote = 35; }
                else if (strNote == "B2") { iNote = 47; }
                else if (strNote == "B3") { iNote = 59; }
                else if (strNote == "B4") { iNote = 71; }
                else if (strNote == "B5") { iNote = 83; }
                else if (strNote == "B6") { iNote = 95; }
                else if (strNote == "B7") { iNote = 107; }
                else if (strNote == "B8") { iNote = 119; }
            }
            return iNote;
        }
    }

    /// <summary>
    /// MIDI tempo event hold class
    /// </summary>
    public class tempoEventBase
    {
        public long nQuoteNoteMicroSec;
        public long nStartTick;
        public long nNextTick;
        public long nPassedTimeMicroSec;
        public long nContinueTick;
        public long nTempo;
        public tempoEventBase(long nUs, long nStart, long nNext, long nPassed, long nVal)
        {
            nQuoteNoteMicroSec = nUs;
            nStartTick = nStart;
            nNextTick = nNext;
            nPassedTimeMicroSec = nPassed;
            nContinueTick = nNextTick - nStartTick;
            nTempo = nVal;
        }
    }

    /// <summary>
    /// tick time to play time transform class
    /// </summary>
    public class tempoHolder
    {
        // memo
        // 1 tick あたりの処理はlong型で処理する
        // シーケンサはdouble型では処理していないっぽい?
        public int iBaseResolution = 0;
        public List<tempoEventBase> tempoList = new List<tempoEventBase>();
        public tempoHolder(string fname)
        {
            // MIDI ファイルを読み込む
            var midiData = MidiReader.ReadFrom(fname, Encoding.GetEncoding("shift-jis"));
            iBaseResolution = midiData.Resolution.Resolution;
            int nTrack = 0;
            if (nTrack >= midiData.Tracks.Count)
            {
                Console.WriteLine("File does not exist");
                return;
            }
#if DEBUG
            //Debug.Print("Track Evnt Note Prog Cont Meta Excl  Title");
            //Debug.Print("----- ---- ---- ---- ---- ---- ----  ----------------");
            //for (int i = 0; i < midiData.Tracks.Count; i++)
            //{
            //    var track = midiData.Tracks[i];

            //    Debug.Print("{0} {1} {2} {3} {4} {5} {6}  {7}",
            //    i.ToString().PadLeft(5),
            //    track.GetData<MidiEvent>().Count.ToString().PadLeft(4),
            //    track.GetData<NoteEvent>().Count.ToString().PadLeft(4),
            //    track.GetData<ProgramEvent>().Count.ToString().PadLeft(4),
            //    track.GetData<ControlEvent>().Count.ToString().PadLeft(4),
            //    track.GetData<MetaEvent>().Count.ToString().PadLeft(4),
            //    track.GetData<ExclusiveEvent>().Count.ToString().PadLeft(4),
            //    track.GetTitle(i == 0));
            //}
            Debug.Print("----- -------- ---- ---- ---- ----------------");
            Debug.Print("                    分母 分子");
            Debug.Print("Track Tick     Num  Note Rhyt RequireToSend");
            Debug.Print("----- -------- ---- ---- ---- ----------------");
            for (int i = 0; i < midiData.Tracks.Count; i++)
            {
                var track = midiData.Tracks[i];
                var metaEvs = track.GetData<NextMidi.DataElement.MetaData.RhythmEvent>();
                if (metaEvs.Count > 0)
                {
                    foreach (var evs in metaEvs)
                    {
                        Debug.Print("{0} {1} {2} {3} {4} {5}",
                            i.ToString().PadLeft(5),
                            evs.Tick.ToString().PadLeft(8),
                            evs.EventNumber.ToString().PadLeft(4),
                            evs.Note.ToString().PadLeft(4),
                            evs.Rhythm.ToString().PadLeft(4),
                            evs.RequireToSend.ToString());
                    }
                }
            }
#endif
            // all tracks tempo search
            for (int i = 0; i < midiData.Tracks.Count; i++)
            {
                var track = midiData.Tracks[i];
                var tempoEvs = track.GetData<TempoEvent>();
                if (tempoEvs.Count > 0)
                {
#if DEBUG
                    Debug.Print("------ ----- -------- -------------------------");
                    Debug.Print("Tick   Tempo MicroSec speed / Oh, tempo.Tempo is integer...");
                    Debug.Print("------ ----- -------- -------------------------");
#endif
                    long nPassedTime = 0;
                    double dPreTickSec = 0.0;
                    long nPreMicroSecond = 0;
                    long nPreTick = 0;
                    long nPreTempo = 0;
                    long nPrePassedTime = 0;
                    foreach (var tempo in tempoEvs)
                    {
                        long nContinueTick = tempo.Tick - nPreTick;
                        nPassedTime += (long)(nContinueTick * dPreTickSec);
                        dPreTickSec = tempo.MicroSecond / 1.0 / midiData.Resolution.Resolution;
#if DEBUG
                        Debug.WriteLine("{0} {1} {2} {3}",
                            tempo.Tick.ToString().PadLeft(6),
                            tempo.Tempo.ToString().PadLeft(5),
                            tempo.MicroSecond.ToString().PadLeft(8),
                            60.0 / (tempo.MicroSecond / 1000000.0));
#endif
                        tempoEventBase _tmp = new tempoEventBase(nPreMicroSecond, nPreTick, tempo.Tick, nPrePassedTime, nPreTempo);
                        if (nPreTick != tempo.Tick)
                        {
                            tempoList.Add(_tmp);
                        }
                        nPreMicroSecond = tempo.MicroSecond;
                        nPreTick = tempo.Tick;
                        nPreTempo = tempo.Tempo;
                        nPrePassedTime = nPassedTime;
                    }
                    tempoEventBase tmp = new tempoEventBase(nPreMicroSecond, nPreTick, Int32.MaxValue - 1, nPrePassedTime, nPreTempo);
                    tempoList.Add(tmp);
                }
            }
#if DEBUG
            //Debug.WriteLine("------------------------------------------------------------");
            //Debug.WriteLine("us/q-note  tick       tick       tmp passed time from top[ms]");
            //Debug.WriteLine("---------- ---------- ---------- --- -----------------------");
            //foreach (tempoEventBase _tmp in tempoList)
            //{
            //    Debug.WriteLine("{0} {1} {2} {3} {4}",
            //        _tmp.nQuoteNoteMicroSec.ToString().PadLeft(10),
            //        _tmp.nStartTick.ToString().PadLeft(10),
            //        _tmp.nNextTick.ToString().PadLeft(10),
            //        _tmp.nTempo.ToString().PadLeft(3),
            //        _tmp.nPassedTimeMicroSec / 1000);
            //}
            //Debug.WriteLine("------------------------------------------------------------");
#endif
        }
        // Tick time to play time from top the sequence[ms]
        // @param iTick -1 shows can't transform
        public long GetGateOnStartTimeMilliSec(long nTick)
        {
            long nStartTimeMilliSec = 0;
            foreach (tempoEventBase _tmp in tempoList)
            {
                if (_tmp.nStartTick <= nTick && nTick < _tmp.nNextTick)
                {
                    nStartTimeMilliSec = (long)(_tmp.nPassedTimeMicroSec +
                        (1.0 * _tmp.nQuoteNoteMicroSec / iBaseResolution) * (nTick - _tmp.nStartTick)
                        ) / 1000;
                    break;
                }
            }
            return nStartTimeMilliSec;
        }
    }
}
