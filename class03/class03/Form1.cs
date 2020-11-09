using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace class03
{
    public partial class Form1 : Form
    {
        private string[] fileNamesWithPath = null;
        string path = null;
        private List<int> countBlankLineList = new List<int>();
        private List<int> countValidLineList = new List<int>();
        private List<int> countTotalLineList = new List<int>();
        private List<string> fileNameListWithoutPath = new List<string>();
        private string regexBlankLine = @"^\s*$"; // 空行
        public Form1()
        {
            InitializeComponent();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Select Source Codes:";
            openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            openFile.Multiselect = true;
            openFile.Filter =
                "C# (*.cs)|*.cs|" +
                "C++ (*.cpp; *.h)|*.cpp;*.h|" +
                "C (*.c)|*.c|" +
                "Java (*.java)|*.java|" +
                "Python (*.py)|*.py";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                fileNamesWithPath = openFile.FileNames;
                path = Path.GetFullPath(openFile.FileName);
                CountSourceCodes();
                label1.Text = CountLines(path).ToString();
                label2.Text = getWordSum(path).ToString();
            }
        }

        private void CountSourceCodes()
        {
            foreach (string fileName in fileNamesWithPath)
            {
                CountLines(fileName);
            }
        }

        private int CountLines(string path)
        {
            string[] codeLines = File.ReadAllLines(path);
            fileNameListWithoutPath.Add(Path.GetFileName(path));

            int countBlankLine = 0;
            int countValidLine = 0;
            int countTotalLine = codeLines.Length;
            foreach (string line in codeLines)
            {
                if (Regex.IsMatch(line, regexBlankLine))
                {
                    countBlankLine++;
                }
            }
            countValidLine = countTotalLine - countBlankLine; 
            countBlankLineList.Add(countBlankLine);
            countValidLineList.Add(countValidLine);
            countTotalLineList.Add(countTotalLine);
            return countValidLine;
        }
        private int getWordSum(string path)
        {
            string text = File.ReadAllText(path, Encoding.UTF8);
            fileNameListWithoutPath.Add(Path.GetFileName(path));
            string textbasic = text;
            char[] basictemp = text.ToCharArray();
            int chfrom = Convert.ToInt32("4e00", 16);
            int chend = Convert.ToInt32("9fff", 16);
            foreach (char c in basictemp)
            {
                if (' ' != c)
                {
                    string temp = c.ToString();
                    int firstcode = char.ConvertToUtf32(temp, 0);
                    if (firstcode >= chfrom && firstcode <= chend)
                    {
                        textbasic = textbasic.Replace(c, ' ');
                    }
                }
            }
            char[] ch = new char[] { ' ', ',', '?', '!', '(', ')', '\n' };
            string[] stemp = textbasic.Split(ch, StringSplitOptions.RemoveEmptyEntries);
            return stemp.Length;
        }

        static void ReadFileInfo(string path, List<string> info)
        {
            string str = string.Empty;
            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                while ((str = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (str.Trim().Length != 0)
                        {
                            if (str.IndexOf("//") != 0)
                            {
                                if (str.IndexOf("//") != -1 && !str.Contains('"'))
                                {
                                    str = str.Substring(0, str.IndexOf("//"));
                                    if (str.Trim().Length != 0)
                                        info.Add(str);
                                }
                                else
                                {
                                    info.Add(str);
                                }
                            }
                        }
                    }
                }
            }
        }
        static void WriterFileInfo(string path, List<string> info)
        {
            using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                for (int i = 0; i < info.Count; i++)
                {
                    writer.WriteLine(info[i]);
                }
            }
        }

        public static void getWordList(string path, ref Hashtable wordList)     //getWordList：从文本文件中统计词频保存在Hashtable中
        {
            StreamReader sr = new StreamReader(path);
            string line;
            int num = 0;
            line = sr.ReadLine();             //按行读取
            while (line != null)
            {
                num++;
                MatchCollection mc;
                Regex rg = new Regex("[A-Za-z-]+");    //用正则表达式匹配单词
                mc = rg.Matches(line);
                for (int i = 0; i < mc.Count; i++)
                {
                    string mcTmp = mc[i].Value.ToLower();    //大小写不敏感
                    if (mcTmp.Length >= 0)
                    {
                        if (!wordList.ContainsKey(mcTmp))     //第一次出现则添加为Key
                        {
                            wordList.Add(mcTmp, 0);
                        }
                        else                                  //不是第一次出现则Value加1
                        {
                            int value = (int)wordList[mcTmp];
                            value++;
                            wordList[mcTmp] = value;
                        }
                    }
                    else
                        continue;
                }
                line = sr.ReadLine();
            }
            sr.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
            List<string> list = new List<string>();
            ReadFileInfo(path, list);
            WriterFileInfo(path, list);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string text = File.ReadAllText(path, Encoding.UTF8);
            Hashtable ha = new Hashtable();
            getWordList(path, ref ha);
            //遍历哈希表
            foreach (DictionaryEntry de in ha)
            {
                int v = Convert.ToInt32(de.Value)+1;
                richTextBox1.AppendText(de.Key + ":" + Convert.ToString(v) + "\n");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

