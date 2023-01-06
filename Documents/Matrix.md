此处只是支持一个简单的矩阵类型。如果需要高级用法，请使用 [Math.NET Numerics](https://numerics.mathdotnet.com/)

# 矩阵使用
这是一个基于double类型写了一个简易版的矩阵类型，不包括矩阵求逆等高级功能。目前已有的简易功能：
* 从double类型数组导入矩阵
* 新建`n*m`大小的矩阵
* 矩阵加法、减法、乘法，均包含矩阵对数值的运算

库中包含两种类型的矩阵：
- `Matrix`：普通类型的矩阵，记录整个矩阵的形状。
- `DenseMatrix`：稠密型矩阵，仅记录行、列、值。

## 新建矩阵
### 直接创建新矩阵
```C#
//新建大小为n*m的0矩阵：
Matrix mt = new Matrix(2, 3); //创建一个2行3列的矩阵

//新建包含具体数值的矩阵
Matrix mt1 = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } }); //mt1:[1,2,3;4,5,6]
Matrix mt2 = new Matrix(new double[,] { { 2, 1 }, { 3, 2 }, { 2, 4 } }); //mt2:[2,1;3,2;2,4]
```

### 根据原有矩阵/稠密矩阵创建新矩阵
```C#
//根据一个已知矩阵复制一个矩阵
Matrix newMt = new Matrix(mt1);
```

```C#
//根据将稠密矩阵DenseMatrix转换为普通矩阵Matix:
Matrix matrix = new Matrix(dmt, rows, cols);
// 输入参数：
// dmt  DenseMatrix对象
// rows 新建矩阵的行数
// cols 新建矩阵的列数
```

示例
```C#
Matrix mt3 = new Matrix(dmt, 9, 9);
// DenseMatrix dmt
// [1 1 2;
//  7 8 8;
//  7 7 8]
// Matrix mt3
// [2 0 0 0 0 0 0 0 0;
//  0 0 0 0 0 0 0 0 0;
//  0 0 0 0 0 0 0 0 0;
//  0 0 0 0 0 0 0 0 0;
//  0 0 0 0 0 0 0 0 0;
//  0 0 0 0 0 0 0 0 0;
//  0 0 0 0 0 0 8 8 0;
//  0 0 0 0 0 0 0 0 0;
//  0 0 0 0 0 0 0 0 0]
```

### 将原有矩阵安排在新矩阵中
函数
```C#
// 将原有矩阵安排在新矩阵的左上角
Matrix matrix = new Matrix(matrix0, rows, cols)
```

```C#
// 将原有矩阵安排在新矩阵的指定行列位置
Matrix matrix = new Matrix(matrix0, rows, cols, row0, col0)
```
函数参数列表
| 参数      | 含义       |
| --------- | ---------- |
| `matrix0` | 原矩阵     |
| `rows`    | 新矩阵行数 |
| `cols`    | 新矩阵列数 |
| `row0`    | 指定行位置 |
| `col0`    | 指定列位置 |

用法示例
```C#
// 创建原有矩阵mt1
Matrix mt1 = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });
Console.WriteLine($"mt1\n{mt1.ValueString}");
// mt1
// [1 2 3;
//  4 5 6]

// 创建4*5的新矩阵，将原有矩阵安放在左上角
Matrix mt2 = new Matrix(mt1, 4, 5);
Console.WriteLine($"mt2\n{mt2.ValueString}");
// mt2
// [1 2 3 0 0;
//  4 5 6 0 0;
//  0 0 0 0 0;
//  0 0 0 0 0]

// 创建4*5的新矩阵，将原有矩阵安放在2行2列
Matrix mt3 = new Matrix(mt1, 4, 5, 2, 2);
Console.WriteLine($"mt3\n{mt3.ValueString}");
// mt3
// [0 0 0 0 0;
//  0 1 2 3 0;
//  0 4 5 6 0;
//  0 0 0 0 0]
```

## 新建稠密矩阵
```C#
//直接创建新的DenseMatrix对象
DenseMatrix dmt = new DenseMatrix();

//通过已知矩阵mt创建DenseMatrix（将Matrix转换为新的DenseMatrix对象）
DenseMatrix dmt2 = new DenseMatrix(mt);

//通过DenseMatrixItem的数组array创建DenseMatrix
DenseMatrix dmt3 = new DenseMatrix(array);
```

为了进一步体现复制的区别，此处对`DenseMatrix`对象做了如下实验：
```C#
// 创建DenseMatrixItem数组，通过这种方式创建DenseMatrix对象
DenseMatrixItem[] array = new DenseMatrixItem[3] {
    new DenseMatrixItem(1, 2, 1), new DenseMatrixItem(2, 2, 2), new DenseMatrixItem(1, 1, 3)
};

DenseMatrix dmt3 = new DenseMatrix(array); //通过数组的方式新建对象
Console.WriteLine("dmt3=\n" + dmt3.ValueString);
// dmt3=
// [1 2 1;
//  2 2 2;
//  1 1 3]
DenseMatrix dmt4 = dmt3; //直接引用
Console.WriteLine("dmt4=\n" + dmt4.ValueString);
// dmt4=
// [1 2 1;
//  2 2 2;
//  1 1 3]
DenseMatrix dmt5 = new DenseMatrix(dmt3); //复制对象
Console.WriteLine("dmt5=\n" + dmt5.ValueString);
// dmt5=
// [1 2 1;
//  2 2 2;
//  1 1 3]

// 更改值
dmt3.Set(1, 1, 4);

// 查看稠密矩阵中的值是否变化
Console.WriteLine("dmt3=\n" + dmt3.ValueString);
// dmt3=
// [1 2 1;
//  2 2 2;
//  1 1 4]
// (发生变化)
Console.WriteLine("dmt4=\n" + dmt4.ValueString);
// dmt4=
// [1 2 1;
//  2 2 2;
//  1 1 4]
// (发生变化)
Console.WriteLine("dmt5=\n" + dmt5.ValueString);
// dmt5=
// [1 2 1;
//  2 2 2;
//  1 1 3]
// (没有变化)
```

在编写`DenseMatrix(DenseMatrix dmt)`的初始化方法时，发现使用`Value = new List(DenseMatrixItem)`也会映射到原来的`DenseMatrixItem`中，是由于`DenseMatrixItem`也是对象，没有进行深拷贝。所以只能通过`Set()`方法逐条新建添加记录。

## 矩阵的值修改
### 根本方法（编程理解）
以上方的mt矩阵为例：
```C#
mt.Value[0,0] = 1; //将矩阵1行1列对应的元素值修改为1
```
对Value的修改是最直接的修改。
值得注意的是：`mt.Value`是一个`double`类型的二维矩阵。如果修改后矩阵的大小发生改变，`mt.Row`和`mt.Col`的值也会被动改变。

### 数学理解的方法
Matrix类中提供了两个函数进行基本的矩阵值操作。
#### Set()
`[matrix].Set(row, col, value)`，将行`row`列`col`的值设置为`value`。
```C#
Matrix mt = new Matrix(3,3);
mt.Set(1,1,1);
mt.Set(2,2,2);
mt.Set(3,3,3);
Console.WriteLine(mt.ValueString);

// 结果
// [1 0 0;
//  0 2 0;
//  0 0 3]
```

#### Get()
`[matrix].Get(row, col)`，通过行数列数寻找元素值。这也将返回一个`double`类型的值。
```C#
Matrix mt = new Matrix(3,3);
mt.Set(1,1,1);
mt.Set(2,2,2);
mt.Set(3,3,3);
Console.WriteLine(mt.Get(1,1));
Console.WriteLine(mt.Get(2,2));
Console.WriteLine(mt.Get(1,2));

// 结果
// 1
// 2
// 0
```

稠密矩阵也支持`Get()`操作
```C#
Matrix mt = new Matrix(3,3);
mt.Set(1,1,1);
mt.Set(2,2,2);
mt.Set(3,3,3);
// 导入矩阵：
// [1 0 0;
//  0 2 0;
//  0 0 3]

// 稠密矩阵操作：
dmt = new DenseMatrix(mt);
Console.WriteLine(dmt.Get(1, 1)); //1
Console.WriteLine(dmt.Get(2, 2)); //2
Console.WriteLine(dmt.Get(1, 2)); //0
```

## 矩阵运算
### 加减法
加法与减法同理

矩阵与矩阵相加
```C#
//矩阵之间的运算（每个数值相加）
// mt1
// [1 4;
//  2 5;
//  3 6]
Matrix newMt1 = mt1 + mt1;
// newMt1
// [2 8;
//  4 10;
//  6 12]

// dmt3
// [1 2 1;
//  2 2 2;
//  1 1 4;
//  2 1 6]
// dmt5
// [1 2 1;
//  2 2 2;
//  1 1 3]
DenseMatrix dmt = dmt3 + dmt5;
// dmt
// [1 2 2;
//  2 2 4;
//  1 1 7;
//  2 1 6]
```

矩阵与数值相加
```C#
//矩阵与数值相加
Matrix Mt11 = mt1 + 1; //[2 5; 3 6; 4 7]
```
## 乘法
```C#
Matrix mt12 = mt1 * mt2; //[14 17; 35 38]
```
## 转置
转置函数会将选定的矩阵转置，并返回值。
```C#
//转置元素本身
mt1.T();
Console.WriteLine($"{mt1.ValueString}\n");
```
```C#
//转置操作本身也会有返回值
Console.WriteLine($"{mt1.T().ValueString}\n");
```
两种操作得到的结果均为`[1 4; 2 5; 3 6]`
## 求和值
获取矩阵中所有元素的求和结果
```C#
Console.WriteLine(mt1.Sum); //21
```

## 子矩阵
### 获取子矩阵
用法:
`[matrix].GetSubMatrix([起始行], [起始列], [截取行数], [截取列数])`
```C#
// mt1:
// [2 5;
//  3 6;
//  4 7]
Console.WriteLine($"{mt1.GetSubMatrix(1, 1, 2, 2).ValueString}\n");
// 截取结果
// [2 5;
//  3 6]
```
### 截取行/列
截取行和列同理。有函数如下：
| 函数名                 | 参数解释                                                                       |
| ---------------------- | ------------------------------------------------------------------------------ |
| `GetRow(row)`          | 截取矩阵中某行，`row`为需要截取的行号，从1开始记起                             |
| `GetRows(startRow, n)` | 截取矩阵中某几行，`startRow`为开始截取的行号，从1开始记起，`n`为需要截取的行数 |
| `GetCol(col)`          | 截取矩阵中某列，`col`为需要截取的列号，从1开始记起                             |
| `GetCols(startCow, n)` | 截取矩阵中某几列，`startCow`为开始截取的列号，从1开始记起，`n`为需要截取的列数 |

以截取列为例
```C#
// mt1:
// [2 3 4;
//  5 6 7]
Console.WriteLine($"{mt1.GetCols(2, 2).ValueString}\n");
// [3 4;
//  6 7]
Console.WriteLine($"{mt1.GetCol(2).ValueString}\n");
// [3;
//  6]
```

### 获取指定值的行列坐标
根据输入的指定值在矩阵中寻找符合的元素的行列坐标列表，用矩阵表示
```C#
Console.WriteLine(mt2.GetList(2).ToString()); // [1 1; 2 2; 3 1]
```
也可以直接使用函数获取最小值和最大值的行列位置列表
```C#
//函数使用方法同上，形式如下
[矩阵对象].GetMinList();
[矩阵对象].GetMaxList();
```

## 属性
| 属性          | 作用                                                                                            |
| ------------- | ----------------------------------------------------------------------------------------------- |
| `Row`         | 实时获取并返回矩阵的行数                                                                        |
| `Col`         | 实时获取并返回矩阵的列数                                                                        |
| `ValueString` | 返回转化为Matlab格式的字符串的矩阵，如`[1 2;\n3 4]`（不需要换行符'\n'的版本可以使用ToString()） |