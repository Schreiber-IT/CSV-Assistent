using System;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.Services
{
    public class WindowRegistration
    {
        public Type ViewModelType { get; }
        public Type WindowType { get; }
        public bool SingleInstance { get; }

        public WindowRegistration(Type viewModelType, Type windowType, bool singleInstance)
        {
            ViewModelType = viewModelType;
            WindowType = windowType;
            SingleInstance = singleInstance;
        }
    }
}
