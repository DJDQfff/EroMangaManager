- [x] 实现远程控制功能：
  >  - [ ]  内置webapplication：内嵌asp.net core的blazor，unolpatform的web端，远程通过网页即可访问
  >  - [x] 内嵌httpapi：需要外部搭建客户端（如unoplatform的安卓端）
  >  - [ ] 搭建httpapi获取参数，smb负责输出文件
- [ ] 漫画相等性比较，可以添加相似比较，

- [ ] manga在外部exe进程结束后，将重新加载信息时。若文件较大，加载时间较长如果此时调用删除方法，两方法将发生冲突，会导致UI线程错误。

  > 可以考虑将manga的io操作设计一个优先级枚举，比如：当删除时，停止读取。可以用一个字典存储不同manga的io枚举状态，正在进行io操作的manga添加到此字典中，未进行io操作的则不需要存储。可以考虑从coversetter类升级改造

- [ ] searchmanga页面的searchbytag控件放到一个checkbox里面，勾选此checkbox以一次性启用或禁用所有tag

- [x] 本子是否相同的算法可以提取出来，同时供searchmanga页面和findsamemanga页面使用
  
  > searchmanga页面的searchbyname控件旁边添加一个combobox，以切换搜索模式：完全相同、或近似相同、或其他符合条件的搜索模式

- [x] 本子页面控件弹出菜单多一个页面，弹出当前本子所有tag（包括括号内的，不在括号内的以空格划分，点击就可以复制）
  
  > 点击查看本子信息页面会弹出页面，显示tag，但没做复制tag功能

- [x] 本子菜单多一个功能，跳转到搜索页面。

- [ ] 目前所有manga的右键菜单都是引用的不同flyout，想办法让这些共用
  
  > 理论上应该是可以共用相同flyout的，但因为x:name和x:key之间的掌握还不够深入，暂时做不到

- [x] 标签管理页面：标签不要存储到数据库里面，软件内单独建立文件夹：直接用txt存，文件名就是分类
  
  > 不做了，以前设计这个是为了导入导出，没必要弄这个功能，反正就我自己用

- [x] 文件名重命名页面升级：右键列表等，让修改更方便

- [x] communitytoolkit的tokeningtextbox和settingcontrols包版本用的是8.1.240916，更高级的版本无法获取到控件的引用
  
  > 高版本的包好像更新修复了

- [x] 首先这个没有子文件夹检测，只能识别你选定的这个文件夹内的zip格式数量，识别不到子文件夹内的zip压缩包。他没有打标签功能，分类要自己整理完再压缩成zip格式，希望更新一个打标签功能，可以按标签进行分类，筛选同一标签下的本子观看。还有一个很大的问题，我用的是7z压成aip格式，在应用内打开图片顺序是混乱的，这个不知道是不是bug。

- [x] 标签管理页面有三个控件无法显示英文
  
  > resw文件修改了，一运行就会变成修改前的内容
  > 
  > 禁用多语言工具就自己好了

- [ ] RemoveRepeatTags页面不知道为什么总是会串

- [x] RepaeatPage的button事件方法有bug，无法解决，StackOverflow上有回复，但还是无法解决
  
  > 其实还是没解决，放弃原来设想的重命名所有，改为手动一个一个重命名

- [x] ReadPage的FlipView的数据源绑定到Entry，切换时加载图象。切换item时，偶尔会发生页面闪烁的情况，有空去stackoverflow问问。
  
  > 这个问题导致阅读位置跳转功能也不正常

- [x] ReaderVM的ShowAllEntry方法耗时很长，已知以下位置会触发bug，改为用Entry
  
  - FindSameManga方法删除。在这个页面打开漫画后，删除很容易引发bug，文件越大越容易。未处理
  
  - ReadPage的FilterThisImage，暂时隐藏
  
  - ReadPage的SaveImageAs方法，暂时隐藏

