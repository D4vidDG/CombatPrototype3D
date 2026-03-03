
using StarterAssets;

public interface State
{
    void Enter(Player player);
    State UpdateState(Player player, PlayerInputs inputs);
    void LateUpdateState(Player player);
    void Exit(Player player);
}
