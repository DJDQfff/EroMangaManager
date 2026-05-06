using EroMangaDatabase.Entities;
using EroMangaDatabase.EntityFactory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EroMangaDatabase
{
    public partial class BasicController
    {
        /// <summary>
        /// 查询所有TagKeywords的所有识别关键词
        /// </summary>
        /// <returns>字典，第一项为TagName，第二项为Kwywords</returns>
        public Dictionary<string, string[]> TagCategory_QueryAll()
        {
            var tags = database.TagCategorys.ToArray();

            //List<(string, string[])> vs = new List<(string, string[])>();

            var keyValuePairs = new Dictionary<string, string[]>();
            foreach (var tag in tags)
            {
                var vs1 = tag.Keywords.Split('\r');

                keyValuePairs.Add(tag.CategoryName, vs1);
            }

            return keyValuePairs;
        }

        /// <summary>
        /// 获取所有tagcategory分类，只包含分类，不包含分类的具体
        /// </summary>
        /// <returns></returns>
        public List<string> TagCategory_Query()
        {
            return database.TagCategorys.Select(x => x.CategoryName).ToList();
        }

        /// <summary>
        /// 查询单一TagKeywords的所有识别关键词
        /// </summary>
        /// <param name="tagname">TagKeywords的名称</param>
        /// <returns></returns>
        public string[] TagCategory_QuerySingle(string tagname)
        {
            string keywords = database.TagCategorys
                .Where(n => n.CategoryName == tagname)
                .Select(n => n.Keywords)
                .Single();
            string[] keywordarray = keywords.Split(['\r', '\n']);

            return keywordarray;
        }

        /// <summary>
        /// 创立一个新的TagKeywords
        /// </summary>
        /// <param name="tagname">TagKeywords名称</param>
        /// <param name="keywords">TagKeywords识别关键词</param>
        /// <returns></returns>
        public TagCategory TagCategory_AddCategorySingle(string tagname, params string[] keywords)
        {
            TagCategory tagKeywords = TagCategoryFactory.Creat(tagname, keywords);
            database.TagCategorys.Add(tagKeywords);
            return tagKeywords;
        }

        /// <summary>
        /// 在某既有TagKeywords后追加一个识别关键词
        /// </summary>
        /// <param name="tagname">TagKeywords的名称</param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task TagCategory_AppendKeywordSingle(string tagname, string keyword)
        {
            TagCategory tagKeywords = database.TagCategorys.Single(n => n.CategoryName == tagname);
            tagKeywords.Keywords += "\r" + keyword;
            await database.SaveChangesAsync();
        }

        /// <summary>
        /// 更新一个既有TagKeywords的所有识别关键字
        /// </summary>
        /// <param name="tagname"></param>
        /// <param name="keywords">更新后的关键词序列</param>
        /// <returns></returns>
        public async Task TagCategory_UpdateTagSingle(string tagname, IEnumerable<string> keywords)
        {
            string keywordString = string.Join("\r", keywords);
            TagCategory tagKeywords = database.TagCategorys.Single(n => n.CategoryName == tagname);
            tagKeywords.Keywords = keywordString;
            database.Update(tagKeywords);
            await database.SaveChangesAsync();
        }

        /// <summary>
        /// 移除某TagKeywords数据库行中的部分关键词
        /// </summary>
        /// <param name="categoryname">要修改的TagKeywords</param>
        /// <param name="tags">要从中移除的关键词序列</param>
        /// <returns></returns>
        public async Task TagCategory_DeleteKeywordsSingle(
            string categoryname,
            params string[] tags
        )
        {
            TagCategory tagKeywords = database.TagCategorys.Single(
                n => n.CategoryName == categoryname
            );
            var keywordsList = tagKeywords.Keywords.Split('\r').ToList();
            foreach (var word in tags)
            {
                keywordsList.Remove(word);
            }
            tagKeywords.Keywords = string.Join("\r", keywordsList);
            await database.SaveChangesAsync();
        }

        /// <summary>
        /// 将指定Keywords从原有TagKeywords中移除，并移动到新的目标TagKeywords
        /// </summary>
        /// <param name="targetpairs">要移动的项集合。第一项为识别关键词，第二项为要移动到的目标的TagName。</param>
        /// <returns></returns>
        public async Task TagCategory_MoveMulti(IDictionary<string, string> targetpairs)
        {
            var keywordContainInfo = DatabaseController.TagCategory_SearchiKeywordsMulti(
                targetpairs.Keys.ToArray()
            );

            foreach (var target in targetpairs)
            {
                string keyword = target.Key;
                string newTagName = target.Value;
                string oldTagName = keywordContainInfo[keyword];

                if (keywordContainInfo[target.Key] != null) // 若这个关键词已包含在数据库中，则从中移除
                {
                    await DatabaseController.TagCategory_DeleteKeywordsSingle(oldTagName, keyword);
                }

                await DatabaseController.TagCategory_AppendKeywordSingle(newTagName, keyword); // 添加的目标Tag
            }
        }

        /// <summary>
        /// 批量查找关键词属于哪些Tag
        /// </summary>
        /// <param name="keywords">要查询的关键词</param>
        /// <returns>字典。第一项为关键词，第二项为其所属的Tag（为null则无所属）</returns>
        public Dictionary<string, string> TagCategory_SearchiKeywordsMulti(
            IEnumerable<string> keywords
        )
        {
            var all = DatabaseController.TagCategory_QueryAll();
            Dictionary<string, string> keyValuePairs = [];
            foreach (var k in keywords)
            {
                string tagname = all.First(x => x.Value.Contains(k)).Key;
                keyValuePairs.Add(k, tagname);
            }
            return keyValuePairs;
        }

        /// <summary>
        /// 移除一个分类
        /// </summary>
        /// <param name="category"></param>
        public void TagCategory_RemoveCategory(string category)
        {
            var a = database.TagCategorys.SingleOrDefault(x => x.CategoryName == category);
            if (a != null)
            {
                database.Remove(a);
            }
        }

        // TOOD 没试过传入null会怎么样
        public void TagCategory_RemoveCategory(TagCategory category)
        {
            database.Remove(category);
        }

        /// <summary>
        /// 查找某关键词在哪个Tag里面
        /// </summary>
        ///
        /// <param name="keyword">要查找的识别关键词</param>
        /// <returns>TagName,如果没有，则为null</returns>
        public string TagCategory_SearchKeywordSingle(string keyword)
        {
            var all = DatabaseController.TagCategory_QueryAll();
            var str = all.First(x => x.Value.Contains(keyword)).Key;
            return str;
        }
    }
}