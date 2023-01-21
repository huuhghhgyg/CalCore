using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CalCore
{
    public class Matrix
    {
        /// <summary>
        /// 通过指定矩阵的行数和列数创建矩阵对象
        /// </summary>
        /// <param name="n">矩阵的行数</param>
        /// <param name="m">矩阵的列数</param>
        public Matrix(int n, int m) //创建一个n行m列的矩阵
        {
            Value = new double[n, m];
        }

        /// <summary>
        /// 通过输入复制二维数组创建新的矩阵对象
        /// </summary>
        /// <param name="value">矩阵数值的二维数组</param>
        public Matrix(double[,] value)
        {
            Value = new double[value.GetLength(0), value.GetLength(1)];
            Array.Copy(value, Value, value.Length);
        }

        /// <summary>
        /// 通过复制已有的矩阵创建新的矩阵对象
        /// </summary>
        /// <param name="matrix"></param>
        public Matrix(Matrix matrix)
        {
            Value = new double[matrix.Row, matrix.Col];
            Array.Copy(matrix.Value, Value, matrix.Value.Length);
        }
        /// <summary>
        /// 将原有矩阵嵌入新矩阵，原矩阵在新矩阵中的坐标为原矩阵右上角的坐标
        /// </summary>
        /// <param name="matrix">矩阵对象</param>
        /// <param name="rows">新建矩阵的行数</param>
        /// <param name="cols">新建矩阵的列数</param>
        /// <param name="row0">原矩阵第一行嵌入新矩阵的行位置(自然位置)</param>
        /// <param name="col0">原矩阵第一列嵌入新矩阵的列位置(自然位置)</param>
        /// <exception cref="ArgumentOutOfRangeException">嵌入位置超过允许范围</exception>
        public Matrix(Matrix matrix, int rows, int cols, int row0, int col0)
        {
            if (row0 <= 0 || col0 <= 0)
                if (row0 <= 0) throw new ArgumentOutOfRangeException(nameof(row0), "指定行位置小于0");
                else throw new ArgumentOutOfRangeException(nameof(col0), "指定列位置小于0");

            Value = new double[rows, cols];
            if (rows - matrix.Row + 1 >= row0 && cols - matrix.Col + 1 >= col0)
                for (int r = 0; r < matrix.Row; r++)
                    for (int c = 0; c < matrix.Col; c++)
                        Value[r + row0 - 1, c + col0 - 1] = matrix.Value[r, c];
            else if (rows - matrix.Row >= row0)
                throw new ArgumentOutOfRangeException(nameof(row0), $"参数row0超过最大允许值{rows - matrix.Row + 1}");
            else
                throw new ArgumentOutOfRangeException(nameof(col0), $"参数col0超过最大允许值{cols - matrix.Col + 1}");
        }
        /// <summary>
        /// 将原有矩阵嵌入新矩阵的第一行第一列
        /// </summary>
        /// <param name="matrix">矩阵对象</param>
        /// <param name="rows">新建矩阵的行数</param>
        /// <param name="cols">新建矩阵的列数</param>
        public Matrix(Matrix matrix, int rows, int cols) => Value = new Matrix(matrix, rows, cols, 1, 1).Value;
        /// <summary>
        /// 根据DenseMatrix新建矩阵
        /// </summary>
        /// <param name="dmt">DenseMatrix对象</param>
        /// <param name="row">矩阵行数</param>
        /// <param name="col">矩阵列数</param>
        /// <param name="baseNum">填充Matrix空白位置的背景值，默认为0</param>
        public Matrix(DenseMatrix dmt, int row, int col, double baseNum = 0)
        {
            Value = new double[row, col];
            if (baseNum != 0)
                for (int i = 0; i < row; i++)
                    for (int j = 0; j < col; j++)
                        Value[i, j] = baseNum; //填充自定义的背景值

            foreach (DenseMatrixItem item in dmt.Values)
            {
                Value[item.Row - 1, item.Col - 1] = item.Value;
            }
        }

        public Matrix(string matrix)
        {
            matrix = matrix.TrimStart('[').TrimEnd(']');
            //符号检验
            if (matrix.IndexOf(',') < 0) throw new ArgumentException("没有检测到列分隔符");

            //字符串分割
            string[] rows = matrix.Split(';');
            int row = rows.Length; //分割得到的行数
            int col = rows[0].Count(c => c == ',') + 1; //逗号数+1

            //数据正确性校验
            if (matrix.Count(c => c == ';') != row - 1) throw new ArgumentException("行数错误：行分隔符\";\"个数错误");
            if (matrix.Count(c => c == ',') != row * (col - 1)) throw new ArgumentException("列数错误：逗号分隔符\",\"个数错误");

            Value = new double[row, col];

            for (int i = 0; i < row; i++) //每行的处理
            {
                string[] c = rows[i].Split(',');
                for (int j = 0; j < col; j++) //每列的处理
                {
                    Value[i, j] = double.Parse(c[j]);
                }
            }
        }

        #region 对象属性
        /// <summary>
        /// 矩阵中存储的数值，为二维数组
        /// </summary>
        public double[,] Value { get; set; }
        /// <summary>
        /// 矩阵的行数
        /// </summary>
        public int Row { get => Value.GetLength(0); }
        /// <summary>
        /// 矩阵的列数
        /// </summary>
        public int Col { get => Value.GetLength(1); }
        #endregion

        #region 映射属性
        /// <summary>
        /// 将矩阵以String形式输出。matlab格式，换行。
        /// </summary>
        public string ValueString
        {
            get => ToString().Replace(";", ";\n ");
        }

        /// <summary>
        /// 获取矩阵中的最大值
        /// </summary>
        public double Max
        {
            get
            {
                double result = double.NegativeInfinity; //将结果设定为一个极小值
                for (int i = 0; i < Row; i++)
                    for (int j = 0; j < Col; j++)
                        if (Value[i, j] > result) result = Value[i, j];
                return result;
            }
        }

        /// <summary>
        /// 获取矩阵中的最小值
        /// </summary>
        public double Min
        {
            get
            {
                double result = double.PositiveInfinity; //将结果设定为一个极大值
                for (int i = 0; i < Row; i++)
                    for (int j = 0; j < Col; j++)
                        if (Value[i, j] < result) result = Value[i, j];
                return result;
            }
        }

        /// <summary>
        /// 获取矩阵中所有元素求和的值
        /// </summary>
        public double Sum
        {
            get
            {
                double result = 0;
                for (int i = 0; i < Row; i++)
                    for (int j = 0; j < Col; j++)
                        result += Value[i, j];
                return result;
            }
        }

        #endregion

        #region 运算
        public static Matrix operator +(Matrix a) => a; //正号
        public static Matrix operator -(Matrix a) //负号
        {
            for (int i = 0; i < a.Row; i++)
                for (int j = 0; j < a.Col; j++)
                    a.Value[i, j] = -a.Value[i, j];
            return a;
        }
        public static Matrix operator +(Matrix a, Matrix b) //矩阵相加（有ref特性，不能直接在a上面计算并返回值）
        {
            if (a.Row == b.Row && a.Col == b.Col) //判断矩阵大小是否相等
            {
                Matrix result = new Matrix(a.Row, a.Col);
                for (int i = 0; i < a.Row; i++)
                    for (int j = 0; j < a.Col; j++)
                        result.Value[i, j] = a.Value[i, j] + b.Value[i, j];
                return result;
            }
            else throw new Exception($"矩阵大小不同无法进行此类型运算:[{a.Row},{a.Col}]*[{b.Row},{b.Col}]");
        }
        public static Matrix operator -(Matrix a, Matrix b) => a + -b; //矩阵相减，调用加法方法
        public static Matrix operator +(Matrix a, double n) //矩阵加实数（快捷方法）
        {
            for (int i = 0; i < a.Row; i++)
                for (int j = 0; j < a.Col; j++)
                    a.Value[i, j] += n;
            return a;
        }
        public static Matrix operator +(double n, Matrix a) => a + n; //交换律映射（矩阵加实数）
        public static Matrix operator -(Matrix a, double n) => a + -n;
        public static Matrix operator -(double n, Matrix a) => n + -a;
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Col == b.Row)
            {
                Matrix result = new Matrix(a.Row, b.Col);
                for (int i = 0; i < a.Row; i++) //a的所有行
                {
                    for (int j = 0; j < b.Col; j++) //b的所有列
                    {
                        double element = 0; //矩阵中的数字
                        for (int k = 0; k < a.Col; k++) //取所有数值
                        {
                            element += a.Value[i, k] * b.Value[k, j];
                        }
                        result.Value[i, j] = element;
                    }
                }
                return result;
            }
            else
            {
                throw new Exception($"矩阵乘法的维度不正确:[{a.Row},{a.Col}]*[{b.Row},{b.Col}]");
            }
        }
        public static Matrix operator *(Matrix a, double n) //矩阵中所有元素乘数
        {
            for (int i = 0; i < a.Row; i++)
                for (int j = 0; j < a.Col; j++)
                    a.Value[i, j] *= n;
            return a;
        }
        public static Matrix operator *(double n, Matrix a) => a * n; //交换律
        #endregion

        #region 函数
        public override string ToString()
        {
            StringBuilder results = new StringBuilder("[");

            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Col; j++)
                {
                    results.Append($"{Value[i, j]} ");
                    if (j == Col - 1)
                    {
                        results.Remove(results.Length - 1, 1); //删除最后添加的空格
                        results.Append(";"); //换行
                    }
                }

            results.Remove(results.Length - 1, 1);
            results.Append(']');
            return results.ToString();
        }
        public Matrix T() //矩阵转置（步骤）
        {
            Matrix result = new Matrix(Col, Row);
            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Col; j++)
                    result.Value[j, i] = Value[i, j];
            Value = result.Value; //导入转置矩阵
            return this;
        }

        /// <summary>
        /// 从指定的行列元素位置开始截取子矩阵
        /// </summary>
        /// <param name="r">开始截取行</param>
        /// <param name="c">开始截取列</param>
        /// <param name="m">截取行数</param>
        /// <param name="n">截取列数</param>
        /// <returns>截取的矩阵</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Matrix GetSubMatrix(int r, int c, int m, int n)
        {
            if (r >= 1 && c >= 1 && r <= Row && c <= Col) //判断参照元素位置
            {
                r -= 1; c -= 1; //转化为index
                if (r + m <= Row && c + n <= Col) //判断截取范围的合理性
                {
                    Matrix result = new Matrix(m, n);
                    for (int valueRow = r, i = 0; i < m; valueRow++, i++)
                        for (int valueCol = c, j = 0; j < n; valueCol++, j++)
                            result.Value[i, j] = Value[valueRow, valueCol];
                    return result;
                }
                else throw new ArgumentOutOfRangeException("截取的矩阵的范围超过了矩阵本身");
            }
            else throw new ArgumentOutOfRangeException("参照元素位置不存在于矩阵内");
        }

        //此处其实可以引用GetSubMatrix，但是出于执行代码量考虑就重新写了
        /// <summary>
        /// 从输入的矩阵中截取多行
        /// </summary>
        /// <param name="startRow">起始行</param>
        /// <param name="n">截取行数</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Matrix GetRows(int startRow, int n)
        {
            if (startRow > 0 && startRow <= Row) //检验起始行
            {
                if (startRow + n - 1 <= Row) //检验截取范围
                {
                    Matrix result = new Matrix(n, Col); //返回一个新的矩阵
                    for (int r = startRow - 1, i = 0; i < n; r++, i++) //i,j为返回结果的index计数
                        for (int j = 0; j < Col; j++)
                            result.Value[i, j] = Value[r, j];
                    return result;
                }
                else throw new ArgumentOutOfRangeException("截取的矩阵的范围超过了矩阵本身");
            }
            else throw new ArgumentException("起始行的位置不存在");
        }
        /// <summary>
        /// 截取某一行(row)
        /// </summary>
        /// <param name="row">矩阵中行的编号</param>
        /// <returns></returns>
        public Matrix GetRow(int row) => GetRows(row, 1); //截取某一列

        /// <summary>
        /// 从输入的矩阵中截取多列
        /// </summary>
        /// <param name="startCol">起始列</param>
        /// <param name="n">截取列数</param>
        /// <returns>截取的多列矩阵</returns>
        /// <exception cref="ArgumentOutOfRangeException">截取的矩阵的范围超过了矩阵本身</exception>
        /// <exception cref="ArgumentException">起始行的位置不存在</exception>
        public Matrix GetCols(int startCol, int n)
        {
            if (startCol > 0 && startCol <= Col) //检验起始行
            {
                if (startCol + n - 1 <= Col) //检验截取范围
                {
                    Matrix result = new Matrix(Row, n); //返回一个新的矩阵
                    for (int c = startCol - 1, j = 0; j < n; c++, j++) //i,j为返回结果的index计数
                        for (int i = 0; i < Row; i++)
                            result.Value[i, j] = Value[i, c];
                    return result;
                }
                else throw new ArgumentOutOfRangeException("截取的矩阵的范围超过了矩阵本身");
            }
            else throw new ArgumentException("起始行的位置不存在");
        }
        /// <summary>
        /// 截取某一列
        /// </summary>
        /// <param name="col">矩阵中列的编号</param>
        /// <returns></returns>
        public Matrix GetCol(int col) => GetCols(col, 1); //截取某一行

        /// <summary>
        /// 使用数学意义上的行和列获取矩阵中的值
        /// </summary>
        /// <param name="row">行，从1开始</param>
        /// <param name="col">列，从1开始</param>
        /// <returns>矩阵指定位置的数值</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public double Get(int row, int col) //使用数学的理解获取值
        {
            if (row < 1 || row > Row || col < 1 || col > Col)
                throw new ArgumentOutOfRangeException("指定位置不存在于此矩阵内");
            else return Value[row - 1, col - 1];
        }

        /// <summary>
        /// 通过函数设置矩阵元素的值（也可以直接操作Value对象）
        /// </summary>
        /// <param name="row">行数</param>
        /// <param name="col">列数</param>
        /// <param name="value">元素值</param>
        public void Set(int row, int col, double value) => Value[row - 1, col - 1] = value;

        /// <summary>
        /// 获取矩阵中符合输入值的行列号列表，列表用矩阵表示
        /// </summary>
        /// <param name="target">要在矩阵中寻找的值</param>
        /// <returns>用矩阵表示的行列号列表，每行表示对应点的行列号</returns>
        public Matrix GetList(double target) //获取矩阵中符合对应值的行列号矩阵
        {
            ArrayList rowList = new ArrayList(); //存放行号
            ArrayList colList = new ArrayList(); //存放列号

            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Col; j++)
                    if (Value[i, j] == target)
                    {
                        rowList.Add(i + 1); //添加行号
                        colList.Add(j + 1); //添加列号
                    }

            double[,] resultList = new double[rowList.Count, 2]; //坐标数组
            for (int i = 0; i < rowList.Count; i++)
            {
                resultList[i, 0] = (int)rowList[i];
                resultList[i, 1] = (int)colList[i];
            }

            return new Matrix(resultList);
        }

        /// <summary>
        /// 获取矩阵中最小值的行列号列表，列表用矩阵表示
        /// </summary>
        /// <returns>用矩阵表示的行列号列表，每行表示对应点的行列号</returns>
        public Matrix GetMinList() => GetList(Min);

        /// <summary>
        /// 获取矩阵中最大值的行列号列表，列表用矩阵表示
        /// </summary>
        /// <returns>用矩阵表示的行列号列表，每行表示对应点的行列号</returns>
        public Matrix GetMaxList() => GetList(Max);
        #endregion
    }
}
