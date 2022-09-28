using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TextFileSln.UI
{
    public partial class WordStatsForm : Form
    {
        private Dictionary<string, WordStat> _stats;
        private int _totalCount;

        //Конструктор формы
        public WordStatsForm(string text)
        {
            InitializeComponent();
            CalculateWordsCount(text);
            FillTable();
            SetTotalCount();
        }

        //Разделители
        static readonly char[] separators = new char[] 
        {
            '.', ',', ':', ';', ' ', '(', ')', '-', '?', '!'
        };


        ////Считает кол-во слов в тексте
        //private Dictionary<string, WordStat> CalculateWordsCount(string text)
        //{
        //    string[] words = text.Split(separators)
        //        .Where((s) => !string.IsNullOrEmpty(s))
        //        .ToArray();

        //    _totalCount = words.Length;

        //    _stats = words.GroupBy((w) => w)
        //    .OrderByDescending((g) => g.Count())
        //    .ToDictionary(
        //        (g) => g.Key,
        //        (g) => new WordStat
        //        {
        //            Count = g.Count(),
        //            Percentage = (int)Math.Round((double)g.Count() / (double)words.Length * 100d)
        //        }
        //    );
        //    return _stats;
        //}

        //Считает кол-во слов в тексте
        private void CalculateWordsCount(string text)
        {
            string[] words = text.Split(separators)
                .Where((s) => !string.IsNullOrEmpty(s))
                .ToArray();
            
            _totalCount = words.Length;

            _stats = words.GroupBy((w) => w)
            .OrderByDescending((g) => g.Count())
            .ToDictionary(
                (g) => g.Key,
                (g) => new WordStat
                {
                    Count = g.Count(),
                    Percentage = (int)Math.Round((double)g.Count() / (double)words.Length * 100d)
                }
            );
        }

        //Сохраняет результаты вычисления файла
        private void SaveStatsToFile()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(sfd.FileName))
                {
                    string content = ConvertStatsToStringTable();
                    writer.Write(content);
                    writer.Flush();
                }
            }
        }

        //Оброботчик события, который спрашивает у пользователя стоит ли сохранять результаты вычисления
        private void WordStatus_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr;
            dr = MessageBox.Show("Сохранить изменения?", "Сохранение изменений",
                                    MessageBoxButtons.YesNoCancel,
                                    MessageBoxIcon.Warning);
            switch (dr)
            {
                case DialogResult.Yes:
                    SaveStatsToFile();
                    break;
                case DialogResult.Cancel:

                    e.Cancel = true;
                    break;
            }
        }

        //Форматирует результаты в виде таблицы и возвращает в строковом виде
        private string ConvertStatsToStringTable()
        {
            var builder = new StringBuilder();

            builder.AppendLine(string.Format("{0,15} | {1,-7} | {2,-5}", "Слово", "Кол-во", "%"));
            builder.AppendLine(new string('=', 35));
            foreach (var pair in _stats)
            {
                builder.AppendLine(string.Format(
                    "{0,15} | {1,-7} | {2,-5}",
                    pair.Key,
                    pair.Value.Count,
                    pair.Value.Percentage + "%"
                ));
                builder.AppendLine(new string('-', 35));
            }

            builder.AppendLine(string.Format("|{0,33} |", $"Total: {_totalCount}"));
            builder.AppendLine(new string('-', 35));
            builder.AppendLine();

            return builder.ToString();
        }

        //Метод обнавления статуса на форме
        private void SetTotalCount()
        {
            statusTotalCount.Text = $"Количество слов: {_totalCount}";
        }

        //Метод, который заполняет таблицу на форме
        private void FillTable()
        {
            foreach(var pair in _stats)
            {
                dgResult.Rows.Add(pair.Key, pair.Value.Count, $"{pair.Value.Percentage}%");
            }
        }
    }
}
