using FlyingLogbook.AbstractClasses;
using FlyingLogbook.DataObjects;
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
    /// Interaction logic for EditTripPage.xaml
    /// </summary>
    public partial class EditTripPage : BasePage
    {
        #region Constants

        private static readonly Regex NumericRegex = new Regex("[^0-9.:]+");

        #endregion

        #region View Model

        public EditTripPageViewModel ViewModel { get; protected set; }

        #endregion

        #region Constructors and Setup

        public EditTripPage(Trip trip, MainWindow ownerWindow)
        {
            this.OwnerWindow = ownerWindow;
            this.ViewModel = new EditTripPageViewModel(trip, ownerWindow);
            this.DataContext = this.ViewModel;

            InitializeComponent();
        }

        #endregion

        #region Public Properties

        public override string PageTitle { get => "Edit Trip"; }

        #endregion

        #region Public Methods

        public override void Cleanup()
        {            
        }

        #endregion

        #region Event Handlers

        private void NumericTextBoxPreviewInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = EditTripPage.NumericRegex.IsMatch(e.Text);
        }

        private void NumericTextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string)e.DataObject.GetData(typeof(string));
                if (EditTripPage.NumericRegex.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void OfficerKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.ViewModel.AddOfficerCommand.Execute(sender);
                e.Handled = true;
            }
        }

        private void OfficerLoaded(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.SkipPilotFocus > 0)
            {
                this.ViewModel.SkipPilotFocus--;
            }
            else
            {
                if (sender is TextBox)
                {
                    Keyboard.Focus(sender as TextBox);
                }
            }
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textbox = sender as TextBox;

            textbox.SelectAll();
        }

        private void TextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            var textbox = sender as TextBox;

            textbox.SelectAll();
        }

        #endregion
    }
}
