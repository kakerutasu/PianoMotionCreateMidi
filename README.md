Automatic generation of keyboard performance motion
---

`PianoMotionCreateMidi` is an MikuMikuMoving plugin for automatic 
generation of keyboard performance motion.
The fingering pose registered as a type on MikuMikuMoving is copied and 
pasted on the timeline according to the fingering information analyzed 
from a standard MIDI file (SMF).

Regarding MikuMikuMoving, please refer to [MikuMikuMoving](https://sites.google.com/site/mikumikumovingeng/).
The operation is verified with version 1.2.7.2.


PianoMotionCreateMidi by kakerutasu is licensed under the Apache License, Version2.0


## Disclaimer

kakerutasu makes no guarantees whatsoever related to this text.


## What the plugin can do

A keyboard performance motion is generated from finger type and MIDI 
data (SMF) registered in the first 18 frames of MikuMikuMoving.
The created motion can be brought into MikuMikuDance in the vmd format etc.

This article explains how to introduce and use it.

Usage is as follows: 
(1) Work in the PMX Editor: Model Rework,  
(2) Work in MikuMikuMoving:type registration,  
(3) Generate keyboard motion, keyboard performance motion.


## Dependencies

* PMX Plugin
   - Finger Cut IK plugin (PmxPluginFingerIKmk4.dll)
   
* MikuMikuMoving Plugin
   - Generate keyboard motion (Midi2PianoKey.dll)
   - Copy and paste with fingers, etc.　 (CopyPasteMidi.dll)
   
* Reading MIDI date
   - [Next MIDI Project](http://starway.s234.xrea.com/wordpress/) 


## Install

Save PmxPluginFingerIKmk4.dll in `_plugin/User`  folder of PMX editor.

Save MikuMikuMoving plugins in `Plugins` folder of MikuMikuMoving.

Save `Next MIDI Project` library in the same `Plugins` folder of MikuMikuMoving.
The library can be obtained from 'ダウンロード'.
When I tested it, the file name was nextmidi.zip.


## How to use

### (1) Working in the PMX Editor: Rework the Model

This plugin assumes that your model is arm cut IK and finger cut IK.
Therefore, we will modify the model.

You are responsible for checking and backup the model.


**MODIFICATION MODEL 1**


Execute [Standard Bone Checker](https://bowlroll.net/file/9611) of T0R0 in PMX editor.

This is a prep for the plugin to require "Fingertips" bones.

This is not necessary if the model you want to use already has "Fingertips" bones.


**MODIFICATION MODEL 2**


Check the bones required for the arm-cut IK.

One of the tools I use is [Semi Standard Bone Addition plugin](http://www.nicovideo.jp/watch/sm14956092).


Note: need '親指0'.


**MODIFICATION MODEL 3**


Execute Arm Cut IK.

One of the tools I use is [Arm IK Creation](https://bowlroll.net/file/9482).

If you want to use T0R0 like arm IK creation, please select '腕IK（腕切りタイプ）'.


Note: to pose, 'センターW','上半身W','上半身2W'.


**MODIFICATION MODEL 4**


Introduces eye IK (Options).

One of the tools I use is [eye IK Creation](https://bowlroll.net/file/9826).


**MODIFICATION MODEL 5**


After performing the above modification, select "PmxPluginFingerIKmk4.dll" in the PMX Editor.

This is a plugin version of MEA's finger IK method (mk4).

Note that models with bones that extend beyond the semi standard will fail.


### (2) Working with MikuMikuMoving: Registering a Type


Registers finger shape and arm position.

Keyframe positions are meaningful, so refer to sample images and motion.


**KEYFRAME REGISTER 1**


Register "Right to fingertips", "Right to finger 3", "Left to fingertips", 
"Left to finger 3", "right wrist", "left wrist".

* At frame 0:
  - Only white keys. The default position before the key is pressed.
  - On the right, the fingertips are in the C4-G4 position. The right wrist is positioned at E4.
  - For the left, the fingertips should be in the C3 to G3 position. The left wrist is positioned at E3.
* At frame 1:
  - Only white keys. The position of the fingertip when the key is pressed.
  - On the right, the thumb is in the position B, the index finger is in the position C, the middle finger is in the position F, the ring finger is in the position G, and the little finger is in the position A.
  - In the case of the left, put the little finger in the position B, the ring finger in the position C, the middle finger in the position D, the index finger in the position G, and the little finger in the position A.
  - Wrist is the same as at frame 0.
* At frame 2:
  - Only white keys. The position of the finger tip when the key is pressed.
  - On the right, the thumb is in the position B, the index finger is in the position C, the middle finger is in the position F, the ring finger is in the position G, and the little finger is in the position A.
  - Wrist is the same as at frame 0.
* At frame 3:
  - Finger stretches. Before you press a key.
  - For right, put thumb in D position. Do not move the index finger or little finger.
  - XYZ of the right wrist is the same as in frame 0, rotated only.
  - Left thumb in F position. Do not move the index to little finger.
  - Left wrist XYZ is the same as in frame 0, rotation only.
* At frame 4:
  - Finger stretches. Press and hold the key. Thumb only.
  - Wrist position is the same as in the third frame.
* At frame 5:
  - Finger stretches. Press and hold the key. Middle finger only.
  - Wrist position is the same as in the third frame.
* At frame 6:
  - White key (Thumb, Pinky finger tip) + black key (Person finger - Ring finger tip). Before you press a key.
  - Wrist position adjusts height and depth relative to zero frame. 
* At frame 7:
  - White key (Thumb, Pinky finger Tip) + black key (Person finger - Ring finger tip). Press and hold the key.
  - Wrist is same as sixth frame.
* At frame 8:
  - White key (Thumb, Pinky finger Tip) + black key (Person - Ring finger tip). The position of the finger tip when the key is pressed.
  - Wrist is same as sixth frame.
* At frame 9:
  - not use
* At frame 10:
  - Only black keys. Before you press a key.
  - Wrist position adjusted height and depth for sixth frame.
* At frame 11:
  - Only black keys. The position of the fingertip when the key is pressed. 
  - Wrist is the same as frame 10.
* At frame 12:
  - Only black keys. The position of the finger tip when the key is pressed.
  - Wrist is the same as frame 10.
* At frame 13:
  - black key (Thumb, Pinky Tip) + white key (Index finger - Ring finger tip). The position of the fingertip when the key is pressed.
  - Wrist is the same as frame 10.
* At frame 14:
  - black key (Thumb, Pinky Tip) + white key (Index finger - Ring finger tip). Press and hold down the key while moving toward the key column.
  - Wrist is the same as frame 10.
* At frame 15:
  - not use
* At frame 16:
  - Open your wrists.
  - Right goes up three octaves and left goes down two octaves.
* At frame 17:
  - not use
* At frame 18:
  - Lift the wrist. position for lifting while playing.
  - to move in global Y direction.


**KEYFRAME REGISTER 2**


After setting the one, register the shake of the wrist when you press the key.
Perform the following keyframe registration for '右手首F (Left wrist F)', '左手首F (right wrist F)'.


* At frame 0:
  - Sets the completion curve only.
* At frame 1:
  - Hold the wrist down. It is convenient to move in Y direction by global.
* At frame 2:
  - Copy and paste frame 0.


**KEYFRAME REGISTER 3**


Register pedal operation type.
The damper pedal is registered in '右足IK (Right foot IK)' and the soft 
pedal is registered in '左足IK (Left foot IK)'.


* At frame 0:
  - Sets the completion curve only.
* At frame 1:
  - Keep the pedal depressed.
* At frame 2:
  - Copy and paste frame 0.


### (3) Use `PianoMotionCreateMidi`

**Generating keyboard key motion**


Target: a keyboard.  
The operation has been confirmed with the mqdl, rosshi, MNTY's keyboards.

Select the target keyboard model from the MikuMikuMoving control panel, 
and select 'Keyboard motion' from the plugin of the ribbon menu.


In order to register only one channel, it is necessary to execute the 
left and right channels respectively.
If you have another pedal track, do the same for the pedal track.


Internally, select the bone name (ntDDD or A to Gn #) with the note 
value obtained from SMF, and register the key frame in on / off time.

The key has a fixed value of -0.04 [rad] for x-axis rotation.
The pedal also has a fixed value of -0.2 [rad] for x-axis rotation.


**Generating keyboard performance motion**


Target: a figure.  

Select the target keyboard model from the MikuMikuMoving control panel, 
and select 'Generate fingering motion for keyboard' from the plugin of the ribbon menu.

Make sure the motion is created.


## Modify Motion

The automatic generation of fingerings is often unnatural because it is 
not well adjusted.
These fingers can be manually modified and re-generated automatically.
When executing the above (3), "SMF name + R.csv" and "SMF name + L.csv" 
files are generated in the same folder.
The fifth and sixth columns of the CSV file are for your fingers and wrists, respectively.
Rewrite the position of your finger or wrist.
Run the plugin again and uncheck "Create fingering" in the configuration form.


I would be happy if this plugin makes it easier to create keyboard performance motion.


## Acknowledgements


MEA has instigated me to discuss the structure of the Finger Cut IK.
I would like to take this opportunity to thank you.


I also referred to the following paper for the generation of fingering.
I would like to take this opportunity to thank you.


* Alia Al Kasimi, http://ismir2007.ismir.net/proceedings/ISMIR2007_p355_kasimi.pdf
* Yuichiro YONEBAYASHI, http://hil.t.u-tokyo.ac.jp/research/introduction/PianoFingering/Yonebayashi2007IJCAI-article/index.html
---



キーボード演奏モーション自動生成プラグイン
---

作成/文責  ×＋

本ツールは MikuMikuMoving プラグインです。
MikuMikuMoving上に型として登録した運指ポーズを、標準MIDIファイル(SMF)から
解析した運指情報に従って、タイムライン上にコピー＆ペーストします。


MikuMikuMoving については、 [MikuMikuMoving開発ページ](https://sites.google.com/site/mikumikumoving/) 
をご参照ください。
なお、動作検証は version 1.2.7.2 で行っています。


## 免責事項

このプラグインは無保証です。ダウンロード、インストール、実行したことによるあら
ゆる不具合について、プラグイン製作者である×＋は責任を持ちません。利用する場合
は必ず自己責任でご利用ください。


## 本プラグインでできること

MikuMikuMovingの先頭18フレームに登録した指の型とMIDIデータ(SMF)からキーボード演奏モーショ
ンを生成します。作成したモーションはvmd形式等でMikuMikuDanceへ持ち込むことが可能です。

本稿では導入方法、使い方について解説します。
使い方としては、(1) PMXエディタでの作業：モデルの改造、(2) MikuMikuMovingでの作業：型の登録、
(3) キーボードモーション、キーボード演奏モーションの生成、となります。


## 配布物

* PMXプラグイン
  - 指切IKプラグイン　 (PmxPluginFingerIKmk4.dll)
* MikuMikuMovingプラグイン
  - キーボードモーション生成　 (Midi2PianoKey.dll)
  - キーボード演奏モーション生成　 (PianoMotionCreateMidi.dll)
  - 運指等コピー＆ペースト　 (CopyPasteMidi.dll)
* ボーン改造サンプル
  - 型サンプル画像　 (00.png〜18.png)
  - サンプルモーション　 (SampleAnimasaMiku.vmd)

PMXプラグインはPMXエディタの_plugin/Userフォルダにコピーしてください。
MikuMikuMovingプラグインはMikuMikuMovingのPluginsフォルダにコピーしてください。


## 依存関係

MIDIデータの読み込みに [Next MIDI Project](http://starway.s234.xrea.com/wordpress/) 
様の Next MIDI ライブラリを使用しています。
Next MIDI Project ホームページの「ダウンロード」からファイルをダウンロードおよ
び解凍し、MikuMikuMoving の Plugins フォルダに dll ファイルを保存してください。
なお、拙筆のテストした時点でのファイル名は nextmidi.zip です。


## インストール

PmxPluginFingerIKmk4.dllをPMXエディタの_plugin/Userフォルダに保存してください。

また、Midi2PianoKey.dll/PianoMotionCreateMidi.dll/CopyPasteMidi.dllを
MikuMikuMovingのPluginsフォルダに保存してください。


## 使用方法

使い方としては、(1) PMXエディタでの作業：モデルの改造、(2) MikuMikuMovingでの作業：型の登録、
(3) キーボードモーション、キーボード演奏モーションの生成、となります。


### (1) PMXエディタでの作業：モデルの改造

本プラグインは、対象のモデルが腕切IK化、指切IK化されていることを前提としています。
そのためモデルの改造を行います。
モデルの規約確認、バックアップは自己責任で実施してください。


・モデル改造 その1  

PMXエディタにて、T0R0様の [標準ボーンチェッカー](https://bowlroll.net/file/9611) 
を実行してください。
本プラグインが「～指先」ボーンを必須としているための下準備です。
利用したいモデルがすでに「～指先」ボーンを備えている場合は不要です。


・モデル改造 その2  

腕切IKに必要なボーンを確認します。
拙筆の利用しているツールとしては、そぼろ様の [準標準ボーン追加プラグイン](http://www.nicovideo.jp/watch/sm14956092) 
があります。
改造対象のモデルのセットアップ、腕切IK化に使用するツールによって必要とする準標
準ボーンが異なりますので、ご注意ください。
また、指切IK化にあたり、親指0を追加してください。


・モデル改造 その3  

腕切IK化を行います。
動作確認しているIK化ツールとしては、T0R0様の [腕IK作成](https://bowlroll.net/file/9482) 
になります。
T0R0様の腕IK作成を利用する場合は「腕IK（腕切りタイプ）」を選択してください。
現在、T0R0様の腕IK（腕切りタイプ）以外では動作しません。

※T0R0様の腕IK（腕切りタイプ）では「左手首」「右手首」を操作します。
「右腕IK」「左腕IK」は操作しません。  

※上半身の動作を入れる場合は、「センターW」「上半身W」「上半身2W」を操作します。  


・モデル改造 その4  

視線IKを導入します(オプション)。  
視線IKボーンが存在していれば、本ツールは上半身モーション作成時に視線も合わせて
生成します。
動作確認しているIK化ツールとしては、T0R0様の [視線IK作成](https://bowlroll.net/file/9826) 
になります。  


・モデル改造 その5  

上記改造を実施後、PMXエディタにて、「ピアノ等の演奏向け指IK_mk4」を
実行してください。
MEA様の指切指IK方式(mk4)を拙筆がプラグイン化したものです。
動作テストはあにまさ式ミクさんで行っています。
このプラグインは、指のIK化の他に、手首の多段化ボーン(～手首F, ～手首FP)を追加し
ます。
なお、準標準より拡張したボーン構成のモデルの場合は、処理に失敗します。  


※本プラグインはT0R0様の腕切IKを前提としていますが、IK Maker Xで作成した腕切IKで
も制限つき(手首の持ち上げ、キー押下時の手首ブレがない)で使用することができます。  

※IK Maker Xで作成した腕切IKの場合、PMXエディタで多段化ボーン(～手首F、～手首FP)
を再設定する必要があります。  

※既知の不具合  
特に親指で顕著ですが、大きく動かすとポリゴンが破綻しやすいようです。
対応策をお教えいただけるとありがたく思います。


### (2) MikuMikuMovingでの作業：型の登録

指の形、腕の位置を登録します。
キーフレーム位置に意味がありますので、サンプル画像、モーションを参考にモデルごと
に調整を行ってください。

「～指先」にて位置(XYZ)の調整、「～指3」にて角度(R)の調整を行います。


・キーフレーム登録 その1  

「右～指先」「右～指3」「左～指先」「左～指3」「右手首」「左手首」にキーフレーム
を登録します。


* 0 フレーム目：
  - 白鍵のみ。キーを押す前のデフォルト位置。
  - 右の場合、指先はC4～G4の位置にする。右手首はE4の位置にする。
  - 左の場合、指先はC3〜G3の位置にする。左手首はE3の位置にする。
* 1 フレーム目：
  - 白鍵のみ。キーを押した状態の指先位置。
  - 右の場合、親指をB、人差し指をC、中指をF、薬指をG、小指をAの位置にする。
  - 左の場合、小指をB、薬指をC、中指をD、人差し指をG、小指をAの位置にする。
  - 手首は0フレーム目と同じ。
* 2 フレーム目：
  - 白鍵のみ。キーを押した状態で広げた指先位置。
  - 右の場合、親指をB、人差し指をC、中指をF、薬指をG、小指をAの位置にする。
  - 手首は0フレーム目と同じ。
* 3 フレーム目：
  - 指くぐり。キーを押す前。
  - 右の場合、親指をDの位置に。人差指～小指は動かさない。
  - 右手首のXYZは0フレーム目と同じ、回転のみで［人指先：B、中指先：C、薬指
    先：D、小指先：E］の位置に来るようにすること。
  - 左の場合、親指をFの位置に。人差指〜小指は動かさない。
  - 左手首のXYZは0フレーム目と同じ、回転のみで［人差先：A、中指先：G、薬指
    先：F、小指先：E］の位置に来るようにすること。
* 4 フレーム目：
  - 指くぐり。キーを押した状態。親指のみ。
  - 手首位置は3フレーム目と同じ。
* 5 フレーム目：
  - 指くぐり。キーを押した状態。中指のみ。
  - 手首位置は3フレーム目と同じ。
* 6 フレーム目：
  - 白鍵(親, 小指先)＋黒鍵(人～薬指先)。キーを押す前。
  - 手首位置は0フレーム目に対し高さ、奥行きを調整。キー列方向に動かしてはならない。
* 7 フレーム目：
  - 白鍵(親, 小指先)＋黒鍵(人～薬指先)。キーを押した状態。
  - 手首は6フレーム目と同じ。
* 8 フレーム目：
  - 白鍵(親, 小指先)＋黒鍵(人～薬指先)。キーを押した状態で広げた指先位置。
  - 手首は6フレーム目と同じ。
* 10フレーム目：
  - 黒鍵のみ。キーを押す前。
  - 手首位置は6フレーム目に対し高さ、奥行きを調整。
    キー列方向は指が自然な位置になるよう半音程度の幅で調整する。
* 11フレーム目：
  - 黒鍵のみ。キーを押した状態の指先位置。globalでY方向に一括移動させると便利。
  - 手首は10フレーム目と同じ。
* 12フレーム目：
  - 黒鍵のみ。キーを押した状態で広げた指先位置。
  - 手首は10フレーム目と同じ。
* 13フレーム目：
  - 黒鍵(親, 小指先)＋白鍵(人～薬指先)。キーを押した状態の指先位置。
  - 手首は10フレーム目と同じ。
* 14フレーム目：
  - 黒鍵(親, 小指先)＋白鍵(人～薬指先)。キーを押した状態でキー列方向に移動した指先位置。
  - 手首は10フレーム目と同じ。
* 16フレーム目：
  - 手首を広げる。
  - 右は3オクターブ上へ、左は2オクターブ下へ。
* 18フレーム目：
  - 手首を持ち上げる。演奏中に持ち上げる場合の位置にする。
  - globalでY方向に一括移動させると便利。


・キーフレーム登録 その2  

その1の設定後、キーを押した瞬間の手首のブレを登録します。
「右手首F」「左手首F」について以下のキーフレーム登録を行ってください。

* 0 フレーム目：
  - 補完曲線の設定のみ行う。
* 1 フレーム目：
  - 手首を押した状態に下げる。globalでY方向に一括移動させると便利。
* 2 フレーム目：
  - 0フレーム目をコピー＆ペースト。


・キーフレーム登録 その3

ペダル操作の型を登録します。
ダンパー・ペダルは「右足IK」に、ソフト・ペダルは「左足IK」に登録します。

* 0 フレーム目：
  - 補完曲線の設定のみ行う。
* 1 フレーム目：
  - ペダルを踏んだ状態にする。
* 2 フレーム目：
  - 0フレーム目をコピー＆ペースト。


一旦ここでプロジェクト名をつけて保存してください。  

添付のサンプルモーションは、あにまさ式ミクさんで上記ポーズを作成しています。


### (3) キーボード演奏モーション自動生成プラグインの実行

・キーボードモーションの生成  

mqdl様、ろっし様、くるくる様(MNTY様)のキーボードで動作確認を行っています。
MikuMikuMovingの操作パネルから対象のキーボードモデルを選択し、リボンメニューのプラグインか
らキーボードモーションを選択します。
設定フォームが表示されるので、SMF(標準MIDIデータファイル)・チャネル・貼り付け開
始位置を指定してください。


1チャネルのみ登録するため、左手と右手それぞれのチャネルを実行する必要があります。
ペダルのトラックが別にあれば、ペダルトラックについても実行してください。

内部的にはSMFから取得したノート値でボーン名(ntDDD または A～Gn#)を選択、オン・
オフ時間にキーフレームを登録します。
キーはx軸回転、-0.04[rad]の固定値を設定しています。
ペダルも同様にx軸回転、-0.2[rad]の固定値を設定しています。  



・キーボード演奏モーションの生成  

MikuMikuMovingの操作パネルから対象のモデルを選択し、リボンメニューのプラグインからキーボー
ド演奏モーションを選択します。
設定フォームが表示されるので、SMF・左右のチャネル・貼り付け開始位置を指定してく
ださい。
「Target bones」はキーフレーム登録するボーンを指定します。
腕切IKをT0R0様のプラグインで作成した場合は、表示されているボーンのままOKボタンを
押下してください。
"IK Make X"で作成した腕切IKの場合は、手首水平移動を「左腕IK」「右腕IK」に選択し
なおしてください。


プラグインを実行すると、「〜指先」「〜指」「〜手首」「〜手首F」「〜手首FP」にキー
フレームを登録します。
上半身モーションは「センターW」「上半身W」「上半身2W」にキーフレームを登録します。


ペダルモーションを生成したくない場合は、「ペダル操作」チェックボックスからチェッ
クを外してください。
また、上半身のモーションを生成したくない場合は、「上半身モーション作成」チェック
ボックスからチェックを外してください。


モーションが作成されているかご確認ください。


## モーションの修正

運指の自動生成はまだ調整が不十分（十分にする方法がわかっていない…）のため、不自
然な運指になる場合が多々あります。
この運指は手作業での修正＋再自動生成が可能です。
上記(3)を実行時に、SMFと同じフォルダに「SMF名＋R.csv」「SMF名＋L.csv」ファイルを
生成しています。
このCSVファイルの5列目が運指、6列目が手首の位置となっています。
メモ帳やエクセル等で対象の運指、手首位置を書き換えてください。
再度プラグインを実行し、設定フォームで「Create fingering」のチェックを外すことで
CSVからモーションを生成します。
なお、この場合もSMFファイルを指定してください(内部的にはSMFを読み込みません)。


作成したモーションはvmd形式等でMikuMikuDanceへ持ち込むことが可能です。


本プラグインにより演奏モーション作成の敷居が少しでも下がれば嬉しく思います。


## 謝辞

指切IKの構造についてはMEA様から詳細をご教唆いただきました。
この場をお借りして、お礼申し上げます。


また、運指の生成には以下の論文を参照しました。
この場をお借りして、お礼申し上げます。（敬称略）

* Alia Al Kasimi, http://ismir2007.ismir.net/proceedings/ISMIR2007_p355_kasimi.pdf
* 米林裕一郎, http://hil.t.u-tokyo.ac.jp/research/introduction/PianoFingering/Yonebayashi2007IJCAI-article/index.html


## 更新履歴

PianoMotionCreateMidi.dll  
* Version 1.01 2016-07-17
  - 手首持ち上げ時の角度付けに対応
* Version 1.00 2016-05-03
  - トリル判定を追加
  - 上半身のモーションを修正
* Version 0.99 2015-10-18
  - 6和音以上同時フィンガリングで手首持ち上げ(手首FP)のバグを修正
  - 上半身のモーション(テスト版)を追加(センターW,上半身W,上半身2W 対応)
* Version 0.92 2015-07-03
  - 6和音以上同時フィンガリングで応答が無くなるバグを修正
  - ペダル対応(MIDI control 64対応)
* Version 0.91 2015-07-01
  - 5和音同時フィンガリングで応答が無くなるバグを修正
  - 手首持ち上げモーション(～手首FP)の補間曲線を修正
* Version 0.9 2015-06-25
  - キーフレーム生成ボーンをフォームから指定できるように改修
  - 分散和音時で手首ブレを生成しないバグを修正
* Version 0.8 2015-06-17
  - 最初の公開バージョン


Midi2PianoKey.dll  
* Version 1.0 2016-04-29
  - キー名(A～Gn#)への対応を追加
* Version 0.9 2015-07-03
  - ペダル対応(MIDI control 64対応)
* Version 0.8 2015-06-17
  - 最初の公開バージョン


CopyPasteMidi.dll
* Version 1.0 2016-05-03
  - 変拍子のSMFに対応
* Version 0.4 2015-02-14
  - 最初の公開バージョン


PmxPluginFingerIKmk4.dll  
* Version 0.9 2015-06-25
  - "IK Maker X"による腕切IK改造モデルでのエラーを修正
* Version 0.8 2015-06-19
  - 最初の公開バージョン
