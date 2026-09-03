using System;
using System.Collections.Generic;
using System.Text;

using WinApp.Strings;

namespace UnoLibrary;

public record NavigationItem (Type Page , StringsEnum Uid ,string UidValue ,SymbolIcon Icon);

