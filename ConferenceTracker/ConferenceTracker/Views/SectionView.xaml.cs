using Autofac;
using ConferenceTracker.Core.Models;
using ConferenceTracker.Models;
using ConferenceTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ConferenceTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SectionView : ContentView
    {

        Action<DateTime> setSelectedTime;
        SectionItem section;
        public SectionView(SectionItem section, Action<DateTime> setSelectedTime)
        {
            this.section = section;
            this.setSelectedTime = setSelectedTime;
            InitializeComponent();
            BindingContext = IocContainter.Container.Resolve<SectionViewModel>(new TypedParameter(typeof(SectionItem), section));
            listView.VerticalScrollBarVisibility = ScrollBarVisibility.Never;
            listView.ItemAppearing += ListView_ItemAppearing;
        }

        private void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var speach = e.Item as Speach;
            System.Diagnostics.Debug.WriteLine($"{listView.FirstVisibleItemIndex}");
            if (listView.FirstVisibleItem is Speach sp)
            {
                setSelectedTime(sp.SpeachStartTime);

                System.Diagnostics.Debug.WriteLine($"{sp.SpeachStartTime}");
            }
        }

        public SectionView()
        {
            InitializeComponent();
            BindingContext = new
            {
                Speaches = new[]
                {
                    new Speach()
                    {
                      SpeachStartTime = DateTime.Now
                    },
                     new Speach()
                    {
                         Speakers = new Speaker[]
                         {
                             new Speaker
                             {
                                 Name = "Анастасия Семенюк",
                                 FaceImageSource = "https://2019.codefest.ru/upload/members/photo_1548869794_200.jpg",
                                 Company = "Вконтакте"
                             }
                         },
                        SpeachStartTime = DateTime.Now.AddHours(1)
                    }
                }
            };
        }

        public void ScrollTo(DateTime dt)
        {
            var speach = section.Speachs.FirstOrDefault(x => x.SpeachStartTime == dt);
            if (speach != null)
                listView.ScrollTo(speach, ScrollToPosition.Start, false);
        }
    }
}