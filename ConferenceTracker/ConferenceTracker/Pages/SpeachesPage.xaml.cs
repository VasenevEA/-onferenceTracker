using Autofac;
using ConferenceTracker.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        }

        public ObservableCollection<View> views = new ObservableCollection<View>();
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var items = await manager.GetSectionItems();

                //CreateVMs
                foreach (var item in items.Data)
                {
                    var view = new SectionView(item);
                    views.Add(view);
                }
        }
    }
}