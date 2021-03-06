﻿using PintheCloudWS.Common;
using PintheCloudWS.Helpers;
using PintheCloudWS.Locale;
using PintheCloudWS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PintheCloudWS.Pages
{
    public partial class PtcPage : Page
    {
        protected const string PICK_FILE_OBJECT_VIEW_MODEL_KEY = "PICK_FILE_OBJECT_VIEW_MODEL_KEY";
        protected const string PIN_FILE_OBJECT_VIEW_MODEL_KEY = "PIN_FILE_OBJECT_VIEW_MODEL_KEY";
        protected const string SPOT_VIEW_MODEL_KEY = "SPOT_VIEW_MODEL_KEY";

        protected const string PREV_PAGE_KEY = "PREV_PAGE";
        protected const string PIVOT_KEY = "PIVOT_KEY";

        protected const int OK_MODE = 0;
        protected const int OK_CANCEL_MODE = 1;

        protected const string NULL_PASSWORD = "null";

        private NavigationHelper navigationHelper;


        /// <summary>
        /// NavigationHelper는 각 페이지에서 탐색 및 프로세스 수명 관리를 
        /// 지원하는 데 사용됩니다.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public PtcPage()
        {
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }


        /// <summary>
        /// 탐색 중 전달된 콘텐츠로 페이지를 채웁니다. 이전 세션의 페이지를
        /// 다시 만들 때 저장된 상태도 제공됩니다.
        /// </summary>
        /// <param name="sender">
        /// 대개 <see cref="NavigationHelper"/>인 이벤트 소스
        /// </param>
        /// <param name="e">다음에 전달된 탐색 매개 변수를 제공하는 이벤트 데이터입니다.
        /// <see cref="Frame.Navigate(Type, Object)"/>에 전달된 매개 변수와
        /// 이전 세션 동안 이 페이지에 유지된
        /// 유지됩니다. 페이지를 처음 방문할 때는 이 상태가 null입니다.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // Get previous page and set it.
            string previousPage = null;
            if (App.ApplicationSessions.Contains(PREV_PAGE_KEY))
                previousPage = (string)App.ApplicationSessions[PREV_PAGE_KEY];
            App.ApplicationSessions[PREV_PAGE_KEY] = this.Frame.CurrentSourcePageType.Name;
            
            // Get previous pivot.
            int previousPivot = 0;
            if (App.ApplicationSessions.Contains(PIVOT_KEY))
                previousPivot = (int)App.ApplicationSessions[PIVOT_KEY];

            EventHelper.FireEvent((string)App.ApplicationSessions[PREV_PAGE_KEY], previousPage, previousPivot);
        }

        /// <summary>
        /// 응용 프로그램이 일시 중지되거나 탐색 캐시에서 페이지가 삭제된 경우
        /// 이 페이지와 관련된 상태를 유지합니다.  값은
        /// <see cref="SuspensionManager.SessionState"/>의 serialization 요구 사항을 만족해야 합니다.
        /// </summary>
        /// <param name="sender"> 대개 <see cref="NavigationHelper"/>인 이벤트 소스</param>
        /// <param name="e">serializable 상태로 채워질
        /// 빈 사전입니다.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
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
            navigationHelper.OnNavigatedTo(e);

            // Get previous page and set it.
            string previousPage = null;
            if (App.ApplicationSessions.Contains(PREV_PAGE_KEY))
                previousPage = (string)App.ApplicationSessions[PREV_PAGE_KEY];
            App.ApplicationSessions[PREV_PAGE_KEY] = this.Frame.Name.Split('?')[0];

            // Get previous pivot.
            int previousPivot = 0;
            if (App.ApplicationSessions.Contains(PIVOT_KEY))
                previousPivot = (int)App.ApplicationSessions[PIVOT_KEY];

            EventHelper.FireEvent((string)App.ApplicationSessions[PREV_PAGE_KEY], previousPage, previousPivot);
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion



        #region Protected Methods

        protected async void SetProgressRing(ProgressRing progressRing, bool value)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                progressRing.IsActive = value;
            });
        }


        protected async void ShowMessageDialog(string message, int mode, Context.TriggerEvent triggerEvent = null)
        {
            // Create the message dialog and set its content
            MessageDialog messageDialog = new MessageDialog(message);

            switch (mode)
            { 
                case OK_MODE:
                    // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
                    messageDialog.Commands.Add(new UICommand(
                        AppResources.OK,
                        new UICommandInvokedHandler((command) => { })));

                    // Set the command to be invoked when escape is pressed
                    messageDialog.CancelCommandIndex = 0;
                    break;

                case OK_CANCEL_MODE:
                    // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
                    messageDialog.Commands.Add(new UICommand(
                        AppResources.OK,
                        new UICommandInvokedHandler((command) => triggerEvent())));

                    messageDialog.Commands.Add(new UICommand(
                        AppResources.Cancel,
                        new UICommandInvokedHandler((command) => { })));

                    // Set the command to be invoked when escape is pressed
                    messageDialog.CancelCommandIndex = 1;
                    break;
            }

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }


        protected string GeolocatorStatusMessage()
        {
            string message = String.Empty;
            switch (App.Geolocator.LocationStatus)
            {
                case PositionStatus.Ready:
                    message = "Location is available.";
                    break;

                case PositionStatus.Initializing:
                    message = "Geolocation service is initializing.";
                    break;

                case PositionStatus.NotInitialized:
                    message = "Location status is not initialized because " +
                                "the app has not yet requested location data.";
                    break;

                case PositionStatus.Disabled:
                    message = "Location services are disabled. Use the " +
                                "Settings charm to enable them.";
                    break;

                case PositionStatus.NoData:
                    message = "Location service data is not available.";
                    break;

                case PositionStatus.NotAvailable:
                    message = "Location services are not supported on your system.";
                    break;

                default:
                    message = "Unknown PositionStatus value.";
                    break;
            };
            return message;
        }

        #endregion
    }
}
