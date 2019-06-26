using System.Windows.Forms;

namespace PianoMotionCreateMidi
{
    public partial class InfoWebForm : Form
    {
        public InfoWebForm(string strHtml)
        {
            InitializeComponent();
            this.webBrowser1.DocumentText = strHtml;
        }
    }
}
