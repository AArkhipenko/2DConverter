using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console.Model
{
    public class ScanModel
    {
        public List<Scan> _model = null;
        public ScanModel(int scan_count)
        {
            this._model = new List<Scan>(scan_count);
            while(scan_count>0)
            {
                this._model.Add(new Scan());
                scan_count--;
            }
        }

        public class Scan
        {
            public double _y = 0.0;
            public List<Coordanates> _xz = null;
            public Scan()
            {
                this._xz = new List<Coordanates>();
            }
            public class Coordanates
            {
                public double _x { get; set; }
                public double _z { get; set; }
            }
        }
    }
}
