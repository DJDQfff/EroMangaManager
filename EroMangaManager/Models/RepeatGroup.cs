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

        public ObservableCollection<TElment> Collections;

        public RepeatGroup (IGrouping<TKey , TElment> _files)
        {
            files = _files;
            Collections = new ObservableCollection<TElment>(files.ToArray());
        }

        public int TryRemoveItem (TElment storageFile)
        {
            if (Collections.Contains(storageFile))
            {
                Collections.Remove(storageFile);
            }

            return Collections.Count;
        }

        public TKey Key => files.Key;

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
