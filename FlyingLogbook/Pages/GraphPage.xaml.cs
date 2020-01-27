using FlyingLogbook.AbstractClasses;
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

namespace FlyingLogbook.Pages
{
    /// <summary>
    /// Interaction logic for GraphPage.xaml
    /// </summary>
    public partial class GraphPage : BasePage
    {
        public GraphPage()
        {
            InitializeComponent();
        }

        public override void Cleanup()
        {
            throw new NotImplementedException();
        }

        public override string PageTitle => throw new NotImplementedException();
    }
}
