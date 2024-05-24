using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
  // Make sure to use AndroidX for fragments

namespace Chess_FirstStep
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class BottomButtonsFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            return inflater.Inflate(Resource.Layout.fragment_bottom_buttons, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ImageButton btnHome = view.FindViewById<ImageButton>(Resource.Id.btnHome);
            ImageButton btnGameHistory = view.FindViewById<ImageButton>(Resource.Id.btnGameHistory);
            ImageButton btnFriends = view.FindViewById<ImageButton>(Resource.Id.btnFriends);

            // Set up button click handlers here
            btnHome.Click += (sender, e) =>
            {
                // Handle Home button click
                Toast.MakeText(Activity, "Home button clicked", ToastLength.Short).Show();
            };

            btnGameHistory.Click += (sender, e) =>
            {
                // Handle Game History button click
                Toast.MakeText(Activity, "Game History button clicked", ToastLength.Short).Show();
            };

            btnFriends.Click += (sender, e) =>
            {
                // Handle Friends button click
                Toast.MakeText(Activity, "Friends button clicked", ToastLength.Short).Show();
            };
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
