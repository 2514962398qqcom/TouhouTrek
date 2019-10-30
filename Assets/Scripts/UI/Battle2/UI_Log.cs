namespace ZMDFQ.UI.Battle
{
    public partial class UI_Log
    {
        partial void Init()
        {
            Log.onLog += Log_onLog;
        }

        private void Log_onLog(string channel, string msg)
        {
            m_Log.AddItemFromPool().asLabel.GetChild("title").text = msg;
            m_Log.scrollPane.ScrollBottom();
        }
    }
}