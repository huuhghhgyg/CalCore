using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CalCore
{
    public class DenseMatrix //稠密矩阵
    {
        #region 初始化
        /// <summary>
        /// 直接创建新的DenseMatrix对象
        /// </summary>
        public DenseMatrix()
        {
            Values = new List<DenseMatrixItem>();
        }
        /// <summary>
        /// 通过传入DenseMatrixItem类型的数组创建新的DenseMatrix对象
        /// </summary>
        /// <param name="array">DenseMatrixItem的数组</param>
        public DenseMatrix(DenseMatrixItem[] array)
        {
            Values = array.ToList();
        }
        /// <summary>
        /// 通过传入Matrix类型的对象创建新的DenseMatrix对象
        /// </summary>
        /// <param name="matrix">要创建为稠密矩阵的矩阵对象</param>
        public DenseMatrix(Matrix matrix)
        {
            Values = new List<DenseMatrixItem>(); //初始化值

            for (int i = 1; i <= matrix.Row; i++)
                for (int j = 1; j <= matrix.Col; j++)
                    if (matrix.Get(i, j) != 0) Set(i, j, matrix.Get(i, j));
        }

        /// <summary>
        /// 复制一个DenseMatrixItem对象
        /// </summary>
        /// <param name="dmt">需要复制的DenseMatrix对象</param>
        public DenseMatrix(DenseMatrix dmt)
        {
            Values = new List<DenseMatrixItem>();
            dmt.Values.ForEach(v => Set(v.Row, v.Col, v.Value)); //将元素复制到新稠密矩阵中
        }
        #endregion

        #region 对象属性
        /// <summary>
        /// DenseMatrix的数据记录为List<DenseMatrixItem>
        /// </summary>
        public List<DenseMatrixItem> Values { get; set; }
        #endregion

        #region 映射属性
        /// <summary>
        /// 获取稠密矩阵中记录的最小值（除默认的0）
        /// </summary>
        public double Min
        {
            get
            {
                var list = from value in Values
                           orderby value.Value
                           select value.Value;
                return list.ToList()[0];
            }
        }
        /// <summary>
        /// 获取稠密矩阵中记录的最大值（除默认的0）
        /// </summary>
        public double Max
        {
            get
            {
                var list = from value in Values
                           orderby value.Value descending
                           select value.Value;
                return list.ToList()[0];
            }
        }

        public double Sum
        {
            get
            {
                double result = 0;
                foreach (DenseMatrixItem item in Values)
                {
                    result += item.Value;
                }
                return result;
            }
        }

        public string ValueString
        {
            get => ToString().Replace(";", ";\n ");
        }
        #endregion

        #region 运算
        public static DenseMatrix operator +(DenseMatrix dmt) => new DenseMatrix(dmt); //正号
        public static DenseMatrix operator -(DenseMatrix dmt) //负号
        {
            DenseMatrix dmt1 = new DenseMatrix();
            foreach (DenseMatrixItem item in dmt.Values) dmt1.Set(item.Row, item.Col, -item.Value);
            return dmt1;
        }
        public static DenseMatrix operator +(DenseMatrix dmt1, DenseMatrix dmt2)
        {
            DenseMatrix dmt3 = new DenseMatrix(dmt1);

            // 获取交集
            var items = from i1 in dmt1.Values
                        from i2 in dmt2.Values
                        where i1.Row == i2.Row && i1.Col == i2.Col
                        select new DenseMatrixItem(i1.Row, i1.Col, i1.Value + i2.Value);
            Console.WriteLine("交集");
            dmt3.Values = items.ToList();

            // 获取差集
            List<DenseMatrixItem> listSum = new List<DenseMatrixItem>();
            List<DenseMatrixItem> listResult = new List<DenseMatrixItem>();
            listSum.AddRange(dmt1.Values);
            listSum.AddRange(dmt3.Values);

            foreach (DenseMatrixItem item1 in listSum)
            {
                bool exist = false;
                foreach (DenseMatrixItem items3 in dmt3.Values)
                {
                    if (item1.Row == items3.Row && item1.Col == items3.Col)
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist) listResult.Add(item1);
            }
            dmt3.Values.AddRange(listResult);

            // 返回值
            return dmt3;
        }
        public static DenseMatrix operator -(DenseMatrix dmt1, DenseMatrix dmt2) => dmt1 + (-dmt2);

        #endregion

        #region 函数
        /// <summary>
        /// 使用DenseMatrixItem向数组中添加新的值
        /// </summary>
        /// <param name="item">DenseMatrixItem项</param>
        public void Set(DenseMatrixItem item)
        {
            // 查找是否已经存在值
            var items = from value in Values
                        where value.Row == item.Row && value.Col == item.Col
                        select value;
            // for debug
            //Console.WriteLine($"查找值（Row={item.Row}, Col={item.Col}）,结果个数{items.ToList().Count},{(items.ToList().Count == 0 ? "添加值" : "修改值")}");
            if (items.ToList().Count == 0) Values.Add(item);
            else items.ToList()[0].Value = item.Value;
        }

        /// <summary>
        /// 直接向DenseMatrix对象中添加新的值
        /// </summary>
        /// <param name="row">值所在矩阵中的行</param>
        /// <param name="col">值所在矩阵中的列</param>
        /// <param name="value">数值</param>
        public void Set(int row, int col, double value) => Set(new DenseMatrixItem(row, col, value));

        public double Get(int row, int col)
        {
            // 查找值
            var items = from value in Values
                        where value.Row == row && value.Col == col
                        select value;
            if (items.ToList().Count == 0) return 0; //没有找到值
            else return items.ToList()[0].Value;
        }

        /// <summary>
        /// 从DenseMatrix对象中删除值
        /// </summary>
        /// <param name="row">值所在行</param>
        /// <param name="col">值所在列</param>
        public void Remove(int row, int col)
        {
            var items = from value in Values
                        where value.Row == row && value.Col == col
                        select value;
            Values.Remove(items.ToList()[0]);
        }

        /// <summary>
        /// 在Console下会显示DenseMatrix对象中的值
        /// </summary>
        /// <returns>DenseMatrix对象中的值对应的string类型变量</returns>
        public void ShowAllValues()
        {
            Console.WriteLine(ValueString);
        }

        /// <summary>
        /// 转换为matlab格式的矩阵(无换行)
        /// </summary>
        /// <returns>string类型变量，matlab格式表示的矩阵</returns>
        public override string ToString()
        {
            StringBuilder results = new StringBuilder("[");
            foreach (var item in Values)
            {
                string line = $"{item.Row} {item.Col} {item.Value};";
                results.Append(line);
            }
            results.Remove(results.Length - 1, 1);
            results.Append(']');
            return results.ToString();
        }
    }
    #endregion

    public class DenseMatrixItem
    {
        public DenseMatrixItem(int row, int col, double value)
        {
            Row = row;
            Col = col;
            Value = value;
        }

        public int Row { get; set; }
        public int Col { get; set; }
        public double Value { get; set; }

        #region 运算
        //重载==运算符，用于判断记录是否相同
        public static bool operator ==(DenseMatrixItem left, DenseMatrixItem right)
        {
            return left.Value == right.Value &&
                left.Row == right.Row &&
                left.Col == right.Col;
        }
        public static bool operator !=(DenseMatrixItem left, DenseMatrixItem right)
        {
            return left.Value != right.Value ||
                left.Row != right.Row ||
                left.Col != right.Col;
        }
        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                return false;
            else
            {
                DenseMatrixItem item = (DenseMatrixItem)obj;
                return Value == item.Value && Row == item.Row && Col == item.Col;
            }
        }

        public override int GetHashCode()
        {
            return (Row.ToString() + ',' + Col.ToString() + ',' + Value.ToString()).GetHashCode();
        }
        #endregion
    }
}
