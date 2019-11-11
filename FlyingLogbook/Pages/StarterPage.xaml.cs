using FlyingLogbook.AbstractClasses;
using FlyingLogbook.Pages.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for StarterPage.xaml
    /// </summary>
    public partial class StarterPage : BasePage
    {
        #region View Model

        public StarterPageViewModel ViewModel { get; protected set; }

        #endregion

        #region Constructors and Setup

        public StarterPage(MainWindow ownerWindow)
        {
            this.OwnerWindow = ownerWindow;
            this.ViewModel = new StarterPageViewModel(this.OwnerWindow);
            this.DataContext = this.ViewModel;

            InitializeComponent();
        }

        #endregion

        public override void Cleanup()
        {
            this.ViewModel.Cleanup();
        }

        #region Public Properties 

        public override string PageTitle { get => "Flying Logbook"; }

        #endregion
    }
}
