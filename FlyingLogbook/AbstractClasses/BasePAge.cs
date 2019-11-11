using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FlyingLogbook.AbstractClasses
{
    public abstract class BasePage : Page
    {
        #region Abstract Methods
        
        /// <summary>
        /// Method to clean up any resources used by the page
        /// </summary>
        public abstract void Cleanup();

        #endregion

        #region Properties
        
        /// <summary>
        /// Reference to the application window
        /// </summary>
        public MainWindow OwnerWindow { get; protected set; }

        /// <summary>
        /// Returns the name of the current page. 
        /// </summary>
        public abstract string PageTitle { get; }

        #endregion
    }
}
