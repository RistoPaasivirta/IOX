public class StupidWalker : Behavior
{
    protected override void OnAwake()
    {
        base.OnAwake();

        character.OnInputRequest = OnInputRequest;
    }

    public override void Restart()
    {
        base.Restart();

        character.OnInputRequest = OnInputRequest;
    }

    private void OnInputRequest(ref MonsterCharacter.InputFrame frame)
    {
        if (target == null)
            return;

        Vec2I smart = SmartMove;

        frame.up = smart.y;
        frame.right = smart.x;

        //frame.WantDirection = Quaternion.Euler(0, 0, AxMath.SafeAtan2(smart.y, smart.x) * Mathf.Rad2Deg);
        frame.WantDirection = TowardsTarget;
    }
}
