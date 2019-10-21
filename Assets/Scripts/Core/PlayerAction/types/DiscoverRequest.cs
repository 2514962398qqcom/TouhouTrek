using System.Collections.Generic;

namespace ZMDFQ.PlayerAction
{
    /// <summary>
    /// 让玩家从给定的一组卡中选择一定数量的卡，类比炉石的发现机制。
    /// </summary>
    public class DiscoverRequest : Request
    {
        /// <summary>
        /// 可供选择的卡片列表
        /// </summary>
        public List<int> SelectableCards;
        /// <summary>
        /// 玩家可以选择的卡片数量
        /// </summary>
        public int Count;
        /// <summary>
        /// 玩家是否必须选择这么多张卡？
        /// </summary>
        public bool EnoughOnly;
    }
    /// <summary>
    /// 让玩家从给定的一组卡中选择一定数量的卡，类比炉石的发现机制。
    /// </summary>
    public class DiscoverResponse : Response
    {
        /// <summary>
        /// 玩家选择的卡片
        /// </summary>
        public List<int> SelectedCards;
    }
}
