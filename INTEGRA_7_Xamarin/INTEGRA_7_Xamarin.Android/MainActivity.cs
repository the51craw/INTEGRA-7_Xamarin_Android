using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Hardware.Usb;
using INTEGRA_7_Xamarin;
using Xamarin.Forms;
using INTEGRA_7_Xamarin.Droid;
using Android.Content;
using System.Timers;

[assembly: Dependency(typeof(MIDI))]
[assembly: Dependency(typeof(MyFileIO))]

namespace INTEGRA_7_Xamarin.Droid
{
    [Activity(Label = "INTEGRA_7_Xamarin", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { UsbManager.ActionUsbDeviceAttached })]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private static String ACTION_USB_PERMISSION = "eu.mrmartin.MIDI.USB_PERMISSION";
        private static String USB_DEVICE_ATTACHED = "android.hardware.usb.action.USB_DEVICE_ATTACHED";
        private static String USB_DEVICE_DETACHED = "android.hardware.usb.action.USB_DEVICE_DETACHED";
        public PendingIntent mPermissionIntent = null;
        public UsbManager usbManager = null;
        public static UsbInterface usbInterface = null;
        public UsbDevice usbDevice = null;
        public UsbEndpoint outputEndpoint = null;
        public UsbEndpoint inputEndpoint = null;
        public USB usb { get; set; }
        public PendingIntent pendingIntent;

        // For accessing INTEGRA_7_Xamarin.MainPage from UWP:
        private INTEGRA_7_Xamarin.MainPage MainPage_Portable;

        public MainActivity mainActivity;

        // Invisible comboboxes used by MIDI class (will always have INTEGRA-7 selected):
        private Picker OutputSelector;
        private Picker InputSelector;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            UIHandler.appType = UIHandler._appType.ANDROID;
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            Init();
        }

        protected override void OnStart()
        {
            base.OnStart();
            //MainPage_Portable.uIHandler.ShowLibrarianPage();
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            //MainPage_Portable.uIHandler.QuerySelectedStudioSet();
        }

        protected override void OnDestroy()
        {
            UnregisterReceiver(usb);
            base.OnDestroy();
        }

        private void Init()
        {
            Xamarin.Forms.DependencyService.Register<IMidi>();

            // Get INTEGRA_7_Xamarin.MainPage:
            MainPage_Portable = INTEGRA_7_Xamarin.MainPage.GetMainPage();
            UIHandler.appType = UIHandler._appType.ANDROID;

            // Pre-draw librarian page, we need the midi pickers before we can create MIDI:
//            MainPage_Portable.uIHandler.DrawLibrarianPage();


            // Let the portable project know this MainActivity:
            //mainActivity = this;
            MainPage_Portable.SetDeviceSpecificMainPage(this);

            // Make Portable project draw UI and get the Pickers for Midi output device:
            //MainPage_Portable.uIHandler.DrawLibrarianPage();

            // Get and initiate USB:
            UsbManager usbManager = (UsbManager)GetSystemService(Context.UsbService);
            usb = new USB(usbManager, this);

            // Hook up USB :
            pendingIntent = PendingIntent.GetBroadcast(this, 0, new Intent(ACTION_USB_PERMISSION), 0);
            IntentFilter filter = new IntentFilter(ACTION_USB_PERMISSION);
            filter.AddAction(USB_DEVICE_ATTACHED);
            filter.AddAction(USB_DEVICE_DETACHED);
            RegisterReceiver(usb, filter);

            // Ask user for permission to use USB if creation and initiation was successful:
            if (usb.Device != null && usb.Interface != null && usb.OutputEndpoint != null && usb.InputEndpoint != null)
            //if (!usb.HasPermission)
            {
                usb.Manager.RequestPermission(usb.Device, pendingIntent);
                usb.HasPermission = usb.Manager.HasPermission(usb.Device);
            }

            // Initiate MIDI:
            //OutputSelector = MainPage_Portable.uIHandler.Librarian_midiOutputDevice;
            //InputSelector = MainPage_Portable.uIHandler.Librarian_midiInputDevice;

            //MainPage_Portable.uIHandler.commonState.Midi.Init(MainPage_Portable, "INTEGRA-7", OutputSelector, InputSelector, this, 0, 0);
            MainPage_Portable.uIHandler.ShowPleaseWaitPage(WaitingFor.STARTUP, UIHandler.CurrentPage.LIBRARIAN, this);
        }
    }
}