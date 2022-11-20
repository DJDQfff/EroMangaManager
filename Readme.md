# 要实现的目标

* ReadingInfo单独作为一个数据库，不和tag数据库放在一起，二者不具有相关性，不应该放在一个数据库里面

* 功能：导出本子为pdf

* 功能：修改tag名

* 使用EFCore的泛型方法，不需要那么多分散的类

* TagKeywords：

> 原来的用法：
> 
>     把很作作者作者名字作为KeyWords属性，TagName为“作者”，起到一个分类的作用。
> 
> 要调整的用法：
> 
>     如“初音未来”和MIKU识别为一个Tag，TagName为初音未来。
> 
> 这样以后以TagKeywords为基本Tag，进行分类

* 使用EFCore的泛型方法，不需要那么多分散的类

* 标签库管理

* 升级版：联网过滤图库
  
  * 过滤图库
  
  * 汉化组库
  
  * 作者库

* 图片ocr识别，即时翻译

* 添加异常捕获、日志功能

# 更新日志

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