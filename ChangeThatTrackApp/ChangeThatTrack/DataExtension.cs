using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ChangeThatTrack
{
    static class DataExtension
    {
        public static string GetString(this Application application, string key)
        {
            var prefs = application.ApplicationContext.GetSharedPreferences(application.PackageName, FileCreationMode.Private);
            return prefs.GetString(key, string.Empty);
        }

        public static void SaveString(this Application application, string key, string value)
        {
            var prefs = application.ApplicationContext.GetSharedPreferences(application.PackageName, FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString(key, value);
            prefEditor.Commit();
        }
    }
}