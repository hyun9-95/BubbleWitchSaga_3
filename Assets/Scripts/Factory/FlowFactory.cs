public struct FlowFactory
{
    public BaseFlow Create(FlowType type)
    {
        switch (type)
        {
            case FlowType.IntroFlow:
                return new IntroFlow();
        }

        return null;
    }
}
