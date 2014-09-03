using DcmsMobile.Inquiry.Areas.Inquiry.SharedViews;
using DcmsMobile.Inquiry.Helpers;
using EclipseLibrary.Oracle;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Web;

namespace DcmsMobile.Inquiry.Areas.Inquiry.CustomerEntity
{
    internal class CustomerEntityRepository:IDisposable
    {
        private readonly OracleDatastore _db;
        public CustomerEntityRepository(string userName, string clientInfo)
        {
            _db = new OracleDatastore(HttpContext.Current.Trace);
            _db.CreateConnection(ConfigurationManager.ConnectionStrings["dcms8"].ConnectionString, userName);
            _db.ModuleName = "Inquiry_CustomerEntity";
            _db.ClientInfo = clientInfo;
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        /// <summary>
        /// return the customer info against scanned customer id.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public Customer GetCustomerInfo(string customerId)
        {
            Contract.Assert(_db != null);
            const string QUERY_CUSTOMER_DETAIL = @"
with PIVOT_CUST_SPLH(CUSTOMER_ID,
EDI753,
AMS,
SCO) AS
 (SELECT *
    FROM (SELECT CUSTOMER_ID, SPLH_ID, SPLH_VALUE FROM <proxy />CUSTSPLH) PIVOT(MAX(SPLH_VALUE) FOR SPLH_ID IN('$EDI753',
                                                                                                      '$AMS',
                                                                                                      '$SCO'))
   where CUSTOMER_ID = :CUSTOMER_ID),
pivot_cust_doc(
NUMBER_OF_MPS,
MPS_SHORT_NAME,
NUMBER_OF_PSPB,
PSPB_SHORT_NAME,
NUMBER_OF_UCC,
UCC_SHORT_NAME,
NUMBER_OF_CCL,
CCL_SHORT_NAME,
NUMBER_OF_SHLBL,
SHLBL_SHORT_NAME
) AS
 (

SELECT *
  FROM (SELECT d.DOCUMENT_ID,
 d.short_description as short_description,
               case
                 when cd.document_id is not null then
                  cd.NUMBER_OF_COPIES
                 else
                  d.default_no_of_copies
               end as NUMBER_OF_COPIES
          FROM <proxy />doc d
          left outer join <proxy/>CUSTDOC cd
            on cd.document_id = d.document_id
           and cd.customer_id = :CUSTOMER_ID
           and cd.number_of_copies is not null) PIVOT(MAX(NUMBER_OF_COPIES),  MAX(short_description) as x FOR DOCUMENT_ID IN('$MPS',
                                                                                                                            '$PSPB',
                                                                                                                            '$UCC',
                                                                                                                            '$CCL',
                                                                                                                            '$SHLBL'))
   )
SELECT CST.CUSTOMER_ID        AS CUSTOMER_ID,
       CST.NAME               AS NAME,
       CST.CATEGORY           AS CATEGORY,
       CTYPE.description      AS ACCOUNT_TYPE,
       CST.CARRIER_ID         AS CARRIER_ID,
       M.DESCRIPTION          AS CARRIER_DESCRIPTION,
       CST.ASN_FLAG           AS ASN_FLAG,
       CST.DEFAULT_PICK_MODE  AS DEFAULT_PICK_MODE,
       CST.MIN_PIECES_PER_BOX AS MIN_PIECES_PER_BOX,
       CST.MAX_PIECES_PER_BOX AS MAX_PIECES_PER_BOX,
       PCS.EDI753             AS EDI753,
       PCS.AMS                AS AMS,
       PCS.SCO                AS SCO,
       pcd.NUMBER_OF_MPS      as NUMBER_OF_MPS,
pcd.MPS_SHORT_NAME as MPS_SHORT_NAME,
pcd.PSPB_SHORT_NAME as PSPB_SHORT_NAME,
pcd.UCC_SHORT_NAME as UCC_SHORT_NAME,
pcd.CCL_SHORT_NAME as CCL_SHORT_NAME,
pcd.SHLBL_SHORT_NAME as SHLBL_SHORT_NAME,
       pcd.NUMBER_OF_PSPB     as NUMBER_OF_PSPB,
       pcd.NUMBER_OF_UCC      as NUMBER_OF_UCC,
       pcd.NUMBER_OF_CCL      as NUMBER_OF_CCL,
       pcd.NUMBER_OF_SHLBL    as NUMBER_OF_SHLBL              
  FROM <proxy />CUST CST
 left outer join custtype CTYPE
    ON CST.ACCOUNT_TYPE = CTYPE.customer_type
 left outer JOIN PIVOT_CUST_SPLH PCS
    ON CST.CUSTOMER_ID = PCS.CUSTOMER_ID
 LEFT OUTER JOIN MASTER_CARRIER M
    ON CST.CARRIER_ID = M.CARRIER_ID
cross join  pivot_cust_doc pcd
 WHERE CST.CUSTOMER_ID = :CUSTOMER_ID
            ";
            var binder = SqlBinder.Create(row => new Customer
            {
                CustomerId = row.GetString("CUSTOMER_ID"),
                CustomerName = row.GetString("NAME"),
                Category = row.GetString("CATEGORY"),
                AccountType = row.GetString("ACCOUNT_TYPE"),
                CarrierId = row.GetString("CARRIER_ID"),
                CarrierDescription = row.GetString("CARRIER_DESCRIPTION"),
                DefaultPickMode = row.GetString("DEFAULT_PICK_MODE"),
                MinPiecesPerBox = row.GetInteger("MIN_PIECES_PER_BOX"),
                MaxPiecesPerBox = row.GetInteger("MAX_PIECES_PER_BOX"),
                AmsFlag = !string.IsNullOrEmpty(row.GetString("AMS")),
                EdiFlag = !string.IsNullOrEmpty(row.GetString("EDI753")),
                ScoFlag = !string.IsNullOrEmpty(row.GetString("SCO")),
                Asn_flag = !string.IsNullOrEmpty(row.GetString("ASN_FLAG")),
                NumberOfMps = row.GetInteger("NUMBER_OF_MPS"),
                MpsShortName = row.GetString("MPS_SHORT_NAME"),
                NumberOfPspb = row.GetInteger("NUMBER_OF_PSPB"),
                PspbShortName = row.GetString("PSPB_SHORT_NAME"),
                NumberOfUcc = row.GetInteger("NUMBER_OF_UCC"),
                UccShortName = row.GetString("UCC_SHORT_NAME"),
                NumberOfCcl = row.GetInteger("NUMBER_OF_CCL"),
                CclShortName = row.GetString("CCL_SHORT_NAME"),
                NumberOfShlbl = row.GetInteger("NUMBER_OF_SHLBL"),
                ShlblShortName = row.GetString("SHLBL_SHORT_NAME"),
            }).Parameter("CUSTOMER_ID", customerId);

            return _db.ExecuteSingle(QUERY_CUSTOMER_DETAIL, binder);
        }

        /// <summary>
        /// This function will return orders summary of Customer for last 180 days.Summary is grouped on the basis of import date and we wont show more than 100 rows.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public IList<PoHeadline> GetRecentOrders(string customerId, int maxRows)
        {
            return SharedRepository.GetRecentOrders(_db, customerId, null, maxRows);
        }


        public IList<Tuple<string, string>> CustomerAutoComplete(string term)
        {
            const string QUERY =
                @"
                SELECT CUST.CUSTOMER_ID AS CUSTOMER_ID, 
                       CUST.NAME AS CUSTOMER_NAME
                FROM <proxy />CUST CUST
                WHERE 1 = 1
                 <if c='$TERM'>
                        AND (UPPER(CUST.CUSTOMER_ID) LIKE '%' || UPPER(:TERM) ||'%' 
                            OR UPPER(CUST.NAME) LIKE '%' || UPPER(:TERM) ||'%')
                 </if>                      
                        AND ROWNUM &lt; 40 and SUBSTR(UPPER(CUST.CUSTOMER_ID), 1, 1) != '$'
                        ORDER BY CUST.CUSTOMER_ID
                ";
            Contract.Assert(_db != null);
            var binder = SqlBinder.Create(row => Tuple.Create(row.GetString("CUSTOMER_ID"), row.GetString("CUSTOMER_NAME")))
                .Parameter("TERM", term);

            return _db.ExecuteReader(QUERY, binder);

        }
    }
}