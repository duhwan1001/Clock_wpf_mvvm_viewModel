using System.Windows;
using VewModelSample.ViewModel;

namespace VewModelSample.View
{
    /// <summary>
    /// StopwatchFunc.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StopwatchFunc : Window
    {
        public StopwatchFunc()
        {
            InitializeComponent();
            this.DataContext = StopwatchViewModel.Instance;
        }
    }
}
