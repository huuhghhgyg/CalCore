这是一个简单的求解最优化网络路径的模块。

# 小技巧
由于CalCore的[Matrix](./Matrix.md)模块使用`double`类型存储数值，而  `double`类型提供了正负无穷的数值如下：
```C#
double positiveInf = Double.PositiveInfinity; //正无穷
double negativeInf = Double.NegativeInfinity; //负无穷
```
正负无穷可以与普通的double数值进行比较，无穷之间自身也可以比较
```C#
Console.WriteLine(3 > Double.PositiveInfinity); //False
Console.WriteLine(Double.PositiveInfinity*-1==Double.NegativeInfinity); //True
```

# 函数
## 求解最短路程
变量定义
```C#
double inf = double.PositiveInfinity;
Matrix pathMatrix = new Matrix(new double[,]
{
    {0, 3, 2, inf, inf, 4},
    {inf, 0, 4, inf, 4, 1},
    {inf, inf, 0, -1, 6, inf},
    {3, -2, inf, 0, 1, inf},
    {5, inf, inf, inf, 0, 3},
    {inf, inf, 3, 3, inf, 0}
});
```

### ShortestPath2AllPoints
输入一个距离矩阵，计算出第一个点到所有点的最短距离行向量
```C#
Console.WriteLine(Network.ShortestPath2AllPoints(pathMatrix).ToString() + "\n");
//[0 -1 2 1 2 0]
```

### ShortestPath2LastPoint
输入一个距离矩阵，计算出所有点到最后一个点的最短距离列向量
```C#
Console.WriteLine(Network.ShortestPath2LastPoint(pathMatrix).ToString() + "\n");
//[0; 1; -2; -1; 3; 0]
```

### GetSpanningTree
输入一个距离方阵，返回一个同样大小的0-1矩阵，表示最小支撑树选中的边

函数形式：
```C#
public static Matrix GetSpanningTree(Matrix pathMatrix)
```
#### 举例
使用Matrix

```C#
double inf = double.PositiveInfinity;
Matrix pathMatrix = new Matrix(new double[,]
{
    {inf, 2, 3, inf, inf, inf, inf, inf, inf, },
    {inf, inf, 3, 2, inf, inf, inf, inf, 5},
    {inf, inf, inf, inf, 2, inf, inf, 6, inf},
    {inf, inf, inf, inf, 1, 3, inf, inf, inf},
    {inf, inf, inf, inf, inf, inf, 3, inf, inf},
    {inf, inf, inf, inf, inf, inf, 3, inf, 2},
    {inf, inf, inf, inf, inf, inf, inf, 2, inf},
    {inf, inf, inf, inf, inf, inf, inf, inf, 4},
    {inf, inf, inf, inf, inf, inf, inf, inf, inf}
});

Matrix result = Network.GetSpanningTree(pathMatrix);
Console.WriteLine(result.ValueString);

// 结果：
// [0 1 0 0 0 0 0 0 0;
//  0 0 1 1 0 0 0 0 0;
//  0 0 0 0 0 0 0 0 0;
//  0 0 0 0 1 0 0 0 0;
//  0 0 0 0 0 0 1 0 0;
//  0 0 0 0 0 0 1 0 1;
//  0 0 0 0 0 0 0 1 0;
//  0 0 0 0 0 0 0 0 0;
//  0 0 0 0 0 0 0 0 0]
```

也可以使用DenseMatrix

```C#
DenseMatrix pdmt = new DenseMatrix();
pdmt.Set(1, 2, 2);
pdmt.Set(1, 3, 3);
pdmt.Set(2, 3, 3);
pdmt.Set(2, 4, 2);
pdmt.Set(2, 9, 5);
pdmt.Set(3, 5, 6);
pdmt.Set(4, 5, 1);
pdmt.Set(4, 6, 3);
pdmt.Set(5, 7, 3);
pdmt.Set(6, 7, 3);
pdmt.Set(6, 9, 2);
pdmt.Set(7, 8, 2);
pdmt.Set(8, 9, 4);

Matrix result = Network.GetSpanningTree(new Matrix(pdmt, 9, 9, double.PositiveInfinity));

DenseMatrix resultd = new DenseMatrix(result);
Console.WriteLine($"RowMax={resultd.RowMax},ColMax={resultd.ColMax}");
Console.WriteLine(resultd.ValueString);

// 结果
// RowMax=7,ColMax=9
// [1 2 1;
//  2 3 1;
//  2 4 1;
//  4 5 1;
//  5 7 1;
//  6 7 1;
//  6 9 1;
//  7 8 1]
```