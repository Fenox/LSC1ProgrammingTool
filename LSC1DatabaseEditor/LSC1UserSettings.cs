using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LSC1DatabaseEditor
{
    public class LSC1UserSettings : ApplicationSettingsBase
    {
        private static readonly LSC1UserSettings instance = new LSC1UserSettings();

        private LSC1UserSettings() { }

        public static LSC1UserSettings Instance
        {
            get
            {
                return instance;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("lsc1")] //lsc1
        public string DatabaseName
        {
            get
            {
                return (string)this["DatabaseName"];
            }
            set
            {
                this["DatabaseName"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("")] //sql
        public string DatabasePasswort
        {
            get
            {
                return (string)this["DatabasePasswort"];
            }
            set
            {
                this["DatabasePasswort"] = value;
            }
        }


        [UserScopedSetting()]
        [DefaultSettingValue("root")] //root
        public string DatabaseUID
        {
            get
            {
                return (string)this["DatabaseUID"];
            }
            set
            {
                this["DatabaseUID"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("127.0.0.1")] //29.47.82.13
        public string DatabaseServer
        {
            get
            {
                return (string)this["DatabaseServer"];
            }
            set
            {
                this["DatabaseServer"] = value;
            }
        }

        [UserScopedSetting()]
        public List<Color> VisualisationColors
        {
            get
            {
                if (this["VisualisationColors"] == null)
                    return new List<Color>()
                    {
                        Colors.Red,
                        Colors.OrangeRed,
                        Colors.Orange,
                        Colors.Green,
                        Colors.GreenYellow,
                        Colors.Blue,
                        Colors.BlueViolet,
                        Colors.Violet,
                        Colors.Black
                    };

                return (List<Color>)this["VisualisationColors"];
            }
            set
            {
                this["VisualisationColors"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("4")]
        public int PointSize
        {
            get
            {
                return (int)this["PointSize"];
            }
            set
            {
                this["PointSize"] = value;
            }
        }


        public LSC1DatabaseConnectionSettings DBSettings
        {
            get
            {
                return new LSC1DatabaseConnectionSettings()
                {
                    Database = DatabaseName,
                    Password = DatabasePasswort,
                    Server = DatabaseServer,
                    Uid = DatabaseUID
                };
            }
        }
    }
}
