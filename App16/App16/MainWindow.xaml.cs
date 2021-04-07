using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials.UI;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App16
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            
        }

        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";

            //Interop helpers for non-CoreWindow
            IntPtr windowHandle = (App.Current as App).WindowHandle;
            System.Diagnostics.Debug.WriteLine("WindowId:" + windowHandle.ToString());
            var result = await UserConsentVerifierInterop.RequestVerificationForWindowAsync(windowHandle, "test");
        }
    }
    [ComImport]
    [Guid("39E050C3-4E74-441A-8DC0-B81104DF949C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IUserConsentVerifierInterop
    {
        // Note: Invoking methods on ComInterfaceType.InterfaceIsIInspectable interfaces
        // is no longer supported in.NET5, but can be simulated with IUnknown.
        void GetIids(out int iidCount, out IntPtr iids);
        void GetRuntimeClassName(out IntPtr className);
        void GetTrustLevel(out TrustLevel trustLevel);
        IntPtr RequestVerificationForWindowAsync(IntPtr appWindow, IntPtr hstrMessage, [In] Guid riid, out IntPtr outPtr);
    }

    //Helper to initialize UserConsentVerifier
    public static class UserConsentVerifierInterop
    {
        public static IAsyncOperation<UserConsentVerificationResult> RequestVerificationForWindowAsync(IntPtr hWnd, string Message)
        {
            Guid guid = typeof(IAsyncOperation<UserConsentVerificationResult>).GUID;

            IUserConsentVerifierInterop userConsentVerifierInterop = UserConsentVerifier.As<IUserConsentVerifierInterop>();
            var marshalStr = MarshalString.CreateMarshaler(Message);
            IntPtr outPtr;
            var hResult = userConsentVerifierInterop.RequestVerificationForWindowAsync(hWnd, MarshalString.GetAbi(marshalStr), guid, out outPtr);
            return (IAsyncOperation<UserConsentVerificationResult>)IInspectable.FromAbi(outPtr);
        }
    }

}
