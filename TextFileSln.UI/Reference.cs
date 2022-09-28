using System;
using System.Windows.Forms;



namespace TextFileSln.UI
{
    public partial class Reference : Form
    {
        //Конструктор формы
        public Reference()
        {
            InitializeComponent();
        }

        //Обработчик кнопки закрытия формы
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
