using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace RST
{
    public sealed class ViewModel : ViewModelBase
    {
        public ObservableCollection<Resources> ProcessingsView { get; private set; }
        public ObservableCollection<Resources> MobilesView { get; private set; }
        public ObservableCollection<Resources> OthersView { get; private set; }

        private string _statusInfo = string.Empty;
        private string _countDown = string.Empty;
        private string _title = string.Empty;
        private string _statusToolTip = string.Empty;
        private string _colorBackground = Variables.stableColor;

        private int cnt = 0;

        private bool _isEnabledUpdate = true;

        private ICommand _addTenMinutes;
        private ICommand _resetCounter;
        private ICommand _Update;

        public bool isEnabledUpdate
        {
            get
            {
                return _isEnabledUpdate;
            }
            set
            {
                _isEnabledUpdate = value;
                RaisePropertyChanged(() => isEnabledUpdate);
            }
        }

        public ICommand Update
        {
            get
            {
                return _Update ?? (_Update = new RelayCommand(() =>
                {
                    if (UpdateMethods.Update() == 0)
                        isEnabledUpdate = false;
                }));
            }
        }

        public ICommand AddTenMinutes
        {
            get
            {
                return _addTenMinutes ?? (_addTenMinutes = new RelayCommand(() =>
                {
                    cnt = cnt + 600;
                }));
            }
        }

        public ICommand ResetCounter
        {
            get
            {
                return _resetCounter ?? (_resetCounter = new RelayCommand(() =>
                {
                    cnt = 0;
                }));
            }
        }

        public string statusToolTip
        {
            get
            {
                return _statusToolTip;
            }
            set
            {
                _statusToolTip = value;
                RaisePropertyChanged(() => statusToolTip);
            }
        }

        public string countDown
        {
            get
            {
                return _countDown;
            }
            set
            {
                _countDown = value;
                RaisePropertyChanged(() => countDown);
            }
        }

        public string title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                RaisePropertyChanged(() => title);
            }
        }

        public string statusInfo
        {
            get
            {
                return _statusInfo;
            }
            set
            {
                _statusInfo = value;
                RaisePropertyChanged(() => statusInfo);
            }
        }

        public string colorBackground
        {
            get
            {
                return _colorBackground;
            }
            set
            {
                _colorBackground = value;
                RaisePropertyChanged(() => colorBackground);
            }
        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            ArrayList al = new ArrayList();

            foreach (Thread t in al)
            {
                t.Abort();
            }
        }

        public void ThreadTestResourses()
        {
            ProcessingsView = new ObservableCollection<Resources> { };
            MobilesView = new ObservableCollection<Resources> { };
            OthersView = new ObservableCollection<Resources> { };

            List<Resources> Resources = new List<Resources>();

            ConfigResourceParameter ResourceData = new ConfigResourceParameter();
            ConfigNotificationParameter NotifData = new ConfigNotificationParameter();
            string[] getStatus = { String.Empty, String.Empty };
            string messageOffline, messageElectCard, message = String.Empty;
            int electCard = 0;

            ResourceData = GetConfigs.ReadResourceConfig();
            NotifData = GetConfigs.ReadNotifConfig();

            if (!String.IsNullOrEmpty(ResourceData.Error) | !String.IsNullOrEmpty(NotifData.Error))
            {
                statusInfo = ResourceData.Error + " " + NotifData.Error;
                colorBackground = Variables.alertColor;
            }
            else
            {
                while (true)
                {
                    Resources.Clear();

                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        ProcessingsView.Clear();
                        MobilesView.Clear();
                        OthersView.Clear();
                    }));

                    Array.Clear(getStatus, 0, 1);
                    messageOffline = string.Empty;
                    messageElectCard = string.Empty;
                    message = string.Empty;
                    countDown = "Пуск";

                    getStatus = AdditionalFunc.GetStatus(ResourceData.FlagFilePath, ResourceData.SleepTime);
                    title = getStatus[0];
                    colorBackground = getStatus[2];

                    for (int j = 0; j < ResourceData.Applications.Length; j++)
                    {
                        statusInfo = "Проверка " + ResourceData.Applications[j].Name;
                        statusToolTip = "Адрес ресурса: " + ResourceData.Applications[j].Address;

                        Resources.Add(ResoursesInfo.GetResourseInfo(ResourceData.Applications[j]));

                        if (String.Equals(ResourceData.Applications[j].AppType, Variables.processingType))
                        {
                            ResourceData.Applications[j].requestId = Resources[j].requestId;
                            ResourceData.Applications[j].cardNum = Resources[j].cardNum;

                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                ProcessingsView.Add(Resources[j]);
                            }));
                        }

                        if (String.Equals(ResourceData.Applications[j].AppType, Variables.mobileType))
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                MobilesView.Add(Resources[j]);
                            }));

                            Int32.TryParse(Resources[j].sucReqElectCard, out electCard);
                        }

                        if (String.Equals(ResourceData.Applications[j].AppType, Variables.otherType))
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                OthersView.Add(Resources[j]);
                            }));
                        }

                        if (String.Equals(Resources[j].status, Variables.offline))
                        {
                            colorBackground = Variables.alertColor;

                            if (String.IsNullOrEmpty(messageOffline))
                                messageOffline = Resources[j].name + " - оффлайн!";
                            else
                                messageOffline = Resources[j].name + ", " + messageOffline;
                        }

                        if (String.Equals(ResourceData.Applications[j].AppType, Variables.mobileType) & (electCard < Variables.electCardAlert))
                        {
                            colorBackground = Variables.alertColor;

                            if (String.IsNullOrEmpty(messageElectCard))
                                messageElectCard = Resources[j].name + " - электронных карт осталось меньше " + Variables.electCardAlert;
                            else
                                messageElectCard = Resources[j].name + ", " + messageElectCard;
                        }
                    }
                    
                    if ((!String.IsNullOrEmpty(messageOffline) & String.Equals(getStatus[1], Variables.sendMessage)) | !String.IsNullOrEmpty(messageElectCard))
                    {
                        if (!String.IsNullOrEmpty(messageOffline) & !String.IsNullOrEmpty(messageElectCard))
                            message = DateTime.Now.ToString("dd-MM HH:mm") + "\n" + getStatus[3] + messageOffline + "\n" + messageElectCard;
                        else
                            message = DateTime.Now.ToString("dd-MM HH:mm") + "\n" + getStatus[3] + messageOffline + messageElectCard;
                        
                        if (NotifData.Channel == "T" | NotifData.Channel == "Telegram")
                            if (TelegramBot.SendTelegram(NotifData.TelegramParams, message) == Variables.requestStateError)
                            {
                                message = "Прокси более не доступен\n" + message;

                                for (int cnt = 0; cnt < NotifData.SMSParams.MobileNotifications.Length; cnt++)
                                    AdditionalFunc.SendSMS(NotifData.SMSParams.MobileNotifications[cnt], NotifData.SMSParams.SenderName, message);
                            }
                       else if (NotifData.Channel == "S" | NotifData.Channel == "SMS")
                                for (int cnt = 0; cnt < NotifData.SMSParams.MobileNotifications.Length; cnt++)
                                    AdditionalFunc.SendSMS(NotifData.SMSParams.MobileNotifications[cnt], NotifData.SMSParams.SenderName, message);
                    }

                    statusInfo = "Проверка закончена!\n" + DateTime.Now;
                    statusToolTip = null;

                    for (cnt = ResourceData.ExaminePause; cnt >= 0; cnt--)
                    {
                        countDown = cnt.ToString();
                        Thread.Sleep(1000);
                    }
                }
            }
        }
        
        public ViewModel()
        {
            if (UpdateMethods.Update() == 0)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }

            Thread oThread = new Thread(new ThreadStart(ThreadTestResourses));
            oThread.IsBackground = true;
            oThread.Start();
        }
    }
}
