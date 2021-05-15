using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using System.Threading.Tasks;
using RED_Notice_App.Models;
using RED_Notice_App.Service;
using Xamarin.Essentials;
using System.Windows.Input;
using Xamarin.Forms;
using System.Runtime.CompilerServices;

namespace RED_Notice_App.ViewModels
{
    public class NoticeListViewModel : INotifyPropertyChanged
    {
        RedNoticeService service;
        bool _refreshing = false;
        bool _notConnected;
        ObservableCollection<Notice> _notices;

        public event PropertyChangedEventHandler PropertyChanged;

        public Response Response { get; private set; }
        public LayoutState State { get; set; }
        public ObservableCollection<Notice> Notices
        {
            get => _notices;
            set
            {
                if (_notices != value)
                {
                    _notices = value;
                    NotifyPropertyChanged(nameof(Notices));
                }
            }
        }
        public LinkedList<Exception> Errors { get; set; } = new LinkedList<Exception>();
        public ICommand RefreshCommand { get; private set; }
        public bool IsNotConnected
        {
            get => _notConnected;
            set
            {
                _notConnected = value;
                NotifyPropertyChanged(nameof(IsNotConnected));
            }
        }

        public bool IsRefreshing
        {
            get => _refreshing;
            set
            {
                _refreshing = value;
                NotifyPropertyChanged(nameof(IsRefreshing));
            }
        }
        public NoticeListViewModel()
        {
            AsyncInitialize();

            RefreshCommand = new Command(() =>
            {
                IsRefreshing = true;
                if (Notices.Count == 0)
                {
                    Notices = new ObservableCollection<Notice>(Response.Embedded.Notices);
                }
                IsRefreshing = false;
            });
        }

        async void RefreshList()
        {
            
        }

        async void AsyncInitialize()
        {
            try
            {
                service = new RedNoticeService();
                Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
                IsNotConnected = Connectivity.NetworkAccess != NetworkAccess.Internet;
                Response = await service.GetResponseAsync();
                Notices = new ObservableCollection<Notice>(Response.Embedded.Notices);
            }
            catch
            {

            }
        }

        void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            IsNotConnected = e.NetworkAccess != NetworkAccess.Internet;
            if (Notices.Count == 0)
            {
                Notices = new ObservableCollection<Notice>(Response.Embedded.Notices);
            }
        }

        ~NoticeListViewModel()
        {
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }
    }
}
