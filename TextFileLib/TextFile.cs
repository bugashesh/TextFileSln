using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;

namespace TextFileLib
{
    public class TextFile
    {
        private string lastError;

        // Для печати
        private Font font;
        private StreamReader reader;

        public string LastError { get { return lastError; } }
        public string Content { get; set; }

        //Метод, который считывает содержимое указанного файла
        public void OpenFile(string fileName)
        {
            try
            {
                StreamReader reader = new StreamReader(fileName, Encoding.UTF8);
                try
                {
                    Content = reader.ReadToEnd();
                }
                finally
                {
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
            }

        }

        //Метод, который сохраняет содержимое в указанный файл
        public void SaveToFile(string fileName)
        {
            try
            {
                StreamWriter writer = new StreamWriter(fileName, false, Encoding.UTF8);
                try
                {
                    writer.Write(Content);
                }
                finally
                {
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
            }
        }

        //Метод, который готовит содержимое файла для печати на принторе
        public bool PrintFileContent(string fileName, Font font)
        {
            try
            {
                reader = new StreamReader(fileName, Encoding.UTF8);
                this.font = font;
                try
                {
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
                    pd.Print();
                }
                finally
                {
                    reader.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Обработчик события печати
        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            int count = 0;
            float linesPerPage = ev.MarginBounds.Height / font.GetHeight(ev.Graphics);
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            string line = null;

            while (count < linesPerPage && ((line = reader.ReadLine()) != null))
            {
                float yPos = topMargin + (count * font.GetHeight(ev.Graphics));
                ev.Graphics.DrawString(line, font, Brushes.Black,
                leftMargin, yPos, new StringFormat());
                count++;
            }

            if (line != null)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
        }

        //Метод, который получает количество слов в тексте
        public int GetWordsCount()
        {
            if (string.IsNullOrEmpty(Content))
            {
                return 0;
            }
            string[] words = Content.Split(' ', ',', '.', '!', ':', ';', '?', '"');
            return words.Length;
        }

        //Метод, который сравнивает два экземпляра класса TextFile
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var file = obj as TextFile;

            if (file == null)
            {
                return false;
            }

            return !string.IsNullOrEmpty(Content)
                && !string.IsNullOrEmpty(file.Content)
                && file.Content.Length.Equals(Content.Length);
        }

        //Перегрузка сравнения двух экземпляров класса (равно)
        public static bool operator ==(TextFile left, TextFile right)
        {
            return left.Equals(right);
        }

        //Перегрузка сранения двух экземпляров класса (не равно)
        public static bool operator !=(TextFile left, TextFile right)
        {
            return !right.Equals(left);
        }

        //Перегрузка сранения двух экземпляров класса (больше)
        public static bool operator >(TextFile left, TextFile right)
        {
            if (string.IsNullOrEmpty(left.Content) && string.IsNullOrEmpty(right.Content))
            {
                return false;
            }

            if (string.IsNullOrEmpty(left.Content))
            {
                return false;
            }

            return left.Content.Length > right.Content.Length;
        }

        //Перегрузка сранения двух экземпляров класса (меньше)
        public static bool operator <(TextFile left, TextFile right)
        {
            if (string.IsNullOrEmpty(left.Content) && string.IsNullOrEmpty(right.Content))
            {
                return false;
            }

            if (string.IsNullOrEmpty(right.Content))
            {
                return false;
            }

            return left.Content.Length < right.Content.Length;
        }

        //Перегрузка сранения двух экземпляров класса (больше или равно)
        public static bool operator >=(TextFile left, TextFile right)
        {
            if (string.IsNullOrEmpty(left.Content) && string.IsNullOrEmpty(right.Content))
            {
                return false;
            }

            if (string.IsNullOrEmpty(left.Content))
            {
                return false;
            }

            return left.Content.Length >= right.Content.Length;
        }

        //Перегрузка сранения двух экземпляров класса (меньше или равно)
        public static bool operator <=(TextFile left, TextFile right)
        {
            if (string.IsNullOrEmpty(left.Content) && string.IsNullOrEmpty(right.Content))
            {
                return false;
            }

            if (string.IsNullOrEmpty(right.Content))
            {
                return false;
            }

            return left.Content.Length <= right.Content.Length;
        }

        //Поиск указанного файла по указанному пути
        public FileInfo[] FindFiles(string path, string searchQuery)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                return dir.GetFiles(searchQuery, SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
            }

            return Array.Empty<FileInfo>();
        }

        //Отсортировать файл по количеству символов
        public FileInfo[] OrderFilesByCharsCount(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                return dir.GetFiles()
                    .OrderBy((f) => f.Length)
                    .ToArray();
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
            }
            return Array.Empty<FileInfo>();
        }

        //Отсортировать файл по количеству слов
        public FileInfo[] OrderFilesByWordsCount(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                return dir.GetFiles()
                    .OrderBy((f) =>
                    {
                        TextFile file = new TextFile();
                        file.OpenFile(f.FullName);
                        return file.GetWordsCount();
                    })
                    .ToArray();
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
            }
            return Array.Empty<FileInfo>();
        }

        //Поиск по окончанию слов
        public string[] SearchByWordsEndWith(string end)
        {
            List<string> result = new List<string>();
            string[] words = Content.Split(' ', ',', '.', '!', ':', ';', '?', '"');
            foreach (var word in words)
            {
                if (word.EndsWith(end))
                {
                    result.Add(word);
                }
            }
            return result.ToArray();
        }
    }
}
