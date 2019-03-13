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
        public SectionView(SectionItem section)
        {
            InitializeComponent();
            BindingContext = IocContainter.Container.Resolve<SectionViewModel>(new TypedParameter(typeof(SectionItem), section));
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
    }
}