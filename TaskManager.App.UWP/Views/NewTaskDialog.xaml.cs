using System;
using TaskManager.App.UWP.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace TaskManager.App.UWP.Views
{
    public enum SignInResult
    {
        SignInOK,
        SignInFail,
        SignInCancel,
        Nothing
    }

    public sealed partial class NewTaskDialog : ContentDialog
    {
        //public SignInResult Result { get; private set; }

        public NewTaskDialog()
        {
            InitializeComponent();
            //RequestedTheme = ThemeSelectorService.Theme;
            Opened += NewTaskContentDialog_Opened;
            Closing += NewTaskContentDialog_Closing;

        }

        //private ElementTheme ConvertAppThemeToElementTheme(ApplicationTheme appTheme)
        //{
        //    switch (appTheme)
        //    {
        //        case ApplicationTheme.Light:
        //            return ElementTheme.Light;
        //        case ApplicationTheme.Dark:
        //            return ElementTheme.Dark;
        //        default:
        //            return ElementTheme.Default; // Fallback, though should ideally never hit this
        //    }
        //}

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //// Ensure the user name and password fields aren't empty. If a required field
            //// is empty, set args.Cancel = true to keep the dialog open.
            //if (string.IsNullOrEmpty(userNameTextBox.Text))
            //{
            //    args.Cancel = true;
            //    errorTextBlock.Text = "User name is required.";
            //}
            //else if (string.IsNullOrEmpty(passwordTextBox.Password))
            //{
            //    args.Cancel = true;
            //    errorTextBlock.Text = "Password is required.";
            //}

            //// If you're performing async operations in the button click handler,
            //// get a deferral before you await the operation. Then, complete the
            //// deferral when the async operation is complete.

            //ContentDialogButtonClickDeferral deferral = args.GetDeferral();
            ////if (await SomeAsyncSignInOperation())
            ////{
            ////    this.Result = SignInResult.SignInOK;
            ////}
            ////else
            ////{
            ////    this.Result = SignInResult.SignInFail;
            ////}
            //deferral.Complete();
        }

        private void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // User clicked Cancel, ESC, or the system back button.
            //this.Result = SignInResult.SignInCancel;
        }

        void NewTaskContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            TaskDescriptionTextBox.Focus(FocusState.Programmatic);
            //this.Result = SignInResult.Nothing;

            //// If the user name is saved, get it and populate the user name field.
            //Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            //if (roamingSettings.Values.ContainsKey("userName"))
            //{
            //    userNameTextBox.Text = roamingSettings.Values["userName"].ToString();
            //    saveUserNameCheckBox.IsChecked = true;
            //}
        }

        void NewTaskContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            // If sign in was successful, save or clear the user name based on the user choice.
            //if (this.Result == SignInResult.SignInOK)
            //{
            //    if (saveUserNameCheckBox.IsChecked == true)
            //    {
            //        SaveUserName();
            //    }
            //    else
            //    {
            //        ClearUserName();
            //    }
            //}

            //// If the user entered a name and checked or cleared the 'save user name' checkbox, then clicked the back arrow,
            //// confirm if it was their intention to save or clear the user name without signing in.
            //if (this.Result == SignInResult.Nothing && !string.IsNullOrEmpty(userNameTextBox.Text))
            //{
            //    if (saveUserNameCheckBox.IsChecked == false)
            //    {
            //        args.Cancel = true;
            //        FlyoutBase.SetAttachedFlyout(this, (FlyoutBase)this.Resources["DiscardNameFlyout"]);
            //        FlyoutBase.ShowAttachedFlyout(this);
            //    }
            //    else if (saveUserNameCheckBox.IsChecked == true && !string.IsNullOrEmpty(userNameTextBox.Text))
            //    {
            //        args.Cancel = true;
            //        FlyoutBase.SetAttachedFlyout(this, (FlyoutBase)this.Resources["SaveNameFlyout"]);
            //        FlyoutBase.ShowAttachedFlyout(this);
            //    }
            //}
        }

        private void SaveUserName()
        {
            //Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            //roamingSettings.Values["userName"] = userNameTextBox.Text;
        }

        private void ClearUserName()
        {
            //Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            //roamingSettings.Values["userName"] = null;
            //userNameTextBox.Text = string.Empty;
        }

        // Handle the button clicks from the flyouts.
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveUserName();
            FlyoutBase.GetAttachedFlyout(this).Hide();
        }

        private void DiscardButton_Click(object sender, RoutedEventArgs e)
        {
            ClearUserName();
            FlyoutBase.GetAttachedFlyout(this).Hide();
        }

        // When the flyout closes, hide the sign in dialog, too.
        private void Flyout_Closed(object sender, object e)
        {
            this.Hide();
        }
    }
}
