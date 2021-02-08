using System;
using System.Text;
using System.IO;

namespace Gorelovskiy.ru_3._0_Console
{
    class WriteFile : IDisposable
    {
        private StreamWriter _writer = null;
        public WriteFile()
        {
            if (File.Exists(Services._3d_file_path))
                File.Delete(Services._3d_file_path);
            this._writer = new StreamWriter(Services._3d_file_path, false, Encoding.GetEncoding(1251));//3DGcode
        }
        public void Dispose()
        {
            if (_writer != null)
            {
                this._writer.WriteLine("M05" + "\r\n" + "G53 G0 Z0" + "\r\n" + "G0 A0" + "\r\n" + "M30");
                this._writer.Close();
                this._writer.Dispose();
                this._writer = null;
            }
        }

        public enum MatchValue
        {
            INFO,
            FIRST,
            G,
            M,
            T,
            S,
            F,
            COORDINATES,
            END
        }

        public void Write(MatchValue val, object data=null)
        {
            switch(val)
            {
                case MatchValue.INFO:
                    {
                        this.WriteInformation((Model.GeneralInfoModel)data);
                        break;
                    }
                case MatchValue.FIRST:
                    {
                        Model.PointModel point_3d = data as Model.PointModel;
                        this._writer.Write("Y" + point_3d._y.ToString1()+ "\r\n");
                        this._writer.Write("X" + point_3d._x.ToString1()+ "\t" + "A" + point_3d._a_grad.ToString1() + "\r\n");
                        this._writer.Write("Z" + point_3d._z.ToString1()+ "\r\n");
                        break;
                    }
                case MatchValue.G:
                case MatchValue.M:
                case MatchValue.T:
                case MatchValue.S:
                case MatchValue.F:
                    {
                        if (val == MatchValue.G && (int)data == 0)
                        {
                            this._writer.Write("G" + 1 + "\t" + "F" + Services._default_f + "\t");
                            break;
                        }
                        this._writer.Write(val.ToString() + data + " \t");
                        break;
                    }
                case MatchValue.COORDINATES:
                    {
                        Model.PointModel point_3d = data as Model.PointModel;
                        this._writer.Write("X" + point_3d._x.ToString1() + "\t");
                        this._writer.Write("Y" + point_3d._y.ToString1() + "\t");
                        this._writer.Write("Z" + point_3d._z.ToString1() + "\t");
                        this._writer.Write("A" + point_3d._a_grad.ToString1() + "\t");
                        break;
                    }
                case MatchValue.END:
                    {
                        this._writer.Write(" \r\n");
                        break;
                    }
                default:
                    break;
            }
        }

        private void WriteInformation(Model.GeneralInfoModel general_info)
        {
            this._writer.WriteLine("(  GORELOVSKIY.RU  )");
            this._writer.WriteLine("(DzRotory = {0})", general_info._spindel_length.ToString1()); //здесь была высота фрезы
            this._writer.WriteLine("(Нижняя точка: X = {0}, Y = {1}, Z = {2})", general_info._min_point._x.ToString1(), general_info._min_point._y.ToString1(), general_info._min_point._z.ToString1());
            this._writer.WriteLine("(Верхняя точка: X = {0}, Y = {1}, Z = {2})", general_info._max_point._x.ToString1(), general_info._max_point._y.ToString1(), general_info._max_point._z.ToString1());
            this._writer.WriteLine("(Точка начала сканирования:X = {0}, Y = {1}, Z = {2})", general_info._start_scan_point._x.ToString1(), general_info._start_scan_point._y.ToString1(), general_info._start_scan_point._z.ToString1());
            this._writer.WriteLine(" \r\n\r\n");
            this._writer.WriteLine("G43");
            this._writer.WriteLine("G0    Z{0}    A0", (general_info._max_point._z + 20).ToString1());
        }

    }

    public static class ExtentionMethods
    {
        public static string ToString1(this double obj)
        {
            return Math.Round(obj,3).ToString(Services._number_info);
        }
    }
}
