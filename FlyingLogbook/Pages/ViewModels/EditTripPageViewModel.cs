using FlyingLogbook.AbstractClasses;
using FlyingLogbook.DataObjects;
using FlyingLogbook.DataPersistence;
using FlyingLogbook.Enums;
using FlyingLogbook.Utilities;
using FlyingLogbook.WPFUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FlyingLogbook.Pages.ViewModels
{
    public class EditTripPageViewModel : BaseViewModel
    {
        #region Constructors and Setup

        public EditTripPageViewModel(Trip editingTrip, MainWindow ownerWindow)
        {
            this.Trip = editingTrip;
            this.OwnerWindow = ownerWindow;
            this.OwnerWindow.Width = Math.Max(this.OwnerWindow.Width, 1000);
            this.OwnerWindow.Height = Math.Max(this.OwnerWindow.Height, 600);

            if (this.Trip.Pilots.Count == 0)
            {
                this.AddNewPilot();

                using (var databaseContext = new LogbookContext())
                {
                    var rankSetting = databaseContext.Settings.SingleOrDefault(s => s.SettingKey == Constants.UserRankSettingKey);

                    if (rankSetting != null)
                    {
                        var userRank = (PilotPositionEnum)rankSetting.SettingValue;

                        switch (userRank)
                        {
                            case PilotPositionEnum.Captain:
                            case PilotPositionEnum.FirstOfficer:
                                var pilot = this.Trip.Pilots.Where(p => p.PilotPosition == userRank).FirstOrDefault()?.Pilot;

                                if (pilot != null)
                                {
                                    pilot.Name = "Self";
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            this.SkipPilotFocus = this.Trip.Pilots.Count;

            this.SetupCommands();
        }

        #endregion

        #region Public Properties

        public Trip Trip { get; protected set; }

        public bool NeedsSave { get; protected set; }

        /// <summary>
        /// This property is used to skip the automatic focusing of the officer text boxes when they are created.
        /// We don't want these text boxes to be focused when the page is first initialised, as they aren't the first
        /// text boxes
        /// </summary>
        public int SkipPilotFocus { get; set; }

        #endregion

        /// <summary>
        /// List of binding properties for the trip object. Avoids cluttering up the data object with UI stuff
        /// </summary>
        #region Trip Data Properties

        public DateTime DateBinding
        {
            get
            {
                return this.Trip.Date;
            }

            set
            {
                if (this.Trip.Date != value)
                {
                    this.Trip.Date = value;
                    this.NotifyPropertyChanged("DateBinding");
                }
            }
        }

        public string TypeBinding
        {
            get
            {
                return this.Trip.PlaneType;
            }

            set
            {
                if (this.Trip.PlaneType != value)
                {
                    this.Trip.PlaneType = value;
                    this.NotifyPropertyChanged("TypeBinding");
                }
            }
        }

        public string RegNumberBinding
        {
            get
            {
                return this.Trip.RegNumber;
            }

            set
            {
                if (this.Trip.RegNumber != value)
                {
                    this.Trip.RegNumber = value;
                    this.NotifyPropertyChanged("RegNumberBinding");
                }
            }
        }

        public string CaptainBinding
        {
            get
            {
                var captain = this.GetOrCreateCaptain();

                return captain.Name;
            }

            set
            {
                var captain = this.GetOrCreateCaptain();

                if (captain.Name != value)
                {
                    captain.Name = value;
                    this.NotifyPropertyChanged("CaptainBinding");
                }
            }
        }

        public IEnumerable<object> OfficersBinding
        {
            get
            {
                return this.Trip.Pilots
                    .Where(tp => tp.PilotPosition == PilotPositionEnum.FirstOfficer)
                    .Select(tp => tp.Pilot);
            }
        }

        public string DepartureAirfieldBinding
        {
            get
            {
                var airfield = this.GetOrCreateAirfield(AirfieldDirectionEnum.Departure);

                return airfield.AirfieldName;
            }

            set
            {
                var airfield = this.GetOrCreateAirfield(AirfieldDirectionEnum.Departure);

                if (airfield.AirfieldName != value)
                {
                    airfield.AirfieldName = value;
                    this.NotifyPropertyChanged("DepartureAirfieldBinding");
                }
            }
        }

        public string ArrivalAirfieldBinding
        {
            get
            {
                var airfield = this.GetOrCreateAirfield(AirfieldDirectionEnum.Arrival);

                return airfield.AirfieldName;
            }

            set
            {
                var airfield = this.GetOrCreateAirfield(AirfieldDirectionEnum.Arrival);

                if (airfield.AirfieldName != value)
                {
                    airfield.AirfieldName = value;
                    this.NotifyPropertyChanged("ArrivalAirfieldBinding");
                }
            }
        }

        public DateTime DepartureTimeBinding
        {
            get
            {
                return this.Trip.DepartureTime;
            }

            set
            {
                if (this.Trip.DepartureTime != value)
                {
                    this.Trip.DepartureTime = value;
                    this.NotifyPropertyChanged("DepartureTimeBinding");
                }
            }
        }

        public DateTime ArrivalTimeBinding
        {
            get
            {
                return this.Trip.ArrivalTime;
            }

            set
            {
                if (this.Trip.ArrivalTime != value)
                {
                    this.Trip.ArrivalTime = value;
                    this.NotifyPropertyChanged("ArrivalTimeBinding");
                }
            }
        }

        private string dayHoursBinding;

        public string DayHoursBinding
        {
            get
            {
                return this.dayHoursBinding ?? ConvertTimeSpanToString(this.Trip.DayHours);
            }

            set
            {
                if (int.TryParse(value, out int hours))
                {
                    this.Trip.DayHours = new TimeSpan(hours, 0, 0);
                }
                else if (ConvertStringToTimeSpan(value, out TimeSpan dayHoursTimeSpan))
                {
                    if (this.Trip.DayHours != dayHoursTimeSpan)
                    {
                        this.Trip.DayHours = dayHoursTimeSpan;
                    }
                }
                else
                {
                    this.dayHoursBinding = value;
                    AddError("DayHoursBinding", $"{this.dayHoursBinding} is not recognised as a valid time");
                }

                this.NotifyPropertyChanged("DayHoursBinding");
            }
        }



        private string nightHoursBinding;

        public string NightHoursBinding
        {
            get
            {
                return this.nightHoursBinding ?? ConvertTimeSpanToString(this.Trip.NightHours);
            }

            set
            {
                if (int.TryParse(value, out int hours))
                {
                    this.Trip.NightHours = new TimeSpan(hours, 0, 0);
                }
                else if (ConvertStringToTimeSpan(value, out TimeSpan nightHoursTimeSpan))
                {
                    if (this.Trip.NightHours != nightHoursTimeSpan)
                    {
                        this.Trip.NightHours = nightHoursTimeSpan;
                    }
                }
                else
                {
                    this.nightHoursBinding = value;
                    AddError("NightHoursBinding", $"{this.nightHoursBinding} is not recognised as a valid time");
                }

                this.NotifyPropertyChanged("NightHoursBinding");
            }
        }

        private string btrtBinding;

        public string BTRTBinding
        {
            get
            {
                return this.btrtBinding ?? this.Trip.BTRT.ToString();
            }

            set
            {
                if (double.TryParse(value, out double btrt))
                {
                    if (btrt != this.Trip.BTRT)
                    {
                        this.Trip.BTRT = btrt;
                    }
                }

                this.btrtBinding = value;
            }
        }

        #endregion

        #region General Methods

        protected void AddNewPilot()
        {
            var newPilot = new Pilot();

            this.Trip.Pilots.Add(new TripPilot(this.Trip, newPilot, PilotPositionEnum.FirstOfficer));
        }

        #endregion

        #region Saving

        private bool AttemptSave()
        {
            if (this.NeedsSave)
            {
                this.Trip.DepartureTime = DateTime.Today.AddTicks(this.Trip.DepartureTime.TimeOfDay.Ticks);
                this.Trip.ArrivalTime = DateTime.Today.AddTicks(this.Trip.ArrivalTime.TimeOfDay.Ticks);

                // Check if the pilots already exist, and if they do get their database id
                foreach (var pilotLinker in this.Trip.Pilots)
                {
                    pilotLinker.Pilot = Pilot.GetOrCreatePilotFromDatabase(pilotLinker.Pilot.Name);
                }

                // Do the same for the airfields
                foreach (var airfieldLinker in this.Trip.Airfields)
                {
                    airfieldLinker.Airfield = Airfield.GetOrCreateAirfieldFromDatabase(airfieldLinker.Airfield.AirfieldName);
                }

                using (var databaseContext = new LogbookContext())
                {
                    databaseContext.Trips.Add(this.Trip);
                    databaseContext.SaveChanges();
                }

                this.NeedsSave = false;

                MessageBox.Show(
                    this.OwnerWindow, 
                    "Trip Saved Successfully", 
                    "Trip Saved", 
                    MessageBoxButton.OK);

                return true;
            }

            return true;
        }

        #endregion

        #region Commands

        protected void SetupCommands()
        {
            this.AddOfficerCommand = new BasicCommand(this.AddOfficer);
            this.RemoveOfficerCommand = new BasicCommand(this.RemoveOfficer);
            this.BTRTCopyCommand = new BasicCommand(this.BTRTCopy);
            this.SaveCommand = new BasicCommand(this.Save, (object parameter) => !this.HasErrors);
            this.MainMenuCommand = new BasicCommand(this.MainMenu);
        }

        public BasicCommand AddOfficerCommand { get; protected set; }

        protected void AddOfficer(object parameter)
        {
            this.AddNewPilot();
            this.NotifyPropertyChanged("OfficersBinding");
        }

        public BasicCommand RemoveOfficerCommand { get; protected set; }

        protected void RemoveOfficer(object parameter)
        {
            var currentPilot = parameter as Pilot;

            this.Trip.Pilots.RemoveAll(tp => tp.Pilot == currentPilot);
            this.NotifyPropertyChanged("OfficersBinding");
        }

        public BasicCommand BTRTCopyCommand { get; protected set; }

        protected void BTRTCopy(object parameter)
        {
            var hours = this.Trip.DayHours.Hours + this.Trip.NightHours.Hours;
            var minutes = (this.Trip.DayHours.Minutes + this.Trip.NightHours.Minutes) / 60.0;

            this.Trip.BTRT = hours + minutes;
            this.btrtBinding = null;
            this.NotifyPropertyChanged("BTRTBinding");
        }

        public BasicCommand SaveCommand { get; protected set; }

        protected void Save(object parameter)
        {
            this.AttemptSave();
        }

        public BasicCommand MainMenuCommand { get; protected set; }

        protected void MainMenu(object parameter)
        {
            if (this.NeedsSave)
            {
                var choice = MessageBox.Show(
                    this.OwnerWindow,
                    "This trip has not been saved. Would you like to save before leaving this page?",
                    "Unsaved Trip",
                    MessageBoxButton.YesNoCancel);

                switch (choice)
                {
                    case MessageBoxResult.Yes:
                        if (this.AttemptSave())
                        {
                            this.OwnerWindow.ViewModel.SetPage(new StarterPage(this.OwnerWindow));
                        }
                        break;

                    case MessageBoxResult.No:
                        this.OwnerWindow.ViewModel.SetPage(new StarterPage(this.OwnerWindow));
                        break;

                    case MessageBoxResult.Cancel:
                    default:
                        break;
                }
            }
            else
            {
                this.OwnerWindow.ViewModel.SetPage(new StarterPage(this.OwnerWindow));
            }
        }

        #endregion

        #region Private Methods

        private Pilot GetOrCreateCaptain()
        {
            var captainLinker = this.Trip.Pilots.SingleOrDefault(p => p.PilotPosition == PilotPositionEnum.Captain);

            if (captainLinker == null)
            {
                var newPilot = new Pilot();

                captainLinker = new TripPilot(this.Trip, newPilot, PilotPositionEnum.Captain);

                this.Trip.Pilots.Add(captainLinker);
            }

            return captainLinker.Pilot;
        }

        private Airfield GetOrCreateAirfield(AirfieldDirectionEnum airfieldDirection)
        {
            var matchingAirfield = this.Trip.Airfields.SingleOrDefault(a => a.AirfieldDirection == airfieldDirection);

            if (matchingAirfield == null)
            {
                var newAirfield = new Airfield();

                matchingAirfield = new TripAirfield(this.Trip, newAirfield, airfieldDirection);

                this.Trip.Airfields.Add(matchingAirfield);
            }

            return matchingAirfield.Airfield;
        }

        public string ConvertTimeSpanToString(TimeSpan time)
        {
            var paddedHours = time.Hours.ToString().PadLeft(2, '0');
            var paddedMinutes = time.Minutes.ToString().PadLeft(2, '0');

            return $"{paddedHours}:{paddedMinutes}";
        }

        public bool ConvertStringToTimeSpan(string timeString, out TimeSpan time)
        {
            return TimeSpan.TryParse(timeString.Replace(".", ":"), out time);
        }

        #endregion

        #region BaseViewModel Overrides

        protected override void OnNotify()
        {
            this.NeedsSave = true;
        }

        #endregion
    }
}
