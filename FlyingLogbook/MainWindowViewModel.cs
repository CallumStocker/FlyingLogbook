using FlyingLogbook.AbstractClasses;
using FlyingLogbook.DataObjects;
using FlyingLogbook.Pages;
using FlyingLogbook.WPFUtilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FlyingLogbook
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Constructors and Setup

        public MainWindowViewModel(MainWindow ownerWindow)
        {
            this.OwnerWindow = ownerWindow;
            this.SetPage(new StarterPage(this.OwnerWindow));
        }

        #endregion

        #region Public Methods

        public void SetPage(BasePage page)
        {
            this.CurrentPage?.Cleanup();
            this.CurrentPage = page;
        }

        #endregion

        #region Public Properties

        public string WindowTitle
        {
            get
            {
                return this.CurrentPage?.PageTitle ?? "Flying Logbook";
            }
        }

        public BasePage CurrentPage
        {
            get
            {
                return this.currentPage;
            }

            set
            {
                if (this.currentPage != value)
                {
                    this.currentPage = value;
                    this.NotifyPropertyChanged("CurrentPage");
                    this.NotifyPropertyChanged("PageTitle");
                }
            }
        }

        #endregion

        #region Fields

        private BasePage currentPage;

        #endregion
    }
}
