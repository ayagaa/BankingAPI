using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BankingAPI.Utils
{
    internal static class JsonUtils
    {
        private static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        internal static RootObject ParseApiData<RootObject>(ref string ApiResult)
        {
            RootObject rootObject;
            try
            {
                rootObject = JsonConvert.DeserializeObject<RootObject>(ApiResult, JsonSerializerSettings);
                return rootObject;
            }
            catch (Exception)
            {
                ApiResult = "Failed";
            }
            return default(RootObject);
        }

        internal static RootObject ParseApiData<RootObject>(string ApiResult)
        {
            RootObject rootObject;
            try
            {
                rootObject = JsonConvert.DeserializeObject<RootObject>(ApiResult, JsonSerializerSettings);
                return rootObject;
            }
            catch (Exception)
            {
            }
            return default(RootObject);
        }

        internal static string SerializeResults<RootObject>(string ApiResult)
        {
            var dataObject = ParseApiData<RootObject>(ApiResult);
            return JsonConvert.SerializeObject(dataObject, JsonSerializerSettings);
        }

        internal static string SerializeResults<RootObject>(RootObject DataObject)
        {
            return JsonConvert.SerializeObject(DataObject, JsonSerializerSettings);
        }

        internal async static Task<string> GetAPiResults<RootObject>(Func<Task<string>> Function, int ResultSize)
        {
            await Task.Delay(0);

            return string.Empty;
        }

    }
}
