using UnoLibrary.Strings;

namespace UnoLibrary;

public record NavigationItem(Type Page, StringsEnum Uid, string UidValue, SymbolIcon Icon);
