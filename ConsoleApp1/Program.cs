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
            //var a = DatabaseController.TagCategory_QueryAll();

            //await DatabaseController.TagCategory_AddTagSingle("fjakjf", new string[] { "djfa", "jfds" });

            //var b = DatabaseController.TagCategory_QueryAll();

            //DatabaseController.Dispose();
            //System.Console.ReadKey();
        }
    }
}