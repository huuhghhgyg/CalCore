此处为简单的矩阵类型。如果需要高级用法，请使用 [Math.NET Numerics](https://numerics.mathdotnet.com/)
# 矩阵使用
这是一个基于double类型写了一个简易版的矩阵类型，不包括矩阵求逆等高级功能。目前已有的简易功能：
* 从double类型数组导入矩阵
* 新建`n*m`大小的矩阵
* 矩阵加法、减法、乘法，均包含矩阵对数值的运算

## 新建矩阵
```C#
//新建大小为n*m的0矩阵：
Matrix mt = new Matrix(2, 3); //创建一个2行3列的矩阵

//新建包含具体数值的矩阵
Matrix mt1 = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } }); //mt1:[1,2,3;4,5,6]
Matrix mt2 = new Matrix(new double[,] { { 2, 1 }, { 3, 2 }, { 2, 4 } }); //mt2:[2,1;3,2;2,4]

//根据一个已知矩阵的值新建一个矩阵
Matrix newMt = new Matrix(mt1);
```

## 矩阵的值修改
### 根本方法（编程理解）
以上方的mt矩阵为例：
```C#
mt.Value[0,0] = 1; //将矩阵1行1列对应的元素值修改为1
```
目前还没有其他更多的操作，可以根据本条自由发挥。
值得注意的是：`mt.Value`是一个`double`类型的二维矩阵。如果修改后矩阵的大小发生改变，`mt.Row`和`mt.Col`的值也会被动改变。

### 数学理解的方法
`[matrix].GetValue(row, col)`，通过行数列数寻找元素值。这也将返回一个`double`类型的值。

## 矩阵运算
### 加减法
加法与减法同理
```C#
//矩阵之间的运算（每个数值相加）
Matrix newMt1 = mt1 + mt1; //[2 8; 4 10; 6 12]

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
// [2 5]
// [3 6]
// [4 7]
Console.WriteLine($"{mt1.GetSubMatrix(1, 1, 2, 2).ValueString}\n");
// 截取结果
// [2 5]
// [3 6]
```
### 截取行/列
截取行和列同理。有函数如下：
|函数名|参数解释|
|---|---|
|`GetRow(row)`|截取矩阵中某行，`row`为需要截取的行号，从1开始记起|
|`GetRows(startRow, n)`|截取矩阵中某几行，`startRow`为开始截取的行号，从1开始记起，`n`为需要截取的行数|
|`GetCol(col)`|截取矩阵中某列，`col`为需要截取的列号，从1开始记起|
|`GetCols(startCow, n)`|截取矩阵中某几列，`startCow`为开始截取的列号，从1开始记起，`n`为需要截取的列数|

以截取列为例
```C#
// mt1:
// [2 3 4]
// [5 6 7]
Console.WriteLine($"{mt1.GetCols(2, 2).ValueString}\n");
// [3 4]
// [6 7]
Console.WriteLine($"{mt1.GetCol(2).ValueString}\n");
// [3]
// [6]
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
|属性|作用|
|---|---|
|`Row`|实时获取并返回矩阵的行数|
|`Col`|实时获取并返回矩阵的列数|
|`ValueString`|返回转化为字符串的矩阵，如`[1 2]\n[3 4]`|