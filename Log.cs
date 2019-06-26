using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PianoMotionCreateMidi
{
    /// <summary>
    /// デバグ用ログクラス
    /// 引用元を失念...
    /// </summary>
    public class Log
    {
        public const int PRINT_ALL = 9;
        public const int PRINT_DEBUG = 8;
        public const int PRINT_INFO = 6;
        public const int PRINT_WARN = 4;
        public const int PRINT_SEVERE = 2;
        public const int PRINT_OFF = 0;

        public static int DEBUG_LEVEL = PRINT_OFF;

        /// <summary>
        /// ログをマイドキュメントにMMMScript.logのファイル名で出力する
        /// </summary>
        /// <param name="str">出力文字列</param>
        public static void Print(string str)
        {

            // string logname = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string logname = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strExt = Path.GetFileName(logname);
            logname = logname.Substring(0, logname.Length - strExt.Length);
#if DEBUG
            logname = System.IO.Directory.GetCurrentDirectory();
#endif

            if (!logname.EndsWith(@"\"))
            {
                logname = logname + @"\";
            }
            logname = logname + "_MMMScript.log";

            Encoding enc = Encoding.GetEncoding("Shift_JIS");
            System.IO.StreamWriter writer = new System.IO.StreamWriter(logname, true, enc);

            writer.Write("[");
            writer.Write(String.Format("{0:yy/MM/dd HH:mm:ss}", DateTime.Now));
            writer.Write("]");
            writer.WriteLine(str);
            writer.Close();
        }

        /// <summary>
        /// ログをマイドキュメントにPMDScript.logのファイル名で出力する（書式付き）
        /// </summary>
        /// <param name="fmt">.NETの書式文字列</param>
        /// <param name="args">値</param>
        public static void Print(string fmt, params object[] args)
        {
            Print(string.Format(fmt, args));
        }

        public static void Debug(string fmt, params object[] args)
        {
            if (DEBUG_LEVEL >= PRINT_DEBUG)
            {
                Print(fmt, args);
            }
        }
    }
}
