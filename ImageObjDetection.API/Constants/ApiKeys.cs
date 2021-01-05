using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageObjDetection.API.Constants
{
	public class ApiKeys
	{
        public Dictionary<string, string> DicApiKey()
        {
            Dictionary<string, string> apiKeyDic =
                new Dictionary<string, string>();

            apiKeyDic.Add("mTwsJ4qXEXPq6E3m/HrPAO5nqbf0qRY60LBCP2j7Ets=", "ClientApp");

            return apiKeyDic;
        }
    }
}
