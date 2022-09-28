using System;
using System.Windows.Forms;
using TextFileLib;

namespace TextFileSln.UI
{
    public partial class MainForm : Form
    {
        private bool textChanged;
        private string filePath;
        private readonly TextFile textFile;

        //Конструктор формы
        public MainForm()
        {
            textChanged = false;
            textFile = new TextFile();
            filePath = string.Empty;
            InitializeComponent();
        }

        //Метод обнавления статуса на форме
        private void setStateStatus(string status)
        {
            statusState.Text = "Состояние: " + status;
        }

        //Метод обнавления статуса текста на форме
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            textFile.Content = textEditor.Text;
            textChanged = true;
            statusCharsCount.Text = "Число знаков: " + textEditor.Text.Length;
            statusLinesCount.Text = "Строк: " + textEditor.Lines.Length;
        }

        //Обработчик переключения панели инструментов
        private void панельИнструментовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            item.Checked = !item.Checked;
            toolStrip.Visible = item.Checked;
            setStateStatus("Переключена видимость панели инструментов");
        }

        //Обработчик пункта меню "Открыть файл"
        private void открытьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filePath = ofd.FileName;
                textFile.OpenFile(filePath);
                textEditor.Text = textFile.Content;
                textChanged = false;
                Text = filePath;
                setStateStatus("Открыт файл");
            }
        }

        //Обработчик пункта меню "Сохранить файл"
        private void сохранитьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                textFile.SaveToFile(filePath);
                textChanged = false;
                setStateStatus("Сохранено в файл");
            }
            else
            {
                сохранитьКакToolStripMenuItem_Click(sender, e);
            }
        }

        //Обработчик пункта меню "Сохранить как"
        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                filePath = sfd.FileName;
                Text = filePath;
                textFile.SaveToFile(filePath);
                textChanged = false;
                setStateStatus("Сохранено в файл");
            }
        }

        //Обработчик пункта меню "Печать"
        private void печатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Сначала сохраните содержимое в файл");
                return;
            }
            textFile.PrintFileContent(filePath, textEditor.Font);
        }

        //Обработчик пункта меню "Параметры шрифта"
        private void параметрыШрифтаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                textEditor.Font = fontDialog.Font;
            }
        }

        //Обработчик пункта меню "Цвет шрифта"
        private void цветШрифтаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                textEditor.ForeColor = colorDialog.Color;
            }
        }

        //Обработчик пункта меню "Поиск слов"
        private void поОкончаниюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchWordsDialog dialog = new SearchWordsDialog(textFile);
            dialog.ShowDialog();
        }

        //Обработчик пункта меню "Справка"
        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reference dialog = new Reference();
            dialog.ShowDialog();
        }

        //Обработчик который спрашивает у пользователя сохранения изменения текста
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (textChanged)
            {
                DialogResult dr;
                dr = MessageBox.Show("Сохранить изменения?", "Сохранение изменений",
                                      MessageBoxButtons.YesNoCancel,
                                      MessageBoxIcon.Warning);
                switch (dr)
                {   
                    case DialogResult.Yes:
                        сохранитьФайлToolStripMenuItem_Click(null, null);
                        break;
                    case DialogResult.Cancel:

                        e.Cancel = true;
                        break;
                }
            }
        }

        //Обработчик пункта меню "Анализ текста"
        private void подсчетСловToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            WordStatsForm form = new WordStatsForm(textEditor.Text);
            form.ShowDialog();
        }
    }
}
