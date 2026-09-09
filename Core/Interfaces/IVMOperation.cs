using System;
using System.Collections.Generic;
using System.Text;
using Core.ViewModels;

namespace Core.Interfaces;

public interface IVMOperation
{
    void GetAllFolders(IEnumerable<string> storageFolders);
}