- [x] 添加文件夹，如果内容很多，页面卡
  
  > 源码的foreach使用List.ToArray()后就解决了这个问题

- [ ] 用readpage打开压缩包会在第一面卡住
  
  > 懒得写这个了
* [x] UWP中，efcore默认数据文件在数据LocaStata中，WinUI3 中，数据库文件在C:\Users\27431\AppData\Local\VirtualStore\Windows\SysWOW64\localdatabase.db这个文件夹里面
- [x] 搜索Orange，会把一个包含在tag的本子搜出来
  
  > 修改本子名解析方法后，这个问题就消失了

- [x] 迁移到WinUI后，暂时懒得弄文件类型关联了，反正也不是用来看本子的

- [x] 单实例多实例问题未仔细看
  
  > 反正也没事，以后再说

- [ ] 扩展标题栏后，Rectangel的Fill属性设置为Transparent，但是无法显示内容

- [ ] 改到winui后全屏功能失效，隐藏控件了
  
  > 不写阅读功能了

- [ ] ReadPage的error.svg无法居中

- [x] 设置页面邮件邮件跳转超链接无效

- [ ] 重新组织本子的功能

- [ ] 现在有了外部打开本子的功能，以后添加一个点击Tag外部打开的功能

- [ ] 翻译本子名的功能，bookcase的contextflyout页显示转换单个，commandbar显示一个转换所有

- [ ] 导出epub，有一个读取epub的库，没有导出的库

- [x] 查找相同本子页面，添加相似查找，及翻译查找，为MangaBook的相同BookName设计一个模糊匹配算法

- [x] 移除重复Tag功能界面，添加自动化功能，不然一个一个手动弄搞到什么时候

- [x] 添加界面化文件名Tag修改界面

- [x] 在初始化GlobalViewModel时添加文件夹的话，会报错（被修改的集合、原来的MangasFolder不初始化）

- [x] Tag管理页面：用于管理所有Tag，

- [x] Bookcase的上下文菜单里面取消删除，然后添加一个本子详情页面，把删除移到里面来

- [ ] LibraryPage添加了一个MissingFolder属性的显示，优化下界面

- [x] 添加功能“手动识别子文件夹”，但不会添加到访问目录里面去，且不是自动

- [x] 添加一项设置，调用外部压缩软件，比如7z或peazip

- [x] 加了一个主题设置
  
  > 但是懒得弄，还得翻看文档

- [x] 多本合并的功能
  
  > 不弄了，有需要的自行手动

- [x] 可以添加多个打开方式，在书架页面添加切换功能。
  
  > 设置界面已实现添加多个exe的功能，但是没弄在Bookcase页面切换的功能，暂时不需要

- [x] MnagsGroup的UpdateStates属性和IsInitialing属性看看存在功能重叠看看能不能消掉IsInitialing

- [x] 若一个文件夹加载时，取消此文件夹，后台任务不会取消
  
  > 改为状态了
* [ ] 显示单个文件夹改为勾选哪些文件夹显示

* [ ] 把所有异常解析的MangaName收集到一个文件
- [x] 对文件夹的初始化使用多线程同时加载
  
  > 因为涉及UI更新，所以无法直接多线程初始化，可以尝试并行任务（还没试过）
  
  > 改为队列加载了，队列中前一个groups没加载完，后一个不会开始加载。

- [ ] 需要优化ReaderVM的实例释放问题，disposs
  
  > 这个曾触发过bug：打开一个本子时，这个本子的所有页面还没有加载完，就打开下一个本子。会提示报错（打开了一个关闭的流）。可能原因：旧本子的加载图片功能还在进行，打开新本子的时候，旧本子的加载图片方法还在进行，但是此时旧本子数据是关闭了的。
  > FindSameMangaPage弹出阅读页面后，在关闭，但是内存每次都会涨，说明ReadVM的资源释放还是有问题

- [ ] 完善异常捕获、日志功能

