using Autofac;
using ConferenceTracker.Core.Models;
using ConferenceTracker.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ConferenceTracker.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpeachesPage : ContentPage
    {
        ConferenceManager manager;
        public SpeachesPage()
        {
            InitializeComponent();

            manager = IocContainter.Container.Resolve<ConferenceManager>();
            NavigationPage.SetHasNavigationBar(this, false);
            myCarousel.ItemsSource = views;
            BindingContext = this;
            myCarousel.Scrolled += MyCarousel_Scrolled;
            myCarousel.PositionSelected += MyCarousel_PositionSelected;
        }

        private void MyCarousel_PositionSelected(object sender, CarouselView.FormsPlugin.Abstractions.PositionSelectedEventArgs e)
        {
            if (views.Count > e.NewValue)
                views[e.NewValue].ScrollTo(StartDateTime);
        }

        private void MyCarousel_Scrolled(object sender, CarouselView.FormsPlugin.Abstractions.ScrolledEventArgs e)
        {
        }

        public ObservableCollection<SectionView> views = new ObservableCollection<SectionView>();

        bool isStarted = false;

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (isStarted)
            {
                return;
            }
            var items = await manager.GetSectionItems();

            var cash = await manager.GetCashData();
            //GetAllTimes
            var times = cash.Data.Speaches.Select(x => x.SpeachStartTime);
            var noDubles = ConferenceManager.RemoveDuplicates(times);
            //CreateVMs
            foreach (var item in items.Data)
            {
                var data = new List<Speach>();

                foreach (var time in noDubles)
                {
                    var speach = item.Speachs.FirstOrDefault(x => x.SpeachStartTime == time);
                    if (speach != null)
                    {
                        data.Add(speach);
                    }
                    else
                    {
                        data.Add(new Speach
                        {
                            SpeachStartTime = time
                        });
                    }
                }
                item.Speachs = data.ToArray();
                var view = new SectionView(item, new Action<DateTime>((dt) =>
                {
                    StartDateTime = dt;
                }));
                views.Add(view);
            }
            isStarted = true;
        }

        public DateTime StartDateTime { get; set; }
    }
}