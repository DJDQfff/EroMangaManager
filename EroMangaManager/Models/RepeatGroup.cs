using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

namespace EroMangaManager.Models
{
    internal class RepeatGroup<TKey,TElment>:IGrouping<TKey,TElment>
    {
        private IGrouping<TKey , TElment> files;

        public ObservableCollection<TElment> StorageFiles;

        public RepeatGroup (IGrouping<TKey , TElment> _files)
        {
            files = _files;
            StorageFiles = new ObservableCollection<TElment>(files.ToArray());
        }

        public int TryRemoveItem (TElment storageFile)
        {
            if (StorageFiles.Contains(storageFile))
            {
                StorageFiles.Remove(storageFile);
            }

            return StorageFiles.Count;
        }

        TKey IGrouping<TKey , TElment>.Key => throw new NotImplementedException();

        public IEnumerator<TElment> GetEnumerator () => files.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator ()
        {
            throw new NotImplementedException();
        }

    }

    internal class MangaBookRepeatGroup : RepeatGroup<string , MangaBook>
    {
        public MangaBookRepeatGroup (IGrouping<string , MangaBook> _files) : base(_files)
        {
        }
    }

}
