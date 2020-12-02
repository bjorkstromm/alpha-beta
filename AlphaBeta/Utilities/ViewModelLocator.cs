using System.IO;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Extensions.Configuration;
using AlphaBeta.ViewModels;
using AlphaBeta.Core;

namespace AlphaBeta.Utilities
{
    public class ViewModelLocator
    {
        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();

        public ViewModelLocator()
        {
            var configRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var configuration = new Configuration();
            configRoot.Bind(configuration);

            SimpleIoc.Default.Register(() => configuration);
            SimpleIoc.Default.Register<ImageService>();
            SimpleIoc.Default.Register<WordService>();
            SimpleIoc.Default.Register<AudioService>();
            SimpleIoc.Default.Register<MainViewModel>();
        }
    }
}