using System.Windows.Forms;

namespace SharpExtensionsUtil.Form
{
    public partial class DialogWithControl : System.Windows.Forms.Form
    {
        private Control mainControl;
        public Control MainControl
        {
            get { return mainControl; }
            set
            {
                Controls.Remove(mainControl);
                mainControl = value;
                mainControl.Dock = DockStyle.Fill;
                Controls.Add(value);
            }
        }

        public DialogWithControl()
        {
            InitializeComponent();
        }
    }
}
