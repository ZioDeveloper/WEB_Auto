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
        public IEnumerable<AGR_SpedizioniWEB_Decoded_vw> AGR_SpedizioniWEB_Decoded_vw { get; set; }
        public IEnumerable<AGR_Porti> AGR_Porti { get; set; }
        public IEnumerable<AGR_ModelliAuto> AGR_ModelliAuto { get; set; }
        public IEnumerable<AGR_TrasportatoriGrimaldi> AGR_TrasportatoriGrimaldi { get; set; }
        public IEnumerable<AGR_TipoRotabile> AGR_TipoRotabile { get; set; }
        public IEnumerable<AGR_PERIZIE_DETT_TEMP_MVC> AGR_PERIZIE_DETT_TEMP_MVC { get; set; }
        public IEnumerable<AGR_PERIZIE_DETT_TEMP_MVC_vw> AGR_PERIZIE_DETT_TEMP_MVC_vw { get; set; }
        public IEnumerable<AGR_Parti> AGR_Parti { get; set; }
        public IEnumerable<WEB_AGR_Parti_vw> WEB_AGR_Parti_vw { get; set; }
        public IEnumerable<WEB_AGR_Danni_vw> WEB_AGR_Danni_vw { get; set; }

    }
}