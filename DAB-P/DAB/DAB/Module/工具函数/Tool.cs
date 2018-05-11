
using System.Data;
using System.Diagnostics;
using System.Xml.Linq;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using AxMSScriptControl;
using System.Collections;
using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.InteropServices;
using System.Data.OleDb;


namespace DAB
    {
    sealed class Tool
        {

        #region 功能：旋转中心
        //校正旋转中心
        public static double[] Get_CenterCircle(double[] arrX, double[] arrY)
            {
            int i = 0;
            double X1 = 0;
            double Y1 = 0;
            double X2 = 0;
            double Y2 = 0;
            double X3 = 0;
            double Y3 = 0;
            double X1Y1 = 0;
            double X1Y2 = 0;
            double X2Y1 = 0;
            double f = 0;
            double d = 0;
            double e = 0;
            double g = 0;
            double h = 0;
            double n = 0;
            double A = 0;
            double B = 0;
            double C = 0;
            double Cx = 0;
            double Cy = 0;
            double R;
            double[] Center_XY = new double[3];

            if ((arrX.Length - 1) + 1 < 3)
                {
                MessageBox.Show("至少大于等于3个数");
                Center_XY[0] = 0;
                Center_XY[1] = 0;
                Center_XY[1] = 0;
                return Center_XY;
                }

            for (i = 0; i <= (arrX.Length - 1); i++)
                {
                X1 = X1 + arrX[i];
                Y1 = Y1 + arrY[i];
                X2 = X2 + Math.Pow(arrX[i], 2);
                Y2 = Y2 + Math.Pow(arrY[i], 2);
                X3 = X3 + Math.Pow(arrX[i], 3);
                Y3 = Y3 + Math.Pow(arrY[i], 3);
                X1Y1 = X1Y1 + arrX[i] * arrY[i];
                X1Y2 = X1Y2 + arrX[i] * Math.Pow(arrY[i], 2);
                X2Y1 = X2Y1 + Math.Pow(arrX[i], 2) * arrY[i];
                }

            n = (arrX.Length - 1) + 1;
            f = n * X2 - X1 * X1;
            d = n * X1Y1 - X1 * Y1;
            e = n * X3 + n * X1Y2 - (X2 + Y2) * X1;
            g = n * Y2 - Y1 * Y1;
            h = n * X2Y1 + n * Y3 - (X2 + Y2) * Y1;
            A = (h * d - e * g) / (f * g - d * d);
            B = (h * f - e * d) / (d * d - g * f);
            C = -(A * X1 + B * Y1 + X2 + Y2) / n;

            Cx = A / (-2);
            Cy = B / (-2);
            R = (Math.Sqrt(A * A + B * B - 4 * C)) / 2;
            Center_XY[0] = 1;
            Center_XY[1] = Cx;
            Center_XY[2] = Cy;

            return Center_XY;

            }

        //验证旋转中心
        public static bool NewCenter(double X1, double Y1, double X0_C, double Y0_C, double Angle, ref double NewMicX, ref double NewMicY)
            {
            double A_Old = 0;
            double L1 = 0;
            L1 = System.Math.Sqrt(Math.Pow((X1 - X0_C), 2) + Math.Pow((Y1 - Y0_C), 2));

            if (Y1 - Y0_C > 0)
                {
                if (X1 - X0_C != 0)
                    {
                    if (X1 - X0_C > 0)
                        {
                        A_Old = System.Math.Atan((Y1 - Y0_C) / (X1 - X0_C)) / (4 * System.Math.Atan(1)) * 180;
                        }
                    else
                        {
                        A_Old = System.Math.Atan((Y1 - Y0_C) / (X1 - X0_C)) / (4 * System.Math.Atan(1)) * 180 + 180;
                        }
                    }
                else
                    {
                    A_Old = 90;
                    }
                }
            else if (Y1 - Y0_C == 0)
                {
                if (X1 - X0_C >= 0)
                    {
                    A_Old = 0;
                    }
                else
                    {
                    A_Old = 180;
                    }
                }
            else
                {
                if (X1 - X0_C < 0)
                    {
                    A_Old = System.Math.Atan((Y1 - Y0_C) / (X1 - X0_C)) / (4 * System.Math.Atan(1)) * 180 - 180;
                    }
                else if (X1 - X0_C == 0)
                    {
                    A_Old = -90;
                    }
                else
                    {
                    A_Old = System.Math.Atan((Y1 - Y0_C) / (X1 - X0_C)) / (4 * System.Math.Atan(1)) * 180;
                    }
                }

            NewMicX = L1 * System.Math.Cos((A_Old + Angle) / 180 * 4 * System.Math.Atan(1)) + X0_C;

            NewMicY = L1 * System.Math.Sin((A_Old + Angle) / 180 * 4 * System.Math.Atan(1)) + Y0_C;
            return true;
            }

        #endregion

        #region 生成2维矩阵
        /// <summary>
        /// FEEDER2维矩阵数据表，参数1：行数；参数2：列数；参数3：行间隔；参数4：列间隔；参数5：X方向阵列数据；参数6：Y方向阵列数据。
        /// </summary>
        /// <remarks></remarks>
        public static void GetArray_Feed4_15(long RowNumber, long ColNumber, double RowDist, double ColDist, ref double[] xArray, ref double[] yArray)
            {
            long i = 0;
            long j = 0;
            for (j = 0; j <= RowNumber - 1; j++) //指定行数
                {
                for (i = 0; i <= ColNumber - 1; i++) //指定列数
                    {
                    xArray[i + System.Convert.ToInt32(j * ColNumber)] = double.Parse(Strings.Format(i * ColDist, "0.000"));
                    yArray[i + System.Convert.ToInt32(j * ColNumber)] = double.Parse(Strings.Format(j * RowDist, "0.000"));
                    }
                }
            }

        public static void GetArray_Feed_4_15(long RowNumber, long ColNumber, double RowDist, double ColDist, ref double[] xArray, ref double[] yArray)
            {
            long i = 0;
            long j = 0;
            for (j = 0; j <= RowNumber - 1; j++) //指定行数
                {
                for (i = 0; i <= ColNumber - 1; i += 2) //指定列数
                    {
                    xArray[i + System.Convert.ToInt32(j * ColNumber)] = double.Parse(Strings.Format(System.Convert.ToInt32(i * ColDist) / 2, "0.000"));
                    yArray[i + System.Convert.ToInt32(j * ColNumber)] = double.Parse(Strings.Format(j * RowDist, "0.000"));
                    }

                for (i = 1; i <= ColNumber - 1; i += 2) //指定列数
                    {
                    xArray[i + System.Convert.ToInt32(j * ColNumber)] = double.Parse(Strings.Format(System.Convert.ToInt32(System.Convert.ToDouble(i - 1) * ColDist) / 2 + 8, "0.000"));
                    yArray[i + System.Convert.ToInt32(j * ColNumber)] = double.Parse(Strings.Format(j * RowDist, "0.000"));
                    }
                }
            }

        public static void GetArray_Feed6_15(long RowNumber, long ColNumber, double RowDist, double ColDist, ref double[] xArray, ref double[] yArray)
            {
            long i = 0;
            long j = 0;
            for (j = 0; j <= RowNumber - 1; j++) //指定行数
                {
                for (i = 0; i <= ColNumber - 1; i += 2) //指定列数
                    {
                    xArray[i + System.Convert.ToInt32(j * ColNumber)] = double.Parse(Strings.Format(System.Convert.ToInt32(i * ColDist) / 2, "0.000"));
                    yArray[i + System.Convert.ToInt32(j * ColNumber)] = double.Parse(Strings.Format(j * RowDist, "0.000"));
                    }

                for (i = 1; i <= ColNumber - 1; i += 2) //指定列数
                    {
                    xArray[i + System.Convert.ToInt32(j * ColNumber)] = double.Parse(Strings.Format(System.Convert.ToInt32(System.Convert.ToDouble(System.Convert.ToDouble(i - 1) * ColDist) / 2) + PVar.Feed1_2Dist_1, "0.000"));
                    yArray[i + System.Convert.ToInt32(j * ColNumber)] = double.Parse(Strings.Format(j * RowDist, "0.000"));
                    }
                }
            }
        #endregion

        #region 功能：读取数组指定位置
        public static string GetArrayIndex(string SectionName, string KeyWord, string ArrayInPut, int Index, string FileName)
            {
            string[] LnArray = null;
            try
                {
                string TemStr = "";
                TemStr = BVar.FileRorW.ReadINI(SectionName, KeyWord, ArrayInPut, FileName);
                LnArray = TemStr.Split(',');
                return LnArray[Index - 1];
                }
            catch (Exception)
                {
                return ArrayInPut;
                }
            }
        #endregion

        #region 延时函数
        public class Delay
            {
            [DllImport("kernel32.dll", EntryPoint = "GetTickCount")]
            public static extern int GetTickCount();
            int _startTime = 0;
            public Delay()
            {
                _startTime = GetTickCount();
            }
            //public Delay(int no1)
            //{
            //    _startTime = GetTickCount();
            //}
            //public Delay(int no1, int no2)
            //{
            //    _startTime = GetTickCount();
            //}
            /// <summary>
            /// 获取当前时间计数
            /// </summary>
            public void InitialTime()
                {
                _startTime = GetTickCount();
                }

            /// <summary>
            /// 时间计数到,单位ms
            /// </summary>
            /// <param name="delayTime"></param>
            /// <returns></returns>
            public bool TimeIsUp(int delayTime)
                {
                if (GetTickCount() - _startTime > delayTime)
                    {
                    return true;
                    }
                else
                    {
                    return false;
                    }

                }
            }
        #endregion

        #region 读EXCEL数据
        /// <summary>
        /// 把EXCEL文件当做一个数据源来进行数据的读取操作
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="sheetIndex"></param>
        /// <returns></returns>
        public static DataTable ExcelToDT(string Path, int sheetIndex)
            {
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + Path + ";" + "Extended Properties=Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            strExcel = "select * from [sheet1$]";
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            myCommand.Fill(ds, "table1");
            dt = ds.Tables[sheetIndex];
            return dt;
            }

        public DataSet ExcelToDS(string Path, int sheetIndex)
            {
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + Path + ";" + "Extended Properties=Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            strExcel = "select * from [sheet" + sheetIndex + "$]";
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            DataSet ds = new DataSet();
            myCommand.Fill(ds, "table1");
            return ds;
            }

        public void DSToExcel(string Path, DataSet oldds)
            {
            //先得到汇总EXCEL的DataSet 主要目的是获得EXCEL在DataSet中的结构   
            string strCon = " Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source =" + Path + ";Extended Properties=Excel 8.0";
            OleDbConnection myConn = new OleDbConnection(strCon);
            string strCom = "select * from [Sheet1$]";
            myConn.Open();
            OleDbDataAdapter myCommand = new OleDbDataAdapter(strCom, myConn);
            OleDbCommandBuilder builder = new OleDbCommandBuilder(myCommand);
            //QuotePrefix和QuoteSuffix主要是对builder生成InsertComment命令时使用。   
            builder.QuotePrefix = "[";     //获取insert语句中保留字符（起始位置）   
            builder.QuoteSuffix = "]"; //获取insert语句中保留字符（结束位置）   
            DataSet newds = new DataSet();
            myCommand.Fill(newds, "Table1");
            for (int i = 0; i < oldds.Tables[0].Rows.Count; i++)
                {
                //在这里不能使用ImportRow方法将一行导入到news中，因为ImportRow将保留原来DataRow的所有设置(DataRowState状态不变)。  
                //在使用ImportRow后newds内有值，但不能更新到Excel中因为所有导入行的DataRowState!=Added   
                DataRow nrow = oldds.Tables["Table1"].NewRow();
                for (int j = 0; j < newds.Tables[0].Columns.Count; j++)
                    {
                    nrow[j] = oldds.Tables[0].Rows[i][j];
                    }
                newds.Tables["Table1"].Rows.Add(nrow);
                }
            myCommand.Update(newds, "Table1");
            myConn.Close();
            }
        #endregion

        }

    }