- [ ] 更新页面和软件说明页面的内容可以转为自动化生成，免得每次都手动更新xaml

## UpdateChanges

## 2025.7.10

* bookcas页面添加一个combobox可以切换书架

## 2025.7.4

* 现在可以读取文件夹

## 2025.3.3

* 删除app.cs里的tokens属性，修改加载方式后，不需要token来管理这个

* 原来添加文件夹后，直接开始加载里面的本子。现在改为：先获取所有文件夹，再按顺序加载每个文件夹的本子，再加载的时候添加新文件夹，也按照顺序等待前面的执行完毕，而不是是立即开始加载。

### 2024.2.25

* 优化漫画打开方式的vm和设置界面

### 2024.2.5

* 添加多个exe路径作为打开方式

### 2024.1.15

* 在初始化globalviewmodels时，忽略不存在的文件夹

* 切换bookcase的item

* 检索子文件夹的设置

* 修复SearchMangaPage页面默认case的bug

### 2023.11.09

* 添加语言设置功能

* sharpconfig更改为config.net

* 添加功能“手动识别子文件夹”，但不会添加到访问目录里面去

* 给数据库设定位置

* 修复导出pdf功能

* MainWindow自定义标题栏为透明Rectangle，

* 修改窗口标题栏

### 2023.10.26

- 把ReaderVM提到standard2.0里面去，同时调整了EroMangaManager.Core这个命名空间

- 添加对移除的文件夹的处理，但是LibraryPage界面太丑，以后优化

- 加了一个控件，用来自动生成UpdataPage和说明Page，但是懒得弄，还是手动复制算了

- 把Bookcase和SearchManga的漫画显示封面放到一个控件里

- 设置UWP项目程序集信息语言为zhcn，不再出现pri警告了

- 优化MangaName解析方法，现在大部分都可以正常解析，不能解析的看注释

- 把SplitAndParse方法返回值改为元祖，第一个为MangaName（可能为null），第二个为tags

### 2023.6.25

- 支持webp

### 2023.6.14

- 把RepeatTagRename功能放到一个页面里，不再弹出对话框。

- 给MangasFolder添加一个显示字符串，并在BookcasePage显示

- 封面加载放在创建MangaBook后，然后再放进集合中

- 给ReadVM添加一个错误png打开时的返回图像，但是无法居中显示

- 文件夹改为存放到EFCore数据库里面，默认文件夹放到SharpConfig设置里面

- 改回了用Entry作为ReadPage的FlipView数据源的模式，但是又出现了切换页面时闪烁的问题

### 2023.4.22

- 修复默认显示文件夹功能

- 修复默认显示文件夹功能

- 不再使用系统设置，改为使用SharpConfig来存储

- ReadPage改为弹出新窗口

- 修复文件关联激活打开的功能的bug

### earlier

- 添加Search结果跳转到Bookcase的控件，实现多线程后台创建封面（但是有点卡）

- 添加SearchPage页面，实现按“本子名”、“标签”查找功能，包含Tag的Linq自动提示，搜索本子

- 给重复tag查找功能添加修改功能，使能移除重复tag

- 修改FunctionRepeatMangaPage 的UI，改为使用CommandBar

- 完善本子重命名功能

- 把导出pdf放到Task.Run方法里面去

- 把MangaBook类，MangasFolder类、ObservableCollectionVM类放到standard2.0库里面

- 创建一个UWP文件权限管理类，存放所有StorageFIle和StorageFolder权限，对传入文件路径时，负责返回权限，负责文件改名、删除。

- 把MangaBook提取到standard2.0库里面去

- 实现漫画文件重命名功能，但UI没有随之变化，需要优化

- 添加检查重复tag的功能。只添加了查看的功能，还没添加修改的功能

- APP 属性observableCollection改名GlobalVewModel

- ReaderVM简单细化，但还是需要更多优化

- MangaBook的CoverPath不用数据绑定了，改为直接一次性，

