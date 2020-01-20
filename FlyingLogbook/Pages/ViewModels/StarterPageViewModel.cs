using FlyingLogbook.AbstractClasses;
using FlyingLogbook.DataObjects;
using FlyingLogbook.DataPersistence;
using FlyingLogbook.Utilities;
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

namespace FlyingLogbook.Pages.ViewModels
{
    public class StarterPageViewModel : BaseViewModel
    {
        #region Constructors and Setup

        public StarterPageViewModel(MainWindow ownerWindow)
        {
            this.OwnerWindow = ownerWindow;

#if DEBUG
            this.ShowImport = true;
#else

            string[] args = Environment.GetCommandLineArgs();

            this.ShowImport = args.Contains("-import");
#endif

            this.DatabaseContext = new LogbookContext();

            this.SetupCommands();
        }

        #endregion

        #region Public Properties

        public bool ShowImport { get; protected set; }

        public Visibility ImportVisibility
        {
            get
            {
                return this.ShowImport ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public DateTime? LastEntryDate
        {
            get
            {
                if (this.DatabaseContext.Trips.Any())
                {
                    return this.DatabaseContext.Trips
                        .Select(t => t.Date)
                        .Max();
                }
                else
                {
                    return null;
                }
            }
        }

        public double HoursLast28Days
        {
            get
            {
                return GetHoursFromLastXDays(28);
            }
        }

        public double HoursLast365Days
        {
            get
            {
                return GetHoursFromLastXDays(365);
            }
        }

        public double HoursThisYear
        {
            get
            {
                if (this.DatabaseContext.Settings.Any(s => s.SettingKey == Constants.YearEndMonthSettingKey))
                {
                    var month = this.DatabaseContext.Settings.SingleOrDefault(s => s.SettingKey == Constants.YearEndMonthSettingKey).SettingValue;
                    var day = this.DatabaseContext.Settings.SingleOrDefault(s => s.SettingKey == Constants.YearEndDaySettingKey).SettingValue;

                    int year;

                    // See if we've passed the year end day in the current year, if not move back to the previous year
                    if (DateTime.Now.Month > month && DateTime.Now.Day > day)
                    {
                        year = DateTime.Now.Year;
                    }
                    else
                    {
                        year = DateTime.Now.Year - 1;
                    }

                    var yearStart = new DateTime(year, month, day);

                    var decimalSum = this.DatabaseContext.Trips
                        .Where(t => t.Date >= yearStart)
                        .ToList() // Makes this null safe
                        .Sum(t => t.DayHoursDouble + t.NightHoursDouble);

                    return decimalSum / 100 * 60;
                }
                else
                {
                    return GetHoursFromLastXDays(365);
                }
            }
        }

        public DateTime? RecencyDate
        {
            get
            {
                DateTime? recencyDate = null;

                var recencyCount = this.DatabaseContext.Settings.Count(s => s.SettingKey == Constants.RecencyDayCountSettingKey);

                // We want the last date based on all saved settings, as this will be the the most pressing recency to satisfy
                for (int recencyCounter = 0; recencyCounter < recencyCount; recencyCounter++)
                {
                    var recencyDays = this.DatabaseContext.Settings.Single(s => s.SettingKey == Constants.RecencyDayCountSettingKey && s.SettingNumber == recencyCounter).SettingValue;
                    var recencyLandings = this.DatabaseContext.Settings.Single(s => s.SettingKey == Constants.RecencyLandingCountSettingKey && s.SettingNumber == recencyCounter).SettingValue;

                    // Count back through the landed trips until we reach the nth landing, where n is the saved value for the landing count setting
                    var oldestValidLanding = this.DatabaseContext.Trips
                        .Where(t => t.Landed)
                        .OrderByDescending(t => t.Date)
                        .Take(recencyLandings)
                        .ToList()
                        .Last();

                    var validToDate = oldestValidLanding.Date.AddDays(recencyDays);

                    if (!recencyDate.HasValue || validToDate > recencyDate)
                    {
                        recencyDate = validToDate;
                    }
                }

                return recencyDate;
            }
        }

        #endregion

        #region Public Methods

        public void Cleanup()
        {
            this.DatabaseContext.Dispose();
        }

        #endregion

        #region Protected Properties

        protected LogbookContext DatabaseContext { get; set; }

        #endregion

        #region Protected Methods

        protected void EditTrip(Trip trip)
        {
            var editPage = new EditTripPage(trip, this.OwnerWindow);

            this.OwnerWindow.ViewModel.SetPage(editPage);
        }

        protected double GetHoursFromLastXDays(int days)
        {
            var startDate = DateTime.Today.AddDays(-days);

            var matchingTrips = this.DatabaseContext.Trips
                .Where(t => t.Date >= startDate)
                .ToList();

            if (matchingTrips.Any())
            {
                var decimalSum = matchingTrips.Sum(t => t.DayHoursDouble + t.NightHoursDouble);

                return (int)decimalSum + ((decimalSum - (int)decimalSum) / 100.0 * 60);
            }
            else
            {
                return 0;
            }
        }

        #endregion

        #region Commands

        public void SetupCommands()
        {
            this.AddNewEntry = new BasicCommand(this.AddNew);
            this.ViewEditEntry = new BasicCommand(this.ViewEdit);
            this.OpenSettings = new BasicCommand(this.OpenSettingsPage);
            this.Import = new BasicCommand(this.RunImport);
            this.Exit = new BasicCommand(this.ExitProgram);
        }
        
        public BasicCommand AddNewEntry { get; protected set; }
        public BasicCommand ViewEditEntry { get; protected set; }
        public BasicCommand OpenSettings { get; protected set; }
        public BasicCommand Import { get; protected set; }
        public BasicCommand Exit { get; protected set; }

        #region Command Methods

        protected void AddNew(object parameter)
        {
            var trip = new Trip();

            EditTrip(trip);
        }

        protected void ViewEdit(object parameter)
        {
            var searchPage = new SearchTripsPage(this.OwnerWindow);
            this.OwnerWindow.ViewModel.SetPage(searchPage);
        }

        protected void RunImport(object parameter)
        {
            var fileDialog = new OpenFileDialog()
            {
                FileName = "Storage.txt",
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };

            var result = fileDialog.ShowDialog(this.OwnerWindow);

            if (result ?? true)
            {
                var filePath = fileDialog.FileName;

                var fileLines = File.ReadAllLines(filePath);
                int counter = 1;

                try
                {
                    var importedTrips = new List<Trip>();
                    var pilots = new List<Pilot>();
                    var airfields = new List<Airfield>();

                    foreach (var line in fileLines)
                    {
                        var loadedTrip = Trip.LoadTripFromLegacy(line, counter - 1, pilots, airfields);
                        importedTrips.Add(loadedTrip);
                        counter++;
                    }

                    using (var sqlContext = new LogbookContext())
                    {
                        sqlContext.Trips.AddRange(importedTrips);
                        sqlContext.SaveChanges();
                    }

                    MessageBox.Show(
                        $"Successfully imported {counter} trips",
                        "Import Successful",
                        MessageBoxButton.OK);

                    this.NotifyPropertyChanged("LastEntryDate");
                    this.NotifyPropertyChanged("HoursLast28Days");
                    this.NotifyPropertyChanged("HoursLast365Days");
                }
                catch (Exception e)
                {
                    MessageBox.Show(
                        $"Error on line {counter}: {e.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        protected void OpenSettingsPage(object parameter)
        {
            var settingsPage = new SettingsPage(this.OwnerWindow);

            this.OwnerWindow.ViewModel.SetPage(settingsPage);
        }

        protected void ExitProgram(object parameter)
        {
            Application.Current.Shutdown();
        }

        #endregion
        
        #endregion
    }
}
