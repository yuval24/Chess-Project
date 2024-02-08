using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Chess_FirstStep.Data_Classes
{
    public class ActivityType
    {
        public const string LOGIN = "LOGIN";
        public const string SIGNUP = "SIGNUP";
        public const string MOVE = "MOVE";
        public const string CHAT = "CHAT";
        public const string REQUEST_TO_PLAY = "REQUEST_TO_PLAY";


    }
}