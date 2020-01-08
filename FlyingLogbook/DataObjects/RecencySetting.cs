using FlyingLogbook.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingLogbook.DataObjects
{
    public class RecencySetting
    {
        public int RecencyNumber { get; set; }

        public int DayCount { get; set; }

        public int Landings { get; set; }

        public List<ProgramSetting> SaveSetting()
        {
            var dayCountSetting = new ProgramSetting()
            {
                SettingKey = Constants.RecencyDayCountSettingKey,
                SettingNumber = this.RecencyNumber,
                SettingValue = this.DayCount
            };

            var landingsSetting = new ProgramSetting()
            {
                SettingKey = Constants.RecencyLandingCountSettingKey,
                SettingNumber = this.RecencyNumber,
                SettingValue = this.Landings
            };

            return new List<ProgramSetting>
            {
                dayCountSetting,
                landingsSetting
            };
        }

        public static List<RecencySetting> LoadRecencySettings(IEnumerable<ProgramSetting> programSettings)
        {
            var recencyList = new List<RecencySetting>();

            var recencySettings = programSettings.Where(s => s.SettingKey == Constants.RecencyDayCountSettingKey);

            if (recencySettings.Count() > 0)
            {
                foreach (var dayCountSetting in recencySettings)
                {
                    var matchingLanding = programSettings.Single(s =>
                        s.SettingKey == Constants.RecencyLandingCountSettingKey
                        && s.SettingNumber == dayCountSetting.SettingNumber);

                    var recencySetting = new RecencySetting()
                    {
                        RecencyNumber = dayCountSetting.SettingNumber,
                        DayCount = dayCountSetting.SettingValue,
                        Landings = matchingLanding.SettingValue
                    };

                    recencyList.Add(recencySetting);
                }
            }
            else
            {
                var recencySetting = new RecencySetting()
                {
                    RecencyNumber = 0,
                    DayCount = 0,
                    Landings = 0
                };

                recencyList.Add(recencySetting);
            }

            return recencyList;
        }
    }
}