- ReaderVM实例释放内存还是有问题，简单修改了一下

- MangaBook类改为使用封面图片路径，不再预加载封面图片缓存，但还是有问题：无法在更新集合的时候显示书架
  
  > 此bug已修复

- 从MangaBook类中移除Cover属性，以后使用CoverPath属性

- MangaBook初始化时不设置ReadingInof了，MangaName和Tags属性改为由方法获取

- 不在ReadingInfo里放文件名自带的tag了（改为需要时，从文件名即时解析），添加一个TagsAddedByUser属性

- 给libraryPage添加一个丑不拉几的本子加载中进度条

- 给ObservableCollectionVM也添加一个IsContentInitializing属性，并在FunctionPage上显示一个控件，提示正在初始化

- 把使用说明和更新日志挪到MainPage

- 给MangasFolder添加一个标志位，显示是否在初始化这个类的数据

- 把ObserVableCollection的初始化工作放大App.OnLaunch方法最后面，以前是放在最前面的。现在不会在开屏卡半天界面了（因为需要创建本子封面，耗费了大量时间）。

- “请添加第一个文件夹“的提示，控件可见性不再写进代码逻辑里面，直接把可见性绑定到ObservableCollection的文件夹个数

- 所有MangaBook类不再在内部默认初始化默认封面，把封面缓存放到CoverHelper静态类里面作为一个属性，所有Mangabook类实例共享同一个封面图片缓存

- 把MangaBook类的ChangeCover方法提出来，放到CoverHelper助手类里面，作为一个扩展方法存在

- 给MangaBook类添加文件大小属性，在Bookcase页面进行显示

- Bookcase页面添加功能：按

- 阅读页面全屏

- MainPage的子页面导航时，左侧导航Item无法自动切换为选中项的

- ReadPage提供全屏功能，但是目前存在bug，暂停
  
  > bug可能存在的一个原因：每次导航是都导向的新的页面实例 一个可能的解决办法：实现阅读进度功能，每次导航的时候定位到进度位置 这样就算你有导航到新页面实例，也不影响
  > 解决了，因为用了两个Frame，虽然是同一个Page，但是不同Frame产生不同Page缓存

- 有原来的BookcaseContainer里面导航到不同Bookcase页面（对应一个文件夹内的漫画），改为一个Bookcase类，其数据源改为使用数据绑定到对应的MangaFolder类

- 添加导出为pdf的功能

- 添加多语种（以后都用自动翻译算了）

- 标签库管理

- 调整页面跳转方式

- 添加单行本、短篇标签

- Tag分析方法可以优化一下

- 一个识别关键词只能归属于一个Tag，一个Tag可以包含多个识别关键词

- ReaderPage页翻页时，页面会闪烁
  
  > 不知道怎么就解决了

- 发现一个bug，如果本子文件名不符合一般本子规律，则报错，如果本子名未分标签，则报错
  
  > 以修改了识别tag方法

- 快速翻页有Bug
  
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

## 结构调整

- Reader页面翻页，一开始是设计的绑定到压缩文件的各个Entry，这样可以节省资源，只在发生切换页面的时候加载图像，但这样会发生两个问题：
  
  1. 解码很慢
  
  2. 有一个bug，压缩文件的ZipArchive类（标准dotnet库或者别的库）不是线程安全，在翻页时候加载图像，就需要多线程解压缩图像，因此线程报错。

             现在改成了提前解压文件获取图像，每个页面直接绑定到图像，不需要SelectionChange事件

3.     绑定到图象很多问题，有改了回去，不过这次用的类库是SharpCompress类库，没有发生线程安全问题了，但是还是会时不时出现切换页面闪烁的问题
* 已经获取到的文件夹token，在文件夹被外部重命名时，数据库里的文件夹名称是不会修改的，因此，再把文件夹名称改回去，然后添加到库里面，是会出现重复的，简单的处理了一下：在初始化时候，对名称进行相同比较