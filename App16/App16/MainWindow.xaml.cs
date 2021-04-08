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
            var result = await UserConsentVerifierInterop.RequestVerificationForWindowAsync(windowHandle, "test");

        }
    }


    //MIDL_INTERFACE("39E050C3-4E74-441A-8DC0-B81104DF949C")
    //IUserConsentVerifierInterop : public IInspectable
    //{
    //public:
    //    virtual HRESULT STDMETHODCALLTYPE RequestVerificationForWindowAsync( 
    //        /* [in] */ HWND appWindow,
    //        /* [in] */ HSTRING message,
    //        /* [in] */ REFIID riid,
    //        /* [iid_is][retval][out] */ void **asyncOperation) = 0;
    //};

    [ComImport]
    [Guid("39E050C3-4E74-441A-8DC0-B81104DF949C")]
    // Note: Invoking methods on ComInterfaceType.InterfaceIsIInspectable interfaces
    // is no longer supported in.NET5, but can be simulated with IUnknown.
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IUserConsentVerifierInterop
    {
        void GetIids(out int iidCount, out IntPtr iids);
        void GetRuntimeClassName(out IntPtr className);
        void GetTrustLevel(out TrustLevel trustLevel);
        void RequestVerificationForWindowAsync(IntPtr appWindow, IntPtr hstrMessage, [In] ref Guid riid, out IntPtr outPtr);
    }

    //Helper to initialize UserConsentVerifier
    public static class UserConsentVerifierInterop
    {
        public static IAsyncOperation<UserConsentVerificationResult> RequestVerificationForWindowAsync(IntPtr hWnd, string Message)
        {
            //Use WinRT's GuidGenerator to get the correct guid
            var guid = GuidGenerator.CreateIID(typeof(IAsyncOperation<UserConsentVerificationResult>));
            
            //leverage winrt .As<> operator to cast winrt type to its interop interface
            IUserConsentVerifierInterop userConsentVerifierInterop = UserConsentVerifier.As<IUserConsentVerifierInterop>();
            
            //Handle marshalling the string to WinRT's HString
            var marshalStr = MarshalString.CreateMarshaler(Message);
            
            //Call the Interop api that pops a dialog, passing in the hWnd parameter
            IntPtr outPtr;
            userConsentVerifierInterop.RequestVerificationForWindowAsync(hWnd, MarshalString.GetAbi(marshalStr), ref guid, out outPtr);
            
            //Marshal the return object as an IAsyncOperation<>
            return (IAsyncOperation<UserConsentVerificationResult>)IInspectable.FromAbi(outPtr);

        }
    }


    //MIDL_INTERFACE("AF86E2E0-B12D-4c6a-9C5A-D7AA65101E90")
    //IInspectable : public IUnknown
    //{
    //public:
    //    virtual HRESULT STDMETHODCALLTYPE GetIids(
    //        /* [out] */ __RPC__out ULONG *iidCount,
    //        /* [size_is][size_is][out] */ __RPC__deref_out_ecount_full_opt(* iidCount) IID **iids) = 0;
    //    virtual HRESULT STDMETHODCALLTYPE GetRuntimeClassName(
    //        /* [out] */ __RPC__deref_out_opt HSTRING *className) = 0;
    //    virtual HRESULT STDMETHODCALLTYPE GetTrustLevel(
    //        /* [out] */ __RPC__out TrustLevel *trustLevel) = 0;
    //};

    //[ComImport]
    //[Guid("AF86E2E0-B12D-4c6a-9C5A-D7AA65101E90")]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //public interface WinRTIInspectable
    //{
    //    void GetIids(out int iidCount, out IntPtr iids);
    //    void GetRuntimeClassName(out IntPtr className);
    //    void GetTrustLevel(out TrustLevel trustLevel);
    //}


}
