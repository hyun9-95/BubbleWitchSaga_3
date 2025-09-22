using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum BattleBossDefine
{
	None = 0,
	BOSS_WILBUR = 30000,

}