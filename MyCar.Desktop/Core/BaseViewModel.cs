using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.Core
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected object mPropertyValueCheckLock = new object();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void SignalChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        protected async Task RunCommandAsync(Expression<Func<bool>> updatingFlag, Func<Task> action)
        {
            lock (mPropertyValueCheckLock)
            {
                if (updatingFlag.GetPropertyValue())
                    return;
                updatingFlag.SetPropertyValue(true);
            }
            try
            {
                await action();
            }
            finally
            {
                updatingFlag.SetPropertyValue(false);
            }
        }
    }
}
