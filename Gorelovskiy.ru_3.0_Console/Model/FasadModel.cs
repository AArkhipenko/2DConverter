using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorelovskiy.ru_3._0_Console.Model
{
    public class FasadModel
    {
        public List<Duga> _model = null;
        public FasadModel()
        {
            this._model = new List<Duga>();
        }

        public class Duga
        {
            public double _y = 0.0;
            public List<Coordanates> _xz = null;
            public Duga()
            {
                this._xz = new List<Coordanates>();
            }
            public class Coordanates
            {
                public double _x { get; set; }
                public double _z { get; set; }
                public double _a { get; set; }
                public double _length { get; set; }
            }
        }
    }
}
