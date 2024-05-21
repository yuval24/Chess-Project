using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Chess_FirstStep
{
    public class SharedPreferencesManager
    {
        private const string JwtTokenKey = "jwt_token";
        private const string UsernameTokenKey = "username";

        private static ISharedPreferences GetSharedPreferences()
        {
            return PreferenceManager.GetDefaultSharedPreferences(Application.Context);
        }

        public static void SaveUsername(string username)
        {
            ISharedPreferencesEditor editor = GetSharedPreferences().Edit();
            editor.PutString(UsernameTokenKey, username);
            editor.Apply();

        }

        public static void DeleteUsername()
        {
            ISharedPreferencesEditor editor = GetSharedPreferences().Edit();
            editor.Remove(UsernameTokenKey);
            editor.Apply();

        }

        public static string GetUsername()
        {
            return GetSharedPreferences().GetString(UsernameTokenKey, null);
        }
        public static void SaveJwtToken(string token)
        {
            ISharedPreferencesEditor editor = GetSharedPreferences().Edit();
            editor.PutString(JwtTokenKey, token);
            editor.Apply();
        }

        public static void DeleteJwtToken()
        {
            ISharedPreferencesEditor editor = GetSharedPreferences().Edit();
            editor.Remove(JwtTokenKey);
            editor.Apply();
        }

        public static string GetJwtToken()
        {
            return GetSharedPreferences().GetString(JwtTokenKey, null);
        }
    }
}