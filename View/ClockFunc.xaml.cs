using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using VewModelSample.ViewModel;

namespace VewModelSample.View
{
    /// <summary>
    /// Clock.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ClockFunc : UserControl
    {
        public ClockFunc()
        {
            InitializeComponent();
            //this.DataContext = ClockViewModel.Instance;
        }
    }
}
