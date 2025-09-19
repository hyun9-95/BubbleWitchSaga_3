using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum BalanceDefine
{
	None = 0,
	BALANCE_TEST = 10000,

}