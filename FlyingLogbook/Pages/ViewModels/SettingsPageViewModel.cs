using FlyingLogbook.AbstractClasses;
using FlyingLogbook.DataObjects;
using FlyingLogbook.DataPersistence;
using FlyingLogbook.Enums;
using FlyingLogbook.Utilities;
using FlyingLogbook.WPFUtilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingLogbook.Pages.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
        #region Constructors and Setup

        public SettingsPageViewModel(MainWindow ownerWindow)
        {
            this.OwnerWindow = ownerWindow;
            this.DbContext = new LogbookContext();

            this.LoadSettings();
            this.SetupCommands();
        }

        #endregion

        #region Public Properties

        public Dictionary<PilotPositionEnum, string> AvailableRanks
        {
            get
            {
                return new Dictionary<PilotPositionEnum, string>
                {
                    { PilotPositionEnum.Captain, "Captain" },
                    { PilotPositionEnum.FirstOfficer, "First Officer" }
                };
            }
        }

        public PilotPositionEnum UserRank
        {
            get
            {
                return this.userRank;
            }

            set
            {
                if (this.userRank != value)
                {
                    this.userRank = value;
                    this.NotifyPropertyChanged("UserRank");
                }
            }
        }

        public Dictionary<int, int> DaysPerMonth
        {
            get
            {
                return new Dictionary<int, int>
                {
                    { 1, 31 },
                    { 2, 28 },
                    { 3, 31 },
                    { 4, 30 },
                    { 5, 31 },
                    { 6, 30 },
                    { 7, 31 },
                    { 8, 31 },
                    { 9, 30 },
                    { 10, 31 },
                    { 11, 30 },
                    { 12, 31 }
                };
            }
        }

        public int YearEndMonth
        {
            get
            {
                return this.yearEndMonth;
            }

            set
            {
                if (this.yearEndMonth != value)
                {
                    this.yearEndMonth = value;
                    
                    // If the previously selected day doesn't exist in the newly selected month, reset the
                    // selected day
                    if (this.DaysPerMonth[this.YearEndMonth] < this.YearEndDay)
                    {
                        this.YearEndDay = 1;
                    }

                    this.NotifyPropertyChanged("YearEndMonth");
                    this.NotifyPropertyChanged("MonthDays");
                }
            }
        }

        public IEnumerable<int> MonthDays
        {
            get
            {
                var daysList = new List<int>(this.DaysPerMonth[this.YearEndMonth]);

                for (int i = 1; i <= this.DaysPerMonth[this.YearEndMonth]; i++)
                {
                    daysList.Add(i);
                }

                return daysList;
            }
        }

        public int YearEndDay
        {
            get
            {
                return this.yearEndDay;
            }

            set
            {
                if (this.yearEndDay != value)
                {
                    this.yearEndDay = value;
                    this.NotifyPropertyChanged("YearEndDay");
                }
            }
        }

        public ObservableCollection<RecencySetting> RecencySettings
        {
            get
            {
                return this.recencySettings;
            }

            set
            {
                if (this.recencySettings != value)
                {
                    this.recencySettings = value;
                    this.NotifyPropertyChanged("RecencySettings");
                }
            }
        }

        #endregion

        #region Public Methods

        public void LoadSettings()
        {
            var savedSettings = this.DbContext.Settings.ToList();

            this.UserRank = (PilotPositionEnum)LoadSetting(savedSettings, Constants.UserRankSettingKey, 1);
            this.YearEndMonth = LoadSetting(savedSettings, Constants.YearEndMonthSettingKey, 1);
            this.YearEndDay = LoadSetting(savedSettings, Constants.YearEndDaySettingKey, 1);
            this.RecencySettings = new ObservableCollection<RecencySetting>(RecencySetting.LoadRecencySettings(savedSettings));
        }

        public void SaveSettings()
        {
            this.DbContext.LoadOrAddSetting(Constants.UserRankSettingKey).SettingValue = (int)this.UserRank;
            this.DbContext.LoadOrAddSetting(Constants.YearEndMonthSettingKey).SettingValue = this.YearEndMonth;
            this.DbContext.LoadOrAddSetting(Constants.YearEndDaySettingKey).SettingValue = this.YearEndDay;

            foreach (var recencySetting in this.RecencySettings)
            {
                this.DbContext.LoadOrAddSetting(Constants.RecencyDayCountSettingKey, recencySetting.RecencyNumber).SettingValue = recencySetting.DayCount;
                this.DbContext.LoadOrAddSetting(Constants.RecencyLandingCountSettingKey, recencySetting.RecencyNumber).SettingValue = recencySetting.Landings;
            }

            this.DbContext.SaveChanges();
        }

        public void Cleanup()
        {
            this.DbContext.Dispose();
        }

        #endregion

        #region Protected Properties

        protected LogbookContext DbContext { get; set; }

        #endregion

        #region Private Fields

        private PilotPositionEnum userRank;

        private int yearEndMonth, yearEndDay;

        private ObservableCollection<RecencySetting> recencySettings;

        #endregion

        #region Private Methods

        private static int LoadSetting(IEnumerable<ProgramSetting> settings, string settingKey, int defaultValue)
        {
            return settings.SingleOrDefault(s => s.SettingKey == settingKey)?.SettingValue ?? defaultValue;
        }

        #endregion

        #region Commands

        protected void SetupCommands()
        {
            this.RemoveRecencyCommand = new BasicCommand(this.RemoveRecency);
            this.AddRecencyCommand = new BasicCommand(this.AddRecency);
            this.SaveCommand = new BasicCommand(this.Save);
            this.MainMenuCommand = new BasicCommand(this.MainMenu);
        }

        public BasicCommand RemoveRecencyCommand { get; protected set; }

        public BasicCommand AddRecencyCommand { get; protected set; }

        public BasicCommand SaveCommand { get; protected set; }

        public BasicCommand MainMenuCommand { get; protected set; }

        #region Command Methods

        protected void RemoveRecency(object parameter)
        {
            var recencyToRemove = parameter as RecencySetting;

            if (recencyToRemove != null)
            {
                this.RecencySettings.Remove(recencyToRemove);
                this.NotifyPropertyChanged("RecencySettings");
            }
        }

        protected void AddRecency(object parameter)
        {
            var newRecency = new RecencySetting() { RecencyNumber = this.RecencySettings.Count };
            this.RecencySettings.Add(newRecency);
            this.NotifyPropertyChanged("RecencySettings");
        }

        protected void Save(object parameter)
        {
            this.SaveSettings();
        }

        protected void MainMenu(object parameter)
        {
            this.OwnerWindow.ViewModel.SetPage(new StarterPage(this.OwnerWindow));
        }

        #endregion

        #endregion
    }
}
