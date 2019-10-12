using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMDFQ.Effects
{
    using PlayerAction;
    public static class UseWayResponse
    {
        /// <summary>
        /// 被要求使用指定卡时的使用返回
        /// </summary>
        /// <param name="game"></param>
        /// <param name="useLimt"></param>
        /// <param name="useInfo"></param>
        /// <param name="nextRequest"></param>
        /// <param name="card"></param>
        /// <returns></returns>
        public static bool CheckLimit(Game game, UseLimitCardRequest useLimt, FreeUse useInfo, ref NextRequest nextRequest,Card card)
        {
            if (useLimt.CardType != CardHelper.getId(card))
            {
                return false;
            }
            else
            {
                if (useInfo.Source.Count != 1)
                {
                    nextRequest = new CardChooseRequest()
                    {
                        RequestInfo = TipHelper.GetText("UseLimitTip", card.Name),
                    };
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
