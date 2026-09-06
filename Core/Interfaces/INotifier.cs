using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces;

public interface INotifier
{
    void Notify(string message, int durationInSeconds = 5);
}
