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
        private Label labelMeaning;
        private ListBox listBoxHistory;
        private Button buttonClearHistory;

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
            this.Text = "DictionaryApp";
            this.ClientSize = new Size(350, 300);

            // Tạo và cấu hình controls
            textBoxTerm = new TextBox();
            textBoxTerm.Location = new Point(10, 10);
            textBoxTerm.Width = 200;

            buttonSearch = new Button();
            buttonSearch.Text = "Search";
            buttonSearch.Location = new Point(220, 10);
            buttonSearch.Width = 80;

            labelMeaning = new Label();
            labelMeaning.Text = "Meaning";
            labelMeaning.Location = new Point(10, 50);
            labelMeaning.AutoSize = true;

            listBoxHistory = new ListBox();
            listBoxHistory.Location = new Point(10, 80);
            listBoxHistory.Size = new Size(300, 150);

            buttonClearHistory = new Button();
            buttonClearHistory.Text = "Clear History";
            buttonClearHistory.Location = new Point(10, 240);
            buttonClearHistory.Width = 100;

            // Đăng ký sự kiện
            buttonSearch.Click += new EventHandler(OnSearchClick);
            buttonClearHistory.Click += new EventHandler(OnClearHistoryClick);
            this.FormClosing += new FormClosingEventHandler(OnFormClosing);

            // Thêm controls vào form
            this.Controls.Add(textBoxTerm);
            this.Controls.Add(buttonSearch);
            this.Controls.Add(labelMeaning);
            this.Controls.Add(listBoxHistory);
            this.Controls.Add(buttonClearHistory);

            // Load dữ liệu từ điển và lịch sử
            dictManager.LoadFromFile(DictFileName);
            string[] saved = fileHandler.ReadAllLines(HistFileName);
            for (int i = 0; i < saved.Length; i = i + 1)
            {
                historyList.Add(saved[i]);
            }

            RefreshHistoryDisplay();
        }

        private void OnSearchClick(object sender, EventArgs e)
        {
            string term = textBoxTerm.Text.Trim();
            string meaning = dictManager.Lookup(term);
            labelMeaning.Text = meaning;
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
        private Dictionary dict;
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