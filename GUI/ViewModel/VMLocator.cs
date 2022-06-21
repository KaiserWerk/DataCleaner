using KaiserMVVMCore;

namespace GUI.ViewModel
{
    public class VMLocator
    {
        public VMLocator()
        {
            IocContainer.Register<MainViewModel>(RegisterMode.Singleton);
       
        }

        public MainViewModel MainViewModelInstance => IocContainer.GetInstance<MainViewModel>();
        
    }
}
