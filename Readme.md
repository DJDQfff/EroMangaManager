# 要实现的目标

* 添加更多排序方式，把排序方式添加到设置

* 在LibraryPage中添加一个开关，是否显示本子数为0 的文件夹

* MangaBook类各个功能打散

* 先暂停tag功能

* 功能：修改tag名

* 使用EFCore的泛型方法，不需要那么多分散的类

* 标签库管理

* 升级版：联网过滤图库
  
  * [ ] 过滤图库
  
  * [ ] 汉化组库
  
  * [ ] 作者库

* 图片ocr识别，即时翻译

* 完善异常捕获、日志功能

# 已实现的

* 检查漫画文件里不是图片的内容
  
  > 仅检查了文件后缀名

# 放弃的功能

* 添加一个应用内全屏的功能（非硬件全屏）
  
  > 反正已经实现全屏了，没必要多此一举

* 把程序页面扩展到标题栏
- ReadingInfo单独作为一个数据库，不和tag数据库放在一起，二者不具有相关性，不应该放在一个数据库里面

> 没必要，ReadingInfo、tag、filteredImage这三个数据库无论哪一个发生更新，程序都必须兼顾更新。简而言之，无论分散和集中，这三个都是同时使用的

# 结构调整

* Reader页面翻页，一开始是设计的绑定到压缩文件的各个Entry，这样可以节省资源，只在发生切换页面的时候加载图像，但这样会发生两个问题：
  
  1. 解码很慢
  
  2. 有一个bug，压缩文件的ZipArchive类（标准dotnet库或者别的库）不是线程安全，在翻页时候加载图像，就需要多线程解压缩图像，因此线程报错。
  
       现在改成了提前解压文件获取图像，每个页面直接绑定到图像，不需要SelectionChange事件

# 潜在bug

* 生成mangasfoldervm时，readinginfo服务存在线程冲突的bug  
  
  试着把他对数据库的写操作挪到一个顺序上上  
  
  思路二，不再分别作为任务，而是把整个作为任务丢后台。

# 更新日志

## 2023.2.2

* 所有MangaBook类不再在内部默认初始化默认封面，把封面缓存放到CoverHelper静态类里面作为一个属性，所有Mangabook类实例共享同一个封面图片缓存

* 把MangaBook类的ChangeCover方法提出来，放到CoverHelper助手类里面，作为一个扩展方法存在

## 2023.1.30

* 给MangaBook类添加文件大小属性，在Bookcase页面进行显示

* Bookcase页面添加功能：按

## 2023.1.25

* 阅读页面全屏

* MainPage的子页面导航时，左侧导航Item无法自动切换为选中项的

* ReadPage提供全屏功能，但是目前存在bug，暂停
  
  > bug可能存在的一个原因：每次导航是都导向的新的页面实例 一个可能的解决办法：实现阅读进度功能，每次导航的时候定位到进度位置 这样就算你有导航到新页面实例，也不影响
  > 
  > 解决了，因为用了两个Frame，虽然是同一个Page，但是不同Frame产生不同Page缓存

## 2023.1.23

* 有原来的BookcaseContainer里面导航到不同Bookcase页面（对应一个文件夹内的漫画），改为一个Bookcase类，其数据源改为使用数据绑定到对应的MangaFolder类

## 2022.12.19

* 添加导出为pdf的功能

* 添加多语种（以后都用自动翻译算了）

## 2022.02.06

* 标签库管理
- 调整页面跳转方式

- 添加单行本、短篇标签

- Tag分析方法可以优化一下

- 一个识别关键词只能归属于一个Tag，一个Tag可以包含多个识别关键词

- ReaderPage页翻页时，页面会闪烁
  
  > 不知道怎么就解决了
* 发现一个bug，如果本子文件名不符合一般本子规律，则报错，如果本子名未分标签，则报错
  
  > 以修改了识别tag方法

* 快速翻页有Bug
  
  > 这个bug，一开始不知道ZipArchive类是线程不安全的，对一个类实例同时打开两个entry，所以报错
  > 
  > 之后调整为两个类实例，就不会报错了。
  > 
  > 但是，在中间测试的时候，把entry筛选条件设为“非文件夹即可”时，此筛选也有报错；把筛选条件恢复正常后，就没报错了。
  > 
  > 总之算是解决了
- 对数据库的操作全部集合到一个类中，只实例化一次DataBase类。
  
  > 原本是每次对数据库读写就实例化一次类，性能很低。
  > 
  > 之后把读写方法封装到类库中去，成为一个

- Reader类实现IDisposable接口

- 对翻译过名称的漫画，保存进数据库，以后加载漫画直接显示翻译过的名字

- BookcasePage，添加一个刷新按钮，刷新以重设封面

- 添加漫画前，检查是否是漫画
  
  1. 是否是zip文件
  
  2. 判断文件名是否含有标签
  
  3. 只获取jpg、png

- 自定义Tag、数据库、读取自定义数据库、调整页

- 解析CM届数

- 把现在的一次性加载所有图片功能改为延迟加载
  
  > 勉强实现，没有使用loaded事件和x:load属性，通过在selectionchanged事件中实现

- 还原被过滤图：把图从过滤库中移出，在ReadPage添加一个刷新按钮，进行刷新，

- 图片过滤功能
  
  - 过滤ReadPage显示图片
  - 修改所有封面
