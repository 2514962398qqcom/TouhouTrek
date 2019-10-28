using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMDFQ
{
    public enum EventEnum
    {
        Nothing,
        GameStart,
        GameEnd,

        RoundStart,
        RoundEnd,

        TurnStart,
        TurnEnd,

        ActionStart,
        ActionEnd,

        DrawEventCard,

        BeforDrawActionCard,
        DrawActionCard,
        BeforePassiveDropActionCard,
        DropActionCard,

        BeforeGameSizeChange,
        BeforePlayrSizeChange,

        AfterGameSizeChange,
        AfterPlayrSizeChange,

        afterDrawcardPhase,
        afterDiscardPhase,

        FaceUp,

        GetHandMax,
        GetPoint,
        /// <summary>
        /// 当检查卡牌或者技能是否可用。注册事件必须注册同步方法，返回Task.CompletedTask
        /// </summary>
        onCheckCanUse,
        changeEventDirection,

        BeforeEventCardEffect,
        AfterEventCardEffect,

        BeforeGetPlayerSizeMax,
        BeforeGetPlayerSizeMin,

        BeforeAddCard,
        AfterAddCard,
        AfterRemoveCard
    }
}
