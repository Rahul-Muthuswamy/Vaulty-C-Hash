﻿using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Lain
{
    public sealed class SettingsJson
    {
        public Theme Color { get; set; }
        public bool Authorize { get; set; }
        public bool AutoStart { get; set; }
        public bool AutoLock { get; set; }
        public bool AutoSizeColumns { get; set; }
        public bool HidePasswords { get; set; }
        public int Minutes { get; set; }
        public int FontSize { get; set; }
        public Size WindowSize { get; set; }
        public Point? WindowLocation { get; set; }
        public FormWindowState WindowState { get; set; }
    }

    internal static class Options
    {
        internal static Color ForegroundColor = Color.FromArgb(153, 102, 204);
        internal static Color ForegroundAccentColor = Color.FromArgb(134, 89, 179);
        internal static Color BackgroundColor = Color.FromArgb(20, 20, 20);

        internal readonly static string ThemeFlag = "themeable";
        readonly static string _settingsFile = Path.Combine(Required.DataFolder, "Lain.json");

        internal static SettingsJson CurrentOptions = new SettingsJson();

        internal static void ApplyTheme(Form f)
        {
            switch (CurrentOptions.Color)
            {
                case Theme.Amber:
                    SetTheme(f, Color.FromArgb(195, 146, 0), Color.FromArgb(171, 128, 0));
                    break;
                case Theme.Jade:
                    SetTheme(f, Color.FromArgb(70, 175, 105), Color.FromArgb(61, 153, 92));
                    break;
                case Theme.Ruby:
                    SetTheme(f, Color.FromArgb(205, 22, 39), Color.FromArgb(155, 17, 30));
                    break;
                case Theme.Silver:
                    SetTheme(f, Color.Gray, Color.DimGray);
                    break;
                case Theme.Azurite:
                    SetTheme(f, Color.FromArgb(0, 127, 255), Color.FromArgb(0, 111, 223));
                    break;
                case Theme.Amethyst:
                    SetTheme(f, Color.FromArgb(153, 102, 204), Color.FromArgb(134, 89, 179));
                    break;
            }
        }

        private static void SetTheme(Form f, Color c1, Color c2)
        {
            dynamic c;
            ForegroundColor = c1;
            ForegroundAccentColor = c2;

            Utilities.GetSelfAndChildrenRecursive(f).ToList().ForEach(x =>
            {
                c = x;

                if (x is Button)
                {
                    c.BackColor = c1;
                    c.FlatAppearance.BorderColor = c1;
                    c.FlatAppearance.MouseDownBackColor = c2;
                    c.FlatAppearance.MouseOverBackColor = c2;
                    c.FlatAppearance.BorderSize = 0;
                }
                if (x is Label || x is CheckBox || x is RadioButton)
                {
                    if ((string)c.Tag == ThemeFlag)
                    {
                        c.ForeColor = c1;
                    }
                }
                if (x is LinkLabel)
                {
                    if ((string)c.Tag == ThemeFlag)
                    {
                        c.LinkColor = c1;
                        c.VisitedLinkColor = c1;
                        c.ActiveLinkColor = c2;
                    }
                }
                c.Invalidate();
            });
        }

        internal static void SaveSettings()
        {
            if (File.Exists(_settingsFile))
            {
                File.WriteAllText(_settingsFile, string.Empty);

                using (FileStream fs = File.Open(_settingsFile, FileMode.OpenOrCreate))
                using (StreamWriter sw = new StreamWriter(fs))
                using (JsonWriter jw = new JsonTextWriter(sw))
                {
                    jw.Formatting = Formatting.Indented;

                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(jw, CurrentOptions);
                }
            }
        }

        internal static void LoadSettings()
        {
            if (!File.Exists(_settingsFile))
            {
                CurrentOptions.Color = Theme.Amethyst;
                CurrentOptions.Authorize = false;
                CurrentOptions.AutoLock = false;
                CurrentOptions.HidePasswords = true;
                CurrentOptions.Minutes = 2;
                CurrentOptions.AutoStart = false;
                CurrentOptions.AutoSizeColumns = true;
                CurrentOptions.WindowLocation = null;
                CurrentOptions.WindowSize = new Size(907, 681);
                CurrentOptions.FontSize = 1;
                CurrentOptions.WindowState = FormWindowState.Normal;

                using (FileStream fs = File.Open(_settingsFile, FileMode.CreateNew))
                using (StreamWriter sw = new StreamWriter(fs))
                using (JsonWriter jw = new JsonTextWriter(sw))
                {
                    jw.Formatting = Formatting.Indented;

                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(jw, CurrentOptions);
                }
            }
            else
            {
                CurrentOptions = JsonConvert.DeserializeObject<SettingsJson>(File.ReadAllText(_settingsFile));

                if (CurrentOptions.WindowSize.IsEmpty)
                {
                    CurrentOptions.WindowSize = new Size(907, 681);
                    SaveSettings();
                }
            }
        }
    }
}
