using System;
using System.Windows.Forms;
using TextFileLib;

namespace TextFileSln.UI
{
    public partial class SearchWordsDialog : Form
    {
        private readonly TextFile textFile;

        //Конструктор формы
        public SearchWordsDialog(TextFile textFile)
        {
            this.textFile = textFile;
            InitializeComponent();
        }

        //Обработчик нажатия кнопки поиска
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите окончание слова.");
                return;
            }

            string[] words = textFile.SearchByWordsEndWith(textBox.Text);
            listBox.Items.Clear();
            foreach (var word in words)
            {
                listBox.Items.Add(word);
            }
        }
    }
}
