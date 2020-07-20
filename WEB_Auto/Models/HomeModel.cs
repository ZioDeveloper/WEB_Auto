using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEB_Auto.Models
{
    public class HomeModel
    {

        public IEnumerable<AGR_Periti_WEB> AGR_Periti_WEB { get; set; }
        public IEnumerable<Periti> Periti { get; set; }
        public IEnumerable<AGR_Spedizioni> AGR_Spedizioni { get; set; }
        public IEnumerable<AGR_SpedizioniWEB_vw> AGR_SpedizioniWEB_vw { get; set; }
        public IEnumerable<AGR_Meteo> AGR_Meteo { get; set; }
        public IEnumerable<AGR_TipiPerizia> AGR_TipiPerizia { get; set; }

    }
}