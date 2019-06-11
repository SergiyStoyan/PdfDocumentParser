using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// Interaction logic for TableRowControlW.xaml
    /// </summary>
    public partial class TableRowControlW : UserControl
    {
        public TableRowControlW()
        {
            InitializeComponent();
            LostFocus += delegate (object sender, RoutedEventArgs e)
            {
                SetTagFromControl();
            };
        }
        
        public DataGridRow Row;
        //protected TemplateForm templateForm;
        protected Action<DataGridRow> onLeft = null;

        public virtual void Initialize(DataGridRow row/*, TemplateForm templateForm*/, Action<DataGridRow> onLeft)
        {
            Row = row;
            //this.templateForm = templateForm;
            this.onLeft = onLeft;
        }

        virtual protected object getObject()
        {
            throw new Exception("Not overrrided!");
        }

        virtual public void SetTagFromControl()
        {
            Row.Tag = getObject();
            onLeft?.Invoke(Row);
        }
    }
}
