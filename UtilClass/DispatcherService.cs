using System;
using System.Windows;
using System.Windows.Threading;

namespace VewModelSample.UtilClass
{
    public static class DispatcherService
    {
        public static void Invoke(Action action) // 동기
        {
            Dispatcher dispatchObject = Application.Current != null ? Application.Current.Dispatcher : null;
            if (dispatchObject == null || dispatchObject.CheckAccess())
                action();
            else
                dispatchObject.Invoke(action);
        }

        public static void BeginInvoke(Action action) // 비동기
        {
            Dispatcher dispatchObject = Application.Current != null ? Application.Current.Dispatcher : null;
            if (dispatchObject == null || dispatchObject.CheckAccess())
                action();
            else
                dispatchObject.BeginInvoke(action);
        }
    }
}
