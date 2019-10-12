using System.Threading.Tasks;

namespace ZMDFQ.Cards
{
    /// <summary>
    /// 东方辉针城：所有牌面上对社群规模和个人影响力进行变动的粗体阿拉伯数字翻倍。
    /// </summary>
    public class G_013 : ThemeCard
    {
        public override void Enable(Game game)
        {
            game.EventSystem.Register(EventEnum.BeforeGameSizeChange, -1, effect_BeforeGameSizeChange);
            game.EventSystem.Register(EventEnum.BeforePlayrSizeChange, -1, effect_BeforePlayerSizeChange);
        }
        public override void Disable(Game game)
        {
            game.EventSystem.Remove(EventEnum.BeforeGameSizeChange, effect_BeforeGameSizeChange);
            game.EventSystem.Remove(EventEnum.BeforePlayrSizeChange, effect_BeforePlayerSizeChange);
        }
        Task effect_BeforeGameSizeChange(object[] args)
        {
            EventData<int> value = args[0] as EventData<int>;
            value.data *= 2;
            return Task.CompletedTask;
        }
        Task effect_BeforePlayerSizeChange(object[] args)
        {
            EventData<int> value = args[2] as EventData<int>;
            value.data *= 2;
            return Task.CompletedTask;
        }
    }
}
