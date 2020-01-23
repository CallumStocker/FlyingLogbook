using FlyingLogbook.AbstractClasses;
using FlyingLogbook.DataObjects;
using FlyingLogbook.DataPersistence;
using FlyingLogbook.Enums;
using FlyingLogbook.UIDataObjects;
using FlyingLogbook.WPFUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlyingLogbook.Pages.ViewModels
{
    public class SearchTripPageViewModel : BaseViewModel
    {
        #region Constructors and Setup

        public SearchTripPageViewModel(MainWindow ownerWindow)
        {
            this.OwnerWindow = ownerWindow;
            this.OwnerWindow.Width = Math.Max(this.OwnerWindow.Width, 1100);
            this.OwnerWindow.Height = Math.Max(this.OwnerWindow.Height, 600);

            this.SetupCommands();
        }

        #endregion

        #region Public Methods

        public void ViewEditSelectedTrip()
        {
            var editPage = new EditTripPage(this.SelectedTrip.DataObject, this.OwnerWindow);

            this.OwnerWindow.ViewModel.SetPage(editPage);
        }

        public void Cleanup()
        {
            this.DatabaseContext.Dispose();
        }

        #endregion

        #region Database Connection

        protected LogbookContext DatabaseContext
        {
            get
            {
                return this.databaseContext ?? (this.databaseContext = new LogbookContext());
            }
        }

        protected LogbookContext databaseContext;

        #endregion

        #region Binding Properties

        public IEnumerable<UITrip> FoundTrips { get; protected set; }

        public UITrip SelectedTrip { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string DepartureAirfield { get; set; }

        public string ArrivalAirfield { get; set; }

        public string Pilot { get; set; }

        public bool IncludeCaptain { get; set; } = true;

        public bool IncludeFO { get; set; } = true;

        #endregion

        #region Commands

        public BasicCommand SearchCommand { get; protected set; }
        public BasicCommand ViewEditCommand { get; protected set; }
        public BasicCommand MainMenuCommand { get; protected set; }

        public void SetupCommands()
        {
            this.SearchCommand = new BasicCommand(this.Search);
            this.ViewEditCommand = new BasicCommand(this.ViewEdit, (object param) => this.SelectedTrip != null);
            this.MainMenuCommand = new BasicCommand(this.MainMenu);
        }

        #endregion

        #region Command Methods

        protected void Search(object parameter)
        {
            var pilotPositions = new List<PilotPositionEnum>(2);

            if (this.IncludeCaptain)
            {
                pilotPositions.Add(PilotPositionEnum.Captain);
            }

            if (this.IncludeFO)
            {
                pilotPositions.Add(PilotPositionEnum.FirstOfficer);
            }

            this.FoundTrips = this.DatabaseContext.Trips
                .Include("Pilots.Pilot")
                .Include("Airfields.Airfield")     
                .Where(t => (!this.FromDate.HasValue || t.Date >= this.FromDate.Value)
                    && (!this.ToDate.HasValue || t.Date <= this.ToDate.Value)
                    && (string.IsNullOrEmpty(this.DepartureAirfield) || t.Airfields.Any(a => a.AirfieldDirection == AirfieldDirectionEnum.Departure && a.Airfield.AirfieldName == this.DepartureAirfield))
                    && (string.IsNullOrEmpty(this.ArrivalAirfield) || t.Airfields.Any(a => a.AirfieldDirection == AirfieldDirectionEnum.Arrival && a.Airfield.AirfieldName == this.ArrivalAirfield))
                    && (string.IsNullOrEmpty(this.Pilot) || t.Pilots.Any(p => p.Pilot.Name == this.Pilot && pilotPositions.Contains(p.PilotPosition)))
                )
                .ToList()
                .Select(t => new UITrip(t))
                .ToList();

            this.SelectedTrip = null;

            this.NotifyPropertyChanged("FoundTrips");
            this.NotifyPropertyChanged("SelectedTrip");
        }

        protected void ViewEdit(object parameter)
        {
            this.ViewEditSelectedTrip();
        }

        protected void MainMenu(object parameter)
        {
            this.OwnerWindow.ViewModel.SetPage(new StarterPage(this.OwnerWindow));
        }

        #endregion
    }
}
