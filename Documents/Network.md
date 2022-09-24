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