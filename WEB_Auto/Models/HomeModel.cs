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
        public IEnumerable<AGR_ModelliAuto_vw> AGR_ModelliAuto_vw { get; set; }

        public IEnumerable<AGR_ModelliAuto_CAB_vw> AGR_ModelliAuto_CAB_vw { get; set; }
        public IEnumerable<AGR_TrasportatoriGrimaldi> AGR_TrasportatoriGrimaldi { get; set; }
        public IEnumerable<AGR_TrasportatoriGrimaldi_vw> AGR_TrasportatoriGrimaldi_vw { get; set; }
        public IEnumerable<AGR_TipoRotabile> AGR_TipoRotabile { get; set; }
        public IEnumerable<AGR_PERIZIE_DETT_TEMP_MVC> AGR_PERIZIE_DETT_TEMP_MVC { get; set; }
        public IEnumerable<AGR_PERIZIE_DETT_TEMP_MVC_vw> AGR_PERIZIE_DETT_TEMP_MVC_vw { get; set; }

        public IEnumerable<AGR_PERIZIE_DETT_TEMP_MVC_vw> AGR_PERIZIE_DETT_TEMP_MVC_vw_Altri { get; set; } 
        public IEnumerable<AGR_Parti> AGR_Parti { get; set; }
        public IEnumerable<WEB_AGR_Parti_vw> WEB_AGR_Parti_vw { get; set; }
        public IEnumerable<WEB_AGR_Danni_vw> WEB_AGR_Danni_vw { get; set; }
        public IEnumerable<WEB_AGR_Gravita_vw> WEB_AGR_Gravita_vw { get; set; }

       
        public IEnumerable<WEB_AUTO_ListaSpedizioni_vw> WEB_AUTO_ListaSpedizioni_vw { get; set; }
        public IEnumerable<WEB_AUTO_ListaSpedizioni_2_vw> WEB_AUTO_ListaSpedizioni_2_vw { get; set; }
        public IEnumerable<WEB_AUTO_ListaSpedizioni_CMN_vw> WEB_AUTO_ListaSpedizioni_CMN_vw { get; set; }

        public IEnumerable<AGR_PERIZIE_TEMP_MVC> AGR_PERIZIE_TEMP_MVC { get; set; }

        public IEnumerable<WEB_AUTO_FOTO> WEB_AUTO_FOTO { get; set; }
        public IEnumerable<WEB_AUTO_PDF> WEB_AUTO_PDF { get; set; }
        public IEnumerable<AGR_Perizie_MVC_Flat_vw> AGR_Perizie_MVC_Flat_vw { get; set; }

        public IEnumerable<AAA_Prova> AAA_Prova { get; set; }
        public IEnumerable<WEB_Auto_ListaPerizieXSpedizione_vw> WEB_Auto_ListaPerizieXSpedizione_vw { get; set; }

        public IEnumerable<WEB_ListaPerizieFlat_MVC_vw> WEB_ListaPerizieFlat_MVC_vw { get; set; }
        public IEnumerable<WEB_ListaPerizieFlat_TMP_vw> WEB_ListaPerizieFlat_TMP_vw { get; set; }
        public IEnumerable<WEB_ListaPerizieFlat_DEF_vw> WEB_ListaPerizieFlat_DEF_vw { get; set; }










    }
}