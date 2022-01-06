using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using EroMangaManager.Database.Entities;

using Microsoft.EntityFrameworkCore;

namespace EroMangaManager.Database.DatabaseOperation
{
    public enum DefaultTagBasementType
    {
        TranslatorTagsFromUser,
        TranslatorTagsFromShared,
        AuthorTagsFromUser,
        AuthorTagsFromShared
    }
}