# CalCore
支持多种类型计算&优化求解

## 💡用法
```C#
// 引用部分
using CalCore;

// 使用部分
string result1 = CalCore.Core.Calculate("+1+2*3+4/5*6-7+8*9+100*0.1*-1*-1*-10*-0.1");
//86.60
string result2 = CalCore.Core.Calculate("4+(((1+2)*3)+(4*(2+2*(1+2)*2)*2+3)-10)*3+5*((1+2*2)*2+3)*2");
//476
```
矩阵部分参考 [📄矩阵说明文档](./Documents/Matrix.md)

网络部分参考 [📄网络优化文档](./Documents/Network.md)

线性模型求解参考 [📄线性优化文档](./Documents/LP.md)

数据处理部分参考 [📄数据处理文档](./Documents/Data.md)

## 🛠功能
- 科学计算（加减乘除优先级）
- 网络优化
- 数据指标权重处理
- 矩阵简单运算

## 💬其它
`2023/01/11`
学了完运筹学感觉单纯形法很重要，想简单实现一下，加深一下理解。上课的时候顺带更了一点有关网络优化的算法，完善了两个矩阵类方便使用。

`2022/09/17`
想要一个能够简单使用的矩阵类，就自己写了一个。虽然功能不算很完善，但是存储结构足够使用了。

`2022/03/09`
虽然功能比较弱鸡，但是这个项目是我从初中开始做的，后来就烂尾了。上个学期在学校学完C语言的课，感触颇深，又把C#从头开始学。现在已经学完了这个项目需要的部分才花了一个晚上把初中挖的坑好好填好了😂。
以后有想法会继续更新这个项目的，毕竟这个项目算是我的第一个大项目😛