using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FlyingLogbook.AbstractClasses
{
    public abstract class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        /// <summary>
        /// Implementation of INotifyPropertyChanged
        /// </summary>
        #region NotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                this.OnNotify();
            }
        }

        protected virtual void OnNotify() { }

        #endregion

        #region INotifyDataErrorInfo

        private Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        public void AddError(string propertyName, string error)
        {
            List<string> propertyErrors;
            
            if (errors.ContainsKey(propertyName))
            {
                propertyErrors = this.errors[propertyName];
            }
            else
            {
                propertyErrors = new List<string>();
                this.errors.Add(propertyName, propertyErrors);
            }

            if (!propertyErrors.Contains(error))
            {
                propertyErrors.Add(error);
                RaiseErrorsChanged(propertyName);
            }
        }

        public void RaiseErrorsChanged(string propertyName)
        {
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !this.errors.ContainsKey(propertyName))
            {
                return null;
            }
            else
            {
                return this.errors[propertyName];
            }
        }

        public bool HasErrors
        {
            get
            {
                return this.errors.Count > 0;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Reference to the application window
        /// </summary>
        public MainWindow OwnerWindow { get; protected set; }

        #endregion
    }
}
