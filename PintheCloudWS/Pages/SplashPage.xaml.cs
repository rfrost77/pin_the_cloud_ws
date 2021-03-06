﻿using PintheCloudWS.Common;
using PintheCloudWS.Helpers;
using PintheCloudWS.Locale;
using PintheCloudWS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 기본 페이지 항목 템플릿에 대한 설명은 http://go.microsoft.com/fwlink/?LinkId=234237에 나와 있습니다.

namespace PintheCloudWS.Pages
{
    /// <summary>
    /// 대부분의 응용 프로그램에 공통되는 특성을 제공하는 기본 페이지입니다.
    /// </summary>
    public sealed partial class SplashPage : PtcPage
    {
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// 이는 강력한 형식의 뷰 모델로 변경될 수 있습니다.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }


        public SplashPage()
        {
            this.InitializeComponent();
        }

        #region NavigationHelper 등록

        /// 이 섹션에서 제공되는 메서드는 NavigationHelper에서
        /// 페이지의 탐색 메서드에 응답하기 위해 사용됩니다.
        /// 
        /// 페이지별 논리는 다음에 대한 이벤트 처리기에 있어야 합니다.  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// 및 <see cref="GridCS.Common.NavigationHelper.SaveState"/>입니다.
        /// 탐색 매개 변수는 LoadState 메서드 
        /// 및 이전 세션 동안 유지된 페이지 상태에서 사용할 수 있습니다.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.NavigationHelper.OnNavigatedTo(e);

            // Check nick name at frist login.
            if (!App.ApplicationSettings.Contains(Switcher.MAIN_PLATFORM_TYPE_KEY))
                App.ApplicationSettings[Switcher.MAIN_PLATFORM_TYPE_KEY] = AppResources.OneDrive;

            // Check nick name at frist login.
            if (!App.ApplicationSettings.Contains(StorageAccount.ACCOUNT_DEFAULT_SPOT_NAME_KEY))
                App.ApplicationSettings[StorageAccount.ACCOUNT_DEFAULT_SPOT_NAME_KEY] = AppResources.AtHere;

            // Check location access consent at frist login.
            if (!App.ApplicationSettings.Contains(StorageAccount.LOCATION_ACCESS_CONSENT_KEY))
                App.ApplicationSettings[StorageAccount.LOCATION_ACCESS_CONSENT_KEY] = false;

            // For Test
            this.SettingsForPresentation();

            // Do Signin work of each cloud storage.
            if (App.AccountManager.IsSignIn())
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    TaskHelper.AddTask(App.AccountManager.GetPtcId(), App.AccountManager.SignIn());
                    using (var itr = StorageHelper.GetStorageEnumerator())
                    {
                        while (itr.MoveNext())
                            if (itr.Current.IsSignIn())
                                TaskHelper.AddSignInTask(itr.Current.GetStorageName(), itr.Current.SignIn());
                    }
                }
                this.Frame.Navigate(typeof(SpotListPage));
            }
            else
            {
                this.Frame.Navigate(typeof(SpotListPage));
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.NavigationHelper.OnNavigatedFrom(e);
        }


        /// <summary>
        /// This Method is for the presentation. It should be Deleted after the presentation
        /// </summary>
        private void SettingsForPresentation()
        {
            App.AccountManager.SavePtcId("HELLO", "password");
        }

        #endregion
    }
}
