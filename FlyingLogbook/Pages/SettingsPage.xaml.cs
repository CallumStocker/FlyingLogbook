using FlyingLogbook.AbstractClasses;
using FlyingLogbook.Pages.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : BasePage
    {
        #region Constants

        private static readonly Regex NumericRegex = new Regex("[^0-9]+");

        #endregion

        #region View Model

        public SettingsPageViewModel ViewModel { get; protected set; }

        #endregion

        #region Constructors and Setup

        public SettingsPage(MainWindow ownerWindow)
        {
            this.OwnerWindow = ownerWindow;
            this.ViewModel = new SettingsPageViewModel(this.OwnerWindow);
            this.DataContext = this.ViewModel;

            InitializeComponent();
        }

        #endregion

        #region Overrides for BasePage

        public override string PageTitle 
        {
            get => "Settings";
        }

        public override void Cleanup()
        {

        }

        #endregion

        #region Event Handlers

        private void NumericTextBoxPreviewInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = SettingsPage.NumericRegex.IsMatch(e.Text);
        }

        private void NumericTextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string)e.DataObject.GetData(typeof(string));
                if (SettingsPage.NumericRegex.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        #endregion
    }
}
