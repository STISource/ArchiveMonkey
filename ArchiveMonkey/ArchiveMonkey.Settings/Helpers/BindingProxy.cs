using System.Windows;

namespace ArchiveMonkey.Settings.Helpers
{
    public class BindingProxy : Freezable
    {
        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        #endregion

        public object Root
        {
            get { return (object)GetValue(RootProperty); }
            set { SetValue(RootProperty, value); }
        }

        public static readonly DependencyProperty RootProperty =
            DependencyProperty.Register("Root", typeof(object),
                                         typeof(BindingProxy));
    }
}
