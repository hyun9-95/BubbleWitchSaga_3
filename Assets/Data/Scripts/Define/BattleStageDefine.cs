using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum BattleStageDefine
{
	None = 0,
	STAGE_WILBUR = 20000,

}