using System.Collections.Generic;
using Newtonsoft.Json;
using ZarinpalRestApi.Helpers;

namespace ZarinpalRestApi.Models
{
    public class ZarinpalModelV4
    {
        public class BaseResponse<T>
        {
            public T Data { get; set; }
            // I Personally never received an error, So although we know it is a list, it's not guaranteed to be a list of `string`
            public List<string> Errors { get; set; } 
        }
        
        public class Payment
        {
            public class Request
            {
                /// <summary>
                /// <i>Mandatory</i> The code that the Zarinpal is provided specifically for this Merchant
                /// </summary>
                public string MerchantId { get; set; }

                /// <summary>
                /// <i>Mandatory</i> The amount of money that should be transfer during this transaction in IRR (Iranian Rials)
                /// </summary>
                [JsonConverter(typeof(DecimalAsStringWithoutFloatingPointConverter))]
                public decimal Amount { get; set; }

                /// <summary>
                /// <i>Mandatory</i> The URL that the bank should send user into after performing the transaction, either successful or unsuccessful
                /// </summary>
                public string CallbackUrl { get; set; }

                /// <summary>
                /// <i>Mandatory</i> 
                /// </summary>
                public string Description { get; set; }
                
                /// <summary>
                /// <i>Optional</i> Provides messaging information to ease user from typing them
                /// </summary>
                public RequestMetadata Metadata { get; set; }
            }
            
            public class RequestMetadata
            {
                /// <summary>
                /// <i>Optional</i> Provides email address to ease user from typing
                /// </summary>
                public string Email { get; set; }
                /// <summary>
                /// <i>Optional</i> Provides mobile number to ease user from typing
                /// </summary>
                public string Mobile { get; set; }
            }

            public class Response : BaseResponse<ResponseData>
            {
            }

            public class ResponseData
            {
                public int Code { get; set; }
                public string Message { get; set; }
                /// <summary>
                /// A unique 32 characters length identifier of type `UUID` (Universal Unique Identifier) that Zarinpal
                /// Sent to client for each payment request. The Identifier always start with 'A' character.
                /// Sample: A 36 character lenght string, starting with A, like: A00000000000000000000000000217885159
                /// </summary>
                public string Authority { get; set; }
                public string FeeType { get; set; }
                public decimal Fee { get; set; }
            }

        }
        
        public class Verify
        {
            public class Request
            {
                /// <summary>
                /// <i>Mandatory</i> The code that the Zarinpal is provided specifically for this Merchant
                /// </summary>
                public string MerchantId { get; set; }

                /// <summary>
                /// <i>Mandatory</i> The amount of money that should be transfer during this transaction in IRR (Iranian Rials)
                /// </summary>
                [JsonConverter(typeof(DecimalAsStringWithoutFloatingPointConverter))]
                public decimal Amount { get; set; }
                /// <summary>
                /// Documentation: A unique 32 characters length identifier of type `UUID` (Universal Unique Identifier) that Zarinpal
                /// Sent to client for each payment request. The Identifier always start with 'A' character.
                /// Sample: A 36 character lenght string, starting with A, like: A00000000000000000000000000217885159
                /// </summary>
                public string Authority { get; set; }
            }
            
            public class Response:BaseResponse<ResponseData>
            {
            }
            
            public class ResponseData
            {
                /// <summary>
                /// The integer value that indicates the State <br/>
                /// <b>100</b>: for successful operation <br/>
                /// <b>101</b>: for repeated yet previously succeeded operation <br/>
                /// <b>-X</b>: <a href="https://docs.zarinpal.com/paymentGateway/error.html">See error codes in here</a>
                /// </summary>
                public int Code { get; set; }
                /// <summary>
                /// The reference number of succeeded payment
                /// </summary>
                public long RefId { get; set; }
                /// <summary>
                /// Masked Credit Card Number, Like: `1234-****-****-6789`
                /// </summary>
                public string CardPan { get; set; }
                /// <summary>
                /// Credit Card Hash Number - Type: SHA256 in case of later needs
                /// </summary>
                public string CardHash { get; set; }
                /// <summary>
                /// The person who pays for the commissions of the Zarinpal, Either the Payer (the one who pays) or the Payee (the one to whom money is paid)
                /// </summary>
                public string FeeType { get; set; }
                /// <summary>
                /// The amount of Zarinpal commissions
                /// </summary>
                public decimal Fee { get; set; }
            }
        }
    }
}