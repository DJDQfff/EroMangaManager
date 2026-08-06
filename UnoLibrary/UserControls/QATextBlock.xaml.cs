//https://go.microsoft.com/fwlink/?LinkId=234236 �Ͻ����ˡ��û��ؼ�����ģ��

namespace WinApp.UserControls
{
    public sealed partial class QATextBlock : UserControl
    {
        public QATextBlock()
        {
            InitializeComponent();
        }

        public QATextBlock(string question, string answer)
        {
            QuestionTextBlock.Text = question;
            AnswerTextBlock.Text = answer;
        }

        public string Answer
        {
            set { AnswerTextBlock.Text = value; }
        }

        public string Question
        {
            set { QuestionTextBlock.Text = value; }
        }
    }
}
