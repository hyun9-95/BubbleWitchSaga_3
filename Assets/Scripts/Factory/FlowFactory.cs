public struct FlowFactory
{
    public BaseFlow Create(FlowType type)
    {
        switch (type)
        {
            case FlowType.IntroFlow:
                return new IntroFlow();

            case FlowType.BattleFlow:
                return new BattleFlow();
        }

        return null;
    }
}
