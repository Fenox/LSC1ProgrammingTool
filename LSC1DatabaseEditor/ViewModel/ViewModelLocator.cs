/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:LSC1DatabaseEditor"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels;

namespace LSC1DatabaseEditor.ViewModel
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

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Unregister<NLog.Logger>();
            SimpleIoc.Default.Register<NLog.Logger>(LoggerFactory);

            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<LSC1EditorMenuVM>();
        }

        public MainWindowViewModel Main
        {
            get => ServiceLocator.Current.GetInstance<MainWindowViewModel>();
        }

        public LSC1EditorMenuVM MenuVM
        {
            get => ServiceLocator.Current.GetInstance<LSC1EditorMenuVM>();
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        private NLog.Logger LoggerFactory() => NLog.LogManager.GetLogger("Usage");
    }
}