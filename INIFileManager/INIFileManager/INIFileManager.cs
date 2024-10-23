using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace INIFilManager
{
    public class INI
    {
        private string filename = Environment.CurrentDirectory + "\\iniFile.ini";
        private string path = Environment.CurrentDirectory;
        //Ini 쓰기 WinApi선언
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        //Ini 읽기 WinApi선언
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, int Key, string Value, [MarshalAs(UnmanagedType.LPArray)] byte[] Result, int Size, string FileName);


        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(int Section, string Key, string Value, [MarshalAs(UnmanagedType.LPArray)] byte[] Result, int Size, string FileName);

        /// <summary>
        /// Override Constructor
        /// </summary>
        /// <param name="INIFileName">File name including path.</param>
        public INI(string INIFileName)
        {
            SetFileName(INIFileName);
        }
        /// <summary>
        /// Default Constructor
        /// </summary>
        public INI()
        {
        }

        /// <summary>
        /// File name setting.
        /// </summary>
        /// <param name="INIFileName">File name including path.</param>
        public void SetFileName(string INIFileName)
        {
            this.filename = INIFileName;
            string[] filepath = filename.Split('\\');
            string createfilepath = "";
            for (int i = 0; i < filepath.Length - 1; i++)
            {
                createfilepath += (filepath[i] + (i != filepath.Length - 2 ? "\\" : ""));
            }
            if (createfilepath == "")
            {
                this.path = Environment.CurrentDirectory;
                this.filename = Environment.CurrentDirectory + "\\" + this.filename;
            }
            else { this.path = createfilepath; }
        }

        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <param name="Section">Section</param>
        /// <param name="Key">Key</param>
        /// <param name="Value">Value</param>
        public void IniWriteValue(string Section, string Key, string Value)
        {
            CreateFolder(this.path);
            WritePrivateProfileString(Section, Key, Value, this.filename);
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <param name="Section">Section</param>
        /// <param name="Key">Key</param>
        /// <returns></returns>
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            if (!File.Exists(this.filename)) { return ""; }
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.filename);
            return temp.ToString();
        }


        /// <summary>
        /// Read all sections
        /// </summary>
        /// <returns></returns>
        public string[] GetSectionNames()  // ini 파일 안의 모든 section 이름 가져오기
        {
            for (int maxsize = 500; true; maxsize *= 2)
            {
                byte[] bytes = new byte[maxsize];
                int size = GetPrivateProfileString(0, "", "", bytes, maxsize, filename);


                if (size < maxsize - 2)
                {
                    string Selected = Encoding.Default.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
                    return Selected.Split(new char[] { '\0' });
                }
            }
        }


        /// <summary>
        /// Read all keys
        /// </summary>
        /// <param name="section">Section</param>
        /// <returns></returns>
        public string[] GetEntryNames(string section)   // 해당 section 안의 모든 키 값 가져오기
        {
            for (int maxsize = 500; true; maxsize *= 2)
            {
                byte[] bytes = new byte[maxsize];
                int size = GetPrivateProfileString(section, 0, "", bytes, maxsize, filename);

                if (size < maxsize - 2)
                {
                    string entries = Encoding.Default.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
                    return entries.Split(new char[] { '\0' });
                }
            }
        }

        private void CreateFolder(string path)
        {
            try
            {
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
