﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPack.Test
{
    internal class TestModel
    {
        public int CodArticulo { get; set; }
        public string Descripcion { get; set; } = null!;
        public decimal Precio { get; set; }
        public decimal Descuento { get; set; }
        public decimal PrecioOferta { get; set; }
        public bool Nuevo { get; set; }
        public bool Favorito { get; set; }
        public string UrlImagen { get; set; } = null!;
        public string Generico { get; set; } = null!;
        public string Formula { get; set; } = null!;
        public string Laboratorio { get; set; } = null!;
        public string Clasificacion { get; set; } = null!;
        public string Indicaciones { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public int Cantidad { get; set; }
        public string FormulaDescripcion { get; set; } = null!;
        public bool Oferta { get; set; }
        public string Promociones { get; set; } = null!;
        public int Existencia { get; set; }
        public bool Control { get; set; }
    }
}
