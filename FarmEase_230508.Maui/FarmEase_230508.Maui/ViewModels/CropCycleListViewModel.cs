using FarmEase_230508.Maui.Data;
using FarmEase_230508.Maui.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FarmEase_230508.Maui.ViewModels {
    public class CropCycleListViewModel : BaseViewModel, IQueryAttributable {
        CropCycleDatabase database;
        private string userName;
        public CropCycleListViewModel(CropCycleDatabase cropCycleDatabase) {
            Title = "Crop Cycles";
            database = cropCycleDatabase;
            UserName = SecureStorage.GetAsync("auth_id").Result;
            Items = new ObservableCollection<CropCycleData>();
            LoadData();
            PullToRefreshCommand = new Command(ExecutePullToRefreshCommand);
            AddCommand = new Command(ExecuteAddCommand);
        }

        async void ExecuteAddCommand() {
            await Navigation.NavigateToAsync<AddCropCycleViewModel>();
        }

        public string UserName {
            get => this.userName;
            set => SetProperty(ref this.userName, value);
        }

        public ObservableCollection<CropCycleData> Items { get; private set; }

        bool isRefreshing = false;
        public bool IsRefreshing {
            get => this.isRefreshing;
            set => SetProperty(ref this.isRefreshing, value);
        }

        ICommand pullToRefreshCommand = null;

        public ICommand PullToRefreshCommand {
            get => pullToRefreshCommand;
            set => SetProperty(ref this.pullToRefreshCommand, value);
        }

        void ExecutePullToRefreshCommand() {
            Task.Factory.StartNew(() => {
                Thread.Sleep(3000);
#pragma warning disable CS0612 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
                Device.BeginInvokeOnMainThread(() => {
                    Items.Clear();
                    LoadData();
                    IsRefreshing = false;
                });
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CS0612 // Type or member is obsolete
            });
        }

        async void LoadData() {
            var items = await database.GetCropCycleByOwner(UserName);
            MainThread.BeginInvokeOnMainThread(() => {
                Items.Clear();
                foreach (var item in items)
                    Items.Add(item);

            });
        }

        public Command AddCommand { get; }

        public void ApplyQueryAttributes(IDictionary<string, object> query) {
            throw new NotImplementedException();
        }
    }
}
