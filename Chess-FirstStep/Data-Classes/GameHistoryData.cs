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
    public class GameHistoryData : Data
    {
        public List<GameHistory> gameHistories { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static GameHistoryData Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<GameHistoryData>(json);
        }
    }
}