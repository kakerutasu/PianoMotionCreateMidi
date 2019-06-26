using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PianoMotionCreateMidi
{
    /// <summary>
    /// This class generate piano fingerings from a midi file.
    /// The algorithm is followed Alia Al Kasimi's "A simple algorithm for automatic generation of polyphonic piano fingerings" in 2005.
    /// For HMMs and viterbi search, refer to Yuichiro YONEBAYASHI's "Automatic Decision of Piano Fingering Based on Hidden Markov Models" in 2006.
    /// 
    /// programmed by SEKIWA, from 2015-03-21
    /// </summary>
    class AutoGenPolyphonicPianoFingerings
    {
        // variables
        protected string strMidiFile;
        protected long nMMDPasteStartPos;
        protected long nPreAction;

        // Steps to reset for Trellis graph. This sample is setted 100.
        int iMaxTrellisStep = 100;
        // Keyframes to reset for Trellis graph. This sample is setted 1sec(30frame).
        int iResetIntervalFrames = 30;

        public AutoGenPolyphonicPianoFingerings(string strSetMidiFile, long nSetMMDPasteStartPos, long nSetPreAction)
        {
            this.strMidiFile = strSetMidiFile;
            this.nMMDPasteStartPos = nSetMMDPasteStartPos;
            this.nPreAction = nSetPreAction;
        }
        /// <summary>
        /// Generate fingerings. This function don't hold fingerings.
        /// </summary>
        /// <param name="iRL">0:Right, 1:Left</param>
        /// <param name="iTracks">smf track number. [0]:Right, [1]:Left</param>
        /// <param name="keyFramePerSec">MMM's frame rate.</param>
        /// <param name="listMidiFingering">fingerings. </param>
        /// <param name="strMidiCsvFile">smf file name.</param>
        /// <returns>0:success, 1:track error, else:error</returns>
        public int GenFingerings(int iRL, int[] iTracks, float keyFramePerSec, ref List<List<long>> listMidiFingering, string strMidiCsvFile)
        {
            int iRet = MidiDataHolder.GetMidiDataInfo(this.strMidiFile, iTracks[iRL], keyFramePerSec, this.nMMDPasteStartPos, this.nPreAction, ref listMidiFingering);
            if (iRet != 0)
                return iRet;

            // - Generate fingerings
            //   Fingerings algorithm by Dr.Alia Al Kasimi
            //   - algorithm
            //     1. 各ノードにおいて、運指は低音から高音に向かって順に割り当てること
            //     2. 同じ指は音を伸ばしている間使い続けること(指の置き換えは許可しない)
            //     3. 同じ指はひとつの音のみ押すこと
            List<List<NodeInfo>> trellis = new List<List<NodeInfo>>();
            long nOldLastFramePasteTo2 = -1;// Dr.Aliaの論文における, 保持音を和音として処理するための変数
            int iOldLastMidiSeqPos = -1;// Dr.Aliaの論文における, 保持音を和音として処理するための変数
            long nLastFramePasteTo2 = 0;// 現在のノードの発音時間の保持フレーム位置
            long nMidiPolyphonyPairs = 1;// 和音の組を記録するためのID
            int iMidiCnt = 0;
            while (iMidiCnt < listMidiFingering.Count)
            {
                // - ノードの取得(a node of the first note), iMidiCntはインクリメントされることに注意.
                List<List<long>> lstLocalNode = new List<List<long>>();
                GetNodeFromList(ref listMidiFingering, ref iMidiCnt, ref lstLocalNode, ref nMidiPolyphonyPairs, ref nLastFramePasteTo2);

                // - 和音(polyphonic)は低音から高音に並び替えする
                if (lstLocalNode.Count > 1)
                {
                    List<List<long>> lstSorted = new List<List<long>>();
                    while (true)
                    {
                        int nMinPos = 0;
                        long nMinNote = lstLocalNode[0][CommonConstants.CSV_COL_NOTE];
                        for (int i = 0; i < lstLocalNode.Count; i++)
                        {
                            if (nMinNote > lstLocalNode[i][CommonConstants.CSV_COL_NOTE])
                            {
                                nMinPos = i;
                                nMinNote = lstLocalNode[i][CommonConstants.CSV_COL_NOTE];
                            }
                        }
                        lstSorted.Add(lstLocalNode[nMinPos]);
                        lstLocalNode.RemoveAt(nMinPos);
                        if (lstLocalNode.Count == 0)
                            break;
                    }
                    for (int i = 0; i < lstSorted.Count; i++)
                        lstLocalNode.Add(lstSorted[i]);
                }
                // - reduce notes under five.
                //   本当は分散和音にしたら1指で複数音を弾く処理にするべきか？
                //   左手なら高音側から保持音を優先して取り除くべきか？
                for (int i = 0; i < lstLocalNode.Count; i++)
                {
                    if (lstLocalNode[i][CommonConstants.CSV_COL_DIVIDE_TIE] == CommonConstants.LIST_POS_TIE)
                    {
                        lstLocalNode.RemoveAt(i);
                    }
                    if (lstLocalNode.Count < 6)
                    {
                        break;
                    }
                }
                for (int i = 5; i < lstLocalNode.Count; i++)
                {
                    lstLocalNode.RemoveAt(0);
                }

                // - Generate one trellise graph.
                List<NodeInfo> nodeInfos = new List<NodeInfo>();
                //   attach fingerings, from lower notes to higher notes.
                int tmpCnt = 0;
                while (true)
                {
                    List<int> tmpFingers = NodeInfo.GetFingerPattern(lstLocalNode.Count, tmpCnt, iRL);
                    if (tmpFingers == null)
                    {
                        break;
                    }
                    for (int i = 0; i < tmpFingers.Count; i++)
                    {
                        lstLocalNode[i][CommonConstants.CSV_COL_FINGER] = tmpFingers[i];
                    }
                    NodeInfo node = new NodeInfo(lstLocalNode, iRL);
                    nodeInfos.Add(node);
                    tmpCnt++;
                }
                // - calcurate Cost_v
                for (int i = 0; i < nodeInfos.Count; i++)
                {
                    NodeInfo node = nodeInfos[i];// ref
                    node.CalcVerticalCost();
                    nodeInfos[i] = node;// not necessary...?
                }

                // - calcurate Cost_h and add the lower cost's parent route.
                if (trellis.Count == 0)// the first trellise graph
                {
                    for (int i = 0; i < nodeInfos.Count; i++)
                    {
                        NodeInfo node = nodeInfos[i];
                        node.CalcHorizontalCost(null, -1);
                    }
                }
                else
                {
                    for (int i = 0; i < nodeInfos.Count; i++)
                    {
                        NodeInfo node = nodeInfos[i];
                        List<NodeInfo> lstPrevious = trellis.Last();
                        for (int j = 0; j < lstPrevious.Count; j++)
                        {
                            node.CalcHorizontalCost(lstPrevious[j], j);
                        }
                    }
                }
                if (nOldLastFramePasteTo2 < nLastFramePasteTo2)
                    nOldLastFramePasteTo2 = nLastFramePasteTo2;
                iOldLastMidiSeqPos = iMidiCnt - 1;

                // - Add a step (nodes) in Trellis graph.
                if (nodeInfos.Count > 0)
                {
                    trellis.Add(nodeInfos);
                }

                // - Decide couner reset and fingerings, when trellise graph's steps is max.
                int iSetFingering = 0;
                if (trellis.Count == iMaxTrellisStep)
                {
                    iSetFingering = 1;
                }
                //   The last node
                if (iMidiCnt >= listMidiFingering.Count)
                {
                    iSetFingering = 2;
                }
                //   保持音の終了Tickと次の開始TickがiResetIntervalFrames以上離れていたら, 運指を決定し, カウンタをリセットする
                if (iMidiCnt < listMidiFingering.Count)
                {
                    if (nLastFramePasteTo2 + iResetIntervalFrames < listMidiFingering[iMidiCnt][0])
                    {
                        iSetFingering = 3;
                    }
                }
                // - Set fingering
                if (iSetFingering > 0)
                {
                    // - 最小コストとなる運指(ルート)を決定
                    //   →同点のときどうする？
                    float fMin = 99999999.0f;
                    int iMinPos = -1;
                    List<NodeInfo> lstPrevious = trellis.Last();// reference
                    for (int j = 0; j < lstPrevious.Count; j++)
                    {
                        if (fMin > lstPrevious[j].GetCost())
                        {
                            fMin = lstPrevious[j].GetCost();
                            iMinPos = j;
                        }
#if DEBUG
                        //Debug.Print("Parent[{0}]:{1}, {2}", j, lstPrevious[j].GetCost(), lstPrevious[j].ToString());
#endif
                    }
#if DEBUG
                    //Debug.Print("=>{3} Parent[{0}]:{1}, {2}", iMinPos, fMin, lstPrevious[iMinPos].ToString(), trellis.Count);
#endif
                    NodeInfo nodeDummy = null;
                    if (trellis.Count > 1)
                    {
                        nodeDummy = new NodeInfo(trellis[trellis.Count - 2][lstPrevious[iMinPos].GetParent()]);
                    }
                    //   運指の割り当て, 手首位置の割り当て
                    for (int iStep = trellis.Count - 1; iStep >= 0; iStep--)
                    {
                        NodeInfo node = trellis[iStep][iMinPos];// reference
                        List<List<long>> midiFingering = node.GetMidiFingering();// reference
                        for (int j = 0; j < midiFingering.Count; j++)
                        {
                            if (midiFingering[j][CommonConstants.CSV_COL_POSITION] >= 0)
                            {
                                listMidiFingering[(int)midiFingering[j][CommonConstants.CSV_COL_POSITION]][CommonConstants.CSV_COL_FINGER] = midiFingering[j][CommonConstants.CSV_COL_FINGER];
                            }
                        }

                        // - 手首位置の割り当て
                        //   和音なら平均値の場所にする
                        long nMeanNote = 0;
                        for (int j = 0; j < midiFingering.Count; j++)
                        {
                            nMeanNote += GetWristNote((int)midiFingering[j][CommonConstants.CSV_COL_FINGER], midiFingering[j][CommonConstants.CSV_COL_NOTE], iRL);
                        }
                        nMeanNote = (int)((1.0f * nMeanNote) / midiFingering.Count + 0.5f);
                        if (!IsWhiteKey(nMeanNote))
                        {
                            nMeanNote += -1;
                        }
                        for (int j = 0; j < midiFingering.Count; j++)
                        {
                            if (midiFingering[j][CommonConstants.CSV_COL_POSITION] >= 0)
                            {
                                listMidiFingering[(int)midiFingering[j][CommonConstants.CSV_COL_POSITION]][CommonConstants.CSV_COL_WRIST] = nMeanNote;
                            }
                        }

                        // 1つ前の trellise step での杯列位置
                        iMinPos = node.GetParent();
                    }

                    // - Reset Trellis graph and set first step.
                    trellis.Clear();
                    //   コスト(Cost_h)の再計算
                    if ((iSetFingering == 1) && (nodeDummy != null))//トレリスグラフの開始ステップ
                    {
                        foreach (var node in nodeInfos)
                        {
                            nodeDummy.ForceSetCost(0.0f);
                            node.ResetPreviousCost();
                            node.CalcHorizontalCost(nodeDummy, 0);
                        }
                    }
                    //   Set first Step
                    trellis.Add(nodeInfos);
                }
            }

            // Save csv file for manual rewirte.
            StreamWriter sw = new StreamWriter(strMidiCsvFile, false, Encoding.GetEncoding("shift-jis"));
            sw.WriteLine("#ON_FRAME, OFF_FRAME, NOTE, VEL, FINGER, WRIST_POS, INDEX, POLYPHONIC_ID, TIE_FLG");
            foreach (List<long> lst in listMidiFingering)
            {
                sw.WriteLine("{0}", DebugListToString(lst));
            }
            sw.Close();

            //bSaveFingeringFile = true;
            return 0;
        }

        /// <summary>
        /// Add a node from MIDI date sequence list (listMidiFingering) to lstLoaclNode.
        /// </summary>
        /// <param name="listMidiFingering">MIDI data sequence list</param>
        /// <param name="iMidiCnt">current sequence number</param>
        /// <param name="lstLocalNode">a node (one harmony)</param>
        /// <param name="nMidiPolyphonyPairs">node id, only set harmony, auto increment</param>
        /// <param name="nLastFramePasteTo2">the longest frame position in a node</param>
        /// <returns></returns>
        public static int GetNodeFromList(ref List<List<long>> listMidiFingering, ref int iMidiCnt, ref List<List<long>> lstLocalNode, ref long nMidiPolyphonyPairs, ref long nLastFramePasteTo2)
        {
            // Get the first note
            lstLocalNode.Add(listMidiFingering[iMidiCnt]);

            int nFirstMidiCnt = iMidiCnt;
            long nNewLastFramePasteTo2 = nLastFramePasteTo2;
            if (nNewLastFramePasteTo2 < listMidiFingering[iMidiCnt][CommonConstants.CSV_COL_OFF_FRAME]) // Do before iMidiCnt++
            {
                nNewLastFramePasteTo2 = listMidiFingering[iMidiCnt][CommonConstants.CSV_COL_OFF_FRAME];
            }
            iMidiCnt++;

            // - Get polyphonic (a node of the second note)
            for (int i = iMidiCnt; i < listMidiFingering.Count; i++)
            {
                if (listMidiFingering[iMidiCnt - 1][CommonConstants.CSV_COL_ON_FRAME] == listMidiFingering[i][CommonConstants.CSV_COL_ON_FRAME])
                {
                    lstLocalNode.Add(listMidiFingering[i]);
                    if (nNewLastFramePasteTo2 < listMidiFingering[iMidiCnt][CommonConstants.CSV_COL_OFF_FRAME]) // Do before iMidiCnt++
                    {
                        nNewLastFramePasteTo2 = listMidiFingering[iMidiCnt][CommonConstants.CSV_COL_OFF_FRAME];
                    }
                    // Set node id
                    listMidiFingering[i - 1][CommonConstants.CSV_COL_POLYPHONIC] = nMidiPolyphonyPairs;
                    listMidiFingering[i][CommonConstants.CSV_COL_POLYPHONIC] = nMidiPolyphonyPairs;
                    // 次のnoteへインクリメント
                    iMidiCnt++;
                }
                else
                {
                    break;
                }
            }

            //   In Dr.Alia's paper, rewrite polyphonic music as a collection of chords by using tie symbol
            int nHoldTieMidiCnt = iMidiCnt;
            if (nLastFramePasteTo2 > lstLocalNode[0][CommonConstants.CSV_COL_ON_FRAME])
            {
                for (int i = nFirstMidiCnt - 1; i >= 0; i--)
                {
                    if (listMidiFingering[i][CommonConstants.CSV_COL_OFF_FRAME] > lstLocalNode[0][CommonConstants.CSV_COL_ON_FRAME])
                    {
                        List<long> lstTmp = new List<long>(listMidiFingering[i]);
                        lstTmp[CommonConstants.CSV_COL_ON_FRAME] = lstLocalNode[0][CommonConstants.CSV_COL_ON_FRAME];
                        lstTmp[CommonConstants.CSV_COL_POLYPHONIC] = lstLocalNode[0][CommonConstants.CSV_COL_POLYPHONIC];
                        lstTmp[CommonConstants.CSV_COL_FINGER] = 0;
                        lstTmp[CommonConstants.CSV_COL_DIVIDE_TIE] = CommonConstants.LIST_POS_TIE;// NOT set fingering: -1
                        // Add a note if not contain the same note
                        int nCheckNote = 0;
                        for (int j = 0; j < lstLocalNode.Count; j++)
                        {
                            if (lstLocalNode[j][CommonConstants.CSV_COL_NOTE] == lstTmp[CommonConstants.CSV_COL_NOTE]) { nCheckNote++; }
                        }
                        if (nCheckNote == 0)
                        {
                            lstTmp[CommonConstants.CSV_COL_POSITION] = nHoldTieMidiCnt;
                            lstLocalNode.Add(lstTmp);
                            listMidiFingering[i][CommonConstants.CSV_COL_OFF_FRAME] = lstLocalNode[0][CommonConstants.CSV_COL_ON_FRAME] - 1;
                            nHoldTieMidiCnt++;
                        }
                    }
                }
                for (int i = 0; i < lstLocalNode.Count; i++)
                {
                    lstLocalNode[i][CommonConstants.CSV_COL_POLYPHONIC] = nMidiPolyphonyPairs;
                }
            }
            //   listMidiFingeringに追加する
            for (int i = 0; i < lstLocalNode.Count; i++)
            {
                if (lstLocalNode[i][CommonConstants.CSV_COL_DIVIDE_TIE] == CommonConstants.LIST_POS_TIE)
                {
                    listMidiFingering.Insert(iMidiCnt, lstLocalNode[i]);
                    iMidiCnt++;
                    for (int j = iMidiCnt; j < listMidiFingering.Count; j++)
                    {
                        listMidiFingering[j][CommonConstants.CSV_COL_POSITION] += 1;
                    }
                }
            }

            //   Polyphonic_IDをインクリメントする
            if (lstLocalNode.Count > 1)
            {
                nMidiPolyphonyPairs++;
            }
            //   1つ前のノードの最大保持フレーム位置を代入する
            nLastFramePasteTo2 = nNewLastFramePasteTo2;

            return lstLocalNode.Count;
        }

        //////////////////////////////////////////////////////////////////////
        /// <summary>
        /// one of Trellis graph's coponent.
        /// </summary>
        public class NodeInfo : ICloneable
        {
            private float fPreviousCost;
            private int iVerticalCost;
            private int iParent;
            private List<List<long>> listMidiFingering;
            private int iRL;
            /// <summary>
            /// return fingering patterns.
            /// mono  nPolyphony:1, nCounter:0 to 4->[1],[2],...,[5]
            /// poly  nPolyphony:2 to 5. nPolyphony=2, nCounter:0～9->[1,2],[1,3],[1,4],...,[4,5]
            /// </summary>
            /// <param name="nPolyphony">notes, 1 means mono(single)</param>
            /// <param name="nCounter">combination counter, start to 0</param>
            /// <param name="iRL">right(0), left(1)</param>
            /// <returns>null means out of counter</returns>
            static public List<int> GetFingerPattern(int nPolyphony, int nCounter, int iRL)
            {
                List<int> lstFingering = null;
                if (iRL == 0)
                {
                    switch (nPolyphony)
                    {
                        case 1:
                            if (nCounter == 0) { lstFingering = new List<int>() { 1 }; }
                            else if (nCounter == 1) { lstFingering = new List<int>() { 2 }; }
                            else if (nCounter == 2) { lstFingering = new List<int>() { 3 }; }
                            else if (nCounter == 3) { lstFingering = new List<int>() { 4 }; }
                            else if (nCounter == 4) { lstFingering = new List<int>() { 5 }; }
                            break;
                        case 2:
                            if (nCounter == 0) { lstFingering = new List<int>() { 1, 2 }; }
                            else if (nCounter == 1) { lstFingering = new List<int>() { 1, 3 }; }
                            else if (nCounter == 2) { lstFingering = new List<int>() { 1, 4 }; }
                            else if (nCounter == 3) { lstFingering = new List<int>() { 1, 5 }; }
                            else if (nCounter == 4) { lstFingering = new List<int>() { 2, 3 }; }
                            else if (nCounter == 5) { lstFingering = new List<int>() { 2, 4 }; }
                            else if (nCounter == 6) { lstFingering = new List<int>() { 2, 5 }; }
                            else if (nCounter == 7) { lstFingering = new List<int>() { 3, 4 }; }
                            else if (nCounter == 8) { lstFingering = new List<int>() { 3, 5 }; }
                            else if (nCounter == 9) { lstFingering = new List<int>() { 4, 5 }; }
                            break;
                        case 3:
                            if (nCounter == 0) { lstFingering = new List<int>() { 1, 2, 3 }; }
                            else if (nCounter == 1) { lstFingering = new List<int>() { 1, 2, 4 }; }
                            else if (nCounter == 2) { lstFingering = new List<int>() { 1, 2, 5 }; }
                            else if (nCounter == 3) { lstFingering = new List<int>() { 1, 3, 4 }; }
                            else if (nCounter == 4) { lstFingering = new List<int>() { 1, 3, 5 }; }
                            else if (nCounter == 5) { lstFingering = new List<int>() { 1, 4, 5 }; }
                            else if (nCounter == 6) { lstFingering = new List<int>() { 2, 3, 4 }; }
                            else if (nCounter == 7) { lstFingering = new List<int>() { 2, 3, 5 }; }
                            else if (nCounter == 8) { lstFingering = new List<int>() { 2, 4, 5 }; }
                            else if (nCounter == 9) { lstFingering = new List<int>() { 3, 4, 5 }; }
                            break;
                        case 4:
                            if (nCounter == 0) { lstFingering = new List<int>() { 1, 2, 3, 4 }; }
                            else if (nCounter == 1) { lstFingering = new List<int>() { 1, 2, 3, 5 }; }
                            else if (nCounter == 2) { lstFingering = new List<int>() { 1, 2, 4, 5 }; }
                            else if (nCounter == 3) { lstFingering = new List<int>() { 1, 3, 4, 5 }; }
                            else if (nCounter == 4) { lstFingering = new List<int>() { 2, 3, 4, 5 }; }
                            break;
                        case 5:
                            if (nCounter == 0) { lstFingering = new List<int>() { 1, 2, 3, 4, 5 }; }
                            break;
                    }
                }
                else if (iRL == 1)
                {
                    switch (nPolyphony)
                    {
                        case 1:
                            if (nCounter == 0) { lstFingering = new List<int>() { 5 }; }
                            else if (nCounter == 1) { lstFingering = new List<int>() { 4 }; }
                            else if (nCounter == 2) { lstFingering = new List<int>() { 3 }; }
                            else if (nCounter == 3) { lstFingering = new List<int>() { 2 }; }
                            else if (nCounter == 4) { lstFingering = new List<int>() { 1 }; }
                            break;
                        case 2:
                            if (nCounter == 0) { lstFingering = new List<int>() { 5, 4 }; }
                            else if (nCounter == 1) { lstFingering = new List<int>() { 5, 3 }; }
                            else if (nCounter == 2) { lstFingering = new List<int>() { 5, 2 }; }
                            else if (nCounter == 3) { lstFingering = new List<int>() { 5, 1 }; }
                            else if (nCounter == 4) { lstFingering = new List<int>() { 4, 3 }; }
                            else if (nCounter == 5) { lstFingering = new List<int>() { 4, 2 }; }
                            else if (nCounter == 6) { lstFingering = new List<int>() { 4, 1 }; }
                            else if (nCounter == 7) { lstFingering = new List<int>() { 3, 2 }; }
                            else if (nCounter == 8) { lstFingering = new List<int>() { 3, 1 }; }
                            else if (nCounter == 9) { lstFingering = new List<int>() { 2, 1 }; }
                            break;
                        case 3:
                            if (nCounter == 0) { lstFingering = new List<int>() { 5, 4, 3 }; }
                            else if (nCounter == 1) { lstFingering = new List<int>() { 5, 4, 2 }; }
                            else if (nCounter == 2) { lstFingering = new List<int>() { 5, 4, 1 }; }
                            else if (nCounter == 3) { lstFingering = new List<int>() { 5, 3, 2 }; }
                            else if (nCounter == 4) { lstFingering = new List<int>() { 5, 3, 1 }; }
                            else if (nCounter == 5) { lstFingering = new List<int>() { 5, 2, 1 }; }
                            else if (nCounter == 6) { lstFingering = new List<int>() { 4, 3, 2 }; }
                            else if (nCounter == 7) { lstFingering = new List<int>() { 4, 3, 1 }; }
                            else if (nCounter == 8) { lstFingering = new List<int>() { 4, 2, 1 }; }
                            else if (nCounter == 9) { lstFingering = new List<int>() { 3, 2, 1 }; }
                            break;
                        case 4:
                            if (nCounter == 0) { lstFingering = new List<int>() { 5, 4, 3, 2 }; }
                            else if (nCounter == 1) { lstFingering = new List<int>() { 5, 4, 3, 1 }; }
                            else if (nCounter == 2) { lstFingering = new List<int>() { 5, 4, 2, 1 }; }
                            else if (nCounter == 3) { lstFingering = new List<int>() { 5, 3, 2, 1 }; }
                            else if (nCounter == 4) { lstFingering = new List<int>() { 4, 3, 2, 1 }; }
                            break;
                        case 5:
                            if (nCounter == 0) { lstFingering = new List<int>() { 5, 4, 3, 2, 1 }; }
                            break;
                    }
                }
                return lstFingering;
            }
            /// <summary>
            /// Calculate vertical cost.
            /// </summary>
            public void CalcVerticalCost()
            {
                // 0:frame-paste-to1, 1:frame-paste-to2, 2:note, 3:vel, 4:finger, 5:wrist center pos, POS, POLYID
                this.iVerticalCost = 0;
                for (int i = 1; i < this.listMidiFingering.Count; i++)
                {
                    this.iVerticalCost += GenerateCost(
                        (int)this.listMidiFingering[i - 1][CommonConstants.CSV_COL_FINGER],
                        (int)this.listMidiFingering[i][CommonConstants.CSV_COL_FINGER],
                        GenerateDistance((int)this.listMidiFingering[i - 1][CommonConstants.CSV_COL_NOTE], (int)this.listMidiFingering[i][CommonConstants.CSV_COL_NOTE]),
                        iRL
                        );
                }
            }
            /// <summary>
            /// Calculate horizontal cost,  sum of previous route cost, and select lower cost previous position.
            /// </summary>
            /// <param name="nodePrevious">One of the node from previous step nodes. </param>
            /// <param name="iPreviousPos">The index of previous node.</param>
            public void CalcHorizontalCost(NodeInfo nodePrevious, int iPreviousPos, bool bDebug = false)
            {
                // Trellis Graph's first step (No previous node == null)
                if (iPreviousPos < 0)
                {
                    // polyphonic ... not evaluate
                    // single phonic
                    if (this.listMidiFingering.Count == 1)
                    {
                        this.fPreviousCost = GetInitCost((int)this.listMidiFingering[0][CommonConstants.CSV_COL_FINGER]);
                    }
                    else
                    {
                        this.fPreviousCost = 0.0f;
                    }
                }
                else
                {
                    // calc horizontal cost, and set lower node to the parent.
                    int iCount = 0;
                    int iCost = 0;
                    for (int i = 0; i < nodePrevious.GetMidiFingering().Count; i++)
                    {
                        for (int j = 0; j < this.listMidiFingering.Count; j++)
                        {
                            iCost += GenerateCost(
                                (int)nodePrevious.GetMidiFingering()[i][CommonConstants.CSV_COL_FINGER],
                                (int)this.listMidiFingering[j][CommonConstants.CSV_COL_FINGER],
                                GenerateDistance((int)nodePrevious.GetMidiFingering()[i][CommonConstants.CSV_COL_NOTE], (int)this.listMidiFingering[j][CommonConstants.CSV_COL_NOTE]),
                                iRL
                                );
                            iCount++;
                        }
                    }
                    // 遷移コストの比較と親の登録
                    // →イコールのときはどうする？？
                    float fCost = (iCost * 1.0f) / iCount + nodePrevious.GetCost();

                    // 「タイは同じ運指でないとコストが高くなる」->おなじ運指でないときは+Xにする
                    int iTieNotSame = 0;
                    int iTieIsSame = 0;
                    for (int i = 0; i < this.listMidiFingering.Count; i++)
                    {
                        if (this.listMidiFingering[i][CommonConstants.CSV_COL_DIVIDE_TIE] == CommonConstants.LIST_POS_TIE)//タイ
                        {
                            for (int j = 0; j < nodePrevious.GetMidiFingering().Count; j++)
                            {
                                if (nodePrevious.GetMidiFingering()[j][CommonConstants.CSV_COL_NOTE] == this.listMidiFingering[i][CommonConstants.CSV_COL_NOTE])
                                {
                                    if (this.listMidiFingering[i][CommonConstants.CSV_COL_FINGER] == 0)
                                    {
                                        iTieNotSame++;
                                    }
                                    else
                                    {
                                        if (nodePrevious.GetMidiFingering()[j][CommonConstants.CSV_COL_FINGER] == this.listMidiFingering[i][CommonConstants.CSV_COL_FINGER])
                                        {
                                            iTieIsSame++;
                                            break;
                                        }
                                        else
                                        {
                                            iTieNotSame++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (iTieNotSame > 0)
                    {
                        fCost += 4.0f;
                    }

                    if (this.fPreviousCost > fCost)
                    {
                        this.fPreviousCost = fCost;
                        this.iParent = iPreviousPos;
                    }

                }
            }
            /// <summary>
            /// constractor
            /// </summary>
            /// <param name="listMidi">MIDIデータから読み込んだシーケンスリスト. 0:frame-paste-to1, 1:frame-paste-to2, 2:note, 3:vel, 4:finger, 5:wrist center pos, (6:frame-copy-from)</param>
            /// <param name="iRL">right(0)/left</param>
            public NodeInfo(List<List<long>> listMidi, int iRL)
            {
                this.iVerticalCost = 0;
                this.iParent = -1;
                this.fPreviousCost = 9999999.9f;
                this.iRL = iRL;
                this.listMidiFingering = new List<List<long>>();
                foreach (List<long> line in listMidi)
                {
                    List<long> tmp = new List<long>(line);
                    this.listMidiFingering.Add(tmp);
                }
            }
            /// <summary>
            /// Copy constractor
            /// </summary>
            /// <param name="org"></param>
            public NodeInfo(NodeInfo org)
            {
                this.iVerticalCost = org.iVerticalCost;
                this.iParent = org.iParent;
                this.fPreviousCost = org.fPreviousCost;
                this.iRL = org.iRL;
                this.listMidiFingering = new List<List<long>>();
                foreach (List<long> line in org.listMidiFingering)
                {
                    List<long> tmp = new List<long>(line);
                    this.listMidiFingering.Add(tmp);
                }
            }
            protected NodeInfo()
            {
                this.iVerticalCost = 0;
                this.iParent = -1;
                this.fPreviousCost = 9999999.9f;
                this.iRL = 0;
                this.listMidiFingering = null;
            }
            public NodeInfo Clone()
            {
                NodeInfo node = new NodeInfo(this);
                return node;
            }
            object ICloneable.Clone()
            {
                return Clone();
            }
            public float GetCost() { return (this.iVerticalCost + this.fPreviousCost); }
            public int GetParent() { return this.iParent; }
            public void ResetPreviousCost() { this.fPreviousCost = 9999999.9f; }
            public void ForceSetCost(float fSet) { this.fPreviousCost = fSet; }
            public List<List<long>> GetMidiFingering() { return this.listMidiFingering; }
            public override string ToString() { return "Costv:" + this.iVerticalCost.ToString() + ", Costh:" + this.fPreviousCost.ToString() + ", Parent:" + this.iParent.ToString() + ", MIDI:" + DebugListToString(this.listMidiFingering); }
        }
        public static string DebugListToString<Type>(IList<Type> list)
        {
            string strMsg = "";
            foreach (Type i in list)
            {
                strMsg += i.ToString() + ",";
            }
            return strMsg;
        }
        public static string DebugListToString(List<List<long>> list)
        {
            string strMsg = "";
            foreach (List<long> line in list)
            {
                strMsg += "[";
                //DebugListToString(line);
                for (int i = 0; i < line.Count; i++)
                {
                    strMsg += line[i].ToString() + ",";
                }
                strMsg += "]";
            }
            return strMsg;
        }
        /// <summary>
        /// 2つの指の間の距離コストを返す. 全音程の場合は+2. cf: C->E=4, C->F=6
        /// Cost function from two fingers distance from Dr.Alia's algorithm.
        /// <para>fingerA:最初の運指, fingerB:次の運指, distance:音の距離, 負のとき下降系を示す</para>
        /// </summary>
        /// <param name="fingerA">最初の運指</param>
        /// <param name="fingerB">次の運指</param>
        /// <param name="distance">音の距離, 負のとき下降系を示す</param>
        /// <param name="iRL">right(0)/left(1)</param>
        /// <returns>コスト</returns>
        /// <remarks>コストは距離18(0～17)、11段階(0～10)とする
        /// 右手系
        /// fingerA ＜ fingerB, distance ≧ 0 → 指順
        /// fingerA ＜ fingerB, distance ＜ 0 → 指潜り, fingerAが親指以外は高コスト
        /// fingerA ＞ fingerB, distance ＞ 0 → 指潜り, fingerBが親指以外は高コスト
        /// fingerA ＞ fingerB, distance ≦ 0 → 指順
        /// 左手系
        /// fingerA ＜ fingerB, distance ≦ 0 → 指順
        /// fingerA ＜ fingerB, distance ＞ 0 → 指潜り, fingerAが親指以外は高コスト
        /// fingerA ＞ fingerB, distance ＜ 0 → 指潜り, fingerBが親指以外は高コスト
        /// fingerA ＞ fingerB, distance ≧ 0 → 指順
        /// </remarks>
        public static int GenerateCost(int fingerA, int fingerB, int distance, int iRL, bool bDebugCost = false)
        {
            List<int> lstCost = null;
            int iDistance = distance;
            bool bCrossOver = false;
            if (iRL == 0) // 右手系
            {
                if (fingerA <= fingerB)
                {
                    if (distance >= 0)
                    {
                        bCrossOver = false;
                    }
                    else
                    {
                        bCrossOver = true;
                        iDistance = distance * -1;
                    }
                }
                else
                {
                    if (distance <= 0)
                    {
                        bCrossOver = false;
                        iDistance = distance * -1;
                    }
                    else
                    {
                        bCrossOver = true;
                    }
                }
            }
            else // 左手系
            {
                if (fingerA <= fingerB)
                {
                    if (distance <= 0)
                    {
                        bCrossOver = false;
                        iDistance = distance * -1;
                    }
                    else
                    {
                        bCrossOver = true;
                    }
                }
                else
                {
                    if (distance >= 0)
                    {
                        bCrossOver = false;
                    }
                    else
                    {
                        bCrossOver = true;
                        iDistance = distance * -1;
                    }
                }
            }
            if (iDistance > 17)
            {
                iDistance = 17;
            }

            // コストの設定
            if (!bCrossOver)
            {
                int Afinger = fingerA;
                int Bfinger = fingerB;
                // 正規化 (1→5の方向へ)
                if (fingerA > fingerB)
                {
                    Afinger = fingerB;
                    Bfinger = fingerA;
                }
                // コスト定義
                if (Afinger == 1)
                {
                    //                                               0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17
                    //if (Bfinger == 1) { lstCost = new List<int>() { 2, 1, 1, 3, 5, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }
                    //if (Bfinger == 1) { lstCost = new List<int>() { 3, 3, 5, 7, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }
                    if (Bfinger == 1) { lstCost = new List<int>() { 3, 2, 2, 3, 5, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }
                    if (Bfinger == 2) { lstCost = new List<int>() { 2, 0, 0, 0, 0, 0, 1, 2, 4, 8, 10, 10, 10, 10, 10, 10, 10, 10 }; }
                    if (Bfinger == 3) { lstCost = new List<int>() { 1, 2, 1, 0, 0, 0, 0, 0, 1, 2, 4, 8, 10, 10, 10, 10, 10, 10 }; }
                    if (Bfinger == 4) { lstCost = new List<int>() { 8, 4, 3, 2, 1, 0, 0, 0, 0, 1, 1, 2, 3, 7, 9, 10, 10, 10 }; }
                    if (Bfinger == 5) { lstCost = new List<int>() { 10, 9, 8, 6, 4, 2, 1, 0, 0, 0, 0, 0, 0, 0, 2, 4, 8, 10 }; }
                }
                else if (Afinger == 2)
                {
                    //                                               0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17
                    //if (Bfinger == 1) { lstCost = new List<int>() { 1, 0, 0, 0, 0, 0, 1, 2, 4, 8, 10, 10, 10, 10, 10, 10, 10, 10 }; }// dummy
                    if (Bfinger == 1) { lstCost = new List<int>() { 3, 1, 1, 2, 2, 5, 4, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }// 指潜り
                    //if (Bfinger == 2) { lstCost = new List<int>() { 3, 5, 7, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }
                    if (Bfinger == 2) { lstCost = new List<int>() { 1, 5, 7, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }
                    if (Bfinger == 3) { lstCost = new List<int>() { 0, 0, 0, 0, 3, 5, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }
                    if (Bfinger == 4) { lstCost = new List<int>() { 10, 10, 7, 3, 0, 0, 0, 0, 2, 7, 9, 10, 10, 10, 10, 10, 10, 10 }; }// Dr.Alia's sample value
                    if (Bfinger == 5) { lstCost = new List<int>() { 10, 10, 10, 8, 4, 1, 0, 0, 0, 0, 3, 7, 10, 10, 10, 10, 10, 10 }; }
                }
                else if (Afinger == 3)
                {
                    //                                               0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17
                    //if (Bfinger == 1) { lstCost = new List<int>() { 1, 2, 1, 0, 0, 0, 0, 0, 1, 2, 4, 8, 10, 10, 10, 10, 10, 10 }; }// dummy
                    //if (Bfinger == 2) { lstCost = new List<int>() { 3, 5, 7, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }// dummy
                    //if (Bfinger == 3) { lstCost = new List<int>() { 4, 8, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }
                    if (Bfinger == 1) { lstCost = new List<int>() { 3, 1, 1, 2, 2, 5, 4, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }// 指潜り
                    if (Bfinger == 2) { lstCost = new List<int>() { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }
                    if (Bfinger == 3) { lstCost = new List<int>() { 2, 8, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }
                    if (Bfinger == 4) { lstCost = new List<int>() { 4, 0, 0, 0, 2, 4, 8, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }
                    if (Bfinger == 5) { lstCost = new List<int>() { 10, 9, 7, 3, 0, 0, 0, 0, 3, 7, 9, 10, 10, 10, 10, 10, 10, 10 }; }
                }
                else if (Afinger == 4)
                {
                    //                                               0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17
                    //if (Bfinger == 1) { lstCost = new List<int>() { 8, 4, 3, 2, 1, 0, 0, 0, 0, 1, 1, 2, 3, 7, 9, 10, 10, 10 }; }// dummy
                    //if (Bfinger == 2) { lstCost = new List<int>() { 10, 10, 7, 3, 0, 0, 0, 0, 2, 7, 9, 10, 10, 10, 10, 10, 10, 10 }; }// dummy
                    //if (Bfinger == 3) { lstCost = new List<int>() { 6, 0, 0, 0, 2, 4, 8, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }// dummy
                    //if (Bfinger == 4) { lstCost = new List<int>() { 7, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }
                    if (Bfinger == 1) { lstCost = new List<int>() { 3, 1, 1, 2, 2, 5, 4, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }// 指潜り
                    if (Bfinger == 2) { lstCost = new List<int>() { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }
                    if (Bfinger == 3) { lstCost = new List<int>() { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }
                    if (Bfinger == 4) { lstCost = new List<int>() { 2, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }
                    if (Bfinger == 5) { lstCost = new List<int>() { 9, 2, 0, 0, 1, 3, 7, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }
                }
                else // else if (finger== 5)
                {
                    //                                               0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17
                    //if (Bfinger == 1) { lstCost = new List<int>() { 10, 9, 8, 6, 4, 2, 1, 0, 0, 0, 0, 0, 0, 0, 2, 4, 6, 8, 10 }; }// dummy
                    //if (Bfinger == 2) { lstCost = new List<int>() { 10, 10, 10, 8, 4, 1, 0, 0, 0, 0, 3, 7, 10, 10, 10, 10, 10, 10 }; }// dummy
                    //if (Bfinger == 3) { lstCost = new List<int>() { 10, 9, 7, 3, 0, 0, 0, 0, 3, 7, 9, 10, 10, 10, 10, 10, 10, 10 }; }// dummy
                    //if (Bfinger == 4) { lstCost = new List<int>() { 7, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }// dummy
                    if (Bfinger == 1) { lstCost = new List<int>() { 3, 5, 5, 9, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }// 指潜り
                    if (Bfinger == 2) { lstCost = new List<int>() { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }// dummy
                    if (Bfinger == 3) { lstCost = new List<int>() { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }// dummy
                    if (Bfinger == 4) { lstCost = new List<int>() { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }// dummy
                    if (Bfinger == 5) { lstCost = new List<int>() { 8, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }
                }
            }
            else
            {
                // 指潜り, fingerAが親指以外は高コスト
                int Afinger = fingerA;
                int Bfinger = fingerB;
                // 正規化 (1→5の方向へ)
                if (fingerA > fingerB)
                {
                    Afinger = fingerB;
                    Bfinger = fingerA;
                }
                // コスト定義
                if (Afinger == 1)
                {
                    if (Bfinger == 1) { lstCost = new List<int>() { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }// dummy
                    if (Bfinger == 2) { lstCost = new List<int>() { 3, 4, 5, 5, 6, 8, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }// dummy
                    if (Bfinger == 3) { lstCost = new List<int>() { 4, 3, 4, 4, 6, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }// dummy
                    if (Bfinger == 4) { lstCost = new List<int>() { 5, 4, 5, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }// dummy
                    if (Bfinger == 5) { lstCost = new List<int>() { 8, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; }// dummy
                }
                else
                {
                    lstCost = new List<int>() { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
                }
            }
#if DEBUG
            if (bDebugCost)
            {
                //Debug.Print("{0},{1},{2}->{3}, {4}={5}", fingerA, fingerB, distance, iDistance, lstCost[iDistance], DebugListToString(lstCost));
            }
#endif
            return lstCost[iDistance];
        }
        /// <summary>
        /// Initial horizon cost.
        /// </summary>
        /// <param name="fingerA">finger, 1 is Thumb, 2 is Index, ..., 5 is Little.</param>
        /// <returns></returns>
        public static int GetInitCost(int fingerA)
        {
            //                                   dmy,T, I, M, R, L
            List<int> lstCost = new List<int>() { 0, 0, 1, 0, 1, 2 };
            return lstCost[fingerA];
        }
        /// <summary>
        /// Key distance function from Dr.Alia's algorithm.
        /// </summary>
        /// <param name="noteNode1">First note or lower note</param>
        /// <param name="noteNode2">Next note or higher note</param>
        /// <returns>distance, in noteNote1 > noteNote2, this value is minus.</returns>
        public static int GenerateDistance(int noteNode1, int noteNode2)
        {
            bool bMinus = false;
            if (noteNode1 > noteNode2)
            {
                int iTmp = noteNode1;
                noteNode1 = noteNode2;
                noteNode2 = iTmp;
                bMinus = true;
            }
            int iOctave = (noteNode2 - noteNode1) / 12;
            int iMidiDist = (noteNode2 - 12 * iOctave) - noteNode1;
            List<int> lstDistance = null;
            int iNote60 = noteNode1 % 12;
            if (iNote60 == 0) { lstDistance = new List<int>() { 0, 1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12 }; }//C
            else if (iNote60 == 1) { lstDistance = new List<int>() { 0, 1, 2, 3, 5, 6, 7, 8, 9, 10, 11, 13 }; }
            else if (iNote60 == 2) { lstDistance = new List<int>() { 0, 1, 2, 4, 5, 6, 7, 8, 9, 10, 12, 13 }; }//D
            else if (iNote60 == 3) { lstDistance = new List<int>() { 0, 1, 3, 4, 5, 6, 7, 8, 9, 11, 12, 13 }; }
            else if (iNote60 == 4) { lstDistance = new List<int>() { 0, 2, 3, 4, 5, 6, 7, 8, 10, 11, 12, 13 }; }//E
            else if (iNote60 == 5) { lstDistance = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12 }; }//F
            else if (iNote60 == 6) { lstDistance = new List<int>() { 0, 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 13 }; }
            else if (iNote60 == 7) { lstDistance = new List<int>() { 0, 1, 2, 3, 4, 6, 7, 8, 9, 10, 12, 13 }; }//G
            else if (iNote60 == 8) { lstDistance = new List<int>() { 0, 1, 2, 3, 5, 6, 7, 8, 9, 11, 12, 13 }; }
            else if (iNote60 == 9) { lstDistance = new List<int>() { 0, 1, 2, 4, 5, 6, 7, 8, 10, 11, 12, 13 }; }//A
            else if (iNote60 == 10) { lstDistance = new List<int>() { 0, 1, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13 }; }
            else if (iNote60 == 11) { lstDistance = new List<int>() { 0, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 13 }; }//B
            int iRet = lstDistance[iMidiDist] + 14 * iOctave;
            if (bMinus)
            {
                iRet *= -1;
            }
            return iRet;
        }
        /// <summary>
        /// Decide wrist position (right hand system).
        /// </summary>
        /// <param name="nFinger">finger (1,2,3,4,5)</param>
        /// <param name="nFingerNote">MIDI note number</param>
        /// <param name="iRL">Right(0)/Left(1)</param>
        /// <returns>wrist position for MIDI note number</returns>
        public static int GetWristNote(int nFinger, long nFingerNote, int iRL)
        {
            int[] arrayPos = null;
            int iSubDist = (int)nFingerNote % 12;
            int iDiv = (int)nFingerNote / 12;
            if (iRL == 0) // Right
            {
                //                                         C  C# D  D# E  F  F# G  G# A  A# B
                if (nFinger == 1) { arrayPos = new int[] { 4, 3, 3, 2, 3, 4, 3, 4, 3, 3, 2, 3 }; }
                //                                              C  C# D  D# E  F  F# G  G# A  A# B
                else if (nFinger == 2) { arrayPos = new int[] { 2, 1, 2, 1, 1, 2, 1, 2, 1, 2, 1, 1 }; }
                //                                              C  C#  D   D# E  F   F# G   G# A   A# B
                else if (nFinger == 3) { arrayPos = new int[] { 0, -1, 0, -1, 0, 0, -1, 0, -1, 0, -1, 0 }; }
                //                                               C   C#  D   D#  E   F   F#  G   G#  A   A#  B
                else if (nFinger == 4) { arrayPos = new int[] { -1, -2, -2, -3, -2, -1, -2, -2, -3, -2, -3, -2 }; }
                //                                               C   C#  D   D#  E   F   F#   G   G#  A   A#  B
                else if (nFinger == 5) { arrayPos = new int[] { -3, -4, -3, -4, -4, -3, -4, -3, -4, -4, -5, -4 }; }
                else { arrayPos = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; }
            }
            else // Left
            {
                //                                         C  C# D  D# E  F  F# G  G# A  A# B
                if (nFinger == 5) { arrayPos = new int[] { 4, 3, 3, 2, 3, 4, 3, 4, 3, 3, 2, 3 }; }
                //                                              C  C# D  D# E  F  F# G  G# A  A# B
                else if (nFinger == 4) { arrayPos = new int[] { 2, 1, 2, 1, 1, 2, 1, 2, 1, 2, 1, 1 }; }
                //                                              C  C#  D   D# E  F   F# G   G# A   A# B
                else if (nFinger == 3) { arrayPos = new int[] { 0, -1, 0, -1, 0, 0, -1, 0, -1, 0, -1, 0 }; }
                //                                               C   C#  D   D#  E   F   F#  G   G#  A   A#  B
                else if (nFinger == 2) { arrayPos = new int[] { -1, -2, -2, -3, -2, -1, -2, -2, -3, -2, -3, -2 }; }
                //                                               C   C#  D   D#  E   F   F#   G   G#  A   A#  B
                else if (nFinger == 1) { arrayPos = new int[] { -3, -4, -3, -4, -4, -3, -4, -3, -4, -4, -5, -4 }; }
                else { arrayPos = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; }
            }
#if DEBUG
            //Debug.Print("Wrist{3} {0},{1},{2}", nFinger, nFingerNote, iDiv * 12 + iSubDist + arrayPos[iSubDist], iRL);
#endif
            return iDiv * 12 + iSubDist + arrayPos[iSubDist];
        }
        /// <summary>
        /// To return Keyboard white or black from MIDI note number.
        /// </summary>
        public static bool IsWhiteKey(long nNote)
        {
            long nModeA0 = (nNote - 21) % 12;
            if (nModeA0 == 1)
            {
                return false;
            }
            else if (nModeA0 == 4)
            {
                return false;
            }
            else if (nModeA0 == 6)
            {
                return false;
            }
            else if (nModeA0 == 9)
            {
                return false;
            }
            else if (nModeA0 == 11)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
