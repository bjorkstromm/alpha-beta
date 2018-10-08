/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:alpha_beta"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using alpha_beta.core;

namespace alpha_beta.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var configuration = new Configuration(
                System.Configuration.ConfigurationManager.AppSettings.Get("locale"),
                System.Configuration.ConfigurationManager.AppSettings.Get("searchApiKey"),
                System.Configuration.ConfigurationManager.AppSettings.Get("speechApiKey"));
            SimpleIoc.Default.Register<Configuration>(() => configuration);
            SimpleIoc.Default.Register<ImageService>();
            SimpleIoc.Default.Register<WordService>();
            SimpleIoc.Default.Register<AudioService>();
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}