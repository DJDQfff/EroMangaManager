namespace Tools;

internal class Class1
{
    public void CheckNonOrigin()
    {
        Console.WriteLine("Hello, World!");
        var directory = new DirectoryInfo(@"D:\Downloads\bika_downloads\commies");
        var directories = directory.EnumerateDirectories();
        foreach (var d in directories)
        {
            var c = d.GetDirectories();
            if (c.Length != 1)
            {
                CommonLibrary.ExplorerFile.ExplorerSelectFile(d.FullName);
                Console.Read();
            }
        }
    }
}
