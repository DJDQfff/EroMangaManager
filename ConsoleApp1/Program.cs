using static EroMangaDB.BasicController;

namespace DatabaseOperation
{
    internal class Program
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private static void Main ()
        {
            DatabaseController.Migrate();
            //var a = DatabaseController.TagKeywords_QueryAll();

            //await DatabaseController.TagKeywords_AddTagSingle("fjakjf", new string[] { "djfa", "jfds" });

            //var b = DatabaseController.TagKeywords_QueryAll();

            //DatabaseController.Dispose();
            //System.Console.ReadKey();
        }
    }
}