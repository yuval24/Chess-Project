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
using Newtonsoft.Json;

namespace Chess_FirstStep.Data_Classes
{
    public class MoveData : Data
    {
        public string move { get; set; }
        
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static MoveData Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<MoveData>(json);
        }
    }
}