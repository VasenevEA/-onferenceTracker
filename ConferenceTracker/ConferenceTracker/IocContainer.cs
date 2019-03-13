using Autofac;
using ConferenceTracker.Pages;

namespace ConferenceTracker
{
    public static class IocContainter
    {
        public static IContainer Container;

        public static void Init()
        {
            var containerBuilder = new ContainerBuilder();
            RegisterDependencies(containerBuilder);
            Container = containerBuilder.Build();
        }

        public static void RegisterDependencies(this ContainerBuilder builder)
        {
            builder.Register(c => new ConferenceManager("https://2019.codefest.ru")).As<ConferenceManager>().SingleInstance();
            //ViewModels 

            //Views 


            //Pages 
            builder.RegisterType<MainPage>();
            builder.RegisterType<SpeachesPage>();

            //Dialogs 
            builder.RegisterType<ViewModels.SectionViewModel>();
        }
    }
}
