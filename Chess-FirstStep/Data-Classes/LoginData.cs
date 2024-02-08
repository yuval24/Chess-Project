using System;
using System.Collections.Generic;


using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Chess_FirstStep.Data_Classes
{
    public class LoginData : Data
    {
        public string username { get; set; }
        public string password { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static LoginData Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<LoginData>(json);
        }
    }
}