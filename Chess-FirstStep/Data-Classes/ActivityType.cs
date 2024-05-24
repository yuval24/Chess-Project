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
using Newtonsoft.Json;

namespace Chess_FirstStep.Data_Classes
{
    public class ActivityType
    {
        public const string LOGIN = "LOGIN";
        public const string SIGNUP = "SIGNUP";
        public const string MOVE = "MOVE";
        public const string CHAT = "CHAT";
        public const string REQUEST_TO_PLAY = "REQUEST_TO_PLAY";
        public const string APPROVE_TO_PLAY = "APPROVE_TO_PLAY";
        public const string END_GAME = "END_GAME";
        public const string LEAVE_GAME = "LEAVE_GAME";
        public const string AUTHENTICATE = "AUTHENTICATE";
        public const string GAME_HISTORY = "GAME_HISTORY";

    }
}