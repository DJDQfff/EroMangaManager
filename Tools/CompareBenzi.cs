namespace Tools;

internal class CompareBenzi
{
    internal void Run()
    {
        // See https://aka.ms/new-console-template for more information

        var folder_disk = "D:\\复制到TF卡";
        var folder_sdcard = "F:\\本子";
        CompareFiles(folder_disk, folder_sdcard);
        CompareFoldersAndFiles(folder_disk, folder_sdcard);
    }

    void CompareFoldersAndFiles(string folder1, string folder2)
    {
        var folder_mangas = Directory.GetDirectories(folder1).Select(x => Path.GetFileName(x));
        var file_mangas = Directory
            .GetFiles(folder2)
            .Select(x => Path.GetFileNameWithoutExtension(x));
        var to_delete = folder_mangas.Except(file_mangas).ToList();
        var path1 = Directory.GetDirectories(folder1);
        int count = 0;
        foreach (var file in path1)
        {
            if (to_delete.Contains(Path.GetFileName(file)))
            {
                count++;
                Directory.Delete(file, true);
            }
        }
        Console.WriteLine(count);
    }

    void CompareFiles(string folder1, string folder2)
    {
        var files1 = Directory.GetFiles(folder1).Select(x => Path.GetFileNameWithoutExtension(x));
        var files2 = Directory.GetFiles(folder2).Select(x => Path.GetFileNameWithoutExtension(x));
        var single = files1.Except(files2).ToList();
        foreach (var item in single)
        {
            WriteLine(item);
        }
        WriteLine(single.Count);
        var path1 = Directory.GetFiles(folder1);
        foreach (var file in path1)
        {
            if (single.Contains(Path.GetFileNameWithoutExtension(file)))
            {
                File.Delete(file);
            }
        }
    }

    void CompareFolders(string folder1, string folder2)
    {
        var files1 = Directory
            .GetDirectories(folder1)
            .Select(x => Path.GetFileNameWithoutExtension(x));
        var files2 = Directory
            .GetDirectories(folder2)
            .Select(x => Path.GetFileNameWithoutExtension(x));
        var single = files1.Except(files2).ToList();
        foreach (var item in single)
        {
            WriteLine(item);
        }
        WriteLine(single.Count);
        var path1 = Directory.GetDirectories(folder1);
        foreach (var file in path1)
        {
            if (single.Contains(Path.GetFileNameWithoutExtension(file)))
            {
                Directory.Delete(file, true);
            }
        }
    }
}
