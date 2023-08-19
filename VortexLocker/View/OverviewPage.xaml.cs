using System.Windows.Controls;
using VortexLocker.ViewModel;

namespace VortexLocker.View
{
    /// <summary>
    /// Interaction logic for OverviewPage.xaml
    /// </summary>
    public partial class OverviewPage : Page
    {
        public OverviewPage()
        {
            InitializeComponent();
            DataContext = new OverviewVM(this);
        }
    }
}
