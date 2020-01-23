using FlyingLogbook.AbstractClasses;
using FlyingLogbook.Pages.ViewModels;
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
    /// Interaction logic for SearchTripsPage.xaml
    /// </summary>
    public partial class SearchTripsPage : BasePage
    {
        #region View Model

        public SearchTripPageViewModel ViewModel { get; protected set; }

        #endregion

        #region Constructors and Setup

        public SearchTripsPage(MainWindow ownerWindow)
        {
            this.OwnerWindow = ownerWindow;
            this.ViewModel = new SearchTripPageViewModel(this.OwnerWindow);
            this.DataContext = this.ViewModel;

            InitializeComponent();
        }

        #endregion

        #region Overrides for BasePage

        public override string PageTitle
        {
            get => "Trips Search";
        }

        public override void Cleanup()
        {
            this.ViewModel.Cleanup();
        }

        #endregion

        #region Event Handlers

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.ViewModel.ViewEditSelectedTrip();
        }

        #endregion
    }
}
