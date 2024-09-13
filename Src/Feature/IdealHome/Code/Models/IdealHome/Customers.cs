using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.IdealHome.Models.IdealHome
{
    public class IdealHomeCustomers
    {
        public string EfolderID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerName_AR { get; set; }
        public string Evaluation { get; set; }

        //Dubai Muncipality  
        public Decimal DM_1_VALUE { get; set; }
        public Decimal DM_TOTAL { get; set; }
        public int DM_COMP { get; set; }

        //Dubai Police
        public Decimal DP_1_VALUE { get; set; }
        public Decimal DP_2_VALUE { get; set; }
        public Decimal DP_3_VALUE { get; set; }
        public Decimal DP_TOTAL { get; set; }
        public int DP_COMP { get; set; }

        //DEWA
        public Decimal DEWA_1_VALUE { get; set; }
        public Decimal DEWA_2_VALUE { get; set; }
        public Decimal DEWA_3_VALUE { get; set; }
        public Decimal DEWA_4_VALUE { get; set; }
        public Decimal DEWA_5_VALUE { get; set; }
        public Decimal DEWA_6_VALUE { get; set; }
        public Decimal DEWA_TOTAL { get; set; }
        public int DEWA_COMP { get; set; }

        //Civil Defence
        public Decimal CD_1_VALUE { get; set; }
        public Decimal CD_2_VALUE { get; set; }
        public Decimal CD_3_VALUE { get; set; }
        public Decimal CD_TOTAL { get; set; }
        public int CD_COMP { get; set; }


        //Community Development Authority
        public Decimal CDA_1_VALUE { get; set; }
        public Decimal CDA_2_VALUE { get; set; }
        public Decimal CDA_3_VALUE { get; set; }
        public Decimal CDA_TOTAL { get; set; }
        public int CDA_COMP { get; set; }

        //Ambulance Service
        public Decimal AS_1_VALUE { get; set; }
        public Decimal AS_2_VALUE { get; set; }
        public Decimal AS_3_VALUE { get; set; }
        public Decimal AS_TOTAL { get; set; }
        public int AS_COMP { get; set; }

        //Dubai Smart Govt
        public Decimal DSG_1_VALUE { get; set; }
        public Decimal DSG_TOTAL { get; set; }
        public int DSG_COMP { get; set; }

        //Dubai Health Authority
        public Decimal DHA_1_VALUE { get; set; }
        public Decimal DHA_2_VALUE { get; set; }
        public Decimal DHA_TOTAL { get; set; }
        public int DHA_COMP { get; set; }

        //RTA
        public Decimal RTA_1_VALUE { get; set; }
        public Decimal RTA_2_VALUE { get; set; }
        public Decimal RTA_TOTAL { get; set; }
        public int RTA_COMP { get; set; }

        //FA
        public Decimal FA_1_VALUE { get; set; }
        public Decimal FA_2_VALUE { get; set; }
        public Decimal FA_TOTAL { get; set; }
        public int FA_COMP { get; set; }


        public List<IdealHomeCustomers> CustomerList { get; set; }

    }
}