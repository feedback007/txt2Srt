using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Txt2SRT
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
#if DEBUG
            TxtFilePath = @"D:\Documents\GitHub\txt2Srt\bin\text.txt";
            SrtFilePath = @"D:\Documents\GitHub\txt2Srt\bin\text.srt";
#endif
            this.richTextBox1.Text = string.Empty;
        }

        #region prop
        string _TxtFilePath = "";
        public string TxtFilePath
        {
            get
            {
                return _TxtFilePath;
            }
            set
            {
                _TxtFilePath = value;
                this.SrtFilePath = System.IO.Path.ChangeExtension(_TxtFilePath, "srt");
                this.tbSrtFile1.Text = this.SrtFilePath;//symn
            }
        }
        public string SrtFilePath { get; set; }

        #endregion


        private void btnOpenTxt1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "*.txt|*.txt";
                dlg.DefaultExt = "txt";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.tbTxtFile1.Text = dlg.FileName;
                    this.TxtFilePath = dlg.FileName;//save to prop
                }
            }
        }

        private void btnSrtFile1_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "*.srt|*.srt";
                dlg.DefaultExt = "srt";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.tbSrtFile1.Text = dlg.FileName;
                    this.SrtFilePath = dlg.FileName;//save to prop
                }

            }
        }

        private void btnTrans_Click(object sender, EventArgs e)
        {
            bool bResult = false;
            //check  is ok
            if (tcMain.SelectedIndex == 0)
            {
                if (string.IsNullOrEmpty(this.TxtFilePath))
                {
                    MessageBox.Show("The Txt File is NOT exist!Please Check it!");
                    return;
                }
                if (string.IsNullOrEmpty(this.SrtFilePath))
                {
                    MessageBox.Show("The SRT File Path is NOT exist!Please Check it!");
                    return;
                }
                ReadTxt2Mem();
                bResult = DoTranlate();
            }
            else
            {
                this.SrtFilePath = this.tbSrtFile2.Text;

                if (string.IsNullOrEmpty(this.richTextBox1.Text))
                {
                    MessageBox.Show("The text is NOT exist!Please Check it!");
                    return;
                }
                if (string.IsNullOrEmpty(this.SrtFilePath))
                {
                    MessageBox.Show("The SRT File Path is NOT exist!Please Check it!");
                    return;
                }

                ReadRichtxt2Mem();
                bResult = DoTranlate();
            }

            MessageBox.Show(bResult ? "Succeed!" : "Faild!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 从界面读取内容
        /// </summary>
        private void ReadRichtxt2Mem()
        {
            
            try
            {
                _ListTxt.Clear();
                _ListTxt.AddRange(this.richTextBox1.Lines);
            }
            finally
            {
                 
            }
        }

        List<string> _ListTxt = new List<string>();//txt cache
        List<string> _ListSrt = new List<string>();//srt cache

        private void ReadTxt2Mem()
        {
            string[] strsAry = System.IO.File.ReadAllLines(this.TxtFilePath);
            try
            {
                _ListTxt.Clear();
                _ListTxt.AddRange(strsAry);
            }
            finally
            {
                strsAry = null;
            }


        }
        bool IsTime(string StrSource)
        {
            //1:20:32 or 4:56
            return Regex.IsMatch(StrSource, @"^([0-9]?\d:[0-5]?\d)$")
                || Regex.IsMatch(StrSource, @"^((20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$");
        }

        private bool DoTranlate()
        {
            _ListSrt.Clear();
            bool bHasTime = false;
            int lastTimeIdx = -1;
            int currentTimeIdx = -1;
            int iCnt = 0;
            for (int i = 0; i < _ListTxt.Count; i++)
            {
                string line = _ListTxt[i];
                //check if time
                if (IsTime(line))
                {
                    bHasTime = true;

                    if (lastTimeIdx == -1)
                        lastTimeIdx = i;
                    if (currentTimeIdx != -1)
                    {
                        iCnt++;
                        _ListSrt.Add(iCnt.ToString());//the srt index
                        //do something about time 
                        string strLastTime = _ListTxt[currentTimeIdx];
                        string strCurrentTime = _ListTxt[i];
                        string timeStr = string.Format("{0},000 --> {1},000", strLastTime, strCurrentTime);
                        _ListSrt.Add(timeStr);
                        //do someting about context
                        string[] aryTmp = new string[i - currentTimeIdx - 1];
                        _ListTxt.CopyTo(currentTimeIdx + 1, aryTmp, 0, aryTmp.Length);
                        _ListSrt.AddRange(aryTmp);

                        _ListSrt.Add(string.Empty);
                        //for (int j = lastTimeIdx; i < currentTimeIdx; j++)
                        //{
                        //}
                        lastTimeIdx = currentTimeIdx;
                    }
                    currentTimeIdx = i;//sync the index to current
                    continue;
                }
                if (bHasTime == false) continue;//first line is contine


            }

            SaveToSRT();

            return _ListSrt.Count != 0;
        }

        private void SaveToSRT()
        {
            //check it?NO
            System.IO.File.WriteAllLines(this.SrtFilePath, _ListSrt.ToArray(), System.Text.Encoding.UTF8);
        }

        private void FormMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                if (path.EndsWith("txt", StringComparison.OrdinalIgnoreCase))
                    e.Effect = DragDropEffects.All;
            }

            else
                e.Effect = DragDropEffects.None;

        }

        private void FormMain_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            if (path.EndsWith("txt", StringComparison.OrdinalIgnoreCase))
            {
                if (tcMain.SelectedIndex == 0)
                {
                    this.tbTxtFile1.Text = path;
                    this.TxtFilePath = path;
                }

            }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "*.srt|*.srt";
                dlg.DefaultExt = "srt";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.tbSrtFile2.Text = dlg.FileName;
                    this.SrtFilePath = dlg.FileName;//save to prop
                }

            }
        }
    }
}
