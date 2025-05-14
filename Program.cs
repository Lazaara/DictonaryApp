using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace DictionaryAppSingleFile
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DictionaryForm());
        }
    }

    public class DictionaryForm : Form
    {
        private TextBox textBoxTerm;
        private Button buttonSearch;
        
        private TextBox textBoxMeaning;
        
        private ListBox listBoxHistory;
        private Label listBoxLabel;
        private Button buttonClearHistory;
        private TextBox addWordK;
        private TextBox addWordV;
        private Button addWord;
        private Label labelAddWordK;
        private Label labelAddWordV;
        private Label labelAddWordStatus;
        private TextBox removeWord;
        private Button removeWordButton;
        private Label removeWordLabel;
        private Label labelRemoveWordStatus;
        private Label labelManageWords;
        

        private DictionaryManager dictManager;
        private HistoryList historyList; // Line 29
        private FileHandler fileHandler;

        private const string DictFileName = "dictionary.txt";
        private const string HistFileName = "history.txt";

        public DictionaryForm()
        {
            // Khởi tạo các thành phần logic
            dictManager = new DictionaryManager();
            historyList = new HistoryList(); // Line 39
            fileHandler = new FileHandler();

            // Thiết lập form
            this.Text = "Từ điển Anh - Việt";
            this.ClientSize = new Size(880, 665);
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.BackgroundImage = Image.FromFile("assets/bg.jpg");

            // Tạo và cấu hình controls
            textBoxTerm = new TextBox();
            textBoxTerm.Location = new Point(10, 18);
            textBoxTerm.Width = 730;

            buttonSearch = new Button();
            buttonSearch.Text = "Tìm kiếm";
            buttonSearch.Location = new Point(750, 18);
            buttonSearch.Width = 120;
            buttonSearch.Height = 30;
            
            textBoxMeaning = new TextBox();
            textBoxMeaning.Text = "Kết quả tra cứu sẽ được hiện ở đây";
            textBoxMeaning.ReadOnly = true;
            textBoxMeaning.Location = new Point(10, 55);
            textBoxMeaning.Width = 860;
            textBoxMeaning.Height = 145;
            textBoxMeaning.Multiline = true;
            
            labelManageWords = new Label();
            labelManageWords.Text = "Quản lý từ vựng:";
            labelManageWords.Location = new Point(10, 210);
            labelManageWords.AutoSize = true;
            
            labelAddWordK = new Label();
            labelAddWordK.Text = "Từ";
            labelAddWordK.Location = new Point(10, 235);
            labelAddWordK.AutoSize = true;
            
            labelAddWordV = new Label();
            labelAddWordV.Text = "Nghĩa";
            labelAddWordV.Location = new Point(165, 235);
            labelAddWordV.AutoSize = true;
            
            labelAddWordStatus = new Label();
            labelAddWordStatus.Text = "";
            labelAddWordStatus.Location = new Point(322, 235);
            labelAddWordStatus.Visible = false;

            addWordK = new TextBox();
            addWordK.Location = new Point(10, 260);
            addWordK.Width = 150;
            
            addWordV = new TextBox();
            addWordV.Location = new Point(165, 260);
            addWordV.Width = 150;

            addWord = new Button();
            addWord.Text = "Thêm từ";
            addWord.Location = new Point(320, 260);
            addWord.Width = 100;
            addWord.Height = 30;
            
            removeWord = new TextBox();
            removeWord.Location = new Point(435, 260);
            removeWord.Width = 330;
            
            removeWordButton = new Button();
            removeWordButton.Text = "Xoá từ";
            removeWordButton.Location = new Point(770, 260);
            removeWordButton.Width = 100;
            removeWordButton.Height = 30;
            
            removeWordLabel = new Label();
            removeWordLabel.Text = "Xoá từ vựng khỏi Dictonary";
            removeWordLabel.Location = new Point(435, 235);
            removeWordLabel.AutoSize = true;
            
            labelRemoveWordStatus = new Label();
            labelRemoveWordStatus.Text = "";
            labelRemoveWordStatus.Location = new Point(772, 235);
            labelRemoveWordStatus.Visible = false;

            
            listBoxLabel = new Label();
            listBoxLabel.Text = "Lịch sử tra cứu:";
            listBoxLabel.Location = new Point(10, 295);
            listBoxLabel.AutoSize = true;
            
            
            listBoxHistory = new ListBox();
            listBoxHistory.Location = new Point(10, 320);
            listBoxHistory.Size = new Size(860, 300);

            buttonClearHistory = new Button();
            buttonClearHistory.Text = "Xoá lịch sử";
            buttonClearHistory.Location = new Point(10, 625);
            buttonClearHistory.Width = 100;
            buttonClearHistory.Height = 30;

            // Đăng ký sự kiện
            buttonSearch.Click += new EventHandler(OnSearchClick);
            buttonClearHistory.Click += new EventHandler(OnClearHistoryClick);
            addWord.Click += new EventHandler(AddNewWord);
            removeWordButton.Click += new EventHandler(RemoveWord);
            this.FormClosing += new FormClosingEventHandler(OnFormClosing);

            // Thêm controls vào form
            this.Controls.Add(textBoxTerm);
            this.Controls.Add(buttonSearch);
            this.Controls.Add(textBoxMeaning);
            this.Controls.Add(listBoxHistory);
            this.Controls.Add(buttonClearHistory);
            this.Controls.Add(addWordK);
            this.Controls.Add(addWordV);
            this.Controls.Add(addWord);
            this.Controls.Add(labelAddWordK);
            this.Controls.Add(labelAddWordV);
            this.Controls.Add(labelAddWordStatus);
            this.Controls.Add(listBoxLabel);
            this.Controls.Add(removeWord);
            this.Controls.Add(removeWordButton);
            this.Controls.Add(removeWordLabel);
            this.Controls.Add(labelRemoveWordStatus);
            this.Controls.Add(labelManageWords);

            foreach (Control ctrl in Controls)
            {
                if (ctrl != null)
                {
                    ctrl.Font = new Font(ctrl.Font.FontFamily, 12, FontStyle.Regular);
                    ctrl.BackColor = Color.White;
                    ctrl.ForeColor = Color.Black;
                    // Workaround để đổi màu readonly textbox
                    ctrl.BackColor = ctrl.BackColor;
                }
            }

            // Load dữ liệu từ điển và lịch sử
            dictManager.LoadFromFile(DictFileName);
            string[] saved = fileHandler.ReadAllLines(HistFileName);
            for (int i = 0; i < saved.Length; i = i + 1)
            {
                historyList.Add(saved[i]);
            }

            RefreshHistoryDisplay();
        }

        private void RemoveWord(object sender, EventArgs e)
        {
            
            string input = removeWord.Text;
            removeWord.Text = "";
            if (dictManager.dict.ContainsKey(input))
            {
                fileHandler.RemoveContainedLines(DictFileName, input);
                dictManager.dict.Remove(input);
                labelRemoveWordStatus.Text = "Thành công!";
                
            }
            else
            {

                labelRemoveWordStatus.Text = "Lỗi!";

            }

            labelRemoveWordStatus.Visible = true;
        }

        private void AddNewWord(object sender, EventArgs e)
        {
            
            string key = addWordK.Text;
            string value = addWordV.Text;
            addWordV.Text = "";
            addWordK.Text = "";
            fileHandler.AddLine(DictFileName, $"{key}|{value}");
            dictManager.LoadFromFile(DictFileName);
            labelAddWordStatus.Text = "Thành công!";
            labelAddWordStatus.Visible = true;

        }

        private void OnSearchClick(object sender, EventArgs e)
        {
            string term = textBoxTerm.Text.Trim();
            string meaning = dictManager.Lookup(term);
            textBoxMeaning.Text = meaning;
            if (meaning != "Không tìm thấy")
            {
                historyList.Add(term);
                RefreshHistoryDisplay();
            }
        }

        private void OnClearHistoryClick(object sender, EventArgs e)
        {
            historyList.Clear();
            RefreshHistoryDisplay();
        }

        private void RefreshHistoryDisplay()
        {
            listBoxHistory.Items.Clear();
            string[] arr = historyList.GetAll();
            for (int i = 0; i < arr.Length; i = i + 1)
            {
                listBoxHistory.Items.Add(arr[i]);
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            string[] arr = historyList.GetAll();
            fileHandler.WriteAllLines(HistFileName, arr);
        }
    }

    public class FileHandler
    {
        public string[] ReadAllLines(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return File.ReadAllLines(path);
                }
                else
                {
                    File.WriteAllText(path, null);
                    return new string[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đọc file: " + ex.Message);
                return new string[0];
            }
        }

        public void AddLine(string path, string input)
        {
            try
            {

                if (File.Exists(path))
                {
                    
                    File.AppendAllText(path, input + Environment.NewLine);
                    
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi file.");
            }
        }

        public void RemoveContainedLines(string path, string input)
        {
            try
            {

                if (File.Exists(path))
                {
                    
                    List<string> lines = File.ReadAllLines(path).ToList();
                    foreach (string line in lines)
                    {
                        if (line.Contains(input))
                        {
                            lines.Remove(line);
                            string[] newFile = lines.ToArray();
                            WriteAllLines(path, newFile);
                            break;
                        }
                        
                    }
                    
                }
                else
                {
                    File.WriteAllText(path, null);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi file.");
            }
        }

        public void WriteAllLines(string path, string[] lines)
        {
            try
            {
                File.WriteAllLines(path, lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi file: " + ex.Message);
            }
        }
    }

    public class DictionaryManager
    {
        public Dictionary dict;
        private FileHandler fileHandler;

        public DictionaryManager()
        {
            dict = new Dictionary();
            fileHandler = new FileHandler();
        }

        public void LoadFromFile(string path)
        {
            string[] lines = fileHandler.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i = i + 1)
            {
                string[] parts = lines[i].Split(new char[] { '|' });
                if (parts.Length == 2 && !dict.ContainsKey(parts[0]))
                {
                    dict.Add(parts[0], parts[1]);
                }
            }
        }

        public string Lookup(string term)
        {
            if (dict.ContainsKey(term))
            {
                return dict.GetValue(term);
            }

            return "Không tìm thấy";
        }
    }

    public class HistoryNode
    {
        public string Data;
        public HistoryNode Next;
    }

    public class HistoryList
    {
        private HistoryNode head;

        public HistoryList()
        {
            head = null;
        }

        public void Add(string term)
        {
            HistoryNode newNode = new HistoryNode { Data = term, Next = head };
            head = newNode;
        }

        public void Clear()
        {
            head = null;
        }

        public string[] GetAll()
        {
            List<string> history = new List<string>();
            HistoryNode current = head;
            while (current != null)
            {
                history.Add(current.Data);
                current = current.Next;
            }
            return history.ToArray();
        }
    }
}