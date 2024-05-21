using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gestures;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Chess_FirstStep.Data_Classes
{
    public class Data
    {
        public string type { get; set; }
        public string sender { get; set; }
        public string recipient { get; set; }
        public string content { get; set; }
         
        public string token { get; set; }
        public bool success { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Data Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<Data>(json);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return  "Type : " + type + ", Sender : " + sender + ", Reccipient : " + recipient +", content : " + content +", token :" + token +", success :" + success;
        }
    }
}