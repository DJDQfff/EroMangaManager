# 要实现的功能

* 查找相同本子页面，添加相似查找，及翻译查找

* 在初始化GlobalViewModel时添加文件夹的话，会报错（被修改的集合、原来的MangasFolder不初始化）

* Tag管理页面：用于管理所有Tag，

* 阅读进度保存，及本子总页数（这两个是一体的）

# 需要持续优化项

* 对文件夹的初始化使用多线程同时加载
  
  > 一直出问题。已知在MainPage里把导航项LibraryPage的代码注释掉的话，就能多线程同时加载，和UI线程冲突

* 需要优化ReaderVM的实例释放问题，disposs
  
  > 这个曾触发过bug：打开一个本子时，这个本子的所有页面还没有加载完，就打开下一个本子。会提示报错（打开了一个关闭的流）。可能原因：旧本子的加载图片功能还在进行，打开新本子的时候，旧本子的加载图片方法还在进行，但是此时旧本子数据是关闭了的。
  > 
  > FindSameMangaPage弹出阅读页面后，在关闭，但是内存每次都会涨，说明ReadVM的资源释放还是有问题

* 完善异常捕获、日志功能

# 更新日志

* 修复默认显示文件夹功能

* 修复默认显示文件夹功能

* 不再使用系统设置，改为使用Config.Net来存储

* ReadPage改为弹出新窗口

* 修复文件关联激活打开的功能的bug

## 更早版本

* 添加Search结果跳转到Bookcase的控件，实现多线程后台创建封面（但是有点卡）

* 添加SearchPage页面，实现按“本子名”、“标签”查找功能，包含Tag的Linq自动提示，搜索本子

* 给重复tag查找功能添加修改功能，使能移除重复tag

* 修改FunctionRepeatMangaPage 的UI，改为使用CommandBar

* 完善本子重命名功能

* 把导出pdf放到Task.Run方法里面去

* 把MangaBook类，MangasFolder类、ObservableCollectionVM类放到standard2.0库里面

* 创建一个UWP文件权限管理类，存放所有StorageFIle和StorageFolder权限，对传入文件路径时，负责返回权限，负责文件改名、删除。

* 把MangaBook提取到standard2.0库里面去

* 实现漫画文件重命名功能，但UI没有随之变化，需要优化

* 添加检查重复tag的功能。只添加了查看的功能，还没添加修改的功能

* APP 属性observableCollection改名GlobalVewModel

* ReaderVM简单细化，但还是需要更多优化

* MangaBook的CoverPath不用数据绑定了，改为直接一次性，

* ReaderVM实例释放内存还是有问题，简单修改了一下

* MangaBook类改为使用封面图片路径，不再预加载封面图片缓存，但还是有问题：无法在更新集合的时候显示书架
  
  > 此bug已修复

* 从MangaBook类中移除Cover属性，以后使用CoverPath属性

* MangaBook初始化时不设置ReadingInof了，MangaName和Tags属性改为由方法获取

* 不在ReadingInfo里放文件名自带的tag了（改为需要时，从文件名即时解析），添加一个TagsAddedByUser属性

* 给libraryPage添加一个丑不拉几的本子加载中进度条

* 给ObservableCollectionVM也添加一个IsContentInitializing属性，并在FunctionPage上显示一个控件，提示正在初始化

* 把使用说明和更新日志挪到MainPage

* 给MangasFolder添加一个标志位，显示是否在初始化这个类的数据

* 把ObserVableCollection的初始化工作放大App.OnLaunch方法最后面，以前是放在最前面的。现在不会在开屏卡半天界面了（因为需要创建本子封面，耗费了大量时间）。

* “请添加第一个文件夹“的提示，控件可见性不再写进代码逻辑里面，直接把可见性绑定到ObservableCollection的文件夹个数

* 所有MangaBook类不再在内部默认初始化默认封面，把封面缓存放到CoverHelper静态类里面作为一个属性，所有Mangabook类实例共享同一个封面图片缓存

* 把MangaBook类的ChangeCover方法提出来，放到CoverHelper助手类里面，作为一个扩展方法存在

* 给MangaBook类添加文件大小属性，在Bookcase页面进行显示

* Bookcase页面添加功能：按

* 阅读页面全屏

* MainPage的子页面导航时，左侧导航Item无法自动切换为选中项的

* ReadPage提供全屏功能，但是目前存在bug，暂停
  
  > bug可能存在的一个原因：每次导航是都导向的新的页面实例 一个可能的解决办法：实现阅读进度功能，每次导航的时候定位到进度位置 这样就算你有导航到新页面实例，也不影响
  > 解决了，因为用了两个Frame，虽然是同一个Page，但是不同Frame产生不同Page缓存

* 有原来的BookcaseContainer里面导航到不同Bookcase页面（对应一个文件夹内的漫画），改为一个Bookcase类，其数据源改为使用数据绑定到对应的MangaFolder类

* 添加导出为pdf的功能

* 添加多语种（以后都用自动翻译算了）

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
  > 之后调整为两个类实例，就不会报错了。
  > 但是，在中间测试的时候，把entry筛选条件设为“非文件夹即可”时，此筛选也有报错；把筛选条件恢复正常后，就没报错了。
  > 总之算是解决了
- 对数据库的操作全部集合到一个类中，只实例化一次DataBase类。
  
  > 原本是每次对数据库读写就实例化一次类，性能很低。
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

# 结构调整

* Reader页面翻页，一开始是设计的绑定到压缩文件的各个Entry，这样可以节省资源，只在发生切换页面的时候加载图像，但这样会发生两个问题：
  
  1. 解码很慢
  
  2. 有一个bug，压缩文件的ZipArchive类（标准dotnet库或者别的库）不是线程安全，在翻页时候加载图像，就需要多线程解压缩图像，因此线程报错。
  
       现在改成了提前解压文件获取图像，每个页面直接绑定到图像，不需要SelectionChange事件

# 放弃的功能

* 添加一个应用内全屏的功能（非硬件全屏）
  
  > 反正已经实现全屏了，没必要多此一举

* 把程序页面扩展到标题栏

* ReadingInfo单独作为一个数据库，不和tag数据库放在一起，二者不具有相关性，不应该放在一个数据库里面

> 没必要，ReadingInfo、tag、filteredImage这三个数据库无论哪一个发生更新，程序都必须兼顾更新。简而言之，无论分散和集中，这三个都是同时使用
