using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VivConverter
{
    public partial class Form1 : Form
    {
        public string viv = "", json = ""; //ファイル全体をstring化
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) //開くボタンの処理
        {
            DialogResult diar = openFileDialog1.ShowDialog();
            if (diar == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e) //変換ボタンの処理
        {
            try
            {
                using (StreamReader strr = new StreamReader(textBox1.Text, Encoding.GetEncoding("UTF-8")))
                {
                    viv = strr.ReadToEnd(); //ファイルロード
                    viv = Regex.Replace(viv, "\n{1,}", "\n"); //余分な空行は読まない
                }

                int ConCheck = ConvJson(); //変換処理
                if (ConCheck == 0)
                {
                    Saver(0); //保存処理, 0=json
                }
            }
            catch
            {
                label2.Text = "ファイルの変換に失敗しました。";
            }
        }

        //====================================================================================================
        //Converter(.viv -> .json)
        //====================================================================================================
        public int ConvJson()
        {
            string[] sep = new string[] { "\r\n" }; //改行コード
            string[] VivLines = viv.Split(sep, StringSplitOptions.RemoveEmptyEntries); //改行コードで分割
            string[] JsonLines = new string[VivLines.Length];

            int LPB = 0, block = 0, num = 0, type = 0; //.jsonより

            int[] NoteState = new int[128]; //1行当たりのノーツ数

            JsonLines[0] = "{\"name\":\"" + VivLines[0] + "\",\"maxBlock\":5,"; //nameの変換
            JsonLines[1] = "\"BPM\":" + VivLines[1] + ","; //ここからノーツ ヘッダ結合
            JsonLines[2] = "\"offset\":" + VivLines[2] + ",\"notes\":[";

            //--------------------------------------------------

            for (int i = 3; i < VivLines.Length; i++) //4行目から1行ずつVivLinesを読む
            {
                block = 0;
                string[] NoteTemp = VivLines[i].Split(',', StringSplitOptions.RemoveEmptyEntries); //コンマで区切る
                string[] JsonShots = new string[NoteTemp.Length];
                int one, ten, hun, tho; //1,10,100,1000の位
                LPB = NoteTemp.Length; //配列長から拍数を算出

                for (int j = 0; j < LPB; j++) //1拍ごとのアレ
                {
                    NoteState[j] = Convert.ToInt32(NoteTemp[j]); //stringのint化
                    one = NoteState[j] % 10;
                    ten = (NoteState[j] % 100 - one)/10;
                    hun = (NoteState[j] % 1000 - one - ten)/100;
                    tho = (NoteState[j] % 10000 - one - ten - hun)/1000;

                    if (one == 0) //0だったらすっとばし
                    {
                        
                    }
                    else if (tho == 0)
                    {
                        block = one;
                        num = LPB * (i - 2) + j;
                        type = ten;
                        JsonShots[j] = "{\"LPB\":" + LPB + ",\"block\":" + block + ",\"notes\":[],\"num\":" + num + ",\"type\":"+type+"}";
                    }
                    else
                    {
                        block = hun;
                        num = LPB * (i - 2) + j;
                        type = tho;
                        JsonShots[j] = "{\"LPB\":" + LPB + ",\"block\":" + block + ",\"notes\":[],\"num\":" + num + ",\"type\":" + type + "},";
                        block = one;
                        num = LPB * (i - 2) + j;
                        type = ten;
                        JsonShots[j] = JsonShots[j] + "{\"LPB\":" + LPB + ",\"block\":" + block + ",\"notes\":[],\"num\":" + num + ",\"type\":" + type + "}";
                    }

                }

                if (i != VivLines.Length - 1)
                {
                    JsonLines[i] = string.Join(",", JsonShots) + ",";
                }
                else
                {
                    JsonLines[i] = string.Join(",", JsonShots);
                }
               
            }

            //--------------------------------------------------

            json = string.Join("", JsonLines); //行結合
            json = json + "]}"; //フッタ結合
            label1.Text = json;
            label2.Text = VivLines.Length.ToString("D");

            return 0;
        }

        //====================================================================================================
        //Saver(.viv -> .json)
        //====================================================================================================

        public int Saver(int i)
        {
            string[] Dir = textBox1.Text.Split('.'); //ディレクトリを取得しピリオドで分割
            switch (i)
            {
                case 0:
                    {
                        Dir[Dir.Length - 1] = "json"; //拡張子を.jsonに変更
                        using (StreamWriter strr = new StreamWriter(string.Join(".", Dir), false, Encoding.GetEncoding("UTF-8")))
                        {
                            strr.WriteLine(json); //.jsonファイルセーブ
                        }
                        break;
                    }
                case 1:
                    {
                        Dir[Dir.Length - 1] = "viv"; //拡張子を.vivに変更
                        using (StreamWriter strr = new StreamWriter(string.Join(".", Dir), false, Encoding.GetEncoding("UTF-8")))
                        {
                            strr.WriteLine(viv); //.vivファイルセーブ
                        }
                        break;
                    }
            }
            return 0;
        }
    }
}
